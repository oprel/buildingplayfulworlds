namespace FESInternal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Palette subsystem
    /// </summary>
    public class FESPalette
    {
        private FESAPI mFESAPI;
        private Color32[] mColors = null;
        private int[,] mPaletteSwap = null;

        private int mPaletteTextureOffsetX = 0;
        private int mPaletteTextureOffsetY = 0;

        // FES default color palette if no palette is provided by user
        private ColorRGBA[] mDefaultPal = new ColorRGBA[]
        {
            new ColorRGBA(0, 0, 0),
            new ColorRGBA(29, 29, 29),
            new ColorRGBA(49, 49, 49),
            new ColorRGBA(116, 116, 116),
            new ColorRGBA(169, 169, 169),
            new ColorRGBA(222, 222, 222),
            new ColorRGBA(255, 255, 255),
            new ColorRGBA(219, 171, 119),
            new ColorRGBA(164, 119, 70),
            new ColorRGBA(79, 51, 21),
            new ColorRGBA(41, 29, 17),
            new ColorRGBA(41, 17, 21),
            new ColorRGBA(79, 21, 29),
            new ColorRGBA(122, 52, 62),
            new ColorRGBA(164, 70, 83),
            new ColorRGBA(219, 119, 133),
            new ColorRGBA(219, 150, 119),
            new ColorRGBA(164, 99, 70),
            new ColorRGBA(79, 37, 21),
            new ColorRGBA(13, 27, 29),
            new ColorRGBA(17, 52, 55),
            new ColorRGBA(50, 104, 108),
            new ColorRGBA(127, 213, 221),
            new ColorRGBA(74, 198, 138),
            new ColorRGBA(54, 150, 104),
            new ColorRGBA(35, 101, 71),
            new ColorRGBA(25, 69, 49),
            new ColorRGBA(20, 56, 39),
            new ColorRGBA(71, 27, 67),
            new ColorRGBA(121, 47, 115),
            new ColorRGBA(153, 59, 145),
            new ColorRGBA(182, 70, 173)
        };

        /// <summary>
        /// Initialize subsystem
        /// </summary>
        /// <param name="api">Subsystem wrapper reference</param>
        /// <returns>True if successful</returns>
        public bool Initialize(FESAPI api)
        {
            if (api == null)
            {
                return false;
            }

            mFESAPI = api;

            mPaletteTextureOffsetX = 0;
            mPaletteTextureOffsetY = FESHW.HW_SYSTEM_TEXTURE_HEIGHT - FESHW.HW_PALETTE_SWAPS - 2;

            // Try to load palette texture if one exists
            Color32[] palettePixels = null;
            var paletteTexture = (Texture2D)Resources.Load(mFESAPI.HW.Palette);
            if (paletteTexture != null)
            {
                palettePixels = paletteTexture.GetPixels32();
            }
            else if (mFESAPI.HW.Palette != null)
            {
                Debug.LogError("Could not load palette from " + mFESAPI.HW.Palette + ", it must be under the Assets/Resources folder. Using default palette");
                palettePixels = null;
            }

            if (palettePixels != null && palettePixels.Length < 1)
            {
                Debug.LogError("Invalid palette texture from " + mFESAPI.HW.Palette + ", using default palette");
                palettePixels = null;
            }

            if (palettePixels != null && palettePixels.Length > FESHW.HW_MAX_SUPPORTED_PALETTE_SIZE)
            {
                Debug.LogError("Indexed color palette cannot contain more than " + FESHW.HW_MAX_SUPPORTED_PALETTE_SIZE +
                    " colors. Each pixel in the palette file counts as one color in the color palette.");
                palettePixels = null;
            }

            if (palettePixels != null)
            {
                mFESAPI.HW.PaletteColorCount = (byte)palettePixels.Length;
            }
            else
            {
                mFESAPI.HW.PaletteColorCount = (byte)mDefaultPal.Length;
            }

            mColors = new Color32[(FESHW.HW_PALETTE_SWAPS + 2) * mFESAPI.HW.PaletteTotalColors];
            mPaletteSwap = new int[(FESHW.HW_PALETTE_SWAPS + 1), mFESAPI.HW.PaletteTotalColors];

            for (int s = 0; s < FESHW.HW_PALETTE_SWAPS + 1; s++)
            {
                for (int i = 0; i < mFESAPI.HW.PaletteTotalColors; i++)
                {
                    mPaletteSwap[s, i] = i;
                }
            }

            int px = 0;
            int py = 0;

            int j;

            for (int s = 0; s < FESHW.HW_PALETTE_SWAPS + 1; s++)
            {
                j = 0;
                if (paletteTexture != null)
                {
                    px = 0;
                    py = paletteTexture.height - 1;
                }

                int rowOffset = s * mFESAPI.HW.PaletteTotalColors;

                for (int i = 0; i < mFESAPI.HW.PaletteColorCount; i++)
                {
                    // If we have a palette pixels from palette texture then use those, otherwise, use default palette
                    if (palettePixels == null)
                    {
                        mColors[i + rowOffset] = mDefaultPal[j].ToColor32();
                        j++;
                    }
                    else
                    {
                        int k = (py * paletteTexture.width) + px;
                        if (k >= 0 && k < palettePixels.Length)
                        {
                            mColors[i + rowOffset] = new Color32(palettePixels[k].r, palettePixels[k].g, palettePixels[k].b, palettePixels[k].a);
                        }
                        else
                        {
                            mColors[i + rowOffset] = new Color32(mColors[j - 1].r, mColors[j - 1].g, mColors[j - 1].b, mColors[j - 1].a);
                        }

                        px++;
                        if (px >= paletteTexture.width)
                        {
                            px = 0;
                            py--;
                        }

                        j++;
                    }
                }

                mColors[mFESAPI.HW.ColorSystemTransparent + rowOffset] = new Color32(0, 0, 0, 0);
                mColors[mFESAPI.HW.ColorSystemBlack + rowOffset] = new Color32(0, 0, 0, 255);
                mColors[mFESAPI.HW.ColorSystemWhite + rowOffset] = new Color32(255, 255, 255, 255);
                mColors[mFESAPI.HW.ColorSystemPurple + rowOffset] = new Color32(255, 0, 255, 255);
            }

            // Next is a row of color indecies used to render solid shapes
            for (int i = 0; i < mFESAPI.HW.PaletteTotalColors; i++)
            {
                mColors[i + ((FESHW.HW_PALETTE_SWAPS + 1) * mFESAPI.HW.PaletteTotalColors)] = new Color32((byte)i, (byte)i, (byte)i, 255);
            }

            UpdateTextureBlock();
            UpdateFontColors();

            return true;
        }

        /// <summary>
        /// Set RGB of color at given color index
        /// </summary>
        /// <param name="colorIndex">Color index</param>
        /// <param name="color">New Color</param>
        public void PaletteColorSet(int colorIndex, ColorRGBA color)
        {
            if (colorIndex < 0 || colorIndex >= mFESAPI.HW.PaletteColorCount)
            {
                return;
            }

            mColors[colorIndex] = color.ToColor32();

            UpdateSwaps();
            UpdateFontColors();
        }

        /// <summary>
        /// Set palette from given palette file
        /// </summary>
        /// <param name="filename">Filename of the palette</param>
        public void PaletteColorSet(string filename)
        {
            // Try to load palette texture if one exists
            var paletteTexture = (Texture2D)Resources.Load(filename);
            if (paletteTexture == null)
            {
                Debug.LogError("Could not load palette from " + filename + ", it must be under the Assets/Resources folder.");
                return;
            }

            var palettePixels = paletteTexture.GetPixels32();

            if (palettePixels.Length < 1)
            {
                Debug.LogError("Invalid palette texture from " + filename);
                return;
            }

            if (palettePixels.Length != mFESAPI.HW.PaletteColorCount)
            {
                Debug.LogError("New color palette must be the same size as the existing color palette (" + mFESAPI.HW.PaletteColorCount + " colors).");
                return;
            }

            int px = 0;
            int py = paletteTexture.height - 1;

            for (int i = 0; i < mFESAPI.HW.PaletteColorCount; i++)
            {
                int k = (py * paletteTexture.width) + px;
                if (k >= 0 && k < palettePixels.Length)
                {
                    PaletteColorSet(i, new ColorRGBA(palettePixels[k].r, palettePixels[k].g, palettePixels[k].b, palettePixels[k].a));
                }

                px++;
                if (px >= paletteTexture.width)
                {
                    px = 0;
                    py--;
                }
            }
        }

        /// <summary>
        /// Setup palette swap
        /// </summary>
        /// <param name="swapIndex">Swap index</param>
        /// <param name="colorIndexes">Color indexes</param>
        public void PaletteSwapSetup(int swapIndex, int[] colorIndexes)
        {
            bool changed = false;

            if (swapIndex <= 0 || swapIndex >= FESHW.HW_PALETTE_SWAPS + 1)
            {
                return;
            }

            int i = 0;
            if (colorIndexes != null)
            {
                for (; i < colorIndexes.Length; i++)
                {
                    int c = colorIndexes[i];
                    if (c < 0 || c >= mFESAPI.HW.PaletteColorCount)
                    {
                        c = mFESAPI.HW.ColorSystemTransparent;
                    }

                    if (mPaletteSwap[swapIndex, i] != c)
                    {
                        changed = true;
                    }

                    mPaletteSwap[swapIndex, i] = c;
                }
            }

            if (changed)
            {
                // Remaining colors match original indecies, not swaped
                for (; i < mFESAPI.HW.PaletteColorCount; i++)
                {
                    mPaletteSwap[swapIndex, i] = i;
                }

                UpdateSwaps();
                UpdateFontColors();
            }
        }

        /// <summary>
        /// Get Color32 at color index
        /// </summary>
        /// <param name="i">Index</param>
        /// <returns>Color</returns>
        public Color32 ColorAtIndex(int i)
        {
            if (i < 0 || i >= mFESAPI.HW.PaletteColorCount)
            {
                return new Color32(0, 0, 0, 0);
            }

            return mColors[i];
        }

        /// <summary>
        /// Palettize the sprites texture, turning RGB colors into indecies into color palette
        /// </summary>
        /// <param name="texture">Texture to palettize</param>
        public void PalettizeSprites(Texture2D texture)
        {
            var pixels = texture.GetPixels32();
            if (pixels == null || pixels.Length == 0)
            {
                return;
            }

#if UNITY_EDITOR
            bool invalidColors = false;
#endif
            // Prescan for invalid colors so we can dump the texture before changing the valid colors into indexes
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a == 0)
                {
                    pixels[i].r = mColors[0].r;
                    pixels[i].g = mColors[0].g;
                    pixels[i].b = mColors[0].b;
                }

                int colorIndex = ColorIndex(pixels[i]);

                // If color not found in palette then it can't be loaded. Color it purple
                if (colorIndex == -1)
                {
                    pixels[i] = new Color32(255, 0, 255, 255);
#if UNITY_EDITOR
                    invalidColors = true;
#endif
                }
            }

