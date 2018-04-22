using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Your game! You can of course rename this class to whatever you'd like.
/// </summary>
public class BrickBustGame : FES.IFESGame
{
    private GameState mState = GameState.NONE;
    private GameState mPendingState = GameState.MAIN_MENU;
    private float mStateChange = 0.0f;

    private MainMenu mMainMenu;
    private GameLevel mLevel;
    private float mShake;

    /// <summary>
    /// Game state
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// No defined state
        /// </summary>
        NONE,

        /// <summary>
        /// At main menu
        /// </summary>
        MAIN_MENU,

        /// <summary>
        /// In game level
        /// </summary>
        LEVEL
    }

    /// <summary>
    /// Current level
    /// </summary>
    public GameLevel Level
    {
        get { return mLevel; }
    }

    /// <summary>
    /// Query hardware. Here you initialize your retro game hardware.
    /// </summary>
    /// <returns>Hardware settings</returns>
    public FES.HardwareSettings QueryHardware()
    {
        var hw = new FES.HardwareSettings();

        int height = 1920;
        if (!UnityEngine.Application.isMobilePlatform)
        {
            height = 1800;
        }

        // Set your display size
        hw.DisplaySize = new Size2i(1080 / 5, height / 5); // 216 x 360 (216 x 384 mobile)

        // Set your preferred color mode, either RGB for a full color mode, or Indexed for an indexed/palettized color mode
        hw.ColorMode = FES.ColorMode.Indexed;

        // If your color mode is Indexed you can set the palette file here, recommended format is PNG. Each pixel in the image
        // represents one color in your palette. Color index 0 is the top left pixel, and the last color index is the bottom
        // right pixel. 250 colors maximum.
        // By default a built-in 32 color FES palette will be used.
        hw.Palette = "Demos/BrickBust/Palette";

        // Set tilemap maximum size, default is 256, 256. Keep this close to your minimum required size to save on memory
        //// hw.MapSize = new Size2i(256, 256);

        // Set tilemap maximum layers, default is 8. Keep this close to your minimum required size to save on memory
        //// hw.MapLayers = 8;

        return hw;
    }

    /// <summary>
    /// Initialize your game here.
    /// </summary>
    /// <returns>Return true if successful</returns>
    public bool Initialize()
    {
        // You can load a spritesheet here
        FES.SpriteSheetSetup(0, "Demos/BrickBust/Sprites", new Size2i(10, 10));
        FES.SpriteSheetSet(0);

        mMainMenu = new MainMenu();

        FES.EffectSet(FES.Effect.Scanlines, 0.2f);
        FES.EffectSet(FES.Effect.Noise, 0.01f);

        FES.PaletteSwapSetup(C.SWAP_BLUE_BRICK, new int[] { 0, 1, 2, 3, 4, 13, 17, 18, 19 });
        FES.PaletteSwapSetup(C.SWAP_GREEN_BRICK, new int[] { 0, 1, 2, 3, 4, 13, 14, 15, 16 });
        FES.PaletteSwapSetup(C.SWAP_BROWN_BRICK, new int[] { 0, 1, 2, 3, 4, 9, 10, 11, 12 });
        FES.PaletteSwapSetup(C.SWAP_BLACK_BRICK, new int[] { 0, 1, 2, 3, 4, 0, 1, 2, 3 });
        FES.PaletteSwapSetup(C.SWAP_PINK_BRICK, new int[] { 0, 1, 2, 3, 4, 20, 21, 22, 23 });

        FES.PaletteSwapSetup(C.SWAP_SHADOW, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        FES.PaletteSwapSetup(C.SWAP_WHITEOUT, new int[] { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 });

        FES.SoundSetup(C.SOUND_HIT_BRICK, "Demos/BrickBust/Sounds/hit");
        FES.SoundSetup(C.SOUND_HIT_WALL, "Demos/BrickBust/Sounds/hit2");
        FES.SoundSetup(C.SOUND_EXPLODE, "Demos/BrickBust/Sounds/explode");
        FES.SoundSetup(C.SOUND_DEATH, "Demos/BrickBust/Sounds/death");
        FES.SoundSetup(C.SOUND_START, "Demos/BrickBust/Sounds/start");
        FES.SoundSetup(C.SOUND_POWERUP, "Demos/BrickBust/Sounds/powerup");
        FES.SoundSetup(C.SOUND_LASERSHOT, "Demos/BrickBust/Sounds/lasershot");
        FES.SoundSetup(C.SOUND_LASERHIT, "Demos/BrickBust/Sounds/laserhit");

        FES.MusicSetup(C.MENU_MUSIC, "Demos/BrickBust/Sounds/music1");
        FES.MusicSetup(C.LEVEL_MUSIC, "Demos/BrickBust/Sounds/music2");

        FES.MusicVolumeSet(0.4f);

        LevelDef.Initialize();

        ChangeState(GameState.MAIN_MENU);

        if (UnityEngine.Application.isMobilePlatform)
        {
            C.ACTION_VERB = "TAP";
        }

        return true;
    }

    /// <summary>
    /// Update, your game logic should live here. Update is called at a fixed interval of 60 times per second.
    /// </summary>
    public void Update()
    {
        if (FES.ButtonPressed(FES.BTN_SYSTEM))
        {
            Application.Quit();
        }

        if (mState == GameState.MAIN_MENU)
        {
            mMainMenu.Update();
        }
        else if (mState == GameState.LEVEL)
        {
            mLevel.Update();
        }

        FES.EffectSet(FES.Effect.Shake, mShake);
        mShake *= 0.85f;

        AnimateStateChange();
    }

    /// <summary>
    /// Render, your drawing code should go here.
    /// </summary>
    public void Render()
    {
        FES.Clear(0);

        if (mState == GameState.MAIN_MENU)
        {
            mMainMenu.Render();
        }
        else if (mState == GameState.LEVEL)
        {
            mLevel.Render();
        }
    }

    /// <summary>
    /// Shake screen by given amount
    /// </summary>
    /// <param name="amount">How much to shake</param>
    public void Shake(float amount)
    {
        mShake = Math.Min(amount, 0.4f);
    }

    /// <summary>
    /// Change game state
    /// </summary>
    /// <param name="state">New state</param>
    public void ChangeState(GameState state)
    {
        if (mStateChange > 0.0f && mStateChange < 1.0f)
        {
            return;
        }

        mPendingState = state;

        if (mState != GameState.NONE)
        {
            mStateChange = 1.0f;
            FES.SoundPlay(C.SOUND_START);
        }
        else
        {
            mStateChange = 0.0f;
        }

        if (state != mState)
        {
            if (state == GameState.MAIN_MENU)
            {
                FES.MusicPlay(C.MENU_MUSIC);
            }
            else if (state == GameState.LEVEL)
            {
                FES.MusicPlay(C.LEVEL_MUSIC);
            }
        }
    }

    /// <summary>
    /// Animate game state change, allowing for nice transitions between screens
    /// </summary>
    public void AnimateStateChange()
    {
        if (mPendingState != GameState.NONE)
        {
            mStateChange -= 0.025f;
            if (mStateChange < 0.0f)
            {
                mState = mPendingState;
                mStateChange = 0.0f;

                if (mState == GameState.LEVEL)
                {
                    int levelIndex = 0;
                    if (mLevel != null)
                    {
                        levelIndex = mLevel.Index + 1;
                    }

                    mLevel = new GameLevel(levelIndex);
                }

                if (mState != GameState.LEVEL)
                {
                    mLevel = null;
                }

                mPendingState = GameState.NONE;
            }

            FES.EffectSet(FES.Effect.Slide, new Vector2i(Mathf.Sin((1.0f - mStateChange) * Mathf.PI / 2) * (FES.DisplaySize.width + 16), 0));
        }
        else
        {
            mStateChange += 0.025f;
            if (mStateChange > 1)
            {
                mStateChange = 1;
            }

            FES.EffectSet(FES.Effect.Slide, new Vector2i(Mathf.Sin((1.0f - mStateChange) * Mathf.PI / 2) * (FES.DisplaySize.width + 16), 0));
        }
    }
}
