namespace FESInternal
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Tilemap subsystem
    /// </summary>
    public partial class FESTilemap
    {
        private const int MAX_QUADS_PER_CHUNK_LAYER = FESHW.HW_MAP_CHUNK_WIDTH * FESHW.HW_MAP_CHUNK_HEIGHT;
        private const int MAX_INDECIES_PER_MESH = 6 * MAX_QUADS_PER_CHUNK_LAYER;
        private const int MAX_VERTEX_PER_MESH = 4 * MAX_QUADS_PER_CHUNK_LAYER;

        // Maximum cached meshes before we start releasing them
        private const int MAX_CACHED_MESHES = 128;

        private FESAPI mFESAPI;

        private int[] mLayerSpriteSheets = new int[FESHW.HW_MAX_MAP_LAYERS];

        private ulong mFrameCounter = 0;
        private Tile[] mTiles;
        private Chunk[] mChunks;
        private List<Chunk> mActiveChunks = new List<Chunk>();
        private List<Mesh> mReleasedMeshes = new List<Mesh>();

        private List<ArraySet> mArraySets = new List<ArraySet>();

        private int mChunksStride;
        private int mChunksPerLayer;

        private Size2i mActualMapSize;

        /// <summary>
        /// Initialize the subsystem
        /// </summary>
        /// <param name="api">Subsystem wrapper reference</param>
        /// <returns>True if successful</returns>
        public bool Initialize(FESAPI api)
        {
            mFESAPI = api;

            mActualMapSize = mFESAPI.HW.MapSize;

            // Round up map size to next multiple of chunk size
            if (mActualMapSize.width % FESHW.HW_MAP_CHUNK_WIDTH > 0)
            {
                mActualMapSize.width -= mActualMapSize.width % FESHW.HW_MAP_CHUNK_WIDTH;
                mActualMapSize.width += FESHW.HW_MAP_CHUNK_WIDTH;
            }

            if (mActualMapSize.height % FESHW.HW_MAP_CHUNK_HEIGHT > 0)
            {
                mActualMapSize.height -= mActualMapSize.height % FESHW.HW_MAP_CHUNK_HEIGHT;
                mActualMapSize.height += FESHW.HW_MAP_CHUNK_HEIGHT;
            }

            mTiles = new Tile[mActualMapSize.width * mActualMapSize.height];
            mChunksStride = (mActualMapSize.width / FESHW.HW_MAP_CHUNK_WIDTH) + 1;
            mChunksPerLayer = mChunksStride * ((mActualMapSize.height / FESHW.HW_MAP_CHUNK_HEIGHT) + 1);
            mChunks = new Chunk[mChunksPerLayer * mFESAPI.HW.MapLayers];

            if (mTiles == null || mChunks == null)
            {
                return false;
            }

            for (int i = 0; i < mTiles.Length; i++)
            {
                mTiles[i] = null;
            }

            for (int i = 0; i < mChunks.Length; i++)
            {
                mChunks[i] = null;
            }

            int size = 4;
            while (true)
            {
                mArraySets.Add(new ArraySet(size));

                if (size >= MAX_VERTEX_PER_MESH)
                {
                    break;
                }

                size *= 2;
                if (size > MAX_VERTEX_PER_MESH)
                {
                    size = MAX_VERTEX_PER_MESH;
                }
            }

            return true;
        }

        /// <summary>
        /// Set the sprite sheet for the map layer
        /// </summary>
        /// <param name="layer">Map layer</param>
        /// <param name="spriteSheetIndex">Sprite sheet index</param>
        public void MapLayerSpriteSheetSet(int layer, int spriteSheetIndex)
        {
            if (mLayerSpriteSheets[layer] != spriteSheetIndex)
            {
                Texture2D oldTexture = null;
                if (mLayerSpriteSheets[layer] >= 0)
                {
                    oldTexture = mFESAPI.Renderer.SpriteSheets[mLayerSpriteSheets[layer]].texture;
                }

                Texture2D newTexture = null;
                if (spriteSheetIndex >= 0)
                {
                    newTexture = mFESAPI.Renderer.SpriteSheets[spriteSheetIndex].texture;
                }

                mLayerSpriteSheets[layer] = spriteSheetIndex;

                // Regen not required
                if (oldTexture == newTexture)
                {
                    return;
                }

                // Regen required
                if ((oldTexture == null && newTexture != null) || (oldTexture != null && newTexture == null))
                {
                    RegenLayerChunks(layer);
                    return;
                }
                else if (oldTexture.width != newTexture.width || oldTexture.height != newTexture.height)
                {
                    RegenLayerChunks(layer);
                    return;
                }
            }
        }

        /// <summary>
        /// Draw map layer at given position offset
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <param name="pos">Position</param>
        public void DrawMapLayer(int layer, Vector2i pos)
        {
            var renderer = mFESAPI.Renderer;
            var renderTarget = renderer.CurrentRenderTexture();

            var oldCamera = renderer.CameraGet();
            var newCamera = renderer.CameraGet();
            newCamera.x -= pos.x;
            newCamera.y -= pos.y;
            renderer.CameraSet(newCamera);

            FESRenderer.SpriteSheet spriteSheet = renderer.SpriteSheets[mLayerSpriteSheets[layer]];

            int drawX = newCamera.x;
            int drawY = newCamera.y;
            int drawYStart = drawY;

            int chunkDrawWidth = spriteSheet.spriteSize.width * FESHW.HW_MAP_CHUNK_WIDTH;
            int chunkDrawHeight = spriteSheet.spriteSize.height * FESHW.HW_MAP_CHUNK_HEIGHT;

            int x0 = drawX;
            int y0 = drawY;
            int x1 = x0 + renderTarget.width;
            int y1 = y0 + renderTarget.height;

            if (newCamera.x > 0 && newCamera.x % chunkDrawWidth != 0)
            {
                x1 += newCamera.x % chunkDrawWidth;
            }

            if (newCamera.x < 0)
            {
                x1 += chunkDrawWidth;
            }

            if (newCamera.y > 0 && newCamera.y % chunkDrawHeight != 0)
            {
                y1 += newCamera.y % chunkDrawHeight;
            }

            if (newCamera.y < 0)
            {
                y1 += chunkDrawHeight;
            }

            for (int x = x0; x < x1; x += chunkDrawWidth)
            {
                int cx = x / spriteSheet.spriteSize.width;

                for (int y = y0; y < y1; y += chunkDrawHeight)
                {
                    int cy = y / spriteSheet.spriteSize.height;

                    var chunk = GetChunk(layer, cx, cy, false);
                    if (chunk != null)
                    {
                        // Generate chunk if needed
                        if (chunk.dirty || chunk.released)
                        {
                            if (chunk.released && chunk.mesh == null)
                            {
                                chunk.mesh = GetNewMesh();
                                if (chunk.mesh == null)
                                {
                                    Debug.LogError("Could not get chunk for tilemap mesh!");
                                }
                            }

                            GenerateChunk(layer, cx, cy);
                            chunk.dirty = false;
                            chunk.released = false;
                        }

                        // Handy tilemap debugging code, draws a random colored rect where the tilemap will be drawn
                        // TODO: This became invalid after pos was backed into camera position... update if needed
#if false
                        if (layer == 0)
                        {
                            FES.AlphaSet(64);
                            Random.InitState(x + y * 1000);
                            FES.DrawRectFill(new Rect2i(x + pos.x, (y - pos.y),
                                FESHW.HW_MAP_CHUNK_WIDTH * (spriteSheet.spriteSize.width),
                                FESHW.HW_MAP_CHUNK_HEIGHT * (spriteSheet.spriteSize.height)),
                                Random.Range(0, 20));
                            FES.AlphaSet(255);
                        }
#endif

                        if (mLayerSpriteSheets[layer] >= 0 && mLayerSpriteSheets[layer] < FESHW.HW_MAX_SPRITESHEETS)
                        {
                            var chunkPos = new Vector2i(x, y);

                            chunkPos -= newCamera;
                            chunkPos.x -= x % (spriteSheet.spriteSize.width * FESHW.HW_MAP_CHUNK_WIDTH);
                            chunkPos.y -= y % (spriteSheet.spriteSize.height * FESHW.HW_MAP_CHUNK_HEIGHT);

                            renderer.DrawPreparedMesh(chunk.mesh, new Rect2i(chunkPos.x, chunkPos.y, chunkDrawWidth - 1, chunkDrawHeight - 1), true, renderer.SpriteSheets[mLayerSpriteSheets[layer]].texture);
                        }
                    }

                    drawY += chunkDrawHeight;
                }

                drawX += chunkDrawWidth;
                drawY = drawYStart;
            }

            renderer.CameraSet(oldCamera);
        }

        /// <summary>
        /// Set sprite for the tile at x, y
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="sprite">Sprite index</param>
        /// <param name="tintColor">Tint color</param>
        /// <param name="swapIndex">Palette swap index</param>
        /// <param name="flags">Flags</param>
        public void SpriteSet(int layer, int x, int y, int sprite, ColorRGBA tintColor, int swapIndex = -1, int flags = 0)
        {
            if (sprite < 0)
            {
                return;
            }

            if (layer < 0 || layer >= mFESAPI.HW.MapLayers)
            {
                return;
            }

            // Bounds check on user defined map size, not our internal map size
            if (x < 0 || y < 0 || x >= mFESAPI.HW.MapSize.width || y >= mFESAPI.HW.MapSize.height)
            {
                return;
            }

            var t = GetTile(x, y, true);

            // TODO: This will not accept tint color changes! Nor flags...
            if (t == null || (t.sprite[layer] == sprite && t.swapIndex[layer] == swapIndex && t.tintColor[layer] == tintColor && t.flags[layer] == flags))
            {
                return;
            }

            int oldSprite = t.sprite[layer];

            // If there are no changes then do nothing
            if (t.sprite[layer] == sprite &&
                t.swapIndex[layer] == (byte)swapIndex &&
                t.tintColor[layer] == tintColor &&
                t.flags[layer] == (byte)flags)
            {
                return;
            }

            t.sprite[layer] = sprite;
            t.swapIndex[layer] = (byte)swapIndex;
            t.tintColor[layer] = tintColor;
            t.flags[layer] = (byte)flags;

            var chunk = GetChunk(layer, x, y, true);
            chunk.dirty = true;

            // Update count of nonempty tiles
            if (oldSprite >= FES.SPRITE_EMPTY && sprite < FES.SPRITE_EMPTY)
            {
                chunk.nonEmptyTiles++;
            }
            else if (oldSprite < FES.SPRITE_EMPTY && sprite >= FES.SPRITE_EMPTY)
            {
                chunk.nonEmptyTiles--;
            }
        }

        /// <summary>
        /// Get sprite index at position
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>Sprite index</returns>
        public int SpriteGet(int layer, int x, int y)
        {
            var t = GetTile(x, y, false);
            if (t == null)
            {
                return FES.SPRITE_EMPTY;
            }

            return t.sprite[layer];
        }

        /// <summary>
        /// Set user data for tile at x, y
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="data">Data</param>
        public void DataSet<T>(int x, int y, T data)
        {
            var t = GetTile(x, y, true);
            if (t == null)
            {
                return;
            }

            t.data = (object)data;
        }

        /// <summary>
        /// Get data for sprite at x, y
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>User data</returns>
        public object DataGet<T>(int x, int y)
        {
            var t = GetTile(x, y, false);
            if (t == null)
            {
                return null;
            }

            return (T)t.data;
        }

        /// <summary>
        /// Clear a map layer, or all layers if -1
        /// </summary>
        /// <param name="layer">Layer</param>
        public void Clear(int layer = -1)
        {
            // Wipe all layers
            if (layer == -1)
            {
                for (int i = 0; i < mTiles.Length; i++)
                {
                    mTiles[i] = null;
                }

                for (int i = 0; i < mChunks.Length; i++)
                {
                    if (mChunks[i] != null)
                    {
                        if (mChunks[i].mesh != null)
                        {
                            mChunks[i].mesh.Clear();
                            ReleaseMesh(mChunks[i].mesh);
                            mChunks[i].mesh = null;
                        }

                        mChunks[i] = null;
                    }
                }

                mActiveChunks.Clear();

                return;
            }

            // Wipe specific layer
            if (layer < 0 || layer >= mFESAPI.HW.MapLayers)
            {
                return;
            }

            for (int i = 0; i < mTiles.Length; i++)
            {
                if (mTiles[i] != null)
                {
                    mTiles[i].sprite[layer] = FES.SPRITE_EMPTY;
                    mTiles[i].swapIndex[layer] = FESInternal.FESHW.HW_PALETTE_SWAPS;
                    mTiles[i].flags[layer] = 0;
                }
            }

            int chunksPerLayer = ((mActualMapSize.width / FESHW.HW_MAP_CHUNK_WIDTH) + 1) * ((mActualMapSize.height / FESHW.HW_MAP_CHUNK_HEIGHT) + 1);
            for (int i = chunksPerLayer * layer; i < chunksPerLayer * (layer + 1); i++)
            {
                if (mChunks[i] != null)
                {
                    if (mChunks[i].mesh != null)
                    {
                        mChunks[i].mesh.Clear();
                        ReleaseMesh(mChunks[i].mesh);
                        mChunks[i].mesh = null;
                    }

                    mChunks[i] = null;
                }
            }

            // Remove all tracked active chunks for this layer
            for (int i = mActiveChunks.Count - 1; i >= 0; i--)
            {
                if (mActiveChunks[i].chunkLayer == layer)
                {
                    mActiveChunks.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Do some cleanup at frame end
        /// </summary>
        public void FrameEnd()
        {
            mFrameCounter++;

            // Every 100 frames do a bit of maintenance, release stale chunks
            if (mFrameCounter % 100 == 0)
            {
                for (int i = mActiveChunks.Count - 1; i >= 0; i--)
                {
                    var chunk = mActiveChunks[i];
                    if (mFrameCounter - chunk.lastRelevantFrame > 200)
                    {
                        if (chunk.mesh != null)
                        {
                            chunk.mesh.Clear();
                            ReleaseMesh(chunk.mesh);
                            chunk.mesh = null;
                        }

                        // Mark the chunk released so its recreated next time we fetch it
                        chunk.released = true;

                        mActiveChunks.RemoveAt(i);
                    }
                }
            }
        }

        private ArraySet GetArraySet(int minVerts)
        {
            for (int i = 0; i < mArraySets.Count; i++)
            {
                var arraySet = mArraySets[i];
                if (arraySet.MaxVerts >= minVerts)
                {
                    return arraySet;
                }
            }

            return null;
        }

        private Chunk GetChunk(int layer, int x, int y, bool create)
        {
            if (x < 0 || x >= mActualMapSize.width || y < 0 || y >= mActualMapSize.height)
            {
                return null;
            }

            x /= FESHW.HW_MAP_CHUNK_WIDTH;
            y /= FESHW.HW_MAP_CHUNK_HEIGHT;

            int i = x + (y * mChunksStride);
            i += layer * mChunksPerLayer;

            if (i < 0 || i >= mChunks.Length)
            {
                Debug.LogError("Chunk out of bounds");
                return null;
            }

            var chunk = mChunks[i];

            if (chunk == null && create)
            {
                chunk = new Chunk();
                chunk.mesh = GetNewMesh();

                if (chunk.mesh == null)
                {
                    Debug.LogError("Could not get chunk for tilemap mesh!");
                }

                chunk.chunkLayer = layer;
                chunk.x = x;
                chunk.y = y;
                mChunks[i] = chunk;
            }

            if (chunk != null)
            {
                chunk.lastRelevantFrame = mFrameCounter;
            }

            return chunk;
        }

        private Tile GetTile(int x, int y, bool create = false)
        {
            // Boundary check on user defined size not internal one
            if (x < 0 || x >= mFESAPI.HW.MapSize.width || y < 0 || y >= mFESAPI.HW.MapSize.height)
            {
                return null;
            }

            var tile = mTiles[x + (y * mActualMapSize.width)];

            // If tile is null then create one if create flag is true, and initialize it
            if (tile == null && create)
            {
                tile = new Tile(mFESAPI.HW.MapLayers);

                for (int i = 0; i < mFESAPI.HW.MapLayers; i++)
                {
                    tile.sprite[i] = FES.SPRITE_EMPTY;
                    tile.swapIndex[i] = FESInternal.FESHW.HW_PALETTE_SWAPS;
                    tile.flags[i] = 0;
                }

                mTiles[x + (y * mActualMapSize.width)] = tile;
            }

            return tile;
        }

        private Mesh GetNewMesh()
        {
            Mesh mesh;
            if (mReleasedMeshes.Count > 0)
            {
                mesh = mReleasedMeshes[mReleasedMeshes.Count - 1];
                mReleasedMeshes.RemoveAt(mReleasedMeshes.Count - 1);
                return mesh;
            }

            mesh = new Mesh();
            mesh.MarkDynamic();
            return mesh;
        }

        private void GenerateChunk(int layer, int offsetX, int offsetY)
        {
            // Clip to upper left corner of the chunk
            int x0 = offsetX - (offsetX % FESHW.HW_MAP_CHUNK_WIDTH);
            int y0 = offsetY - (offsetY % FESHW.HW_MAP_CHUNK_HEIGHT);
            int x1 = x0 + FESHW.HW_MAP_CHUNK_WIDTH;
            int y1 = y0 + FESHW.HW_MAP_CHUNK_HEIGHT;

            int i = 0;
            int j = 0;

            int spriteSheetIndex = mLayerSpriteSheets[layer];
            if (spriteSheetIndex < 0)
            {
                spriteSheetIndex = 0;
            }

            FESRenderer.SpriteSheet spriteSheet = mFESAPI.Renderer.SpriteSheets[mLayerSpriteSheets[layer]];
            int spriteSheetWidth = spriteSheet.spriteSize.width;
            int spriteSheetHeight = spriteSheet.spriteSize.height;
            int spriteSheetTextureWidth = spriteSheet.textureSize.width;
            int spriteSheetTextureHeight = spriteSheet.textureSize.height;

            int maxSpriteIndex = spriteSheet.columns * spriteSheet.rows;

            // Tilemaps chunks are always rendered from user texture, so the flags should be 1 = user texture, 0 = system texture
            Vector2 chunkFlags = new Vector2(1, 0);
            var chunk = GetChunk(layer, x0, y0, true);
            var arraySet = GetArraySet(chunk.nonEmptyTiles * 4);

            var ChunkVerticies = arraySet.ChunkVerticies;
            var ChunkUvs = arraySet.ChunkUvs;
            var ChunkFlags = arraySet.ChunkFlags;
            var ChunkColors = arraySet.ChunkColors;
            var ChunkIndecies = arraySet.ChunkIndecies;

            int spritesGenerated = 0;
            bool doneEarly = false;
            for (int x = x0; x < x1 && !doneEarly; x++)
            {
                for (int y = y0; y < y1; y++)
                {
                    var t = GetTile(x, y, false);
                    if (t == null)
                    {
                        continue;
                    }

                    int spriteIndex = t.sprite[layer];

                    if (spriteIndex < 0 || spriteIndex >= maxSpriteIndex)
                    {
                        continue;
                    }

                    int dx0 = x * spriteSheetWidth;
                    int dy0 = y * spriteSheetHeight;
                    int dx1 = (x + 1) * spriteSheetWidth;
                    int dy1 = (y + 1) * spriteSheetHeight;

                    float ux0raw = ((spriteIndex % spriteSheet.columns) * spriteSheetWidth) / (float)spriteSheetTextureWidth;
                    float uy0raw = ((spriteIndex / spriteSheet.columns) * spriteSheetHeight) / ((float)spriteSheetTextureHeight);
                    float ux1raw = ux0raw + (spriteSheetWidth / (float)spriteSheetTextureWidth);
                    float uy1raw = uy0raw + (spriteSheetHeight / (float)spriteSheetTextureHeight);

                    float ux0, uy0, ux1, uy1;

                    int flags = (int)t.flags[layer];

                    if ((flags & FES.FLIP_H) == 0)
                    {
                        ux0 = ux0raw;
                        ux1 = ux1raw;
                    }
                    else
                    {
                        ux0 = ux1raw;
                        ux1 = ux0raw;
                    }

                    if ((flags & FES.FLIP_V) == 0)
                    {
                        uy0 = uy0raw;
                        uy1 = uy1raw;
                    }
                    else
                    {
                        uy0 = uy1raw;
                        uy1 = uy0raw;
                    }

                    uy0 = 1.0f - uy0;
                    uy1 = 1.0f - uy1;

                    int swapIndex = t.swapIndex[layer] + 1;

                    Color32 color = t.tintColor[layer].ToColor32();

                    if ((flags & FES.ROT_90_CW) == 0)
                    {
                        ChunkVerticies[i].x = dx0;
                        ChunkVerticies[i].y = dy0;
                        ChunkVerticies[i].z = swapIndex;
                        ChunkUvs[i].x = ux0;
                        ChunkUvs[i].y = uy0;
                        ChunkFlags[i] = chunkFlags;
                        ChunkColors[i] = color;

                        i++;

                        ChunkVerticies[i].x = dx1;
                        ChunkVerticies[i].y = dy0;
                        ChunkVerticies[i].z = swapIndex;
                        ChunkUvs[i].x = ux1;
                        ChunkUvs[i].y = uy0;
                        ChunkFlags[i] = chunkFlags;
                        ChunkColors[i] = color;

                        i++;

                        ChunkVerticies[i].x = dx1;
                        ChunkVerticies[i].y = dy1;
                        ChunkVerticies[i].z = swapIndex;
                        ChunkUvs[i].x = ux1;
                        ChunkUvs[i].y = uy1;
                        ChunkFlags[i] = chunkFlags;
                        ChunkColors[i] = color;

                        i++;

                        ChunkVerticies[i].x = dx0;
                        ChunkVerticies[i].y = dy1;
                        ChunkVerticies[i].z = swapIndex;
                        ChunkUvs[i].x = ux0;
                        ChunkUvs[i].y = uy1;
                        ChunkFlags[i] = chunkFlags;
                        ChunkColors[i] = color;

                        i++;
                    }
                    else
                    {
                        ChunkVerticies[i].x = dx1;
                        ChunkVerticies[i].y = dy0;
                        ChunkVerticies[i].z = swapIndex;
                        ChunkUvs[i].x = ux0;
                        ChunkUvs[i].y = uy0;
                        ChunkFlags[i] = chunkFlags;
                        ChunkColors[i] = color;

                        i++;

                        ChunkVerticies[i].x = dx1;
                        ChunkVerticies[i].y = dy1;
                        ChunkVerticies[i].z = swapIndex;
                        ChunkUvs[i].x = ux1;
                        ChunkUvs[i].y = uy0;
                        ChunkFlags[i] = chunkFlags;
                        ChunkColors[i] = color;

                        i++;

                        ChunkVerticies[i].x = dx0;
                        ChunkVerticies[i].y = dy1;
                        ChunkVerticies[i].z = swapIndex;
                        ChunkUvs[i].x = ux1;
                        ChunkUvs[i].y = uy1;
                        ChunkFlags[i] = chunkFlags;
                        ChunkColors[i] = color;

                        i++;

                        ChunkVerticies[i].x = dx0;
                        ChunkVerticies[i].y = dy0;
                        ChunkVerticies[i].z = swapIndex;
                        ChunkUvs[i].x = ux0;
                        ChunkUvs[i].y = uy1;
                        ChunkFlags[i] = chunkFlags;
                        ChunkColors[i] = color;

                        i++;
                    }

                    ChunkIndecies[j++] = i - 4;
                    ChunkIndecies[j++] = i - 3;
                    ChunkIndecies[j++] = i - 2;

                    ChunkIndecies[j++] = i - 2;
                    ChunkIndecies[j++] = i - 1;
                    ChunkIndecies[j++] = i - 4;

                    spritesGenerated++;

                    if (spritesGenerated >= chunk.nonEmptyTiles)
                    {
                        doneEarly = true;
                        break;
                    }
                }
            }

            if (j > 0)
            {
                if (chunk != null && chunk.mesh != null)
                {
                    System.Array.Clear(ChunkIndecies, j, arraySet.MaxIndecies - j);

                    // No need to clear mesh if it's the same size as new vertex data, the values will just
                    // be overwritten
                    if (chunk.prevVertCount != ChunkVerticies.Length)
                    {
                        chunk.mesh.Clear();
                    }

                    chunk.mesh.vertices = ChunkVerticies;
                    chunk.mesh.uv = ChunkUvs;
                    chunk.mesh.uv2 = ChunkFlags;
                    chunk.mesh.colors32 = ChunkColors;
                    chunk.mesh.SetIndices(ChunkIndecies, MeshTopology.Triangles, 0, false);

                    chunk.prevVertCount = ChunkVerticies.Length;

                    chunk.mesh.UploadMeshData(false);

                    mActiveChunks.Add(chunk);
                }
            }
            else
            {
                ReleaseChunk(layer, x0, y0);
            }
        }

        private void RegenLayerChunks(int layer)
        {
            if (layer < 0 || layer >= mFESAPI.HW.MapLayers)
            {
                return;
            }

            int chunksPerLayer = mChunksStride * ((mActualMapSize.height / FESHW.HW_MAP_CHUNK_HEIGHT) + 1);
            for (int i = chunksPerLayer * layer; i < chunksPerLayer * (layer + 1); i++)
            {
                if (mChunks[i] != null)
                {
                    mChunks[i].dirty = true;
                }
            }
        }

        private void ReleaseChunk(int layer, int x, int y)
        {
            if (x < 0 || x >= mActualMapSize.width || y < 0 || y >= mActualMapSize.height)
            {
                return;
            }

            x /= FESHW.HW_MAP_CHUNK_WIDTH;
            y /= FESHW.HW_MAP_CHUNK_HEIGHT;

            int i = x + (y * mChunksStride);
            i += layer * mChunksPerLayer;

            if (i < 0 || i >= mChunks.Length)
            {
                Debug.LogError("Chunk out of bounds");
                return;
            }

            if (mChunks[i] != null && mChunks[i].mesh != null)
            {
                ReleaseMesh(mChunks[i].mesh);
                mChunks[i].mesh = null;
            }

            mChunks[i] = null;
        }

        private void ReleaseMesh(Mesh mesh)
        {
            if (mReleasedMeshes.Count < MAX_CACHED_MESHES)
            {
                mReleasedMeshes.Add(mesh);
            }
            else
            {
                GameObject.DestroyImmediate(mesh, true);
            }
        }

        private class Tile
        {
            public int[] sprite = null;
            public byte[] swapIndex = null;
            public ColorRGBA[] tintColor;
            public byte[] flags = null;

            public object data = null;

            public Tile(int mapLayers)
            {
                sprite = new int[mapLayers];
                swapIndex = new byte[mapLayers];
                tintColor = new ColorRGBA[mapLayers];
                flags = new byte[mapLayers];
            }
        }

        private class Chunk
        {
            public Mesh mesh;
            public bool dirty = false;
            public bool released = false;
            public int chunkLayer = -1;
            public int nonEmptyTiles = 0;
            public int prevVertCount = 0;

            /// <summary>
            /// Last frame in which the chunk was still needed
            /// </summary>
            public ulong lastRelevantFrame = 0;
            public int x, y;
        }

        private class ArraySet
        {
            public Vector3[] ChunkVerticies;
            public Vector2[] ChunkUvs;
            public Vector2[] ChunkFlags;
            public Color32[] ChunkColors;
            public int[] ChunkIndecies;

            public int MaxVerts;
            public int MaxIndecies;

            public ArraySet(int maxVerts)
            {
                MaxVerts = maxVerts;
                MaxIndecies = maxVerts / 4 * 6;

                ChunkVerticies = new Vector3[MaxVerts];
                ChunkUvs = new Vector2[MaxVerts];
                ChunkFlags = new Vector2[MaxVerts];
                ChunkColors = new Color32[MaxVerts];
                ChunkIndecies = new int[MaxIndecies];
            }
        }
    }
}
