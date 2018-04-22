namespace FESInternal
{
    using System.IO;
    using System.Xml;
    using UnityEngine;

    /// <summary>
    /// Tilemap subsystem
    /// </summary>
    public partial class FESTilemap
    {
        /// <summary>
        /// Load a tilemap from a TMX file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="layerName">Layer name in TMX</param>
        /// <param name="mapLayer">Layer index</param>
        /// <param name="mapSize">Resulting map size</param>
        /// <returns>True if successful</returns>
        public bool LoadTMX(string fileName, string layerName, int mapLayer, out Size2i mapSize)
        {
            mapSize = Size2i.zero;

            if (fileName == null || layerName == null)
            {
                return false;
            }

            fileName = Path.GetDirectoryName(fileName) + "/" + Path.GetFileNameWithoutExtension(fileName);
            fileName = fileName.Replace('\\', '/');

            if (mapLayer < 0 || mapLayer >= mFESAPI.HW.MapLayers)
            {
                return false;
            }

            var tmxFile = Resources.Load<TextAsset>(fileName);

            if (tmxFile == null)
            {
                Debug.LogError("Can't find TMX file: " + fileName + ". Make sure the TMX file is saved with .XML file extension, and that it's somewhere under the Assets/Resources folder.");
                return false;
            }

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(tmxFile.text);
            }
            catch (XmlException e)
            {
                Debug.LogError("Can't parse TMX file: " + e.ToString());
                return false;
            }

            var mapNodeElements = xmlDoc.GetElementsByTagName("map");
            var mapNode = mapNodeElements.Count > 0 ? mapNodeElements.Item(0) : null;
            if (mapNode == null)
            {
                Debug.LogError("TMX parsing error, <map> not found");
                return false;
            }

            if (mapNode.Attributes["orientation"] != null)
            {
                if (mapNode.Attributes["orientation"].Value.ToLower() != "orthogonal")
                {
                    Debug.LogError("TMX error, only orthogonal maps supported");
                    return false;
                }
            }
            else
            {
                Debug.LogError("TMX parsing error, orientation attribute not availalbe");
                return false;
            }

            int tmxMapWidth = 0;
            int tmxMapHeight = 0;
            if (mapNode.Attributes["width"] != null && mapNode.Attributes["height"] != null)
            {
                if (!int.TryParse(mapNode.Attributes["width"].Value, out tmxMapWidth) ||
                    !int.TryParse(mapNode.Attributes["height"].Value, out tmxMapHeight))
                {
                    Debug.LogError("TMX parsing error, can't parse width, or height");
                }
            }
            else
            {
                Debug.LogError("TMX parsing error, width and/or height attributes not availalbe");
                return false;
            }

            if (tmxMapWidth == 0 || tmxMapHeight == 0)
            {
                // Nothing to do
                return true;
            }

            if (tmxMapWidth < 0 || tmxMapHeight < 0 || tmxMapWidth > mFESAPI.HW.MapSize.width || tmxMapHeight > mFESAPI.HW.MapSize.height)
            {
                Debug.Log("TMX read map dimensions " + tmxMapWidth + "x" + tmxMapHeight + ", maximum is " + mFESAPI.HW.MapSize.width + "x" + mFESAPI.HW.MapSize.height + ", map will be truncated");
            }

            int tileSetsCount = XMLCountChildren(mapNode.ChildNodes, "tileset");
            if (tileSetsCount == 0)
            {
                Debug.LogError("TMX parsing error, could not find any tilesets");
                return false;
            }

            int[] firstGid = new int[tileSetsCount];
            int j = 0;
            for (int i = 0; i < mapNode.ChildNodes.Count; i++)
            {
                XmlNode node = mapNode.ChildNodes.Item(i);
                if (node.LocalName.ToLower() == "tileset")
                {
                    if (node.Attributes["firstgid"] == null)
                    {
                        Debug.LogError("TMX parsing error, tileset does not have firstgid attribute");
                        return false;
                    }

                    firstGid[j] = 0;
                    if (!int.TryParse(node.Attributes["firstgid"].Value, out firstGid[i]))
                    {
                        Debug.LogError("TMX parsing error, tileset firstgid invalid");
                        return false;
                    }

                    j++;
                }
            }

            int layerNodesCount = XMLCountChildren(mapNode.ChildNodes, "layer");
            XmlNode layerNode = null;

            if (layerNodesCount <= 0)
            {
                Debug.LogError("TMX parsing error, can't find any layers");
                return false;
            }

            for (int i = 0; i < mapNode.ChildNodes.Count; i++)
            {
                XmlNode node = mapNode.ChildNodes.Item(i);
                if (node.LocalName.ToLower() == "layer")
                {
                    if (node.Attributes["name"] != null)
                    {
                        string tmxLayerName = node.Attributes["name"].Value.ToLower();
                        if (tmxLayerName == layerName.ToLower())
                        {
                            layerNode = node;
                        }
                    }
                }
            }

            if (layerNode == null)
            {
                Debug.LogError("Layer \"" + layerName + "\" not found in TMX file");

                return false;
            }

            string encoding = "tile";

            XmlNode dataNode = null;
            for (int i = 0; i < layerNode.ChildNodes.Count; i++)
            {
                XmlNode node = layerNode.ChildNodes.Item(i);
                if (node.LocalName.ToLower() == "data")
                {
                    dataNode = node;
                }
            }

            if (dataNode == null)
            {
                Debug.LogError("TMX parsing error, there is no data node");
            }

            if (dataNode.InnerText == null || dataNode.InnerText.Length == 0)
            {
                Debug.Log("TMX error, data node is empty");
                return false;
            }

            if (dataNode.Attributes["encoding"] != null)
            {
                encoding = dataNode.Attributes["encoding"].Value;
            }

            string compression = "none";

            if (dataNode.Attributes["compression"] != null)
            {
                compression = dataNode.Attributes["compression"].Value;
            }

            if (encoding == "tile")
            {
                Debug.Log("TSX error, <tile> encoding not supported (and is inefficient), use csv, or base64");
                return false;
            }

            if (encoding != "csv" && encoding != "base64")
            {
                Debug.Log("TSX error, " + encoding + " encoding not suported");
                return false;
            }

            if (compression != "none" && compression != "gzip" && compression != "zlib")
            {
                Debug.Log("Compression format " + compression + " not supported, please use gzip or no compression");
                return false;
            }

            if (encoding == "csv" && compression != "none")
            {
                Debug.Log("TMX error, compression not valid for csv encoding format");
                return false;
            }

            if (encoding == "csv")
            {
                bool ret = LoadTMXCSV(dataNode.InnerText, mapLayer, tmxMapWidth, firstGid);
                if (ret == true)
                {
                    mapSize.width = tmxMapWidth;
                    mapSize.height = tmxMapHeight;
                }

                return ret;
            }

            if (encoding == "base64")
            {
                byte[] base64Data = null;
                if (compression == "none")
                {
                    base64Data = System.Convert.FromBase64String(dataNode.InnerText);
                }
                else if (compression == "zlib")
                {
                    base64Data = System.Convert.FromBase64String(dataNode.InnerText);
                    try
                    {
                        base64Data = FESDeflate.Deflate(base64Data);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Failed to decompress TMX file: " + e.ToString());
                        return false;
                    }
                }
                else if (compression == "gzip")
                {
                    Debug.Log("TMX gzip compression format not supported. Please use zlib compression, or no compression.");
                    return false;
                }
                else
                {
                    Debug.Log("TMX compression format " + compression + " unknown.");
                    return false;
                }

                if (base64Data == null)
                {
                    Debug.Log("TMX error, could not decode base64 data");
                    return false;
                }

                if (base64Data.Length % 4 != 0)
                {
                    Debug.Log("TMX error, base64 tile data length is not divisible by 4, it must be");
                    return false;
                }

                bool ret = LoadTMXBase64(base64Data, mapLayer, tmxMapWidth, firstGid);
                if (ret == true)
                {
                    mapSize.width = tmxMapWidth;
                    mapSize.height = tmxMapHeight;
                }

                return ret;
            }

            return false;
        }

        private int XMLCountChildren(XmlNodeList list, string childName)
        {
            if (list == null)
            {
                return 0;
            }

            childName = childName.ToLower();

            int c = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (list.Item(i).LocalName.ToLower() == childName)
                {
                    c++;
                }
            }

            return c;
        }

        private void DecodeTile(uint tileInfo, int[] firstGid, out int tileId, out int flags)
        {
            flags = 0;
            int tmxFlags = (int)((tileInfo & 0xE0000000) >> 29);

            // Translate flags
            switch (tmxFlags)
            {
                case 0: // 000
                    flags = 0;
                    break;
                case 1: // 001
                    // This is not technically correct, but TMX never encodes this value, but if its encoded manually this is the result in editor
                    flags = FES.FLIP_V | FES.ROT_90_CW;
                    break;
                case 2: // 010
                    flags = FES.FLIP_V;
                    break;
                case 3: // 011
                    flags = FES.ROT_90_CCW;
                    break;
                case 4: // 100
                    flags = FES.FLIP_H;
                    break;
                case 5: // 101
                    flags = FES.ROT_90_CW;
                    break;
                case 6: // 110
                    flags = FES.FLIP_H | FES.FLIP_V;
                    break;
                case 7: // 111
                    // This is not technically correct, but TMX never encodes this value, but if its encoded manually this is the result in editor
                    flags = FES.FLIP_H | FES.ROT_90_CW;
                    break;
                default:
                    flags = 0;
                    break;
            }

            tileId = (int)(tileInfo & 0x1FFFFFFF);

            // Check for invalid tileid (out of range) and set it to empty
            if (tileId < 0)
            {
                tileId = FES.SPRITE_EMPTY;
            }

            for (int i = firstGid.Length - 1; i >= 0; i--)
            {
                if (firstGid[i] <= tileId)
                {
                    tileId -= firstGid[i];
                    tileId = tileId < 0 ? FES.SPRITE_EMPTY : tileId;
                    return;
                }
            }

            tileId -= firstGid[0];
            tileId = tileId < 0 ? FES.SPRITE_EMPTY : tileId;
        }

        private bool LoadTMXCSV(string csvData, int mapLayer, int csvWidth, int[] firstGid)
        {
            if (csvData == null)
            {
                return false;
            }

            char[] delimiterChars = { ' ', ',', '\n', '\r' };
            string[] csvValues = csvData.Split(delimiterChars);

            ColorRGBA color = new ColorRGBA(255, 255, 255);

            int row = 0;
            int col = 0;
            for (int i = 0; i < csvValues.Length; i++)
            {
                if (csvValues[i] == null)
                {
                    continue;
                }

                string csvValue = csvValues[i].Trim();
                if (csvValue == string.Empty)
                {
                    continue;
                }

                uint tileInfo;
                if (uint.TryParse(csvValue, out tileInfo))
                {
                    int flags, tileId;
                    DecodeTile(tileInfo, firstGid, out tileId, out flags);

                    SpriteSet(mapLayer, col, row, tileId, color, 0, flags);

                    col++;
                    if (col >= csvWidth)
                    {
                        col = 0;
                        row++;
                    }
                }
            }

            return true;
        }

        private bool LoadTMXBase64(byte[] base64Data, int mapLayer, int csvWidth, int[] firstGid)
        {
            if (base64Data == null)
            {
                return false;
            }

            ColorRGBA tintColor = new ColorRGBA(255, 255, 255);

            int row = 0;
            int col = 0;
            for (int i = 0; i < base64Data.Length; i += 4)
            {
                uint tileInfo = ((uint)base64Data[i + 3] << 24) | ((uint)base64Data[i + 2] << 16) | ((uint)base64Data[i + 1] << 8) | ((uint)base64Data[i]);

                int flags, tileId;
                DecodeTile(tileInfo, firstGid, out tileId, out flags);

                SpriteSet(mapLayer, col, row, tileId, tintColor, 0, flags);

                col++;
                if (col >= csvWidth)
                {
                    col = 0;
                    row++;
                }
            }

            return true;
        }
    }
}