#if UNITY_EDITOR
            if (invalidColors)
            {
                Debug.LogError("SpriteSheet has colors that are not compatible with your color palette. They have been highlighted in purple, " +
                    "view InvalidSpritePixels.png in your project root folder to see the problematic pixels. If you think this is incorrect try " +
                    "reimporting the texture into Unity, making sure it is imported Point filter mode, with no compression.");
                texture.SetPixels32(pixels);
                texture.Apply();
                byte[] bytes = texture.EncodeToPNG();
                System.IO.File.WriteAllBytes(Application.dataPath + "/../InvalidSpritePixels.png", bytes);
            }
#endif

            for (int i = 0; i < pixels.Length; i++)
            {
                int colorIndex = ColorIndex(pixels[i]);

                // If color not found in palette then it can't be loaded. Color it purple
                if (colorIndex == -1)
                {
                    pixels[i] = new Color32(mFESAPI.HW.ColorSystemPurple, mFESAPI.HW.ColorSystemPurple, mFESAPI.HW.ColorSystemPurple, 255);
                }
                else
                {
                    pixels[i] = new Color32((byte)colorIndex, (byte)colorIndex, (byte)colorIndex, 255);
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();
        }

        private void UpdateSwaps()
        {
            for (int s = 1; s < FESHW.HW_PALETTE_SWAPS + 1; s++)
            {
                int rowOffset = s * mFESAPI.HW.PaletteTotalColors;

                for (int i = 0; i < mFESAPI.HW.PaletteColorCount; i++)
                {
                    int c = mPaletteSwap[s, i];
                    mColors[i + rowOffset] = mColors[c];
                }
            }

            UpdateTextureBlock();
        }

        private void UpdateFontColors()
        {
            FESRenderer.TextureCopyBlock cb;
            cb.rect = new Rect2i(mPaletteTextureOffsetX, mPaletteTextureOffsetY - mFESAPI.HW.PaletteTotalColors, 1, mFESAPI.HW.PaletteTotalColors);
            cb.colors = mColors;
            cb.updatePixelsArray = false;

            mFESAPI.Renderer.AddPendingTextureCopyBlock(cb);
        }

        private void UpdateTextureBlock()
        {
            FESRenderer.TextureCopyBlock cb;
            cb.rect = new Rect2i(mPaletteTextureOffsetX, mPaletteTextureOffsetY, mFESAPI.HW.PaletteTotalColors, FESHW.HW_PALETTE_SWAPS + 2);
            cb.colors = mColors;
            cb.updatePixelsArray = false;

            mFESAPI.Renderer.AddPendingTextureCopyBlock(cb);
        }

        private int ColorIndex(Color32 c)
        {
            if (c.a == 0)
            {
                return mFESAPI.HW.ColorSystemTransparent;
            }

            for (int i = 0; i < mFESAPI.HW.PaletteColorCount; i++)
            {
                if (mColors[i].r == c.r && mColors[i].g == c.g && mColors[i].b == c.b)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
