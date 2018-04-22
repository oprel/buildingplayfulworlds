namespace FESDemo
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Demo Reel demo program. Shows off a majority of the FES features. Note that this demo can be ran
    /// in either indexed color mode, or rgb color mode.
    /// Note that this code is not pretty, its runnable in both RGB color mode and Indexed color mode, which
    /// makes for a lot of awkwards while handling both.
    /// </summary>
    public class DemoReel : FES.IFESGame
    {
        private int mCurrentScene = 0;
        private List<SceneDemo> mScenes = new List<SceneDemo>();
        private FES.ColorMode mColorMode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mode">Which color mode to run in</param>
        public DemoReel(FES.ColorMode mode)
        {
            mColorMode = mode;
        }

        /// <summary>
        /// Get the color mode that the demo is running in
        /// </summary>
        public FES.ColorMode ColorMode
        {
            get { return mColorMode; }
        }

        /// <summary>
        /// Query hardware
        /// </summary>
        /// <returns>Hardware configuration</returns>
        public FES.HardwareSettings QueryHardware()
        {
            var hw = new FES.HardwareSettings();

            hw.DisplaySize = new Size2i(640, 360);
            hw.ColorMode = mColorMode;

            if (mColorMode == FES.ColorMode.Indexed)
            {
                hw.Palette = "Palettes/Palette_FES32";
            }

            return hw;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful.</returns>
        public bool Initialize()
        {
            FES.SpriteSheetSetup(0, "Demos/DemoReel/Sprites", new Size2i(16, 16));
            FES.SpriteSheetSetup(1, "Demos/DemoReel/Logo", new Size2i(176, 84));
            FES.SpriteSheetSet(0);

            mScenes.Add(new SceneGameLoop());
            mScenes.Add(new SceneDrawing());
            mScenes.Add(new SceneText());
            mScenes.Add(new SceneClipOffscreen());
            mScenes.Add(new SceneTilemap());
            mScenes.Add(new SceneShader());
            mScenes.Add(new ScenePixelStyle(FES.PixelStyle.Wide));
            mScenes.Add(new ScenePixelStyle(FES.PixelStyle.Tall));
            mScenes.Add(new SceneSound());
            mScenes.Add(new SceneInput());
            mScenes.Add(new SceneEffects(FES.Effect.Scanlines));
            mScenes.Add(new SceneEffects(FES.Effect.Noise));
            mScenes.Add(new SceneEffects(FES.Effect.Desaturation));
            mScenes.Add(new SceneEffects(FES.Effect.Curvature));
            mScenes.Add(new SceneEffects(FES.Effect.Slide));
            mScenes.Add(new SceneEffects(FES.Effect.Wipe));
            mScenes.Add(new SceneEffects(FES.Effect.Shake));
            mScenes.Add(new SceneEffects(FES.Effect.Zoom));
            mScenes.Add(new SceneEffects(FES.Effect.Rotation));
            mScenes.Add(new SceneEffects(FES.Effect.ColorFade));
            mScenes.Add(new SceneEffects(FES.Effect.ColorTint));
            mScenes.Add(new SceneEffects(FES.Effect.Negative));
            mScenes.Add(new SceneEffects(FES.Effect.Pixelate));
            mScenes.Add(new SceneEffects(FES.Effect.Pinhole));
            mScenes.Add(new SceneEffects(FES.Effect.InvertedPinhole));
            mScenes.Add(new SceneEffects(FES.Effect.Fizzle));
            mScenes.Add(new SceneEffectShader());
            mScenes.Add(new SceneEffectApply());

            mCurrentScene = 0;
            mScenes[mCurrentScene].Enter();

            return true;
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            if (FES.ButtonPressed(FES.BTN_SYSTEM))
            {
                Application.Quit();
            }

            mScenes[mCurrentScene].Update();
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            mScenes[mCurrentScene].Render();
        }

        /// <summary>
        /// Switch to next scene, wrap around at the end
        /// </summary>
        public void NextScene()
        {
            int newScene = mCurrentScene + 1;
            if (newScene >= mScenes.Count)
            {
                newScene = 0;
            }

            mScenes[mCurrentScene].Exit();
            mCurrentScene = newScene;
            mScenes[mCurrentScene].Enter();
        }
    }
}
