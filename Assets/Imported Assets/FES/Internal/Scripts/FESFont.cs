namespace FESInternal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Font subsystem
    /// </summary>
    public class FESFont
    {
        private const char ESCAPE_CHAR = '@';
        private const int MAX_LINE_WIDTHS = 65536;
        private FESAPI mFESAPI;

        private FontDef[] mFonts = new FontDef[FESHW.HW_FONTS + 1];

        private int[] mLinePixelWidths = new int[MAX_LINE_WIDTHS];
        private int[] mLineCharacterWidths = new int[MAX_LINE_WIDTHS];

        /// <summary>
        /// Initialize the subsystem
        /// </summary>
        /// <param name="api">Reference to subsystem wrapper</param>
        /// <returns>True if successful</returns>
        public bool Initialize(FESAPI api)
        {
            if (api == null)
            {
                return false;
            }

            mFESAPI = api;

            // Initialize system font
            Size2i glyphSize = new Size2i(5, 6);

            var textureSystemFont = mFESAPI.ResourceBucket.LoadTexture2D("FESFont" + glyphSize.width + "x" + glyphSize.height);
            if (textureSystemFont == null)
            {
                Debug.LogError("Failed to load system font texture from FESInternal/Font" + glyphSize.width + "x" + glyphSize.height + ", has it been delete or moved?");
                return false;
            }

            var systemFontPixels = textureSystemFont.GetPixels32();
            var texturePixels = mFESAPI.Renderer.SystemTexturePixels;

            // Filter out colors
            for (int i = 0; i < systemFontPixels.Length; i++)
            {
                if (systemFontPixels[i].r > 0)
                {
                    if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
                    {
                        systemFontPixels[i].r = systemFontPixels[i].g = systemFontPixels[i].b = 0;
                    }
                    else
                    {
                        systemFontPixels[i].r = systemFontPixels[i].g = systemFontPixels[i].b = 255;
                    }

                    systemFontPixels[i].a = 255;
                }
                else
                {
                    if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
                    {
                        systemFontPixels[i].r = systemFontPixels[i].g = systemFontPixels[i].b = mFESAPI.HW.ColorSystemTransparent;
                        systemFontPixels[i].a = 255;
                    }
                    else
                    {
                        systemFontPixels[i].r = systemFontPixels[i].g = systemFontPixels[i].b = 0;
                        systemFontPixels[i].a = 0;
                    }
                }
            }

            FESRenderer.TextureCopyBlock cb;
            cb.rect = new Rect2i(0, 0, textureSystemFont.width, glyphSize.height);
            cb.colors = systemFontPixels;
            cb.updatePixelsArray = true;

            mFESAPI.Renderer.AddPendingTextureCopyBlock(cb);

            FontSetup(FESHW.HW_SYSTEM_FONT, (int)'!', (int)'!' + 101, new Vector2i(0, FESHW.HW_SYSTEM_TEXTURE_HEIGHT - glyphSize.height), 0, glyphSize, 9999, 1, 2, false, true, false, true);

            return true;
        }

        /// <summary>
        /// Setup a custom font
        /// </summary>
        /// <param name="fontIndex">Font slot index</param>
        /// <param name="asciiStart">First ASCII character</param>
        /// <param name="asciiEnd">Last ASCII character</param>
        /// <param name="srcPos">Position in spritesheet</param>
        /// <param name="spriteSheetIndex">The index of the sprite sheet containing the font</param>
        /// <param name="glyphSize">Size of a single glyph</param>
        /// <param name="glyphsPerRow">Glyphs per row</param>
        /// <param name="characterSpacing">Spacing between characters</param>
        /// <param name="lineSpacing">Line spacing between text</param>
        /// <param name="monospaced">Is monospaced</param>
        /// <param name="tintable">Is tintable</param>
        /// <param name="calculateNow">Should calculate font info now</param>
        /// <param name="fromSystemTexture">Font is from system texture instead of sprite sheet</param>
        public void FontSetup(int fontIndex, int asciiStart, int asciiEnd, Vector2i srcPos, int spriteSheetIndex, Size2i glyphSize, int glyphsPerRow, int characterSpacing, int lineSpacing, bool monospaced, bool tintable, bool calculateNow, bool fromSystemTexture)
        {
            if (fontIndex < 0 || fontIndex >= mFonts.Length)
            {
                return;
            }

            mFonts[fontIndex] = new FontDef();
            var font = mFonts[fontIndex];

            font.asciiStart = asciiStart;
            font.asciiEnd = asciiEnd;

            font.srcPos = srcPos;

            font.glyphSize = glyphSize;
            font.glyphsPerRow = glyphsPerRow;

            font.monospaced = monospaced;
            font.tintable = tintable;

            font.glyphsCalculated = calculateNow;
            font.fromSystemTexture = fromSystemTexture;

            font.spriteSheetIndex = spriteSheetIndex;

            if (monospaced)
            {
                font.spaceWidth = glyphSize.width;
            }
            else
            {
                font.spaceWidth = (int)((glyphSize.width / 2.0f) - 0.5f);
            }

            font.characterSpacing = characterSpacing;
            font.lineSpacing = lineSpacing;

            if (calculateNow)
            {
                CalculateGlyphWidths(font);
                return;
            }
        }

        /// <summary>
        /// Print text using given font
        /// </summary>
        /// <param name="fontIndex">Font index</param>
        /// <param name="textRect">Rectangular area to print to</param>
        /// <param name="colorIndex">Color Index</param>
        /// <param name="color">RGB color</param>
        /// <param name="alignFlags">Any combination of flags: <see cref="FES.ALIGN_H_LEFT"/>, <see cref="FES.ALIGN_H_RIGHT"/>,
        /// <see cref="FES.ALIGN_H_CENTER"/>, <see cref="FES.ALIGN_V_TOP"/>, <see cref="FES.ALIGN_V_BOTTOM"/>,
        /// <see cref="FES.ALIGN_V_CENTER"/>, <see cref="FES.TEXT_OVERFLOW_CLIP"/>, <see cref="FES.TEXT_OVERFLOW_WRAP"/>.</param>
        /// <param name="text">Text to print</param>
        /// <param name="measureOnly">Measure only, don't draw</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public void Print(int fontIndex, Rect2i textRect, int colorIndex, Color32 color, int alignFlags, string text, bool measureOnly, out int width, out int height)
        {
            width = height = 0;

            if (fontIndex < 0 || fontIndex >= mFonts.Length)
            {
                return;
            }

            var font = mFonts[fontIndex];
            if (font == null)
            {
                return;
            }

            if (font.glyphsCalculated == false)
            {
                CalculateGlyphWidths(font);
            }

            if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed && !font.tintable)
            {
                colorIndex = FESHW.HW_PALETTE_SWAPS - mFESAPI.Renderer.PaletteSwapGet();
            }

            Rect2i previousClipRect = FES.ClipGet();
            int previousSpriteSheetIndex = 0;
            if (!measureOnly && !font.fromSystemTexture)
            {
                previousSpriteSheetIndex = mFESAPI.Renderer.CurrentSpriteSheetIndex;
                mFESAPI.Renderer.SpriteSheetSet(font.spriteSheetIndex);
            }

            if (!measureOnly)
            {
                if ((alignFlags & FES.TEXT_OVERFLOW_CLIP) != 0)
                {
                    var cameraPos = FES.CameraGet();
                    FES.ClipSet(new Rect2i(textRect.x - cameraPos.x, textRect.y - cameraPos.y, textRect.width, textRect.height));
                }
            }

            int lineCount = 0;
            int lineIndex = 0;
            int charactersInLine = 0;

            Size2i textSize = new Size2i(0, 0);

            int maxLineWidth = (alignFlags & FES.TEXT_OVERFLOW_WRAP) != 0 ? textRect.width : int.MaxValue;

            // Only need to measure line widths if not aligned to top left
            if ((alignFlags & FES.ALIGN_H_RIGHT) != 0 || (alignFlags & FES.ALIGN_H_CENTER) != 0 ||
                (alignFlags & FES.ALIGN_V_BOTTOM) != 0 || (alignFlags & FES.ALIGN_V_CENTER) != 0 ||
                (alignFlags & FES.TEXT_OVERFLOW_WRAP) != 0)
            {
                MeasureLineWidths(font, text, maxLineWidth, out textSize, out lineCount);
            }

            int px = textRect.x;
            int py = textRect.y;
            int largestWidth = 0;
            int totalHeight = 0;
            int lineStartIndex = 0;
            int originalColorIndex = colorIndex;
            color = (Color)color * (Color)mFESAPI.Renderer.TintColorGet();
            Color32 originalColor = color;

            SetLinePosition(font, textRect, lineIndex, alignFlags, out px);

            if ((alignFlags & FES.ALIGN_V_BOTTOM) != 0)
            {
                py = (textRect.y + textRect.height) - (lineCount * (font.glyphSize.height + font.lineSpacing));

                // Compensate for line spacing on last line
                if (!font.monospaced && lineCount > 0)
                {
                    py += font.lineSpacing;
                }
            }
            else if ((alignFlags & FES.ALIGN_V_CENTER) != 0)
            {
                int offset = (textRect.height / 2) - ((lineCount * (font.glyphSize.height + font.lineSpacing)) / 2);

                // If monospaced  make sure it's still lined up on line boundaries even if centered!
                if (font.monospaced)
                {
                    offset -= offset % (font.glyphSize.height + font.lineSpacing);
                }

                py = textRect.y + offset;

                // Compensate for line spacing on last line
                if (!font.monospaced && lineCount > 0)
                {
                    py += font.lineSpacing / 2;
                }
            }

            bool forceEndOfLine = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n' || forceEndOfLine)
                {
                    py += font.glyphSize.height + font.lineSpacing;

                    if (measureOnly)
                    {
                        largestWidth = Mathf.Max(px - textRect.x, largestWidth);
                        totalHeight += font.glyphSize.height + font.lineSpacing;
                    }

                    lineIndex++;
                    SetLinePosition(font, textRect, lineIndex, alignFlags, out px);

                    lineStartIndex = i + 1;
                    if (forceEndOfLine)
                    {
                        lineStartIndex--;
                    }

                    if (forceEndOfLine && text[i] != '\n')
                    {
                        i--;
                    }

                    forceEndOfLine = false;

                    charactersInLine = 0;

                    continue;
                }
                else if (text[i] == ESCAPE_CHAR && i < text.Length - 1)
                {
                    i++;
                    if (text[i] == '-')
                    {
                        colorIndex = originalColorIndex;
                        color = originalColor;
                        continue;
                    }
                    else
                    {
                        if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
                        {
                            if (text[i] >= '0' && text[i] <= '9' && i < text.Length - 1)
                            {
                                if (i >= text.Length - 2)
                                {
                                    i--;
                                    continue;
                                }

                                if (text[i + 1] >= '0' && text[i + 1] <= '9' && text[i + 2] >= '0' && text[i + 2] <= '9')
                                {
                                    colorIndex = ((int)(text[i] - '0') * 100) + ((int)(text[i + 1] - '0') * 10) + (int)(text[i + 2] - '0');
                                    i += 2;
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            // Decode the hex string for color rgb
                            if (IsCharHex(text[i]))
                            {
                                bool validHex = true;
                                string hexString = null;
                                if (text.Length - i >= 6)
                                {
                                    hexString = text.Substring(i, 6);

                                    for (int j = 0; j < hexString.Length; j++)
                                    {
                                        if (!IsCharHex(hexString[j]))
                                        {
                                            validHex = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    validHex = false;
                                }

                                if (!validHex)
                                {
                                    i--;
                                    continue;
                                }
                                else
                                {
                                    i += hexString.Length - 1;
                                    int hex = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
                                    color = (Color)(new Color32((byte)((hex & 0xFF0000) >> 16), (byte)((hex & 0x00FF00) >> 8), (byte)(hex & 0x0000FF), 255)) * (Color)mFESAPI.Renderer.TintColorGet();
                                    continue;
                                }
                            }
                        }
                    }

                    if (text[i] != ESCAPE_CHAR)
                    {
                        i--;
                        continue;
                    }
                }

                int advance = 0;
                bool printable = true;
                var rect = new Rect2i(0, 0, 0, 0);

                // For invalid characters insert a space
                if (text[i] >= 0 && text[i] <= ' ')
                {
                    advance = font.spaceWidth;
                    printable = false;
                }
                else
                {
                    rect = font.glyphRect[(int)text[i]];
                    advance = rect.width;
                }

                if (lineIndex < MAX_LINE_WIDTHS && (alignFlags & FES.TEXT_OVERFLOW_WRAP) != 0 && charactersInLine > mLineCharacterWidths[lineIndex])
                {
                    // Only if we printed at least one character, otherwise we'll get an infinite loop
                    if (i - lineStartIndex > 0)
                    {
                        forceEndOfLine = true;
                        i--;
                        continue;
                    }
                }

                if (printable && !measureOnly)
                {
                    int swapIndex = 0;
                    if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
                    {
                        if (font.tintable)
                        {
                            swapIndex = FESHW.HW_PALETTE_SWAPS + (mFESAPI.HW.PaletteTotalColors - colorIndex);
                        }
                        else
                        {
                            // In non-tintable mode colorIndex is actually swapIndex
                            swapIndex = colorIndex;
                        }
                    }

                    Color32 previousTintColor = mFESAPI.Renderer.TintColorGet();
                    int oldSwapIndex = mFESAPI.Renderer.PaletteSwapGet();
                    if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
                    {
                        if (font.tintable)
                        {
                            mFESAPI.Renderer.PaletteSwapDirect(swapIndex);
                        }
                        else
                        {
                            mFESAPI.Renderer.PaletteSwapSet(swapIndex);
                        }
                    }
                    else
                    {
                        mFESAPI.Renderer.TintColorSet(color);
                    }

                    mFESAPI.Renderer.DrawTexture(rect, new Rect2i(px, py, rect.width, rect.height), new Vector2i(0, 0), 0, 0, font.fromSystemTexture);

                    if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
                    {
                        mFESAPI.Renderer.PaletteSwapSet(oldSwapIndex);
                    }
                    else
                    {
                        mFESAPI.Renderer.TintColorSet(previousTintColor);
                    }
                }

                px += advance + font.characterSpacing;
                charactersInLine++;
            }

            if (measureOnly)
            {
                largestWidth = Mathf.Max(px - textRect.x, largestWidth);

                // Compensate for space added after each character
                if (largestWidth != 0)
                {
                    largestWidth--;
                }

                width = largestWidth;
                if (width == 0)
                {
                    height = 0;
                }
                else
                {
                    height = totalHeight + font.glyphSize.height;
                }
            }

            if (!measureOnly && !font.fromSystemTexture)
            {
                previousSpriteSheetIndex = mFESAPI.Renderer.CurrentSpriteSheetIndex;
                mFESAPI.Renderer.SpriteSheetSet(previousSpriteSheetIndex);
            }

            if (!measureOnly)
            {
                if ((alignFlags & FES.TEXT_OVERFLOW_CLIP) != 0)
                {
                    FES.ClipSet(previousClipRect);
                }
            }
        }

        // Calculate the width of each line in the string as its printed. This is necessary to do right/center alignment
        private void MeasureLineWidths(FontDef font, string text, int maxLineWidth, out Size2i textSize, out int lineCount)
        {
            textSize = new Size2i(0, 0);
            lineCount = 0;

            if (font.glyphsCalculated == false)
            {
                CalculateGlyphWidths(font);
            }

            int largestWidth = 0;
            int totalHeight = 0;
            int lineIndex = 0;
            int lineStartIndex = 0;
            int lastSpaceIndex = -1;
            int lastCharactersInLine = -1;
            int lastSpaceWidth = 0;
            int charactersInLine = 0;

            int px = 0;
            int py = 0;

            bool forceEndOfLine = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n' || forceEndOfLine)
                {
                    py += font.glyphSize.height + font.lineSpacing;

                    if (forceEndOfLine)
                    {
                        if (lastSpaceIndex > 0)
                        {
                            px = lastSpaceWidth;
                            i = lastSpaceIndex + 1;
                            charactersInLine = lastCharactersInLine;
                        }
                        else
                        {
                            charactersInLine--;
                        }
                    }

                    largestWidth = Mathf.Max(px, largestWidth);
                    totalHeight += font.glyphSize.height + font.lineSpacing;

                    if (lineIndex < MAX_LINE_WIDTHS)
                    {
                        mLinePixelWidths[lineIndex] = px;

                        // Compensate for space character
                        if (px > 0 && !forceEndOfLine)
                        {
                            mLinePixelWidths[lineIndex]--;
                        }

                        mLineCharacterWidths[lineIndex] = charactersInLine;
                    }

                    lineIndex++;
                    lineStartIndex = i + 1;
                    if (forceEndOfLine)
                    {
                        lineStartIndex--;
                    }

                    if (forceEndOfLine && text[i] != '\n')
                    {
                        i--;
                    }

                    px = 0;

                    forceEndOfLine = false;

                    lastSpaceIndex = -1;
                    lastSpaceWidth = -1;
                    lastCharactersInLine = -1;

                    charactersInLine = 0;

                    continue;
                }
                else if (text[i] == ESCAPE_CHAR && i < text.Length - 1)
                {
                    i++;
                    if (text[i] == '-')
                    {
                        continue;
                    }
                    else
                    {
                        if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
                        {
                            if (text[i] >= '0' && text[i] <= '9' && i < text.Length - 1)
                            {
                                if (i >= text.Length - 2)
                                {
                                    i--;
                                    continue;
                                }

                                if (text[i + 1] >= '0' && text[i + 1] <= '9' && text[i + 2] >= '0' && text[i + 2] <= '9')
                                {
                                    i += 2;
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            // Decode the hex string for color rgb
                            if (IsCharHex(text[i]))
                            {
                                bool validHex = true;
                                string hexString = null;
                                if (text.Length - i >= 6)
                                {
                                    hexString = text.Substring(i, 6);

                                    for (int j = 0; j < hexString.Length; j++)
                                    {
                                        if (!IsCharHex(hexString[j]))
                                        {
                                            validHex = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    validHex = false;
                                }

                                if (!validHex)
                                {
                                    i--;
                                    continue;
                                }
                                else
                                {
                                    i += hexString.Length - 1;
                                    continue;
                                }
                            }
                        }
                    }

                    if (text[i] != ESCAPE_CHAR)
                    {
                        i--;
                        continue;
                    }
                }

                int advance = 0;

                // For invalid characters insert a space
                if (text[i] >= 0 && text[i] <= ' ')
                {
                    advance = font.spaceWidth;
                }
                else
                {
                    advance = font.glyphRect[(int)text[i]].width;
                }

                if (px + advance + font.characterSpacing >= maxLineWidth)
                {
                    // Only if we printed at least one character, otherwise we'll get an infinite loop
                    if (i - lineStartIndex > 0)
                    {
                        forceEndOfLine = true;
                        i--;
                        continue;
                    }
                }

                if (text[i] >= 0 && text[i] <= ' ')
                {
                    lastSpaceIndex = i;
                    lastSpaceWidth = px;
                    lastCharactersInLine = charactersInLine;
                }

                px += advance + font.characterSpacing;
                charactersInLine++;
            }

            if (lineIndex < MAX_LINE_WIDTHS)
            {
                mLinePixelWidths[lineIndex] = px;

                // Compensate for space character
                if (px > 0)
                {
                    mLinePixelWidths[lineIndex]--;
                }

                // Last line can be as long as it wants to be
                mLineCharacterWidths[lineIndex] = int.MaxValue;
            }

            largestWidth = Mathf.Max(px, largestWidth);

            // Compensate for space added after each character
            if (largestWidth != 0)
            {
                largestWidth--;
            }

            textSize.width = largestWidth;
            if (textSize.width == 0)
            {
                textSize.height = 0;
            }
            else
            {
                textSize.height = totalHeight + font.glyphSize.height;
            }

            if (textSize.height > 0)
            {
                lineCount = lineIndex + 1;
            }
        }

        private void SetLinePosition(FontDef font, Rect2i textRect, int lineIndex, int alignFlags, out int px)
        {
            px = textRect.x;

            if ((alignFlags & FES.ALIGN_H_RIGHT) != 0)
            {
                px = textRect.x + textRect.width - mLinePixelWidths[lineIndex];
            }
            else if ((alignFlags & FES.ALIGN_H_CENTER) != 0)
            {
                int offset = (textRect.width / 2) - (mLinePixelWidths[lineIndex] / 2);

                // If monospaced  make sure it's still lined up on character boundaries even if centered!
                if (font.monospaced)
                {
                    offset -= offset % (font.glyphSize.width + font.characterSpacing);
                }

                px = textRect.x + offset;
            }
        }

        private void CalculateGlyphWidths(FontDef font)
        {
            Color32[] pixels;
            int texWidth;
            int texHeight;

            if (font.fromSystemTexture)
            {
                pixels = mFESAPI.Renderer.SystemTexturePixels;
                texWidth = FESHW.HW_SYSTEM_TEXTURE_WIDTH;
                texHeight = FESHW.HW_SYSTEM_TEXTURE_HEIGHT;
            }
            else
            {
                pixels = mFESAPI.Renderer.SpriteSheets[font.spriteSheetIndex].texture.GetPixels32();
                texWidth = mFESAPI.Renderer.SpriteSheets[font.spriteSheetIndex].texture.width;
                texHeight = mFESAPI.Renderer.SpriteSheets[font.spriteSheetIndex].texture.height;
            }

            // Calculate glyph widths
            for (int i = 0; i < font.asciiEnd - font.asciiStart + 1; i++)
            {
                int x0 = 0;
                int x1 = 0;
                int col = i % font.glyphsPerRow;
                int row = i / font.glyphsPerRow;

                if (!font.monospaced)
                {
                    x0 = int.MaxValue;
                    x1 = int.MinValue;
                    for (int y = (row * font.glyphSize.height) + font.srcPos.y + 1; y < (row * font.glyphSize.height) + font.srcPos.y + font.glyphSize.height + 1; y++)
                    {
                        for (int x = (col * font.glyphSize.width) + font.srcPos.x; x < (col * font.glyphSize.width) + font.srcPos.x + font.glyphSize.width; x++)
                        {
                            Color32 c = pixels[x + ((texHeight - y) * texWidth)];
                            int offset = x - ((col * font.glyphSize.width) + font.srcPos.x);

                            if (font.tintable && mFESAPI.HW.ColorMode == FES.ColorMode.Indexed && (c.r != 0 && c.r != mFESAPI.HW.ColorSystemTransparent))
                            {
                                FESUtil.LogErrorOnce("Font in spritesheet " + font.spriteSheetIndex + " at position " + font.srcPos.ToString() + " should be drawn using only color index 0 if palette swap support is turned off.");
                            }

                            if (c.a > 0 && (c.r < mFESAPI.HW.PaletteColorCount || mFESAPI.HW.ColorMode == FES.ColorMode.RGB))
                            {
                                if (offset < x0)
                                {
                                    x0 = offset;
                                }

                                if (offset > x1)
                                {
                                    x1 = offset;
                                }
                            }
                        }
                    }

                    if (x0 == int.MaxValue)
                    {
                        x0 = 0;
                    }

                    if (x1 == int.MinValue)
                    {
                        x1 = 0;
                    }
                }
                else
                {
                    x0 = 0;
                    x1 = font.glyphSize.width - 1;
                }

                font.glyphRect[i + font.asciiStart] = new Rect2i((col * font.glyphSize.width) + font.srcPos.x + x0, (row * font.glyphSize.height) + font.srcPos.y, x1 - x0 + 1, font.glyphSize.height);
            }

            font.glyphsCalculated = true;
        }

        private bool IsCharHex(char c)
        {
            if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
            {
                return true;
            }

            return false;
        }

        private class FontDef
        {
            public int asciiStart;
            public int asciiEnd;

            public Vector2i srcPos;

            public bool monospaced;
            public bool tintable;

            public int glyphsPerRow;
            public Size2i glyphSize;
            public Rect2i[] glyphRect = new Rect2i[256];

            public bool glyphsCalculated = false;

            public int spaceWidth;
            public int characterSpacing;
            public int lineSpacing;

            public bool fromSystemTexture;

            public int spriteSheetIndex;
        }
    }
}
