namespace FESInternal
{
    using UnityEngine;

    /// <summary>
    /// Hardware subsystem
    /// </summary>
    public sealed class FESHW
    {
        /// <summary>
        /// Maximum sound slots
        /// </summary>
        public const int HW_SOUND_SLOTS = 1024;

        /// <summary>
        /// Maximum music slots
        /// </summary>
        public const int HW_MUSIC_SLOTS = 128;

        /// <summary>
        /// Max display dimension
        /// </summary>
        public const int HW_MAX_DISPLAY_DIMENSION = 16384;

        /// <summary>
        /// System texture width
        /// </summary>
        public const int HW_SYSTEM_TEXTURE_WIDTH = 512;

        /// <summary>
        /// System texture height
        /// </summary>
        public const int HW_SYSTEM_TEXTURE_HEIGHT = 512;

        /// <summary>
        /// Maximum supported spritesheets
        /// </summary>
        public const int HW_MAX_SPRITESHEETS = 128;

        /// <summary>
        /// Maximum supported shaders
        /// </summary>
        public const int HW_MAX_SHADERS = 2048;

        /// <summary>
        /// Maximum supported palette size
        /// </summary>
        public const int HW_MAX_SUPPORTED_PALETTE_SIZE = 250;

        /// <summary>
        /// Maximum palette swaps
        /// </summary>
        public const int HW_PALETTE_SWAPS = 64;

        /// <summary>
        /// Maximum fonts
        /// </summary>
        public const int HW_FONTS = 32;

        /// <summary>
        /// System font index
        /// </summary>
        public const int HW_SYSTEM_FONT = HW_FONTS;

        /// <summary>
        /// Maximum render targets. Currently only onscreen and offscreen are supported
        /// </summary>
        public const int HW_RENDER_TARGETS = 256;

        /// <summary>
        /// Tile columns in a single tilemap chunk
        /// </summary>
        public const int HW_MAP_CHUNK_WIDTH = 16;

        /// <summary>
        /// Tile row in a single tilemap chunk
        /// </summary>
        public const int HW_MAP_CHUNK_HEIGHT = 16;

        /// <summary>
        /// Maximum supported players. This is tied into Input subsystem
        /// </summary>
        public const int HW_MAX_PLAYERS = 4;

        /// <summary>
        /// Maximum sound channels, which represent maximum simulataneous sounds
        /// </summary>
        public const int HW_MAX_SOUND_CHANNELS = 16;

        /// <summary>
        /// Maximum map layers
        /// </summary>
        public const int HW_MAX_MAP_LAYERS = 64;

        /// <summary>
        /// Maximum total map tiles
        /// </summary>
        public const int HW_MAX_MAP_TILES = 100000000;

        /// <summary>
        /// Maximum points in a polygon
        /// </summary>
        public const int HW_MAX_POLY_POINTS = 65536;

        private int mFPS;
        private float mSecondsPerUpdate;

        private Size2i mDisplaySize;
        private Size2i mMapSize;
        private int mMapLayers;
        private string mPalette;
        private FES.ColorMode mColorMode;

        private byte mPaletteColorCount;
        private byte mColorSystemTransparent;
        private byte mColorSystemBlack;
        private byte mColorSystemWhite;
        private byte mColorSystemPurple;
        private byte mPaletteTotalColors;

        private FES.PixelStyle mPixelStyle;

        /// <summary>
        /// Get display size
        /// </summary>
        public Size2i DisplaySize
        {
            get { return mDisplaySize; }
            set { mDisplaySize = value; }
        }

        /// <summary>
        /// Get map size
        /// </summary>
        public Size2i MapSize
        {
            get { return mMapSize; }
        }

        /// <summary>
        /// Get amount of map layers
        /// </summary>
        public int MapLayers
        {
            get { return mMapLayers; }
        }

        /// <summary>
        /// Get palette filename
        /// </summary>
        public string Palette
        {
            get { return mPalette; }
        }

        /// <summary>
        /// Get color mode
        /// </summary>
        public FES.ColorMode ColorMode
        {
            get { return mColorMode; }
        }

        /// <summary>
        /// Get system transparent color
        /// </summary>
        public byte ColorSystemTransparent
        {
            get { return mColorSystemTransparent; }
        }

        /// <summary>
        /// Get system black color
        /// </summary>
        public byte ColorSystemBlack
        {
            get { return mColorSystemBlack; }
        }

