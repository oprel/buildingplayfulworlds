namespace FESDemo
{
    using System.Text;

    /// <summary>
    /// Demo utility methods
    /// </summary>
    public class DemoUtil
    {
        // FES default color palette if no palette is provided by user
        private static ColorRGBA[] mPaletteLookup = new ColorRGBA[]
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
        /// Highlight code in given string
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Highlighted code</returns>
        public static string HighlightCode(string str)
        {
            DemoReel game = (DemoReel)FES.Game;
            var mode = game.ColorMode;

            StringBuilder sb = new StringBuilder();

            if (mode == FES.ColorMode.Indexed)
            {
                sb.Append("@005");
            }

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '@')
                {
                    i++;
                    switch (str[i])
                    {
                        case 'I': // Indexed color
                            int colorIndex = ((int)(str[i + 1] - '0') * 10) + (int)(str[i + 2] - '0');
                            if (mode == FES.ColorMode.Indexed)
                            {
                                if (colorIndex > 10)
                                {
                                    sb.Append(IndexToColorString(24) + str[i + 1] + str[i + 2] + IndexToColorString(5));
                                }
                                else
                                {
                                    sb.Append(IndexToColorString(24) + str[i + 2] + IndexToColorString(5));
                                }

                                i += 2;
                            }
                            else
                            {
                                ColorRGBA rgb = IndexToRGB(colorIndex);
                                sb.Append(IndexToColorString(21) + "new" + IndexToColorString(22) + " ColorRGBA" +
                                    IndexToColorString(5) + "(" +
                                    IndexToColorString(24) + rgb.r + IndexToColorString(5) + ", " +
                                    IndexToColorString(24) + rgb.g + IndexToColorString(5) + ", " +
                                    IndexToColorString(24) + rgb.b + IndexToColorString(5) + ")");
                                i += 2;
                            }

                            break;

                        case 'K': // Keyword
                            sb.Append(IndexToColorString(21));
                            break;

                        case 'M': // Class or Method
                            sb.Append(IndexToColorString(22));
                            break;

                        case 'L': // Literal
                            sb.Append(IndexToColorString(24));
                            break;

                        case 'C': // Comment
                            sb.Append(IndexToColorString(25));
                            break;

                        case 'S': // String literal
                            sb.Append(IndexToColorString(15));
                            break;

                        case 's': // String escape code
                            sb.Append(IndexToColorString(14));
                            break;

                        case 'E':
                            sb.Append(IndexToColorString(23));
                            break;

                        case 'N': // Normal/Other
                            sb.Append(IndexToColorString(5));
                            break;

                        case 'D': // Dark
                            sb.Append(IndexToColorString(3));
                            break;

                        case '@':
                            sb.Append("@@");
                            break;

                        default:
                            sb.Append(IndexToColorString(5));
                            break;
                    }
                }
                else
                {
                    sb.Append(str[i]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Draw output frame
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="frameOuter">Outter frame color</param>
        /// <param name="frameInner">Inner frame color</param>
        /// <param name="fillColorIndex">Fill color</param>
        public static void DrawOutputFrame(Rect2i rect, int frameOuter, int frameInner, int fillColorIndex)
        {
            var demo = (DemoReel)FES.Game;

            if (frameOuter != -1)
            {
                if (demo.ColorMode == FES.ColorMode.Indexed)
                {
                    FES.DrawRect(new Rect2i(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4), frameOuter);
                }
                else
                {
                    FES.DrawRect(new Rect2i(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4), IndexToRGB(frameOuter));
                }
            }

            if (frameInner != -1)
            {
                if (demo.ColorMode == FES.ColorMode.Indexed)
                {
                    FES.DrawRect(new Rect2i(rect.x - 1, rect.y - 1, rect.width + 2, rect.height + 2), frameInner);
                }
                else
                {
                    FES.DrawRect(new Rect2i(rect.x - 1, rect.y - 1, rect.width + 2, rect.height + 2), IndexToRGB(frameInner));
                }
            }

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawRectFill(rect, fillColorIndex);
            }
            else
            {
                FES.DrawRectFill(rect, IndexToRGB(fillColorIndex));
            }
        }

        /// <summary>
        /// Color index to RGB
        /// </summary>
        /// <param name="color">Color index</param>
        /// <returns>RGB color</returns>
        public static ColorRGBA IndexToRGB(int color)
        {
            if (color < 0 || color >= mPaletteLookup.Length)
            {
                return new ColorRGBA(255, 0, 255);
            }

            return mPaletteLookup[color];
        }

        /// <summary>
        /// Color index to color string
        /// </summary>
        /// <param name="color">Color index</param>
        /// <returns>String</returns>
        public static string IndexToColorString(int color)
        {
            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                return "@" + color.ToString("000");
            }
            else
            {
                ColorRGBA rgb = IndexToRGB(color);
                return "@" + rgb.r.ToString("X2") + rgb.g.ToString("X2") + rgb.b.ToString("X2");
            }
        }
    }
}
