using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FES retro game framework
/// </summary>
public sealed class FES
{
    /// <summary>
    /// Invalid sprite index
    /// </summary>
    public const int SPRITE_INVALID = 0x7FFFFFFF;

    /// <summary>
    /// An empty sprite index, use this to clear a tile in a tilemap
    /// </summary>
    public const int SPRITE_EMPTY = 0x7FFFFFFE;

    /// <summary>
    /// Flip sprite horizontally
    /// </summary>
    public const int FLIP_H = 1 << 0;

    /// <summary>
    /// Flip sprite vertically
    /// </summary>
    public const int FLIP_V = 1 << 1;

    /// <summary>
    /// Rotate sprite 90 degrees clockwise
    /// </summary>
    public const int ROT_90_CW = 1 << 2;

    /// <summary>
    /// Rotate sprite 90 degrees counter-clockwise. This is equivalent to <see cref="FES.ROT_90_CW"/> | <see cref="FES.FLIP_H"/> | <see cref="FES.FLIP_V"/>
    /// </summary>
    public const int ROT_90_CCW = ROT_90_CW | FLIP_H | FLIP_V;

    /// <summary>
    /// Rotate sprite 180 degrees counter-clockwise. This is equivalent to <see cref="FES.FLIP_H"/> | <see cref="FES.FLIP_V"/>
    /// </summary>
    public const int ROT_180_CCW = FLIP_H | FLIP_V;

    /// <summary>
    /// Rotate sprite 90 degrees clockwise. This is equivalent to <see cref="FES.FLIP_H"/> | <see cref="FES.FLIP_V"/>
    /// </summary>
    public const int ROT_180_CW = FLIP_H | FLIP_V;

    /// <summary>
    /// Rotate sprite 270 degrees clockwise. This is equivalent to <see cref="FES.ROT_90_CW"/> | <see cref="FES.FLIP_H"/> | <see cref="FES.FLIP_V"/>
    /// </summary>
    public const int ROT_270_CW = ROT_90_CW | FLIP_H | FLIP_V;

    /// <summary>
    /// Rotate sprite 270 degrees counter-clockwise. This is equivalent to <see cref="FES.ROT_90_CW"/>
    /// </summary>
    public const int ROT_270_CCW = ROT_90_CW;

    /// <summary>
    /// Align to the left edge
    /// </summary>
    public const int ALIGN_H_LEFT = 1 << 9;

    /// <summary>
    /// Align to the right edge
    /// </summary>
    public const int ALIGN_H_RIGHT = 1 << 10;

    /// <summary>
    /// Center horizontally
    /// </summary>
    public const int ALIGN_H_CENTER = 1 << 11;

    /// <summary>
    /// Align to the top edge
    /// </summary>
    public const int ALIGN_V_TOP = 1 << 12;

    /// <summary>
    /// Align to the bottom edge
    /// </summary>
    public const int ALIGN_V_BOTTOM = 1 << 13;

    /// <summary>
    /// Center vertically
    /// </summary>
    public const int ALIGN_V_CENTER = 1 << 14;

    /// <summary>
    /// Clip text to given rectangular area
    /// </summary>
    public const int TEXT_OVERFLOW_CLIP = 1 << 16;

    /// <summary>
    /// Wrap text to the next line so that it doesn't overflow the given text area horizontally.
    /// </summary>
    public const int TEXT_OVERFLOW_WRAP = 1 << 17;

    /// <summary>
    /// Up D-pad Button on gamepad, or <see cref="KeyCode.W"/> for player one, or <see cref="KeyCode.UpArrow"/> for player two.
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_UP = 1 << 0;

    /// <summary>
    /// Down D-pad Button on gamepad, or <see cref="KeyCode.S"/> for player one, or <see cref="KeyCode.DownArrow"/> for player two.
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_DOWN = 1 << 1;

    /// <summary>
    /// Left D-pad Button on gamepad, or <see cref="KeyCode.A"/> for player one, or <see cref="KeyCode.LeftArrow"/> for player two.
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_LEFT = 1 << 2;

    /// <summary>
    /// Right D-pad Button on gamepad, or <see cref="KeyCode.D"/> for player one, or <see cref="KeyCode.RightArrow"/> for player two.
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_RIGHT = 1 << 3;

    /// <summary>
    /// A Button on gamepad, or <see cref="KeyCode.B"/>, <see cref="KeyCode.Space"/> for player one, or <see cref="KeyCode.Semicolon"/>, <see cref="KeyCode.Keypad1"/>, <see cref="KeyCode.RightControl"/> for player two.
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_A = 1 << 4;

    /// <summary>
    /// B Button on gamepad, or <see cref="KeyCode.N"/> for player one, or <see cref="KeyCode.Quote"/>, <see cref="KeyCode.Keypad2"/> for player two.
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_B = 1 << 5;

    /// <summary>
    /// X Button on gamepad, or <see cref="KeyCode.G"/> for player one, or <see cref="KeyCode.P"/>, <see cref="KeyCode.Keypad4"/> for player two.
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_X = 1 << 6;

    /// <summary>
    /// Y Button on gamepad, or <see cref="KeyCode.H"/> for player one, or <see cref="KeyCode.LeftBracket"/>, <see cref="KeyCode.Keypad5"/> for player two.
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_Y = 1 << 7;

    /// <summary>
    /// Left Shoulder Button on gamepad, or <see cref="KeyCode.T"/> for player one, or <see cref="KeyCode.Alpha0"/>, <see cref="KeyCode.Keypad7"/> for player two.
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_LS = 1 << 8;

    /// <summary>
    /// Right Shoulder Button on gamepad, or <see cref="KeyCode.Y"/> for player one, or <see cref="KeyCode.Minus"/>, <see cref="KeyCode.Keypad8"/> for player two.
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_RS = 1 << 9;

    /// <summary>
    /// Menu Button on gamepad, or <see cref="KeyCode.Alpha5"/> for player one, or <see cref="KeyCode.Backspace"/>, <see cref="KeyCode.KeypadDivide"/> for player two.
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_MENU = 1 << 10;

    /// <summary>
    /// System Button on either gamepad, or <see cref="KeyCode.Escape"/>
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_SYSTEM = 1 << 11;

    /// <summary>
    /// Screen touch, or <see cref="KeyCode.Mouse0"/>
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_POINTER_A = 1 << 12;

    /// <summary>
    /// <see cref="KeyCode.Mouse1"/>
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_POINTER_B = 1 << 13;

    /// <summary>
    /// <see cref="KeyCode.Mouse2"/>
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_POINTER_C = 1 << 14;

    /// <summary>
    /// Equivalent to <see cref="FES.BTN_A"/> | <see cref="FES.BTN_B"/> | <see cref="FES.BTN_X"/> | <see cref="FES.BTN_Y"/>
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_ABXY = BTN_A | BTN_B | BTN_X | BTN_Y;

    /// <summary>
    /// Equivalent to <see cref="FES.BTN_POINTER_A"/> | <see cref="FES.BTN_POINTER_B"/> | <see cref="FES.BTN_POINTER_C"/>
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_POINTER_ABC = BTN_POINTER_A | BTN_POINTER_B | BTN_POINTER_C;

    /// <summary>
    /// Equivalent to <see cref="FES.BTN_LS"/> | <see cref="FES.BTN_RS"/>
    /// Use <see cref="FES.InputOverride(InputOverrideMethod)"/> to override input mapping
    /// </summary>
    public const int BTN_SHOUDLER = BTN_LS | BTN_RS;

    /// <summary>
    /// Specifies player one, can be used with <see cref="FES.ButtonDown(int, int)"/>, <see cref="FES.ButtonPressed(int, int)"/> and <see cref="FES.ButtonReleased(int, int)"/>
    /// </summary>
    public const int PLAYER_ONE = 1 << 0;

    /// <summary>
    /// Specifies player two, can be used with <see cref="FES.ButtonDown(int, int)"/>, <see cref="FES.ButtonPressed(int, int)"/> and <see cref="FES.ButtonReleased(int, int)"/>
    /// </summary>
    public const int PLAYER_TWO = 1 << 1;

    /// <summary>
    /// Specifies player two, can be used with <see cref="FES.ButtonDown(int, int)"/>, <see cref="FES.ButtonPressed(int, int)"/> and <see cref="FES.ButtonReleased(int, int)"/>
    /// </summary>
    public const int PLAYER_THREE = 1 << 2;

    /// <summary>
    /// Specifies player two, can be used with <see cref="FES.ButtonDown(int, int)"/>, <see cref="FES.ButtonPressed(int, int)"/> and <see cref="FES.ButtonReleased(int, int)"/>
    /// </summary>
    public const int PLAYER_FOUR = 1 << 3;

    /// <summary>
    /// Equivalent to <see cref="FES.PLAYER_ONE"/> | <see cref="FES.PLAYER_TWO"/> | <see cref="FES.PLAYER_THREE"/> | <see cref="FES.PLAYER_FOUR"/>
    /// </summary>
    public const int PLAYER_ANY = PLAYER_ONE | PLAYER_TWO | PLAYER_THREE | PLAYER_FOUR;

    private static FES mInstance;
    private IFESGame mFESGame;
    private FESInternal.FESAPI mFESAPI;
    private Color32 mSolidWhite = new Color32(255, 255, 255, 255);
    private bool mInitialized = false;

    /// <summary>
    /// Delegate for overriding input mapping
    /// </summary>
    /// <param name="button">The button being queried</param>
    /// <param name="player">The player for which the button is being queried</param>
    /// <param name="handled">Set to true if the button override was handled, or false if default FES mapping should be used</param>
    /// <returns>Return true if the button is currently held down, or false if it is up</returns>
    public delegate bool InputOverrideMethod(int button, int player, out bool handled);

    /// <summary>
    /// Post-processing effect type
    /// </summary>
    public enum Effect
    {
        /// <summary>
        /// Retro CRT scanline effect
        /// </summary>
        Scanlines,

        /// <summary>
        /// Retro CRT screen noise
        /// </summary>
        Noise,

        /// <summary>
        /// Desaturate colors
        /// </summary>
        Desaturation,

        /// <summary>
        /// Retro CRT screen curvature
        /// </summary>
        Curvature,

        /// <summary>
        /// Slide display in or out from given direction
        /// </summary>
        Slide,

        /// <summary>
        /// Wipe display out from given direction
        /// </summary>
        Wipe,

        /// <summary>
        /// Shake the display
        /// </summary>
        Shake,

        /// <summary>
        /// Zoom display in or out
        /// </summary>
        Zoom,

        /// <summary>
        /// Rotate display by given angle
        /// </summary>
        Rotation,

        /// <summary>
        /// Fade the screen to given color
        /// </summary>
        ColorFade,

        /// <summary>
        /// Apply a tint of given color
        /// </summary>
        ColorTint,

        /// <summary>
        /// Turn colors to negative
        /// </summary>
        Negative,

        /// <summary>
        /// Pixelate effect
        /// </summary>
        Pixelate,

        /// <summary>
        /// Pinhole
        /// </summary>
        Pinhole,

        /// <summary>
        /// Inverted pinhole
        /// </summary>
        InvertedPinhole,

        /// <summary>
        /// Fizzle out
        /// </summary>
        Fizzle
    }

    /// <summary>
    /// Display color mode enumeration
    /// </summary>
    public enum ColorMode
    {
        /// <summary>
        /// Indexed color mode, with a restricted color palette
        /// </summary>
        Indexed,

        /// <summary>
        /// RGB color mode
        /// </summary>
        RGB
    }

    /// <summary>
    /// Display pixel style
    /// </summary>
    public enum PixelStyle
    {
        /// <summary>
        /// Normal square pixels
        /// </summary>
        Square,

        /// <summary>
        /// Wide pixels
        /// </summary>
        Wide,

        /// <summary>
        /// Tall pixels
        /// </summary>
        Tall
    }

    /// <summary>
    /// Texture filter to apply when using custom shaders
    /// </summary>
    public enum Filter
    {
        /// <summary>
        /// No smoothing, sample from a single nearest pixel only
        /// </summary>
        Nearest,

        /// <summary>
        /// Linear interpolation, averaged between neighbouring pixels
        /// </summary>
        Linear
    }

    /// <summary>
    /// Defines IFESGame interface. Every FES game must implement this interface.
    /// </summary>
    public interface IFESGame
    {
        /// <summary>
        /// Called once on startup to query the game for its hardware capabilities. This call happens before any other call to this interface.
        /// </summary>
        /// <returns>Hardware capabilities</returns>
        HardwareSettings QueryHardware();

        /// <summary>
        /// Called once after QueryHardware. The game should initialize it's state here.
        /// </summary>
        /// <returns>Return true if initialization was successful, false otherwise</returns>
        bool Initialize();

        /// <summary>
        /// Called at a fixed interval of 60 times per second. Game logic goes in here.
        /// </summary>
        void Update();

        /// <summary>
        /// Called to render to display, all draw APIs should be called here. Unlike <see cref="IFESGame.Update"/> this method is not called at
        /// a fixed framerate.
        /// </summary>
        void Render();
    }

    /// <summary>
    /// Reference to the Game running under FES. This is the same instance that is given to FES by the <see cref="FES.Initialize"/> method.
    /// </summary>
    public static IFESGame Game
    {
        get { return Instance.mFESGame; }
    }

    /// <summary>
    /// Display size as given in <see cref="IFESGame.QueryHardware"/>.
    /// </summary>
    public static Size2i DisplaySize
    {
        get { return Instance.mFESAPI.HW.DisplaySize; }
    }

    /// <summary>
    /// Tilemap size as given in <see cref="IFESGame.QueryHardware"/>.
    /// </summary>
    public static Size2i MapSize
    {
        get { return Instance.mFESAPI.HW.MapSize; }
    }

    /// <summary>
    /// Amount of map layers as given in <see cref="IFESGame.QueryHardware"/>.
    /// </summary>
    public static int MapLayers
    {
        get { return Instance.mFESAPI.HW.MapLayers; }
    }

    /// <summary>
    /// Get target FPS as set by <see cref="IFESGame.QueryHardware"/>.
    /// </summary>
    public static int FPS
    {
        get { return Instance.mFESAPI.HW.FPS; }
    }

    /// <summary>
    /// Get update interval which is equivalent to 1 / <see cref="FES.FPS"/>.
    /// </summary>
    public static float UpdateInterval
    {
        get { return Instance.mFESAPI.HW.UpdateInterval; }
    }

    /// <summary>
    /// Represents the count of calls to <see cref="IFESGame.Update"/> since startup, or since <see cref="FES.TicksReset"/> was called.
    /// </summary>
    public static ulong Ticks
    {
        get { return Instance.mFESAPI.Ticks; }
    }