        /// <summary>
        /// Get system white color
        /// </summary>
        public byte ColorSystemWhite
        {
            get { return mColorSystemWhite; }
        }

        /// <summary>
        /// Get system purple color
        /// </summary>
        public byte ColorSystemPurple
        {
            get { return mColorSystemPurple; }
        }

        /// <summary>
        /// Get the target FPS
        /// </summary>
        public int FPS
        {
            get { return mFPS; }
        }

        /// <summary>
        /// Get the interval between updates in milliseconds
        /// </summary>
        public float UpdateInterval
        {
            get { return mSecondsPerUpdate; }
        }

        /// <summary>
        /// Get/Set palette color count
        /// </summary>
        public byte PaletteColorCount
        {
            get
            {
                return mPaletteColorCount;
            }

            set
            {
                mPaletteColorCount = value;
                mColorSystemTransparent = (byte)mPaletteColorCount;
                mColorSystemBlack = (byte)(mPaletteColorCount + 1);
                mColorSystemWhite = (byte)(mPaletteColorCount + 2);
                mColorSystemPurple = (byte)(mPaletteColorCount + 3);
                mPaletteTotalColors = (byte)(mPaletteColorCount + 4);
            }
        }

        /// <summary>
        /// Get total palette colors (with system colors)
        /// </summary>
        public byte PaletteTotalColors
        {
            get { return mPaletteTotalColors; }
        }

        /// <summary>
        /// Get the pixel style
        /// </summary>
        public FES.PixelStyle PixelStyle
        {
            get { return mPixelStyle; }
            set { mPixelStyle = value; }
        }

        /// <summary>
        /// Initialize the hardware subsystem
        /// </summary>
        /// <param name="hardwareSettings">Hardware settings</param>
        /// <returns>True if successful</returns>
        public bool Initialize(FES.HardwareSettings hardwareSettings)
        {
            if (!ValidateHWSettings(hardwareSettings))
            {
                return false;
            }

            mDisplaySize = hardwareSettings.DisplaySize;

            mMapSize = hardwareSettings.MapSize;
            mMapLayers = hardwareSettings.MapLayers;

            mPalette = hardwareSettings.Palette;

            mColorMode = hardwareSettings.ColorMode;

            mFPS = hardwareSettings.FPS;
            mSecondsPerUpdate = 1.0f / mFPS;

            mPixelStyle = hardwareSettings.PixelStyle;

            return true;
        }

        private bool ValidateHWSettings(FES.HardwareSettings hw)
        {
            if (hw.DisplaySize.width <= 0 || hw.DisplaySize.width >= HW_MAX_DISPLAY_DIMENSION || hw.DisplaySize.height <= 0 || hw.DisplaySize.height >= HW_MAX_DISPLAY_DIMENSION)
            {
                Debug.LogError("Display resolution is invalid");
                return false;
            }

            if (hw.DisplaySize.width % 2 != 0 || hw.DisplaySize.height % 2 != 0)
            {
                Debug.LogError("Display width and height must both be divisible by 2!");
                return false;
            }

            if (hw.MapSize.width <= 0 || hw.MapSize.height <= 0 || hw.MapLayers <= 0)
            {
                Debug.LogError("Invalid map size");
                return false;
            }

            if (hw.MapLayers > HW_MAX_MAP_LAYERS)
            {
                Debug.LogError("Maximum map layers cannot exceed " + HW_MAX_MAP_LAYERS);
                return false;
            }

            if (hw.MapSize.width * hw.MapSize.height * hw.MapLayers > HW_MAX_MAP_TILES)
            {
                Debug.LogError("Maximum map tiles (width * height * layers) cannot exceed " + HW_MAX_MAP_TILES);
                return false;
            }

            if (hw.ColorMode == FES.ColorMode.RGB && hw.Palette != null)
            {
                Debug.LogError("Palettes are only valid in indexed color mode");
                return false;
            }

            if (hw.FPS < 20 || hw.FPS > 200)
            {
                Debug.LogError("FPS is invalid, should be between 20 and 200 frames per second");
                return false;
            }

            if ((int)hw.PixelStyle < (int)FES.PixelStyle.Square || (int)hw.PixelStyle > (int)FES.PixelStyle.Tall)
            {
                Debug.LogError("Pixel style is invalid");
                return false;
            }

            return true;
        }
    }
}