    private static FES Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new FES();
            }

            return mInstance;
        }
    }

    /// <summary>
    /// Initialize FES with the given game instance. As part of initialization FES will call <see cref="IFESGame.QueryHardware"/>,
    /// followed by <see cref="IFESGame.Initialize"/>.
    /// </summary>
    /// <param name="fesGame">A FES Game</param>
    /// <returns>True if successful</returns>
    public static bool Initialize(IFESGame fesGame)
    {
        if (Instance.mInitialized)
        {
            if (Instance.mFESGame != null)
            {
                Debug.LogError("FES is already initialized with the program " + Instance.mFESGame.GetType().Name + "!");
            }
            else
            {
                Debug.LogError("FES is already initialized!");
            }

            return false;
        }

        var fesInternalObj = GameObject.Find("FESInternal");
        if (fesInternalObj != null)
        {
            Instance.mFESAPI = fesInternalObj.GetComponent<FESInternal.FESAPI>();
        }

        if (Instance.mFESAPI == null)
        {
            Debug.LogError("Can't find FESInternal gameObject, did you add the FES prefab to your scene?");
            Instance.mInitialized = false;
            Instance.mFESAPI.FinalizeInitialization(Instance.mInitialized);
            return false;
        }

        Instance.mFESGame = fesGame;

#if UNITY_EDITOR
        if (Instance.mFESAPI.Initialized)
        {
            Debug.Log("FES does not support live code editing.");
            Application.Quit();
            return true;
        }
#endif

        var settings = Instance.mFESGame.QueryHardware();

        bool ret = Instance.mFESAPI.Initialize(settings);
        Instance.mInitialized = false;
        if (ret)
        {
            if (Instance.mFESGame.Initialize())
            {
                Instance.mInitialized = true;
            }
            else
            {
                Debug.LogError("Game Initialize() returned failure");
            }
        }

        // Highly recommended to keep CPU usage low
        QualitySettings.vSyncCount = 1;

#if !UNITY_WEBGL
        Application.targetFrameRate = Instance.mFESAPI.HW.FPS;
#endif

        Time.fixedDeltaTime = Instance.mFESAPI.HW.UpdateInterval;

        Instance.mFESAPI.FinalizeInitialization(Instance.mInitialized);

        if (Instance.mInitialized)
        {
            return true;
        }
        else
        {
            Debug.LogError("Initialize failed");
            return false;
        }
    }

    /// <summary>
    /// Reset the ticks counter
    /// </summary>
    public static void TicksReset()
    {
        Instance.mFESAPI.TicksReset();
    }

    /// <summary>
    /// Set display mode to given resolution and pixel style. Note that this sets only the FES pixel resolution, and does not affect the native
    /// window size. To change the native window size you can use the Unity Screen.SetResolution() API.
    /// </summary>
    /// <param name="resolution">Resolution</param>
    /// <param name="pixelStyle">Pixel style, one of <see cref="FES.PixelStyle.Square"/>, <see cref="FES.PixelStyle.Wide"/>, <see cref="FES.PixelStyle.Tall"/></param>
    /// <returns>True if mode was successfuly set, false otherwise</returns>
    public static bool DisplayModeSet(Size2i resolution, PixelStyle pixelStyle = PixelStyle.Square)
    {
        if (!Instance.mInitialized)
        {
            Debug.LogError("FES is not initialized yet, set the initialize resolution via IFESGame.QueryHardware()");
            return false;
        }

        if (Instance.mFESAPI.Renderer.RenderEnabled)
        {
            Debug.LogError("Can't change display mode in Render() call!");
            return false;
        }

        if (resolution.width <= 0 || resolution.height <= 0 ||
            resolution.width >= FESInternal.FESHW.HW_MAX_DISPLAY_DIMENSION ||
            resolution.height >= FESInternal.FESHW.HW_MAX_DISPLAY_DIMENSION)
        {
            Debug.LogError("Invalid resolution");
            return false;
        }

        if (resolution.width % 2 != 0 || resolution.height % 2 != 0)
        {
            Debug.LogError("Display width and height must both be divisible by 2!");
            return false;
        }

        if ((int)pixelStyle < (int)PixelStyle.Square || (int)pixelStyle > (int)PixelStyle.Tall)
        {
            Debug.LogError("Invalid pixel style");
            return false;
        }

        return Instance.mFESAPI.Renderer.DisplayModeSet(resolution, pixelStyle);
    }

    /// <summary>
    /// Get Sprite size of the current sprite sheet.
    /// </summary>
    /// <returns>Sprite size</returns>
    public static Size2i SpriteSize()
    {
        return Instance.mFESAPI.Renderer.CurrentSpriteSheet.spriteSize;
    }

    /// <summary>
    /// Get Sprite size of the given sprite sheet.
    /// </summary>
    /// <param name="index">Spritesheet index</param>
    /// <returns>Sprite size</returns>
    public static Size2i SpriteSize(int index)
    {
        var renderer = Instance.mFESAPI.Renderer;

        if (index < 0 || index >= renderer.SpriteSheets.Length)
        {
            return Size2i.zero;
        }

        return renderer.SpriteSheets[index].spriteSize;
    }

    /// <summary>
    /// Get Sprite sheet size of the current sprite sheet.
    /// </summary>
    /// <returns>Sprite sheet size</returns>
    public static Size2i SpriteSheetSize()
    {
        return Instance.mFESAPI.Renderer.CurrentSpriteSheet.textureSize;
    }

    /// <summary>
    /// Get size of the given sprite sheet.
    /// </summary>
    /// <param name="index">Spritesheet index</param>
    /// <returns>Return sprite sheet size</returns>
    public static Size2i SpriteSheetSize(int index)
    {
        var renderer = Instance.mFESAPI.Renderer;

        if (index < 0 || index >= renderer.SpriteSheets.Length)
        {
            return Size2i.zero;
        }

        return renderer.SpriteSheets[index].textureSize;
    }

    /// <summary>
    /// Clear the display.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="colorIndex">Color to clear with</param>
    public static void Clear(int colorIndex)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("Clear for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount)
        {
            return;
        }

        Instance.mFESAPI.Renderer.Clear(colorIndex, Instance.mSolidWhite);
    }

    /// <summary>
    /// Clear the display.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="color">Color to clear with</param>
    public static void Clear(ColorRGBA color)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("Clear for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Renderer.Clear(0, color.ToColor32());
    }

    /// <summary>
    /// Clear the offscreen surface to transparency.
    /// </summary>
    /// <param name="offscreenIndex">Index of the offscreen to clear</param>
    public static void OffscreenClear(int offscreenIndex = 0)
    {
        if (offscreenIndex < 0 || offscreenIndex >= FESInternal.FESHW.HW_RENDER_TARGETS)
        {
            Debug.LogError("OffscreenClear offscreen index is invalid!");
            return;
        }

        var renderer = Instance.mFESAPI.Renderer;

        if (!renderer.OffscreenValid(offscreenIndex))
        {
            FESInternal.FESUtil.LogErrorOnce("Offscreen is not setup, or was deleted. Use FES.OffscreenSetup to create an offscreen surface.");
            return;
        }

        renderer.OffscreenClear(offscreenIndex);
    }

    /// <summary>
    /// Helper function for calculating sprite index given sprite sheet column and row.
    /// </summary>
    /// <remarks>There is no bounds checking performed, invalid column/row will produce invalid sprite index</remarks>
    /// <param name="column">Column</param>
    /// <param name="row">Row</param>
    /// <returns>Sprite index</returns>
    public static int SpriteIndex(int column, int row)
    {
        return column + (row * Instance.mFESAPI.Renderer.CurrentSpriteSheet.columns);
    }

    /// <summary>
    /// Helper function for calculating sprite index given sprite sheet column and row.
    /// </summary>
    /// <remarks>There is no bounds checking performed, invalid column/row will produce invalid sprite index</remarks>
    /// <param name="spriteSheetIndex">Spritesheet for which to get sprite index for</param>
    /// <param name="column">Column</param>
    /// <param name="row">Row</param>
    /// <returns>Sprite index</returns>
    public static int SpriteIndex(int spriteSheetIndex, int column, int row)
    {
        var renderer = Instance.mFESAPI.Renderer;
        if (spriteSheetIndex < 0 || spriteSheetIndex >= renderer.SpriteSheets.Length)
        {
            return 0;
        }

        var spriteSheet = renderer.SpriteSheets[spriteSheetIndex];
        return column + (row * spriteSheet.columns);
    }

    /// <summary>
    /// Helper function for calculating sprite index given sprite sheet column and row
    /// </summary>
    /// <remarks>There is no bounds checking performed, invalid column/row will produce invalid sprite index</remarks>
    /// <param name="spriteSheetIndex">Spritesheet for which to get sprite index for</param>
    /// <param name="cell">Cell in sprite sheet where <see cref="Vector2i.x"/> is the column and <see cref="Vector2i.y"/> is the row.</param>
    /// <returns>Sprite index</returns>
    public static int SpriteIndex(int spriteSheetIndex, Vector2i cell)
    {
        var renderer = Instance.mFESAPI.Renderer;
        if (spriteSheetIndex < 0 || spriteSheetIndex >= renderer.SpriteSheets.Length)
        {
            return 0;
        }

        var spriteSheet = renderer.SpriteSheets[spriteSheetIndex];
        return cell.x + (cell.y * spriteSheet.columns);
    }

    /// <summary>
    /// Load a sprite sheet and define the size of the sprites in it.
    /// </summary>
    /// <remarks>The size of sprites in the sprite sheet also determines with size of each tile in a tilemap layer that uses this sprite sheet.</remarks>
    /// <param name="index">Sprite sheet slot index to load into</param>
    /// <param name="filename">Filename of the sprite sheet file, must be within a Resources folder</param>
    /// <param name="spriteSize">The size of sprites in this sprite sheet</param>
    public static void SpriteSheetSetup(int index, string filename, Size2i spriteSize)
    {
        if (index < 0 || index >= FESInternal.FESHW.HW_MAX_SPRITESHEETS)
        {
            Debug.Log("Invalid sprite sheet index");
            return;
        }

        if (spriteSize.width <= 0 || spriteSize.height <= 0)
        {
            Debug.Log("Invalid sprite size");
            return;
        }

        Instance.mFESAPI.Renderer.SpriteSheetSetup(index, filename, spriteSize);
    }

    /// <summary>
    /// Delete the given sprite sheet, freeing up GPU resources
    /// </summary>
    /// <param name="index">Sprite sheet slot index to delete</param>
    public static void SpriteSheetDelete(int index)
    {
        if (index < 0 || index >= FESInternal.FESHW.HW_MAX_SPRITESHEETS)
        {
            Debug.Log("Invalid sprite sheet index");
            return;
        }

        Instance.mFESAPI.Renderer.SpriteSheetSetup(index, null, Size2i.zero);
    }

    /// <summary>
    /// Switch to the given sprite sheet.
    /// </summary>
    /// <remarks>All sprite rendering from this point on will use this sprite sheet until a different sprite sheet is chosen.</remarks>
    /// <param name="index">Sprite sheet slot index to switch to.</param>
    public static void SpriteSheetSet(int index)
    {
        if (index < 0 || index >= FESInternal.FESHW.HW_MAX_SPRITESHEETS)
        {
            Debug.Log("Invalid sprite sheet index");
            return;
        }

        var renderer = Instance.mFESAPI.Renderer;

        // Do nothing if no change
        if (index == renderer.CurrentSpriteSheetIndex)
        {
            return;
        }

        renderer.SpriteSheetSet(index);
    }

    /// <summary>
    /// Gets the index of the current sprite sheet.
    /// </summary>
    /// <returns>Index of the current sprite sheet</returns>
    public static int SpriteSheetGet()
    {
        return Instance.mFESAPI.Renderer.CurrentSpriteSheetIndex;
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet.
    /// </summary>
    /// <param name="spriteIndex">Sprite index</param>
    /// <param name="pos">Position on display</param>
    /// <param name="flags">Any combination of flags: <see cref="FES.FLIP_H"/>, <see cref="FES.FLIP_V"/>,
    /// <see cref="FES.ROT_90_CW"/>, <see cref="FES.ROT_180_CW"/>, <see cref="FES.ROT_270_CW"/>,
    /// <see cref="FES.ROT_90_CCW"/>, <see cref="FES.ROT_180_CCW"/>, <see cref="FES.ROT_270_CCW"/>.</param>
    public static void DrawSprite(int spriteIndex, Vector2i pos, int flags = 0)
    {
        var renderer = Instance.mFESAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        if (spriteIndex < 0 || spriteIndex >= (currentSpriteSheet.rows * currentSpriteSheet.columns))
        {
            return;
        }

        int row = spriteIndex % currentSpriteSheet.columns;
        int col = spriteIndex / currentSpriteSheet.columns;

        var spriteSize = renderer.CurrentSpriteSheet.spriteSize;

        Rect2i srcRect = new Rect2i(row * spriteSize.width, col * spriteSize.height, spriteSize.width, spriteSize.height);

        renderer.DrawTexture(srcRect, new Rect2i(pos.x, pos.y, spriteSize.width, spriteSize.height), new Vector2i(0, 0), 0, flags, false);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, and a destination rectangle.
    /// </summary>
    /// <param name="spriteIndex">Sprite index</param>
    /// <param name="destRect">Destination rectangle on the display</param>
    /// <param name="flags">Any combination of flags: <see cref="FES.FLIP_H"/>, <see cref="FES.FLIP_V"/>,
    /// <see cref="FES.ROT_90_CW"/>, <see cref="FES.ROT_180_CW"/>, <see cref="FES.ROT_270_CW"/>,
    /// <see cref="FES.ROT_90_CCW"/>, <see cref="FES.ROT_180_CCW"/>, <see cref="FES.ROT_270_CCW"/>.</param>
    public static void DrawSprite(int spriteIndex, Rect2i destRect, int flags = 0)
    {
        var renderer = Instance.mFESAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        if (spriteIndex < 0 || spriteIndex >= (currentSpriteSheet.rows * currentSpriteSheet.columns))
        {
            return;
        }

        int row = spriteIndex % currentSpriteSheet.columns;
        int col = spriteIndex / currentSpriteSheet.columns;

        var spriteSize = renderer.CurrentSpriteSheet.spriteSize;

        Rect2i srcRect = new Rect2i(row * spriteSize.width, col * spriteSize.height, spriteSize.width, spriteSize.height);

        renderer.DrawTexture(srcRect, destRect, new Vector2i(0, 0), 0, flags, false);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a position on the display, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteIndex">Sprite index</param>
    /// <param name="pos">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the sprites top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="FES.FLIP_H"/>, <see cref="FES.FLIP_V"/>,
    /// <see cref="FES.ROT_90_CW"/>, <see cref="FES.ROT_180_CW"/>, <see cref="FES.ROT_270_CW"/>,
    /// <see cref="FES.ROT_90_CCW"/>, <see cref="FES.ROT_180_CCW"/>, <see cref="FES.ROT_270_CCW"/>.</param>
    public static void DrawSprite(int spriteIndex, Vector2i pos, Vector2i pivot, float rotation, int flags = 0)
    {
        var renderer = Instance.mFESAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        if (spriteIndex < 0 || spriteIndex >= (currentSpriteSheet.rows * currentSpriteSheet.columns))
        {
            return;
        }

        int row = spriteIndex % currentSpriteSheet.columns;
        int col = spriteIndex / currentSpriteSheet.columns;

        var spriteSize = renderer.CurrentSpriteSheet.spriteSize;

        Rect2i srcRect = new Rect2i(row * spriteSize.width, col * spriteSize.height, spriteSize.width, spriteSize.height);

        renderer.DrawTexture(srcRect, new Rect2i(pos.x, pos.y, spriteSize.width, spriteSize.height), pivot, rotation, flags, false);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a destination rectangle on the display, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteIndex">Sprite index</param>
    /// <param name="destRect">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="FES.FLIP_H"/>, <see cref="FES.FLIP_V"/>,
    /// <see cref="FES.ROT_90_CW"/>, <see cref="FES.ROT_180_CW"/>, <see cref="FES.ROT_270_CW"/>,
    /// <see cref="FES.ROT_90_CCW"/>, <see cref="FES.ROT_180_CCW"/>, <see cref="FES.ROT_270_CCW"/>.</param>
    public static void DrawSprite(int spriteIndex, Rect2i destRect, Vector2i pivot, float rotation, int flags = 0)
    {
        var renderer = Instance.mFESAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        if (spriteIndex < 0 || spriteIndex >= (currentSpriteSheet.rows * currentSpriteSheet.columns))
        {
            return;
        }

        int row = spriteIndex % currentSpriteSheet.columns;
        int col = spriteIndex / currentSpriteSheet.columns;

        var spriteSize = renderer.CurrentSpriteSheet.spriteSize;

        Rect2i srcRect = new Rect2i(row * spriteSize.width, col * spriteSize.height, spriteSize.width, spriteSize.height);

        renderer.DrawTexture(srcRect, destRect, pivot, rotation, flags, false);
    }

    /// <summary>
    /// Copy a rectangular region from the sprite sheet to a position on the display.
    /// </summary>
    /// <param name="srcRect">Source rectangle on the sprite sheet</param>
    /// <param name="pos">Position on the display</param>
    /// <param name="flags">Any combination of flags: <see cref="FES.FLIP_H"/>, <see cref="FES.FLIP_V"/>,
    /// <see cref="FES.ROT_90_CW"/>, <see cref="FES.ROT_180_CW"/>, <see cref="FES.ROT_270_CW"/>,
    /// <see cref="FES.ROT_90_CCW"/>, <see cref="FES.ROT_180_CCW"/>, <see cref="FES.ROT_270_CCW"/>.</param>
    public static void DrawCopy(Rect2i srcRect, Vector2i pos, int flags = 0)
    {
        var renderer = Instance.mFESAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        if (srcRect.x < 0)
        {
            srcRect.x = 0;
        }

        if (srcRect.y < 0)
        {
            srcRect.y = 0;
        }

        if (srcRect.x + srcRect.width > currentSpriteSheet.textureSize.width)
        {
            srcRect.width -= srcRect.x + srcRect.width - currentSpriteSheet.textureSize.width;
        }

        if (srcRect.y + srcRect.height > currentSpriteSheet.textureSize.height)
        {
            srcRect.height -= srcRect.y + srcRect.height - currentSpriteSheet.textureSize.height;
        }

        renderer.DrawTexture(srcRect, new Rect2i(pos.x, pos.y, srcRect.width, srcRect.height), new Vector2i(0, 0), 0, flags, false);
    }

    /// <summary>
    /// Copy a rectangular region from the sprite sheet to a position on the display, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="srcRect">Source rectangle on the sprite sheet</param>
    /// <param name="pos">Position on the display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the rectangles top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="FES.FLIP_H"/>, <see cref="FES.FLIP_V"/>,
    /// <see cref="FES.ROT_90_CW"/>, <see cref="FES.ROT_180_CW"/>, <see cref="FES.ROT_270_CW"/>,
    /// <see cref="FES.ROT_90_CCW"/>, <see cref="FES.ROT_180_CCW"/>, <see cref="FES.ROT_270_CCW"/>.</param>
    public static void DrawCopy(Rect2i srcRect, Vector2i pos, Vector2i pivot, float rotation, int flags = 0)
    {
        var renderer = Instance.mFESAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        if (srcRect.x < 0 || srcRect.y < 0 || srcRect.x + srcRect.width > currentSpriteSheet.textureSize.width || srcRect.y + srcRect.height > currentSpriteSheet.textureSize.height)
        {
            return;
        }

        renderer.DrawTexture(srcRect, new Rect2i(pos.x, pos.y, srcRect.width, srcRect.height), pivot, rotation, flags, false);
    }

    /// <summary>
    /// Copy a rectangular region from the sprite sheet to a destination rectangle on the display.
    /// </summary>
    /// <param name="srcRect">Source rectangle on the sprite sheet</param>
    /// <param name="destRect">Destination rectangle on the display</param>
    /// <param name="flags">Any combination of flags: <see cref="FES.FLIP_H"/>, <see cref="FES.FLIP_V"/>,
    /// <see cref="FES.ROT_90_CW"/>, <see cref="FES.ROT_180_CW"/>, <see cref="FES.ROT_270_CW"/>,
    /// <see cref="FES.ROT_90_CCW"/>, <see cref="FES.ROT_180_CCW"/>, <see cref="FES.ROT_270_CCW"/>.</param>
    public static void DrawCopy(Rect2i srcRect, Rect2i destRect, int flags = 0)
    {
        var renderer = Instance.mFESAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        if (srcRect.x < 0 || srcRect.y < 0 || srcRect.x + srcRect.width > currentSpriteSheet.textureSize.width || srcRect.y + srcRect.height > currentSpriteSheet.textureSize.height)
        {
            return;
        }

        renderer.DrawTexture(srcRect, destRect, new Vector2i(0, 0), 0, flags, false);
    }

    /// <summary>
    /// Copy a rectangular region from the sprite sheet to a destination rectangle on the display, with the given pivot point and rotation in degrees.
    /// </summary>
    /// <param name="srcRect">Source rectangle on the sprite sheet</param>
    /// <param name="destRect">Destination rectangle on the display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="FES.FLIP_H"/>, <see cref="FES.FLIP_V"/>,
    /// <see cref="FES.ROT_90_CW"/>, <see cref="FES.ROT_180_CW"/>, <see cref="FES.ROT_270_CW"/>,
    /// <see cref="FES.ROT_90_CCW"/>, <see cref="FES.ROT_180_CCW"/>, <see cref="FES.ROT_270_CCW"/>.</param>
    public static void DrawCopy(Rect2i srcRect, Rect2i destRect, Vector2i pivot, float rotation, int flags = 0)
    {
        var renderer = Instance.mFESAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        if (srcRect.x < 0 || srcRect.y < 0 || srcRect.x + srcRect.width > currentSpriteSheet.textureSize.width || srcRect.y + srcRect.height > currentSpriteSheet.textureSize.height)
        {
            return;
        }

        renderer.DrawTexture(srcRect, destRect, pivot, rotation, flags, false);
    }

    /// <summary>
    /// Copy a rectangular region from the offscreen surface to a position on the display.
    /// </summary>
    /// <remarks>To switch between drawing on the offscreen surface and the display see <see cref="FES.Offscreen"/> and <see cref="FES.Onscreen"/></remarks>
    /// <param name="offscreenIndex">Offscreen to copy from</param>
    /// <param name="srcRect">Source rectangle on the offscreen surface</param>
    /// <param name="pos">Position on the display</param>
    /// <param name="flags">Any combination of flags: <see cref="FES.FLIP_H"/>, <see cref="FES.FLIP_V"/>,
    /// <see cref="FES.ROT_90_CW"/>, <see cref="FES.ROT_180_CW"/>, <see cref="FES.ROT_270_CW"/>,
    /// <see cref="FES.ROT_90_CCW"/>, <see cref="FES.ROT_180_CCW"/>, <see cref="FES.ROT_270_CCW"/>.</param>
    public static void DrawCopyOffscreen(int offscreenIndex, Rect2i srcRect, Vector2i pos, int flags = 0)
    {
        var renderer = Instance.mFESAPI.Renderer;

        if (!renderer.OffscreenValid(offscreenIndex))
        {
            FESInternal.FESUtil.LogErrorOnce("Offscreen is not setup, or was deleted. Use FES.OffscreenSetup to create an offscreen surface.");
            return;
        }

        renderer.DrawTexture(srcRect, new Rect2i(pos.x, pos.y, srcRect.width, srcRect.height), new Vector2i(0, 0), 0, flags, false, offscreenIndex);
    }

    /// <summary>
    /// Copy a rectangular region from the offscreen surface to a position on the display with the given pivot point and rotation in degrees.
    /// </summary>
    /// <param name="offscreenIndex">Offscreen to copy from</param>
    /// <param name="srcRect">Source rectangle on the offscreen surface</param>
    /// <param name="pos">Position on the display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="FES.FLIP_H"/>, <see cref="FES.FLIP_V"/>,
    /// <see cref="FES.ROT_90_CW"/>, <see cref="FES.ROT_180_CW"/>, <see cref="FES.ROT_270_CW"/>,
    /// <see cref="FES.ROT_90_CCW"/>, <see cref="FES.ROT_180_CCW"/>, <see cref="FES.ROT_270_CCW"/>.</param>
    public static void DrawCopyOffscreen(int offscreenIndex, Rect2i srcRect, Vector2i pos, Vector2i pivot, float rotation, int flags = 0)
    {
        var renderer = Instance.mFESAPI.Renderer;

        if (!renderer.OffscreenValid(offscreenIndex))
        {
            FESInternal.FESUtil.LogErrorOnce("Offscreen is not setup, or was deleted. Use FES.OffscreenSetup to create an offscreen surface.");
            return;
        }

        renderer.DrawTexture(srcRect, new Rect2i(pos.x, pos.y, srcRect.width, srcRect.height), pivot, rotation, flags, false, offscreenIndex);
    }

    /// <summary>
    /// Copy a rectangular region from the offscreen surface to a destination rectangle on the display.
    /// </summary>
    /// <param name="offscreenIndex">Offscreen to copy from</param>
    /// <param name="srcRect">Source rectangle on the offscreen surface</param>
    /// <param name="destRect">Destination rectangle on the display</param>
    /// <param name="flags">Any combination of flags: <see cref="FES.FLIP_H"/>, <see cref="FES.FLIP_V"/>,
    /// <see cref="FES.ROT_90_CW"/>, <see cref="FES.ROT_180_CW"/>, <see cref="FES.ROT_270_CW"/>,
    /// <see cref="FES.ROT_90_CCW"/>, <see cref="FES.ROT_180_CCW"/>, <see cref="FES.ROT_270_CCW"/>.</param>
    public static void DrawCopyOffscreen(int offscreenIndex, Rect2i srcRect, Rect2i destRect, int flags = 0)
    {
        var renderer = Instance.mFESAPI.Renderer;

        if (!renderer.OffscreenValid(offscreenIndex))
        {
            FESInternal.FESUtil.LogErrorOnce("Offscreen is not setup, or was deleted. Use FES.OffscreenSetup to create an offscreen surface.");
            return;
        }

        renderer.DrawTexture(srcRect, destRect, new Vector2i(0, 0), 0, flags, false, offscreenIndex);
    }

    /// <summary>
    /// Copy a rectangular region from the offscreen surface to a destination rectangle on the display, given pivot point and rotation in degrees.
    /// </summary>
    /// <param name="offscreenIndex">Offscreen to copy from</param>
    /// <param name="srcRect">Source rectangle on the offscreen surface</param>
    /// <param name="destRect">Destination rectangle on the display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="FES.FLIP_H"/>, <see cref="FES.FLIP_V"/>,
    /// <see cref="FES.ROT_90_CW"/>, <see cref="FES.ROT_180_CW"/>, <see cref="FES.ROT_270_CW"/>,
    /// <see cref="FES.ROT_90_CCW"/>, <see cref="FES.ROT_180_CCW"/>, <see cref="FES.ROT_270_CCW"/>.</param>
    public static void DrawCopyOffscreen(int offscreenIndex, Rect2i srcRect, Rect2i destRect, Vector2i pivot, float rotation, int flags = 0)
    {
        var renderer = Instance.mFESAPI.Renderer;

        if (!renderer.OffscreenValid(offscreenIndex))
        {
            FESInternal.FESUtil.LogErrorOnce("Offscreen is not setup, or was deleted. Use FES.OffscreenSetup to create an offscreen surface.");
            return;
        }

        renderer.DrawTexture(srcRect, destRect, pivot, rotation, flags, false, offscreenIndex);
    }

    /// <summary>
    /// Draw a nice-slice sprite. Only need to pass one corner, one side, and middle, the rest is mirrored.
    /// </summary>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="srcTopLeftCornerRect">Source rectangle of the top left corner</param>
    /// <param name="srcTopSideRect">Source rectangle of the top side</param>
    /// <param name="srcMiddleRect">Source rectangle of the middle</param>
    public static void DrawNineSlice(Rect2i destRect, Rect2i srcTopLeftCornerRect, Rect2i srcTopSideRect, Rect2i srcMiddleRect)
    {
        Instance.mFESAPI.Renderer.DrawNineSlice(
            destRect,
            srcTopLeftCornerRect,
            0,
            srcTopSideRect,
            0,
            srcTopLeftCornerRect,
            FES.FLIP_H,
            srcTopSideRect,
            FES.ROT_90_CCW,
            srcMiddleRect,
            srcTopSideRect,
            FES.ROT_90_CW,
            srcTopLeftCornerRect,
            FES.FLIP_V,
            srcTopSideRect,
            FES.FLIP_V,
            srcTopLeftCornerRect,
            FES.FLIP_H | FES.FLIP_V);
    }

    /// <summary>
    /// Draw a nice-slice sprite.
    /// </summary>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="srcTopLeftCornerRect">Source rectangle of the top left corner</param>
    /// <param name="srcTopSideRect">Source rectangle of the top side</param>
    /// <param name="srcTopRightCornerRect">Source rectangle of the top right corner</param>
    /// <param name="srcLeftSideRect">Source rectangle of the left side</param>
    /// <param name="srcMiddleRect">Source rectangle of the middle</param>
    /// <param name="srcRightSideRect">Source rectangle of the right side</param>
    /// <param name="srcBottomLeftCornerRect">Source rectangle of the bottom left corner</param>
    /// <param name="srcBottomSideRect">Source rectangle of the bottom side</param>
    /// <param name="srcBottomRightCornerRect">Source rectangle of the bottom right corner</param>
    public static void DrawNineSlice(
        Rect2i destRect,
        Rect2i srcTopLeftCornerRect,
        Rect2i srcTopSideRect,
        Rect2i srcTopRightCornerRect,
        Rect2i srcLeftSideRect,
        Rect2i srcMiddleRect,
        Rect2i srcRightSideRect,
        Rect2i srcBottomLeftCornerRect,
        Rect2i srcBottomSideRect,
        Rect2i srcBottomRightCornerRect)
    {
        Instance.mFESAPI.Renderer.DrawNineSlice(
            destRect,
            srcTopLeftCornerRect,
            0,
            srcTopSideRect,
            0,
            srcTopRightCornerRect,
            0,
            srcLeftSideRect,
            0,
            srcMiddleRect,
            srcRightSideRect,
            0,
            srcBottomLeftCornerRect,
            0,
            srcBottomSideRect,
            0,
            srcBottomRightCornerRect,
            0);
    }

    /// <summary>
    /// Draw a nice-slice sprite.
    /// </summary>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="nineSlice">NineSlice defining the parts of the nine-slice image</param>
    public static void DrawNineSlice(Rect2i destRect, NineSlice nineSlice)
    {
        Instance.mFESAPI.Renderer.DrawNineSlice(
            destRect,
            nineSlice.TopLeftCornerRect,
            nineSlice.FlagsTopLeftCorner,
            nineSlice.TopSideRect,
            nineSlice.FlagsTopSide,
            nineSlice.TopRightCornerRect,
            nineSlice.FlagsTopRightCorner,
            nineSlice.LeftSideRect,
            nineSlice.FlagsLeftSide,
            nineSlice.MiddleRect,
            nineSlice.RightSideRect,
            nineSlice.FlagsRightSide,
            nineSlice.BottomLeftCornerRect,
            nineSlice.FlagsBottomLeftCorner,
            nineSlice.BottomSideRect,
            nineSlice.FlagsBottomSide,
            nineSlice.BottomRightCornerRect,
            nineSlice.FlagsBottomRightCorner);
    }

    /// <summary>
    /// Draw a pixel on the display.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="pos">Position on the display</param>
    /// <param name="colorIndex">Color</param>
    public static void DrawPixel(Vector2i pos, int colorIndex)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawPixel for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount)
        {
            return;
        }

        Instance.mFESAPI.Renderer.DrawPixel(pos.x, pos.y, colorIndex, Instance.mSolidWhite);
    }

    /// <summary>
    /// Draw a pixel on the display.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="pos">Position on the display</param>
    /// <param name="color">Color</param>
    public static void DrawPixel(Vector2i pos, ColorRGBA color)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawPixel for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Renderer.DrawPixel(pos.x, pos.y, 0, color.ToColor32());
    }

    /// <summary>
    /// Draw a rectangle outline on the display
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="rect">Rectangular area</param>
    /// <param name="colorIndex">Color</param>
    public static void DrawRect(Rect2i rect, int colorIndex)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawRect for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount)
        {
            return;
        }

        Instance.mFESAPI.Renderer.DrawRect(rect, colorIndex, Instance.mSolidWhite, Vector2i.zero);
    }

    /// <summary>
    /// Draw a rectangle outline on the display with a pivot point and rotation in degrees
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="rect">Rectangular area</param>
    /// <param name="colorIndex">Color</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawRect(Rect2i rect, int colorIndex, Vector2i pivot, float rotation)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawRect for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount)
        {
            return;
        }

        Instance.mFESAPI.Renderer.DrawRect(rect, colorIndex, Instance.mSolidWhite, pivot, rotation);
    }

    /// <summary>
    /// Draw a rectangle outline on the display
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="rect">Rectangular area</param>
    /// <param name="color">Color</param>
    public static void DrawRect(Rect2i rect, ColorRGBA color)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawRect for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Renderer.DrawRect(rect, 0, color.ToColor32(), Vector2i.zero);
    }

    /// <summary>
    /// Draw a rectangle outline on the display with a pivot point and rotation in degrees
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="rect">Rectangular area</param>
    /// <param name="color">Color</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawRect(Rect2i rect, ColorRGBA color, Vector2i pivot, float rotation)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawRect for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Renderer.DrawRect(rect, 0, color.ToColor32(), pivot, rotation);
    }

    /// <summary>
    /// Draw a filled rectangle on the display
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="rect">Rectangular area</param>
    /// <param name="colorIndex">Color</param>
    public static void DrawRectFill(Rect2i rect, int colorIndex)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawRectFill for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount)
        {
            return;
        }

        Instance.mFESAPI.Renderer.DrawRectFill(rect, colorIndex, Instance.mSolidWhite, Vector2i.zero);
    }

    /// <summary>
    /// Draw a filled rectangle on the display with a pivot point and rotation in degrees
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="rect">Rectangular area</param>
    /// <param name="colorIndex">Color</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawRectFill(Rect2i rect, int colorIndex, Vector2i pivot, float rotation)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawRectFill for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount)
        {
            return;
        }

        Instance.mFESAPI.Renderer.DrawRectFill(rect, colorIndex, Instance.mSolidWhite, pivot, rotation);
    }

    /// <summary>
    /// Draw a filled rectangle on the display
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="rect">Rectangular area</param>
    /// <param name="color">Color</param>
    public static void DrawRectFill(Rect2i rect, ColorRGBA color)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawRectFill for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Renderer.DrawRectFill(rect, 0, color.ToColor32(), Vector2i.zero);
    }

    /// <summary>
    /// Draw a filled rectangle on the display with a pivot point and rotation in degrees
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="rect">Rectangular area</param>
    /// <param name="color">Color</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawRectFill(Rect2i rect, ColorRGBA color, Vector2i pivot, float rotation)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawRectFill for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Renderer.DrawRectFill(rect, 0, color.ToColor32(), pivot, rotation);
    }

    /// <summary>
    /// Draw a line on the display between two points.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="colorIndex">Color</param>
    public static void DrawLine(Vector2i p0, Vector2i p1, int colorIndex)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawLine for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount)
        {
            return;
        }

        Instance.mFESAPI.Renderer.DrawLine(p0, p1, colorIndex, Instance.mSolidWhite, Vector2i.zero, 0);
    }

    /// <summary>
    /// Draw a line on the display between two points, with a pivot point and rotation in degrees
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="colorIndex">Color</param>
    /// <param name="pivot">Pivot point</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawLine(Vector2i p0, Vector2i p1, int colorIndex, Vector2i pivot, float rotation)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawLine for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount)
        {
            return;
        }

        Instance.mFESAPI.Renderer.DrawLine(p0, p1, colorIndex, Instance.mSolidWhite, pivot, rotation);
    }

    /// <summary>
    /// Draw a line on the display between two points.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="color">Color</param>
    public static void DrawLine(Vector2i p0, Vector2i p1, ColorRGBA color)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawLine for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Renderer.DrawLine(p0, p1, 0, color.ToColor32(), new Vector2i(0, 0), 0);
    }

    /// <summary>
    /// Draw a line on the display between two points, with a pivot point and rotation in degrees.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="color">Color</param>
    /// <param name="pivot">Pivot point</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawLine(Vector2i p0, Vector2i p1, ColorRGBA color, Vector2i pivot, float rotation)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawLine for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Renderer.DrawLine(p0, p1, 0, color.ToColor32(), pivot, rotation);
    }

    /// <summary>
    /// Draw an ellipse outline on the display.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="center">Center position</param>
    /// <param name="radius">Radius</param>
    /// <param name="colorIndex">Color</param>
    public static void DrawEllipse(Vector2i center, Vector2i radius, int colorIndex)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawEllipse for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount)
        {
            return;
        }

        if (radius.x < 0 || radius.y < 0)
        {
            return;
        }

        Instance.mFESAPI.Renderer.DrawEllipse(center, radius, colorIndex, Instance.mSolidWhite);
    }

    /// <summary>
    /// Draw an ellipse outline on the display.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="center">Center position</param>
    /// <param name="radius">Radius</param>
    /// <param name="color">Color</param>
    public static void DrawEllipse(Vector2i center, Vector2i radius, ColorRGBA color)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawEllipse for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        if (radius.x < 0 || radius.y < 0)
        {
            return;
        }

        Instance.mFESAPI.Renderer.DrawEllipse(center, radius, 0, color.ToColor32());
    }

    /// <summary>
    /// Draw a filled ellipse on the display.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="center">Center position</param>
    /// <param name="radius">Radius</param>
    /// <param name="colorIndex">Color</param>
    public static void DrawEllipseFill(Vector2i center, Vector2i radius, int colorIndex)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawEllipseFill for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount)
        {
            return;
        }

        if (radius.x < 0 || radius.y < 0)
        {
            return;
        }

        Instance.mFESAPI.Renderer.DrawEllipseFill(center, radius, colorIndex, Instance.mSolidWhite, false);
    }

    /// <summary>
    /// Draw a filled ellipse on the display.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="center">Center position</param>
    /// <param name="radius">Radius</param>
    /// <param name="color">Color</param>
    public static void DrawEllipseFill(Vector2i center, Vector2i radius, ColorRGBA color)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawEllipseFill for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        if (radius.x < 0 || radius.y < 0)
        {
            return;
        }

        Instance.mFESAPI.Renderer.DrawEllipseFill(center, radius, 0, color.ToColor32(), false);
    }

    /// <summary>
    /// Draw an inversely filled ellipse on the display. The fill covers the area outside of the ellipse, while leaving
    /// the inside of the ellipse empty.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="center">Center position</param>
    /// <param name="radius">Radius</param>
    /// <param name="colorIndex">Color</param>
    public static void DrawEllipseInvertedFill(Vector2i center, Vector2i radius, int colorIndex)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawEllipseInverseFill for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount)
        {
            return;
        }

        if (radius.x < 0 || radius.y < 0)
        {
            return;
        }

        Instance.mFESAPI.Renderer.DrawEllipseFill(center, radius, colorIndex, Instance.mSolidWhite, true);
    }

    /// <summary>
    /// Draw an inversely filled ellipse on the display. The fill covers the area outside of the ellipse, while leaving
    /// the inside of the ellipse empty.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="center">Center position</param>
    /// <param name="radius">Radius</param>
    /// <param name="color">Color</param>
    public static void DrawEllipseInvertedFill(Vector2i center, Vector2i radius, ColorRGBA color)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("DrawEllipseInverseFill for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        if (radius.x < 0 || radius.y < 0)
        {
            return;
        }

        Instance.mFESAPI.Renderer.DrawEllipseFill(center, radius, 0, color.ToColor32(), true);
    }

    /// <summary>
    /// Draw the given map layer to the display.
    /// </summary>
    /// <param name="layer">Layer number to draw</param>
    public static void DrawMapLayer(int layer)
    {
        if (layer < 0 || layer >= FES.MapLayers)
        {
            Debug.Log("DrawMapLayer invalid map layer, you can request more layers in IFESGame.QueryHardware");
            return;
        }

        Instance.mFESAPI.Tilemap.DrawMapLayer(layer, new Vector2i(0, 0));
    }

    /// <summary>
    /// Draw the given map layer to the display with an offset.
    /// </summary>
    /// <param name="layer">Layer number to draw</param>
    /// <param name="pos">Offset</param>
    public static void DrawMapLayer(int layer, Vector2i pos)
    {
        if (layer < 0 || layer >= FES.MapLayers)
        {
            Debug.Log("DrawMapLayer invalid map layer, you can request more layers in IFESGame.QueryHardware");
            return;
        }

        Instance.mFESAPI.Tilemap.DrawMapLayer(layer, pos);
    }

    /// <summary>
    /// Set the alpha transparency value for drawing.
    /// </summary>
    /// <param name="a">A value between 0 (invisible) to 255 (solid)</param>
    public static void AlphaSet(byte a)
    {
        Instance.mFESAPI.Renderer.AlphaSet(a);
    }

    /// <summary>
    /// Get the current alpha transparency value
    /// </summary>
    /// <returns>Transparency</returns>
    public static byte AlphaGet()
    {
        return Instance.mFESAPI.Renderer.AlphaGet();
    }

    /// <summary>
    /// Set the tint color for drawing.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="tintColor">Tint color</param>
    public static void TintColorSet(ColorRGBA tintColor)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("TintColorSet called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Renderer.TintColorSet(tintColor.ToColor32());
    }

    /// <summary>
    /// Get the current tint color.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <returns>Tint color</returns>
    public static ColorRGBA TintColorGet()
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("TintColorGet called when Hardware is in Indexed mode!");
            return new ColorRGBA(0, 0, 0);
        }

        return Instance.mFESAPI.Renderer.TintColorGet();
    }

    /// <summary>
    /// Set the palette color at the given index to the given RGB color.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="colorIndex">Color index</param>
    /// <param name="color">RGB color</param>
    public static void PaletteColorSet(int colorIndex, ColorRGBA color)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("PaletteColorSet called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount)
        {
            return;
        }

        Instance.mFESAPI.Palette.PaletteColorSet(colorIndex, color);
    }

    /// <summary>
    /// Replace the palette with a new one from the given palette file
    /// </summary>
    /// <param name="filename">Filename of new palette file, must be within a Resources folder</param>
    public static void PaletteColorSet(string filename)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("PaletteColorSet called when Hardware is in RGB mode!");
            return;
        }

        Instance.mFESAPI.Palette.PaletteColorSet(filename);
    }

    /// <summary>
    /// Set up palette color swaps for the given swap index.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="swapIndex">Swap index, index 0 is reserved for default palette</param>
    /// <param name="colorIndexes">Array of color indexes where each element represents a color index and the value to swap for that color index.</param>
    public static void PaletteSwapSetup(int swapIndex, int[] colorIndexes)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("PaletteSwapSet called when Hardware is in RGB mode!");
            return;
        }

        if (swapIndex == 0)
        {
            FESInternal.FESUtil.LogErrorOnce("PaletteSwapSet index 0 is reserved for default palette colors");
            return;
        }

        if (swapIndex < 0 || swapIndex >= FESInternal.FESHW.HW_PALETTE_SWAPS)
        {
            return;
        }

        Instance.mFESAPI.Palette.PaletteSwapSetup(swapIndex, colorIndexes);
    }

    /// <summary>
    /// Set the current palette swap index for drawing
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="swapIndex">Swap index, 0 for original palette</param>
    public static void PaletteSwapSet(int swapIndex)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("PaletteSwapSet called when Hardware is in RGB mode!");
            return;
        }

        if (swapIndex < 0 || swapIndex >= FESInternal.FESHW.HW_PALETTE_SWAPS)
        {
            return;
        }

        Instance.mFESAPI.Renderer.PaletteSwapSet(swapIndex);
    }

    /// <summary>
    /// Get the current palette swap index
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <returns>Swap index</returns>
    public static int PaletteSwapGet()
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("PaletteSwapGet called when Hardware is in RGB mode!");
            return 0;
        }

        return FESInternal.FESHW.HW_PALETTE_SWAPS - Instance.mFESAPI.Renderer.PaletteSwapGet();
    }

    /// <summary>
    /// Set the current camera position for drawing
    /// </summary>
    /// <param name="pos">Position</param>
    public static void CameraSet(Vector2i pos)
    {
        Instance.mFESAPI.Renderer.CameraSet(pos);
    }

    /// <summary>
    /// Reset the camera position back to 0, 0. This is equivalent to <see cref="FES.CameraSet"/>(0, 0).
    /// </summary>
    public static void CameraReset()
    {
        Instance.mFESAPI.Renderer.CameraSet(Vector2i.zero);
    }

    /// <summary>
    /// Get the current camera position
    /// </summary>
    /// <returns>Camera position</returns>
    public static Vector2i CameraGet()
    {
        return Instance.mFESAPI.Renderer.CameraGet();
    }

    /// <summary>
    /// Set a rectangular clipping region. All drawing outside of this region will be clipped away. By default the clipping
    /// region covers the entire display.
    /// </summary>
    /// <param name="rect">Rectangular clip region</param>
    public static void ClipSet(Rect2i rect)
    {
        if (rect.width < 0 || rect.height < 0)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ClipSet(rect);
    }

    /// <summary>
    /// Reset the clipping region back to covering the entire render surface, be it the display, or current offscreen target.
    /// </summary>
    public static void ClipReset()
    {
        var renderer = Instance.mFESAPI.Renderer;
        var renderTarget = renderer.CurrentRenderTexture();
        if (renderTarget != null)
        {
            renderer.ClipSet(new Rect2i(0, 0, renderTarget.width, renderTarget.height));
        }
    }

    /// <summary>
    /// Get the current clipping region.
    /// </summary>
    /// <returns>Clipping region</returns>
    public static Rect2i ClipGet()
    {
        return Instance.mFESAPI.Renderer.ClipGet();
    }

    /// <summary>
    /// Enable clip region debugging by drawing rectangles around the clip regions with the given color index. Subsequent
    /// calls to ClipDebugEnable will change the color for the following clip regions. This debug state is reset at the
    /// start of each frame.
    /// </summary>
    /// <param name="colorIndex">Color index</param>
    public static void ClipDebugEnable(int colorIndex = 0)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("ClipDebugEnable for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= Instance.mFESAPI.HW.PaletteColorCount)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ClipDebugSet(true, colorIndex, ColorRGBA.white);
    }

    /// <summary>
    /// Enable clip region debugging by drawing rectangles around the clip regions with the given color index. Subsequent
    /// calls to ClipDebugEnable will change the color for subsequent calls to <ref>FES.ClipSet</ref>. This debug state is reset at the
    /// start of each frame.
    /// </summary>
    /// <param name="color">RGBA color</param>
    public static void ClipDebugEnable(ColorRGBA color)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("ClipDebugEnable for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Renderer.ClipDebugSet(true, 0, color);
    }

    /// <summary>
    /// Disable clip region debugging, will not draw clip region outlines for subsequent calls to <ref>FES.ClipSet</ref>.
    /// </summary>
    public static void ClipDebugDisable()
    {
        Instance.mFESAPI.Renderer.ClipDebugSet(false, 0, ColorRGBA.white);
    }

    /// <summary>
    /// Enable a batch debugging overlay which shows how many draw batches are being used
    /// </summary>
    /// <param name="fontColorIndex">Font color to use</param>
    /// <param name="backgroundColorIndex">Background color to use</param>
    public static void BatchDebugEnable(int fontColorIndex, int backgroundColorIndex)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("BatchDebugEnable for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (fontColorIndex < 0 || fontColorIndex >= Instance.mFESAPI.HW.PaletteColorCount)
        {
            FESInternal.FESUtil.LogErrorOnce("BatchDebugEnable invalid font color index");
            return;
        }

        if (backgroundColorIndex < 0 || backgroundColorIndex >= Instance.mFESAPI.HW.PaletteColorCount)
        {
            FESInternal.FESUtil.LogErrorOnce("BatchDebugEnable invalid background color index");
            return;
        }

        Instance.mFESAPI.Renderer.FlashDebugSet(true, fontColorIndex, backgroundColorIndex, ColorRGBA.white, ColorRGBA.black);
    }

    /// <summary>
    /// Enable a batch debugging overlay which shows how many draw batches are being used
    /// </summary>
    /// <param name="fontColor">Font color to use</param>
    /// <param name="backgroundColor">Background color to use</param>
    public static void BatchDebugEnable(ColorRGBA fontColor, ColorRGBA backgroundColor)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("BatchDebugEnable for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Renderer.FlashDebugSet(true, 1, 0, fontColor, backgroundColor);
    }

    /// <summary>
    /// Disable batch debugging overlay
    /// </summary>
    public static void BatchDebugDisable()
    {
        Instance.mFESAPI.Renderer.FlashDebugSet(false, 1, 0, ColorRGBA.white, ColorRGBA.black);
    }

    /// <summary>
    /// Setup a new offscreen surface of the given size. If one already exists it will be replaced.
    /// </summary>
    /// <param name="offscreenIndex">Offscreen index to setup</param>
    /// <param name="size">Size of the offscreen surface</param>
    /// <returns>True if successful</returns>
    public static bool OffscreenSetup(int offscreenIndex, Size2i size)
    {
        if (offscreenIndex < 0 || offscreenIndex >= FESInternal.FESHW.HW_RENDER_TARGETS - 1)
        {
            Debug.LogError("OffscreenSetup offscreen index is invalid!");
            return false;
        }

        if (size.width <= 0 || size.height <= 0 || size.width >= FESInternal.FESHW.HW_MAX_DISPLAY_DIMENSION || size.height >= FESInternal.FESHW.HW_MAX_DISPLAY_DIMENSION)
        {
            Debug.LogError("OffscreenSetup dimensions are invalid!");
            return false;
        }

        if (size.width % 2 != 0 || size.height % 2 != 0)
        {
            Debug.LogError("OffscreenSetup dimensions must be a multiple of 2");
            return false;
        }

        return Instance.mFESAPI.Renderer.OffscreenCreate(offscreenIndex, size.width, size.height);
    }

    /// <summary>
    /// Delete an existing offscreen surface, any rendering to or from this surface will fail.
    /// </summary>
    /// <param name="offscreenIndex">Offscreen surface to delete</param>
    public static void OffscreenDelete(int offscreenIndex)
    {
        if (offscreenIndex < 0 || offscreenIndex >= FESInternal.FESHW.HW_RENDER_TARGETS - 1)
        {
            Debug.LogError("OffscreenDelete offscreen index is invalid!");
            return;
        }

        Instance.mFESAPI.Renderer.OffscreenCreate(offscreenIndex, 0, 0);
    }

    /// <summary>
    /// Switch to drawing to the offscreen surface. See <see cref="FES.DrawCopyOffscreen(int, Rect2i, Rect2i, int)"/> for copying
    /// rectangular regions from the offscreen surface back to the display.
    /// </summary>
    /// <param name="offscreenIndex">Offscreen to switch to</param>
    public static void Offscreen(int offscreenIndex = 0)
    {
        var renderer = Instance.mFESAPI.Renderer;

        if (offscreenIndex < 0 || offscreenIndex >= FESInternal.FESHW.HW_RENDER_TARGETS - 1)
        {
            Debug.LogError("Offscreen index is invalid!");
            return;
        }

        if (!renderer.OffscreenValid(offscreenIndex))
        {
            FESInternal.FESUtil.LogErrorOnce("Offscreen is not setup, or was deleted. Use FES.OffscreenSetup to create an offscreen surface.");
            return;
        }

        renderer.OffscreenTarget(offscreenIndex);
    }

    /// <summary>
    /// Switch to drawing to the display.
    /// </summary>
    public static void Onscreen()
    {
        Instance.mFESAPI.Renderer.Onscreen();
    }

    /// <summary>
    /// Set the tilemap sprite index at the given tile position, with palette swap index and optional flags.
    /// </summary>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <param name="sprite">Sprite index</param>
    /// <param name="swapIndex">Palette swap index</param>
    /// <param name="flags">Sprite flags</param>
    public static void MapSpriteSet(int layer, Vector2i tilePos, int sprite, int swapIndex = -1, int flags = 0)
    {
        if (layer < 0 || layer >= FES.MapLayers)
        {
            Debug.Log("MapSpriteSet invalid map layer, you can request more layers in IFESGame.QueryHardware");
            return;
        }

        swapIndex++;
        swapIndex = FESInternal.FESHW.HW_PALETTE_SWAPS - swapIndex;

        Instance.mFESAPI.Tilemap.SpriteSet(layer, tilePos.x, tilePos.y, sprite, new ColorRGBA(255, 255, 255), swapIndex, flags);
    }

    /// <summary>
    /// Set the tilemap sprite index at the given tile position, with color tint (RGB color mode only) and optional flags.
    /// </summary>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <param name="sprite">Sprite index</param>
    /// <param name="tintColor">Tint color</param>
    /// <param name="flags">Sprite flags</param>
    public static void MapSpriteSet(int layer, Vector2i tilePos, int sprite, ColorRGBA tintColor, int flags = 0)
    {
        if (layer < 0 || layer >= FES.MapLayers)
        {
            Debug.Log("MapSpriteSet invalid map layer, you can request more layers in IFESGame.QueryHardware");
            return;
        }

        Instance.mFESAPI.Tilemap.SpriteSet(layer, tilePos.x, tilePos.y, sprite, tintColor, FESInternal.FESHW.HW_PALETTE_SWAPS, flags);
    }

    /// <summary>
    /// Get the sprite index at the given tile position
    /// </summary>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <returns>Sprite index</returns>
    public static int MapSpriteGet(int layer, Vector2i tilePos)
    {
        if (layer < 0 || layer >= FES.MapLayers)
        {
            Debug.Log("MapSpriteGet invalid map layer, you can request more layers in IFESGame.QueryHardware");
            return FES.SPRITE_INVALID;
        }

        return Instance.mFESAPI.Tilemap.SpriteGet(layer, tilePos.x, tilePos.y);
    }

    /// <summary>
    /// Set user data for the tilemap at the given tile position
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    /// <param name="tilePos">Tile position</param>
    /// <param name="data">Data</param>
    public static void MapDataSet<T>(Vector2i tilePos, T data)
    {
        Instance.mFESAPI.Tilemap.DataSet(tilePos.x, tilePos.y, data);
    }

    /// <summary>
    /// Get user data for the tilemap at the given tile position
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    /// <param name="tilePos">Tile position</param>
    /// <returns>Data</returns>
    public static T MapDataGet<T>(Vector2i tilePos)
    {
        object data = Instance.mFESAPI.Tilemap.DataGet<object>(tilePos.x, tilePos.y);
        if (data != null)
        {
            return (T)data;
        }
        else
        {
            return default(T);
        }
    }

    /// <summary>
    /// Clear the entire tilemap, on all layers.
    /// </summary>
    public static void MapClear()
    {
        Instance.mFESAPI.Tilemap.Clear(-1);
    }

    /// <summary>
    /// Clear only the given tilemap layer.
    /// </summary>
    /// <param name="layer">Layer to clear</param>
    public static void MapClear(int layer)
    {
        if (layer < 0 || layer >= FES.MapLayers)
        {
            return;
        }

        Instance.mFESAPI.Tilemap.Clear(layer);
    }

    /// <summary>
    /// Load a tilemap from a TMX tilemap format.
    /// </summary>
    /// <remarks>Unity will not package .tmx file resources, so TMX tilemaps must be saved with the .xml extension instead.
    /// File has to be somewhere inside a Resources folder.</remarks>
    /// <param name="fileName">Filename of the tilemap, must be within the Resources folder of your Unity project</param>
    /// <param name="layerName">Name of the layer to load, as specified inside of the TMX file</param>
    /// <param name="mapLayer">Map layer number to load into</param>
    /// <returns>True if successful</returns>
    public static bool MapLoadTMX(string fileName, string layerName, int mapLayer)
    {
        if (mapLayer < 0 || mapLayer >= FES.MapLayers)
        {
            Debug.Log("MapLoadTMX invalid map layer, you can request more layers in IFESGame.QueryHardware");
            return false;
        }

        Size2i dummySize;
        return MapLoadTMX(fileName, layerName, mapLayer, out dummySize);
    }

    /// <summary>
    /// Load a tilemap from a TMX tilemap format, and return the loaded map size.
    /// </summary>
    /// <remarks>Unity will not package .tmx file resources, so TMX tilemaps must be saved with the .xml extension instead.
    /// File has to be somewhere inside a Resources folder.</remarks>
    /// <param name="fileName">Filename of the tilemap, must be within the Resources folder of your Unity project</param>
    /// <param name="layerName">Name of the layer to load, as specified inside of the TMX file</param>
    /// <param name="mapLayer">Map layer number to load into</param>
    /// <param name="mapSize">Loaded map size</param>
    /// <returns>True if successful</returns>
    public static bool MapLoadTMX(string fileName, string layerName, int mapLayer, out Size2i mapSize)
    {
        if (mapLayer < 0 || mapLayer >= FES.MapLayers)
        {
            FESInternal.FESUtil.LogErrorOnce("MapLoadTMX invalid map layer, you can request more layers in IFESGame.QueryHardware");
            mapSize = Size2i.zero;
            return false;
        }

        return Instance.mFESAPI.Tilemap.LoadTMX(fileName, layerName, mapLayer, out mapSize);
    }

    /// <summary>
    /// Set the sprite sheet to be used by the given tilemap layer.
    /// </summary>
    /// <remarks>Each layer can use a different sprite sheet, and therefore each layer can have different tile sizes
    /// that match it's sprite sheet's sprite size.</remarks>
    /// <param name="mapLayer">Map layer number</param>
    /// <param name="spriteSheetIndex">Sprite sheet slot index to use</param>
    public static void MapLayerSpriteSheetSet(int mapLayer, int spriteSheetIndex)
    {
        if (mapLayer < 0 || mapLayer >= FES.MapLayers)
        {
            FESInternal.FESUtil.LogErrorOnce("MapLayerSpriteSheetSet invalid map layer, you can request more layers in IFESGame.QueryHardware");
            return;
        }

        if (spriteSheetIndex < 0 || spriteSheetIndex >= FESInternal.FESHW.HW_MAX_SPRITESHEETS)
        {
            FESInternal.FESUtil.LogErrorOnce("MapLayerSpriteSheetSet invalid sprite sheet index");
            return;
        }

        Instance.mFESAPI.Tilemap.MapLayerSpriteSheetSet(mapLayer, spriteSheetIndex);
    }

    /// <summary>
    /// Print text to display using system font at the given position.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="pos">Position</param>
    /// <param name="colorIndex">Color</param>
    /// <param name="text">Text</param>
    public static void Print(Vector2i pos, int colorIndex, string text)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("Print for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= Instance.mFESAPI.HW.PaletteColorCount || text == null)
        {
            return;
        }

        int width, height;
        Instance.mFESAPI.Font.Print(FESInternal.FESHW.HW_SYSTEM_FONT, new Rect2i(pos.x, pos.y, 1, 1), colorIndex, Instance.mSolidWhite, 0, text, false, out width, out height);
    }

    /// <summary>
    /// Print text to display using system font at the given position.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="pos">Position</param>
    /// <param name="color">Color</param>
    /// <param name="text">Text</param>
    public static void Print(Vector2i pos, ColorRGBA color, string text)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("Print for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        if (text == null)
        {
            return;
        }

        int width, height;
        Instance.mFESAPI.Font.Print(FESInternal.FESHW.HW_SYSTEM_FONT, new Rect2i(pos.x, pos.y, 1, 1), 0, color.ToColor32(), 0, text, false, out width, out height);
    }

    /// <summary>
    /// Print text to display using system font at the given position.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="colorIndex">Color</param>
    /// <param name="alignFlags">Any combination of flags: <see cref="FES.ALIGN_H_LEFT"/>, <see cref="FES.ALIGN_H_RIGHT"/>,
    /// <see cref="FES.ALIGN_H_CENTER"/>, <see cref="FES.ALIGN_V_TOP"/>, <see cref="FES.ALIGN_V_BOTTOM"/>,
    /// <see cref="FES.ALIGN_V_CENTER"/>, <see cref="FES.TEXT_OVERFLOW_CLIP"/>, <see cref="FES.TEXT_OVERFLOW_WRAP"/>.</param>
    /// <param name="text">Text</param>
    public static void Print(Rect2i rect, int colorIndex, int alignFlags, string text)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("Print for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= Instance.mFESAPI.HW.PaletteColorCount || text == null)
        {
            return;
        }

        int width, height;
        Instance.mFESAPI.Font.Print(FESInternal.FESHW.HW_SYSTEM_FONT, rect, colorIndex, Instance.mSolidWhite, alignFlags, text, false, out width, out height);
    }

    /// <summary>
    /// Print text to display using system font at the given position.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="color">Color</param>
    /// <param name="alignFlags">Any combination of flags: <see cref="FES.ALIGN_H_LEFT"/>, <see cref="FES.ALIGN_H_RIGHT"/>,
    /// <see cref="FES.ALIGN_H_CENTER"/>, <see cref="FES.ALIGN_V_TOP"/>, <see cref="FES.ALIGN_V_BOTTOM"/>,
    /// <see cref="FES.ALIGN_V_CENTER"/>, <see cref="FES.TEXT_OVERFLOW_CLIP"/>, <see cref="FES.TEXT_OVERFLOW_WRAP"/>.</param>
    /// <param name="text">Text</param>
    public static void Print(Rect2i rect, ColorRGBA color, int alignFlags, string text)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("Print for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        if (text == null)
        {
            return;
        }

        int width, height;
        Instance.mFESAPI.Font.Print(FESInternal.FESHW.HW_SYSTEM_FONT, rect, 0, color.ToColor32(), alignFlags, text, false, out width, out height);
    }

    /// <summary>
    /// Measure a text string without printing it, using system font.
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Dimensions</returns>
    public static Size2i PrintMeasure(string text)
    {
        if (text == null)
        {
            return Size2i.zero;
        }

        int width, height;
        Instance.mFESAPI.Font.Print(FESInternal.FESHW.HW_SYSTEM_FONT, new Rect2i(0, 0, 1, 1), 0, Instance.mSolidWhite, 0, text, true, out width, out height);
        return new Size2i(width, height);
    }

    /// <summary>
    /// Measure a text string without printing it, using system font.
    /// </summary>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="alignFlags">Any combination of flags: <see cref="FES.ALIGN_H_LEFT"/>, <see cref="FES.ALIGN_H_RIGHT"/>,
    /// <see cref="FES.ALIGN_H_CENTER"/>, <see cref="FES.ALIGN_V_TOP"/>, <see cref="FES.ALIGN_V_BOTTOM"/>,
    /// <see cref="FES.ALIGN_V_CENTER"/>, <see cref="FES.TEXT_OVERFLOW_CLIP"/>, <see cref="FES.TEXT_OVERFLOW_WRAP"/>.</param>
    /// <param name="text">Text</param>
    /// <returns>Dimensions</returns>
    public static Size2i PrintMeasure(Rect2i rect, int alignFlags, string text)
    {
        if (text == null)
        {
            return Size2i.zero;
        }

        int width, height;
        Instance.mFESAPI.Font.Print(FESInternal.FESHW.HW_SYSTEM_FONT, rect, 0, Instance.mSolidWhite, alignFlags, text, true, out width, out height);
        return new Size2i(width, height);
    }

    /// <summary>
    /// Print text to display using custom font at the given position.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="fontIndex">Font index</param>
    /// <param name="pos">Position</param>
    /// <param name="colorIndex">Color</param>
    /// <param name="text">Text</param>
    public static void Print(int fontIndex, Vector2i pos, int colorIndex, string text)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("Print for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount || text == null)
        {
            return;
        }

        if (fontIndex < 0 || fontIndex >= FESInternal.FESHW.HW_FONTS)
        {
            return;
        }

        int width, height;
        Instance.mFESAPI.Font.Print(fontIndex, new Rect2i(pos.x, pos.y, 1, 1), colorIndex, Instance.mSolidWhite, 0, text, false, out width, out height);
    }

    /// <summary>
    /// Print text to display using custom font at the given position.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="fontIndex">Font index</param>
    /// <param name="pos">Position</param>
    /// <param name="color">Color</param>
    /// <param name="text">Text</param>
    public static void Print(int fontIndex, Vector2i pos, ColorRGBA color, string text)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("Print for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        if (fontIndex < 0 || fontIndex >= FESInternal.FESHW.HW_FONTS || text == null)
        {
            return;
        }

        int width, height;
        Instance.mFESAPI.Font.Print(fontIndex, new Rect2i(pos.x, pos.y, 1, 1), 0, color.ToColor32(), 0, text, false, out width, out height);
    }

    /// <summary>
    /// Print text to display using custom font at the given position.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="fontIndex">Font index</param>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="colorIndex">Color</param>
    /// <param name="alignFlags">Any combination of flags: <see cref="FES.ALIGN_H_LEFT"/>, <see cref="FES.ALIGN_H_RIGHT"/>,
    /// <see cref="FES.ALIGN_H_CENTER"/>, <see cref="FES.ALIGN_V_TOP"/>, <see cref="FES.ALIGN_V_BOTTOM"/>,
    /// <see cref="FES.ALIGN_V_CENTER"/>, <see cref="FES.TEXT_OVERFLOW_CLIP"/>, <see cref="FES.TEXT_OVERFLOW_WRAP"/>.</param>
    /// <param name="text">Text</param>
    public static void Print(int fontIndex, Rect2i rect, int colorIndex, int alignFlags, string text)
    {
        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("Print for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount || text == null)
        {
            return;
        }

        if (fontIndex < 0 || fontIndex >= FESInternal.FESHW.HW_FONTS)
        {
            return;
        }

        int width, height;
        Instance.mFESAPI.Font.Print(fontIndex, rect, colorIndex, Instance.mSolidWhite, alignFlags, text, false, out width, out height);
    }

    /// <summary>
    /// Print text to display using custom font at the given position.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="fontIndex">Font index</param>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="color">Color</param>
    /// <param name="alignFlags">Any combination of flags: <see cref="FES.ALIGN_H_LEFT"/>, <see cref="FES.ALIGN_H_RIGHT"/>,
    /// <see cref="FES.ALIGN_H_CENTER"/>, <see cref="FES.ALIGN_V_TOP"/>, <see cref="FES.ALIGN_V_BOTTOM"/>,
    /// <see cref="FES.ALIGN_V_CENTER"/>, <see cref="FES.TEXT_OVERFLOW_CLIP"/>, <see cref="FES.TEXT_OVERFLOW_WRAP"/>.</param>
    /// <param name="text">Text</param>
    public static void Print(int fontIndex, Rect2i rect, ColorRGBA color, int alignFlags, string text)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("Print for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        if (fontIndex < 0 || fontIndex >= FESInternal.FESHW.HW_FONTS || text == null)
        {
            return;
        }

        int width, height;
        Instance.mFESAPI.Font.Print(fontIndex, rect, 0, color.ToColor32(), alignFlags, text, false, out width, out height);
    }

    /// <summary>
    /// Measure a text string without printing it, using custom font.
    /// </summary>
    /// <param name="fontIndex">Font index</param>
    /// <param name="text">Text</param>
    /// <returns>Dimensions</returns>
    public static Size2i PrintMeasure(int fontIndex, string text)
    {
        if (fontIndex < 0 || fontIndex >= FESInternal.FESHW.HW_FONTS || text == null)
        {
            return Size2i.zero;
        }

        int width, height;
        Instance.mFESAPI.Font.Print(fontIndex, new Rect2i(0, 0, 1, 1), 0, Instance.mSolidWhite, 0, text, true, out width, out height);
        return new Size2i(width, height);
    }

    /// <summary>
    /// Measure a text string without printing it, using custom font.
    /// </summary>
    /// <param name="fontIndex">Font index</param>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="alignFlags">Any combination of flags: <see cref="FES.ALIGN_H_LEFT"/>, <see cref="FES.ALIGN_H_RIGHT"/>,
    /// <see cref="FES.ALIGN_H_CENTER"/>, <see cref="FES.ALIGN_V_TOP"/>, <see cref="FES.ALIGN_V_BOTTOM"/>,
    /// <see cref="FES.ALIGN_V_CENTER"/>, <see cref="FES.TEXT_OVERFLOW_CLIP"/>, <see cref="FES.TEXT_OVERFLOW_WRAP"/>.</param>
    /// <param name="text">Text</param>
    /// <returns>Dimensions</returns>
    public static Size2i PrintMeasure(int fontIndex, Rect2i rect, int alignFlags, string text)
    {
        if (text == null)
        {
            return Size2i.zero;
        }

        int width, height;
        Instance.mFESAPI.Font.Print(fontIndex, rect, 0, Instance.mSolidWhite, alignFlags, text, true, out width, out height);
        return new Size2i(width, height);
    }

    /// <summary>
    /// Setup a custom font from the sprite sheet.
    /// </summary>
    /// <remarks>The glyphs in your sprite sheet should be organized into a grid, with each cell size being the same. If <paramref name="monospaced"/> is false then FES will automatically trim any
    /// horizontal empty space on either side of the glyph. If <paramref name="monospaced"/> is true then the empty space is retained.</remarks>
    /// <param name="fontIndex">Font index</param>
    /// <param name="asciiStart">First ascii character in the font</param>
    /// <param name="asciiEnd">Last ascii character in the font</param>
    /// <param name="srcPos">Top left corner of the set of glyphs in the sprite sheet</param>
    /// <param name="spriteSheetIndex">The index of the sprite sheet containing the font</param>
    /// <param name="glyphSize">Dimensions of a single glyph</param>
    /// <param name="glyphsPerRow">Amount of glyphs per row in the sprite sheet</param>
    /// <param name="characterSpacing">Spacing between characters</param>
    /// <param name="lineSpacing">Line spacing between lines of text</param>
    /// <param name="monospaced">True if font is monospaced</param>
    /// <param name="paletteSwapSupport">True if font should support palette swaps. Indexed mode only.</param>
    public static void FontSetup(int fontIndex, int asciiStart, int asciiEnd, Vector2i srcPos, int spriteSheetIndex, Size2i glyphSize, int glyphsPerRow, int characterSpacing, int lineSpacing, bool monospaced, bool paletteSwapSupport = false)
    {
        if (fontIndex < 0 || fontIndex >= FESInternal.FESHW.HW_FONTS)
        {
            Debug.Log("Invalid font index");
            return;
        }

        if (asciiStart < 0 || asciiEnd >= 256 || asciiEnd < asciiStart)
        {
            Debug.Log("Invalid ascii range");
            return;
        }

        if (glyphSize.width < 0 || glyphSize.height < 0)
        {
            Debug.Log("Invalid glyph size");
            return;
        }

        if (glyphsPerRow <= 0)
        {
            Debug.Log("Invalid glyphs per row");
            return;
        }

        if (spriteSheetIndex < 0 || spriteSheetIndex >= FESInternal.FESHW.HW_MAX_SPRITESHEETS)
        {
            Debug.Log("Invalid sprite sheet index");
            return;
        }

        var renderer = Instance.mFESAPI.Renderer;

        if (srcPos.x < 0 || srcPos.y < 0 ||
            srcPos.x + (glyphsPerRow * glyphSize.width) > renderer.SpriteSheets[spriteSheetIndex].texture.width ||
            srcPos.y + ((((asciiEnd - asciiStart) / glyphsPerRow) + 1) * glyphSize.height) > renderer.SpriteSheets[spriteSheetIndex].texture.height)
        {
            Debug.Log("Invalid font sprite sheet coordinates, out of bounds");
            return;
        }

        Instance.mFESAPI.Font.FontSetup(fontIndex, asciiStart, asciiEnd, srcPos, spriteSheetIndex, glyphSize, glyphsPerRow, characterSpacing, lineSpacing, monospaced, !paletteSwapSupport, true, false);
    }

    /// <summary>
    /// Return true if any of the given button(s) are held down.
    /// </summary>
    /// <param name="button">A bitmask of one or multiple buttons.</param>
    /// <param name="player">Player to check, or <see cref="FES.PLAYER_ANY"/> to check any player. Defaults to <see cref="FES.PLAYER_ONE"/></param>
    /// <returns>True if button(s) held down</returns>
    public static bool ButtonDown(int button, int player = FES.PLAYER_ONE)
    {
        return Instance.mFESAPI.Input.ButtonDown(button, player);
    }

    /// <summary>
    /// Return true if any of the given button(s) went from an up to down state since last <see cref="IFESGame.Update"/> call.
    /// </summary>
    /// <param name="button">A bitmask of one or multiple buttons.</param>
    /// <param name="player">Player to check, or <see cref="FES.PLAYER_ANY"/> to check any player. Defaults to <see cref="FES.PLAYER_ONE"/></param>
    /// <returns>True if button(s) pressed</returns>
    public static bool ButtonPressed(int button, int player = FES.PLAYER_ONE)
    {
        return Instance.mFESAPI.Input.ButtonPressed(button, player);
    }

    /// <summary>
    /// Return true if any of the given button(s) went from a down to up state since last <see cref="IFESGame.Update"/> call.
    /// </summary>
    /// <param name="button">A bitmask of one or multiple buttons.</param>
    /// <param name="player">Player to check, or <see cref="FES.PLAYER_ANY"/> to check any player. Defaults to <see cref="FES.PLAYER_ONE"/></param>
    /// <returns>True if button(s) released</returns>
    public static bool ButtonReleased(int button, int player = FES.PLAYER_ONE)
    {
        return Instance.mFESAPI.Input.ButtonReleased(button, player);
    }

    /// <summary>
    /// Return true if given key is held down.
    /// </summary>
    /// <param name="keyCode">Key code</param>
    /// <returns>True if key held down</returns>
    public static bool KeyDown(KeyCode keyCode)
    {
        return Instance.mFESAPI.Input.KeyDown(keyCode);
    }

    /// <summary>
    /// Return true if given key went from an up to down state since last <see cref="IFESGame.Update"/> call.
    /// </summary>
    /// <param name="keyCode">Key code</param>
    /// <returns>True if key pressed</returns>
    public static bool KeyPressed(KeyCode keyCode)
    {
        return Instance.mFESAPI.Input.KeyPressed(keyCode);
    }

    /// <summary>
    /// Return true if given key went from a down to up state since last <see cref="IFESGame.Update"/> call.
    /// </summary>
    /// <param name="keyCode">Key code</param>
    /// <returns>True if key released</returns>
    public static bool KeyReleased(KeyCode keyCode)
    {
        return Instance.mFESAPI.Input.KeyReleased(keyCode);
    }

    /// <summary>
    /// Returns of string of characters entered since last <see cref="IFESGame.Update"/> call. Normally this string
    /// will be 0 or 1 characters long, but it is possible that the user may quickly perform two key strokes within the
    /// spawn of a single game loop.
    /// </summary>
    /// <returns>Character string</returns>
    public static string InputString()
    {
        return Instance.mFESAPI.Input.InputString();
    }

    /// <summary>
    /// Get the current pointer position. If there is a mouse detected then the position is the position of the mouse pointer in
    /// game display coordinates. If there is a touch device then the position is the current touch position. If there is no
    /// mouse connected, and there are no active touches then the position is undefined. See <see cref="FES.PointerPosValid"/> for checking
    /// if the pointer position is valid.
    /// </summary>
    /// <returns>Pointer position</returns>
    public static Vector2i PointerPos()
    {
        return Instance.mFESAPI.Input.PointerPos();
    }

    /// <summary>
    /// Returns true if the pointer position is valid. A pointer position may be undefined if there is no mouse connected, and there are no
    /// current touches.
    /// </summary>
    /// <returns>True if pointer position is valid</returns>
    public static bool PointerPosValid()
    {
        return Instance.mFESAPI.Input.PointerPosValid();
    }

    /// <summary>
    /// Get the delta of the mouse scroll position since last <see cref="IFESGame.Update"/> call. The delta is always 0 if there
    /// is no mouse connected.
    /// </summary>
    /// <returns>Delta position</returns>
    public static float PointerScrollDelta()
    {
        return Instance.mFESAPI.Input.PointerScrollDelta();
    }

    /// <summary>
    /// Provide a delegate for overriding FES button mapping. The delegate implementation takes a button and returns true if the button
    /// is held down, or false otherwise. How the button state is determined is entirely up to the delegate implementation. Delegate should
    /// set the "handled" out parameter to true if it was able to determine the state of the button, or false if it wants to fallback on
    /// FES using its default mapping for this button.
    /// </summary>
    /// <param name="overrideMethod">Input override delegate</param>
    public static void InputOverride(InputOverrideMethod overrideMethod)
    {
        Instance.mFESAPI.Input.InputOverride(overrideMethod);
    }

    /// <summary>
    /// Setup sound clip at the given sound slot index.
    /// </summary>
    /// <param name="slotIndex">Sound slot index</param>
    /// <param name="fileName">Filename of the sound file, must be within a Resources folder</param>
    public static void SoundSetup(int slotIndex, string fileName)
    {
        Instance.mFESAPI.Audio.SoundSet(slotIndex, fileName);
    }

    /// <summary>
    /// Remove sound clip from the given slot index, freeing up resources.
    /// </summary>
    /// <param name="slotIndex">Sound slot index</param>
    public static void SoundDelete(int slotIndex)
    {
        Instance.mFESAPI.Audio.SoundSet(slotIndex, null);
    }

    /// <summary>
    /// Play the sound at the given sound slot index. This function returns the <see cref="SoundReference"/> of the playing
    /// sound, which can then be used to adjust the playback of the sound.
    /// </summary>
    /// <param name="slotIndex">Sound slot index</param>
    /// <param name="volume">Volume to play at where 0 is silent, 1 is the original clip volume and values beyond 1 indicate amplification beyond the recorded volume. Defaults to 1</param>
    /// <param name="pitch">Pitch to play at, where 1 is the original sound pitch, values less than 1 indicate lower pitch, and values greater than 1 indicate higher pitch. Defaults to 1</param>
    /// <returns>Sound reference</returns>
    public static SoundReference SoundPlay(int slotIndex, float volume = 1.0f, float pitch = 1.0f)
    {
        return Instance.mFESAPI.Audio.SoundPlay(slotIndex, volume, pitch);
    }

    /// <summary>
    /// Check if a sound is playing by using its <see cref="SoundReference"/>.
    /// If a sound is no longer playing it's reference is considered no longer valid and can be disposed of.
    /// </summary>
    /// <param name="soundReference">Sound reference</param>
    /// <returns>True if still playing, false otherwise.</returns>
    public static bool SoundIsPlaying(SoundReference soundReference)
    {
        return Instance.mFESAPI.Audio.SoundIsPlaying(soundReference);
    }

    /// <summary>
    /// Set the volume of the currently playing sound by using its <see cref="SoundReference"/>
    /// </summary>
    /// <param name="soundReference">Sound reference</param>
    /// <param name="volume">New volume to play at where 0 is silent, 1 is the original clip volume and values beyond 1 indicate amplification beyond the recorded volume</param>
    public static void SoundVolumeSet(SoundReference soundReference, float volume)
    {
        Instance.mFESAPI.Audio.SoundVolumeSet(soundReference, volume);
    }

    /// <summary>
    /// Set the pitch of the currently playing sound by using its <see cref="SoundReference"/>
    /// </summary>
    /// <param name="soundReference">Sound reference</param>
    /// <param name="pitch">New pitch to play at, where 1 is the original sound pitch, values less than 1 indicate lower pitch, and values greater than 1 indicate higher pitch</param>
    public static void SoundPitchSet(SoundReference soundReference, float pitch)
    {
        Instance.mFESAPI.Audio.SoundPitchSet(soundReference, pitch);
    }

    /// <summary>
    /// Get the volume of the currently playing sound by using its <see cref="SoundReference"/>
    /// </summary>
    /// <param name="soundReference">Sound reference</param>
    /// <returns>Volume</returns>
    public static float SoundVolumeGet(SoundReference soundReference)
    {
        return Instance.mFESAPI.Audio.SoundVolumeGet(soundReference);
    }

    /// <summary>
    /// Get the pitch of the currently playing sound by using its <see cref="SoundReference"/>
    /// </summary>
    /// <param name="soundReference">Sound reference</param>
    /// <returns>Pitch</returns>
    public static float SoundPitchGet(SoundReference soundReference)
    {
        return Instance.mFESAPI.Audio.SoundPitchGet(soundReference);
    }

    /// <summary>
    /// Stop the sound currently playing by using its <see cref="SoundReference"/>. Once the sound is stopped it can no longer be resumed and the specified <see cref="FES.SoundReference"/> is
    /// no longer valid.
    /// </summary>
    /// <param name="soundReference">Sound reference</param>
    public static void SoundStop(SoundReference soundReference)
    {
        Instance.mFESAPI.Audio.SoundStop(soundReference);
    }

    /// <summary>
    /// Specify whether the currently playing sound specified by <see cref="SoundReference"/> should keep looping.
    /// </summary>
    /// <param name="soundReference">Sound reference</param>
    /// <param name="loop">True if sound should loop</param>
    public static void SoundLoopSet(SoundReference soundReference, bool loop)
    {
        Instance.mFESAPI.Audio.SoundLoopSet(soundReference, loop);
    }

    /// <summary>
    /// Setup music at the given music slot index.
    /// </summary>
    /// <param name="slotIndex">Music slot index</param>
    /// <param name="fileName">Filename of the music file, must be within a Resources folder</param>
    public static void MusicSetup(int slotIndex, string fileName)
    {
        Instance.mFESAPI.Audio.MusicSet(slotIndex, fileName);
    }

    /// <summary>
    /// Remove music clip from the given slot index, freeing up resources.
    /// </summary>
    /// <param name="slotIndex">Music slot index</param>
    public static void MusicDelete(int slotIndex)
    {
        Instance.mFESAPI.Audio.MusicSet(slotIndex, null);
    }

    /// <summary>
    /// Play music stored at the given slot index. If there already is music playing then it is cross-faded to the newly specified music.
    /// </summary>
    /// <param name="slotIndex">Music slot index</param>
    public static void MusicPlay(int slotIndex)
    {
        Instance.mFESAPI.Audio.MusicPlay(slotIndex);
    }

    /// <summary>
    /// Stop currently playing music.
    /// </summary>
    public static void MusicStop()
    {
        Instance.mFESAPI.Audio.MusicStop();
    }

    /// <summary>
    /// Set the volume of music.
    /// </summary>
    /// <param name="volume">New volume to play at where 0 is silent, 1 is the original music volume and values beyond 1 indicate amplification beyond the recorded volume</param>
    public static void MusicVolumeSet(float volume = 1.0f)
    {
        Instance.mFESAPI.Audio.MusicVolumeSet(volume);
    }

    /// <summary>
    /// Set the pitch of music.
    /// </summary>
    /// <param name="pitch">New pitch to play at, where 1 is the original music pitch, values less than 1 indicate lower pitch, and values greater than 1 indicate higher pitch</param>
    public static void MusicPitchSet(float pitch = 1.0f)
    {
        Instance.mFESAPI.Audio.MusicPitchSet(pitch);
    }

    /// <summary>
    /// Get the current music volume.
    /// </summary>
    /// <returns>Volume</returns>
    public static float MusicVolumeGet()
    {
        return Instance.mFESAPI.Audio.MusicVolumeGet();
    }

    /// <summary>
    /// Get the current music pitch.
    /// </summary>
    /// <returns>Pitch</returns>
    public static float MusicPitchGet()
    {
        return Instance.mFESAPI.Audio.MusicPitchGet();
    }

    /// <summary>
    /// Set values for the given post processing effect. Setting <paramref name="intensity"/> to 0 turns off the effect entirely.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="type">Effect type</param>
    /// <param name="intensity">Intensity</param>
    /// <param name="parameters">Additional parameters</param>
    /// <param name="colorIndex">Color</param>
    public static void EffectSet(Effect type, float intensity, Vector2i parameters, int colorIndex)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("EffectSet for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        Instance.mFESAPI.Effects.EffectSet(type, intensity, parameters, colorIndex, new ColorRGBA(0, 0, 0));
    }

    /// <summary>
    /// Set values for the given post processing effect. Setting <paramref name="intensity"/> to 0 turns off the effect entirely.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="type">Effect type</param>
    /// <param name="intensity">Intensity</param>
    /// <param name="parameters">Additional parameters</param>
    /// <param name="color">Color</param>
    public static void EffectSet(Effect type, float intensity, Vector2i parameters, ColorRGBA color)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("EffectSet for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Effects.EffectSet(type, intensity, parameters, 0, color);
    }

    /// <summary>
    /// Set intensity for the given post processing effect. Setting <paramref name="intensity"/> to 0 turns off the effect entirely.
    /// </summary>
    /// <param name="type">Effect type</param>
    /// <param name="intensity">Intensity</param>
    public static void EffectSet(Effect type, float intensity)
    {
        Instance.mFESAPI.Effects.EffectSet(type, intensity, Vector2i.zero, 0, new ColorRGBA(0, 0, 0));
    }

    /// <summary>
    /// Set parameters for the given post processing effect. The effect is not yet active if its intensity is 0.
    /// </summary>
    /// <param name="type">Effect type</param>
    /// <param name="parameters">Additional parameters</param>
    public static void EffectSet(Effect type, Vector2i parameters)
    {
        Instance.mFESAPI.Effects.EffectSet(type, 0, parameters, 0, new ColorRGBA(0, 0, 0));
    }

    /// <summary>
    /// Set color for the given post processing effect. The effect is not yet active if its intensity is 0.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.Indexed"/> mode only.</remarks>
    /// <param name="type">Effect type</param>
    /// <param name="colorIndex">Color</param>
    public static void EffectSet(Effect type, int colorIndex)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("EffectSet for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        Instance.mFESAPI.Effects.EffectSet(type, 0, Vector2i.zero, colorIndex, new ColorRGBA(0, 0, 0));
    }

    /// <summary>
    /// Set color for the given post processing effect. The effect is not yet active if its intensity is 0.
    /// </summary>
    /// <remarks>For <see cref="FES.ColorMode.RGB"/> mode only.</remarks>
    /// <param name="type">Effect type</param>
    /// <param name="color">Color</param>
    public static void EffectSet(Effect type, ColorRGBA color)
    {
        if (Instance.mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("EffectSet for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Effects.EffectSet(type, 0, Vector2i.zero, 0, color);
    }

    /// <summary>
    /// Set a custom post-processing effect shader. Note that some FES built-in shaders may not work if
    /// a custom shader is specified. For loading shaders see <see cref="ShaderSetup"/>
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    public static void EffectShader(int shaderIndex)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        Instance.mFESAPI.Effects.EffectShaderSet(shaderIndex);
    }

    /// <summary>
    /// Specify texture filtering to use with custom post-processing effect shader, see <see cref="EffectShader"/>.
    /// Default filter is <see cref="FES.Filter.Nearest"/>.
    /// </summary>
    /// <param name="filter"><see cref="FES.Filter.Nearest"/> or <see cref="FES.Filter.Linear"/></param>
    public static void EffectFilter(Filter filter)
    {
        FilterMode filterMode = filter == Filter.Nearest ? FilterMode.Point : FilterMode.Bilinear;
        Instance.mFESAPI.Effects.EffectFilterSet(filterMode);
    }

    /// <summary>
    /// Reset all post-processing effect back to default/off state.
    /// </summary>
    public static void EffectReset()
    {
        Instance.mFESAPI.Effects.EffectReset();
    }

    /// <summary>
    /// Apply post-processing effects at this point. This is useful if you don't want to apply effects to all layers, for
    /// example you may want your GUI to not be affected by post processing effects. After <see cref="EffectApplyNow"/> you
    /// will likely want to change your effects, or call <see cref="EffectReset"/>. <see cref="EffectApplyNow"/> is called
    /// automatically at the end of <see cref="IFESGame.Render"/>.
    /// </summary>
    public static void EffectApplyNow()
    {
        Instance.mFESAPI.Renderer.EffectApplyNow();
    }

    /// <summary>
    /// Load a shader into the given shader slot, from the given filename.
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="filename">Shader filename, must be within a Resources folder</param>
    public static void ShaderSetup(int shaderIndex, string filename)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ShaderSetup(shaderIndex, filename);
    }

    /// <summary>
    /// Set the current shader
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    public static void ShaderSet(int shaderIndex)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ShaderSet(shaderIndex);
    }

    /// <summary>
    /// Apply shader now, this casues an implicit flush of the rendering pipeline. You may want to call <see cref="ShaderApplyNow"/> if
    /// you want the current shader parameters to apply to earlier Draw calls before you make further changes to the shader parameters.
    /// </summary>
    public static void ShaderApplyNow()
    {
        Instance.mFESAPI.Renderer.ShaderApplyNow();
    }

    /// <summary>
    /// Reset the shader back to the default FES shader.
    /// </summary>
    public static void ShaderReset()
    {
        Instance.mFESAPI.Renderer.ShaderReset();
    }

    /// <summary>
    /// Get a unique ID of a shader property as specified in a shader file. It is more efficient to pass around a shader ID then it's string name.
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <returns>Unique ID</returns>
    public static int ShaderPropertyID(string name)
    {
        return Shader.PropertyToID(name);
    }

    /// <summary>
    /// Set a shader color property
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="color">Color</param>
    public static void ShaderColorSet(int shaderIndex, string name, ColorRGBA color)
    {
        ShaderColorSet(shaderIndex, Shader.PropertyToID(name), color);
    }

    /// <summary>
    /// Set a shader color property
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="color">Color</param>
    public static void ShaderColorSet(int shaderIndex, int propertyID, ColorRGBA color)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.Indexed)
        {
            FESInternal.FESUtil.LogErrorOnce("ShaderSetColor for RGB mode called when Hardware is in Indexed mode!");
            return;
        }

        Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetColor(propertyID, color.ToColor32());
    }

    /// <summary>
    /// Set a shader color property
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="colorIndex">Color index</param>
    public static void ShaderColorSet(int shaderIndex, string name, int colorIndex)
    {
        ShaderColorSet(shaderIndex, Shader.PropertyToID(name), colorIndex);
    }

    /// <summary>
    /// Set a shader color property
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="colorIndex">Color index</param>
    public static void ShaderColorSet(int shaderIndex, int propertyID, int colorIndex)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("ShaderSetColor for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorIndex < 0 || colorIndex >= hw.PaletteColorCount)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetColor(propertyID, Instance.mFESAPI.Palette.ColorAtIndex(colorIndex));
    }

    /// <summary>
    /// Set a shader color array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="colorArray">Color list</param>
    public static void ShaderColorArraySet(int shaderIndex, string name, List<ColorRGBA> colorArray)
    {
        ShaderColorArraySet(shaderIndex, Shader.PropertyToID(name), colorArray.ToArray());
    }

    /// <summary>
    /// Set a shader color array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="colorArray">Color list</param>
    public static void ShaderColorArraySet(int shaderIndex, int propertyID, List<ColorRGBA> colorArray)
    {
        ShaderColorArraySet(shaderIndex, propertyID, colorArray.ToArray());
    }

    /// <summary>
    /// Set a shader color array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="colorArray">Color array</param>
    public static void ShaderColorArraySet(int shaderIndex, string name, ColorRGBA[] colorArray)
    {
        ShaderColorArraySet(shaderIndex, Shader.PropertyToID(name), colorArray);
    }

    /// <summary>
    /// Set a shader color array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="colorArray">Color array</param>
    public static void ShaderColorArraySet(int shaderIndex, int propertyID, ColorRGBA[] colorArray)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        if (colorArray != null)
        {
            Color[] colorArray32 = new Color[colorArray.Length];
            for (int i = 0; i < colorArray.Length; i++)
            {
                colorArray32[i] = colorArray[i].ToColor32();
            }

            Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetColorArray(propertyID, colorArray32);
        }
        else
        {
            Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetColorArray(propertyID, new List<Color>());
        }
    }

    /// <summary>
    /// Set a shader color array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="colorArray">Color index list</param>
    public static void ShaderColorArraySet(int shaderIndex, string name, List<int> colorArray)
    {
        ShaderColorArraySet(shaderIndex, Shader.PropertyToID(name), colorArray.ToArray());
    }

    /// <summary>
    /// Set a shader color array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="colorArray">Color index list</param>
    public static void ShaderColorArraySet(int shaderIndex, int propertyID, List<int> colorArray)
    {
        ShaderColorArraySet(shaderIndex, propertyID, colorArray.ToArray());
    }

    /// <summary>
    /// Set a shader color array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="colorArray">Color index array</param>
    public static void ShaderColorArraySet(int shaderIndex, string name, int[] colorArray)
    {
        ShaderColorArraySet(shaderIndex, Shader.PropertyToID(name), colorArray);
    }

    /// <summary>
    /// Set a shader color array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="colorArray">Color index array</param>
    public static void ShaderColorArraySet(int shaderIndex, int propertyID, int[] colorArray)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        var hw = Instance.mFESAPI.HW;

        if (hw.ColorMode == FES.ColorMode.RGB)
        {
            FESInternal.FESUtil.LogErrorOnce("ShaderSetColor for Indexed mode called when Hardware is in RGB mode!");
            return;
        }

        if (colorArray != null)
        {
            Color[] colorArray32 = new Color[colorArray.Length];
            for (int i = 0; i < colorArray.Length; i++)
            {
                colorArray32[i] = Instance.mFESAPI.Palette.ColorAtIndex(colorArray[i]);
            }

            Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetColorArray(propertyID, colorArray32);
        }
        else
        {
            Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetColorArray(propertyID, new List<Color>());
        }
    }

    /// <summary>
    /// Set a shader float property
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="value">Float value</param>
    public static void ShaderFloatSet(int shaderIndex, string name, float value)
    {
        ShaderFloatSet(shaderIndex, Shader.PropertyToID(name), value);
    }

    /// <summary>
    /// Set a shader float property
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="value">Float value</param>
    public static void ShaderFloatSet(int shaderIndex, int propertyID, float value)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetFloat(propertyID, value);
    }

    /// <summary>
    /// Set a shader float array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="values">Float array</param>
    public static void ShaderFloatArraySet(int shaderIndex, string name, float[] values)
    {
        ShaderFloatArraySet(shaderIndex, Shader.PropertyToID(name), values);
    }

    /// <summary>
    /// Set a shader float array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="values">Float array</param>
    public static void ShaderFloatArraySet(int shaderIndex, int propertyID, float[] values)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetFloatArray(propertyID, values);
    }

    /// <summary>
    /// Set a shader float array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="values">Float list</param>
    public static void ShaderFloatArraySet(int shaderIndex, string name, List<float> values)
    {
        ShaderFloatArraySet(shaderIndex, Shader.PropertyToID(name), values);
    }

    /// <summary>
    /// Set a shader float array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property name</param>
    /// <param name="values">Float list</param>
    public static void ShaderFloatArraySet(int shaderIndex, int propertyID, List<float> values)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetFloatArray(propertyID, values);
    }

    /// <summary>
    /// Set a shader integer property
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="value">Integer value</param>
    public static void ShaderIntSet(int shaderIndex, string name, int value)
    {
        ShaderIntSet(shaderIndex, Shader.PropertyToID(name), value);
    }

    /// <summary>
    /// Set a shader integer property
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="value">Integer value</param>
    public static void ShaderIntSet(int shaderIndex, int propertyID, int value)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetInt(propertyID, value);
    }

    /// <summary>
    /// Set a shader matrix
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="matrix">Matrix</param>
    public static void ShaderMatrixSet(int shaderIndex, string name, Matrix4x4 matrix)
    {
        ShaderMatrixSet(shaderIndex, Shader.PropertyToID(name), matrix);
    }

    /// <summary>
    /// Set a shader matrix
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="matrix">Matrix</param>
    public static void ShaderMatrixSet(int shaderIndex, int propertyID, Matrix4x4 matrix)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetMatrix(propertyID, matrix);
    }

    /// <summary>
    /// Set a shader matrix array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="matrices">Matrix array</param>
    public static void ShaderMatrixArraySet(int shaderIndex, string name, Matrix4x4[] matrices)
    {
        ShaderMatrixArraySet(shaderIndex, Shader.PropertyToID(name), matrices);
    }

    /// <summary>
    /// Set a shader matrix array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="matrices">Matrix array</param>
    public static void ShaderMatrixArraySet(int shaderIndex, int propertyID, Matrix4x4[] matrices)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetMatrixArray(propertyID, matrices);
    }

    /// <summary>
    /// Set a shader matrix array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="matrices">Matrix list</param>
    public static void ShaderMatrixArraySet(int shaderIndex, string name, List<Matrix4x4> matrices)
    {
        ShaderMatrixArraySet(shaderIndex, Shader.PropertyToID(name), matrices);
    }

    /// <summary>
    /// Set a shader matrix array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="matrices">Matrix list</param>
    public static void ShaderMatrixArraySet(int shaderIndex, int propertyID, List<Matrix4x4> matrices)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetMatrixArray(propertyID, matrices);
    }

    /// <summary>
    /// Set a shader vector property
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="vector">Vector</param>
    public static void ShaderVectorSet(int shaderIndex, string name, Vector4 vector)
    {
        ShaderVectorSet(shaderIndex, Shader.PropertyToID(name), vector);
    }

    /// <summary>
    /// Set a shader vector property
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="vector">Vector</param>
    public static void ShaderVectorSet(int shaderIndex, int propertyID, Vector4 vector)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetVector(propertyID, vector);
    }

    /// <summary>
    /// Set a shader vector array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="vectors">Vector array</param>
    public static void ShaderVectorArraySet(int shaderIndex, string name, Vector4[] vectors)
    {
        ShaderVectorArraySet(shaderIndex, Shader.PropertyToID(name), vectors);
    }

    /// <summary>
    /// Set a shader vector array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="vectors">Vector array</param>
    public static void ShaderVectorArraySet(int shaderIndex, int propertyID, Vector4[] vectors)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetVectorArray(propertyID, vectors);
    }

    /// <summary>
    /// Set a shader vector array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="vectors">Vector list</param>
    public static void ShaderVectorArraySet(int shaderIndex, string name, List<Vector4> vectors)
    {
        ShaderVectorArraySet(shaderIndex, Shader.PropertyToID(name), vectors);
    }

    /// <summary>
    /// Set a shader vector array
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="vectors">Vector list</param>
    public static void ShaderVectorArraySet(int shaderIndex, int propertyID, List<Vector4> vectors)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetVectorArray(propertyID, vectors);
    }

    /// <summary>
    /// Set a shader offscreen texture
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="offscreenIndex">Offscreen index</param>
    public static void ShaderOffscreenTextureSet(int shaderIndex, string name, int offscreenIndex)
    {
        ShaderOffscreenTextureSet(shaderIndex, Shader.PropertyToID(name), offscreenIndex);
    }

    /// <summary>
    /// Set a shader offscreen texture
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="propertyID">Property ID</param>
    /// <param name="offscreenIndex">Offscreen index</param>
    public static void ShaderOffscreenTextureSet(int shaderIndex, int propertyID, int offscreenIndex)
    {
        if (shaderIndex < 0 || shaderIndex >= FESInternal.FESHW.HW_MAX_SHADERS)
        {
            return;
        }

        var renderer = Instance.mFESAPI.Renderer;
        var tex = renderer.OffscreenGetTexture(offscreenIndex);

        renderer.ShaderParameters(shaderIndex).SetTexture(propertyID, tex);
    }

    /// <summary>
    /// Set an offscreen texture filter to be used by the shader. The default filter is <see cref="Filter.Nearest"/>.
    /// </summary>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="offscreenIndex">Offscreen index</param>
    /// <param name="filter"><see cref="Filter.Nearest"/> or <see cref="Filter.Linear"/></param>
    public static void ShaderOffscreenFilterSet(int shaderIndex, int offscreenIndex, Filter filter)
    {
        FilterMode filterMode = filter == Filter.Nearest ? FilterMode.Point : FilterMode.Bilinear;
        Instance.mFESAPI.Renderer.ShaderParameters(shaderIndex).SetOffscreenFilter(offscreenIndex, filterMode);
    }

    /// <summary>
    /// A reference to currently playing sound. This structure is returned by <see cref="FES.SoundPlay"/> and can be used with
    /// <see cref="FES.SoundStop"/>, <see cref="FES.SoundVolumeSet"/>, <see cref="FES.SoundVolumeGet"/>, <see cref="FES.SoundPitchSet"/>
    /// and <see cref="FES.SoundPitchGet"/> to manage the sound as it plays.
    /// </summary>
    public struct SoundReference
    {
        /// <summary>
        /// Sound channel. Do not modify.
        /// </summary>
        public int SoundChannel;

        /// <summary>
        /// Sound sequence. Do not modify
        /// </summary>
        public long Sequence;

        /// <summary>
        /// Sound reference constructor. Used internally by FES.
        /// </summary>
        /// <param name="channel">Sound channel</param>
        /// <param name="seq">Sound sequence</param>
        public SoundReference(int channel, long seq)
        {
            SoundChannel = channel;
            Sequence = seq;
        }
    }

    /// <summary>
    /// Defines hardware settings for the FES game. A FES Game fills out this structure in response to the call to <see cref="IFESGame.QueryHardware"/>.
    /// </summary>
    public class HardwareSettings
    {
        /// <summary>
        /// Display size, dimensions must be divisible by 2
        /// </summary>
        public Size2i DisplaySize = new Size2i(480, 270);

        /// <summary>
        /// Maximum tilemap size, in tiles. This number should be kept close to the maximum size required by your game, in order to conserve memory.
        /// </summary>
        public Size2i MapSize = new Size2i(256, 256);

        /// <summary>
        /// Maximum amount of tilemap layers. This number should be kept close to the maximum layer count required by your game, in order to conserve memory.
        /// </summary>
        public int MapLayers = 8;

        /// <summary>
        /// Color mode. This can be either <see cref="ColorMode.Indexed"/> for an indexed mode with a strictly limited palette of colors, or <see cref="ColorMode.RGB"/> for any RGB color.
        /// </summary>
        public ColorMode ColorMode = ColorMode.RGB;

        /// <summary>
        /// File name of the color palette. Ignored if <see cref="HardwareSettings.ColorMode"/> is <see cref="ColorMode.RGB"/>. Each pixel in the color palette is considered a color palette entry.
        /// The pixels are read from top-left to bottom-right. Maximum of 250 colors are supported in a palette, and so the color palette may not contain more than 250 pixels. The size of the
        /// color palette cannot be changed after the call to <see cref="IFESGame.QueryHardware"/>. The palette file must be located somewhere under a Resources folder.
        /// </summary>
        public string Palette = null;

        /// <summary>
        /// Target frames per second. Defaults to 60.
        /// </summary>
        public int FPS = 60;

        /// <summary>
        /// Display pixel style
        /// </summary>
        public PixelStyle PixelStyle = PixelStyle.Square;
    }
}
