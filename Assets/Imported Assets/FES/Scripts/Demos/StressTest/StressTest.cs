namespace FESDemo
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Stress Test. Tests blitting performance of FES
    /// </summary>
    public class StressTest : FES.IFESGame
    {
        private int mCurrentScene = 0;
        private List<SceneDemo> mScenes = new List<SceneDemo>();

        /// <summary>
        /// Constructor
        /// </summary>
        public StressTest()
        {
        }

        /// <summary>
        /// Query hardware
        /// </summary>
        /// <returns>Hardware configuration</returns>
        public FES.HardwareSettings QueryHardware()
        {
            var hw = new FES.HardwareSettings();

            hw.DisplaySize = new Size2i(640, 360);
            hw.ColorMode = FES.ColorMode.Indexed;
            hw.Palette = "Palettes/Palette_FES32";

            return hw;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful.</returns>
        public bool Initialize()
        {
            FES.SpriteSheetSetup(0, "Demos/StressTest/Sprites", new Size2i(32, 32));
            FES.SpriteSheetSet(0);

            mScenes.Add(new SceneSpriteStress());
            mScenes.Add(new SceneTextStress());
            mScenes.Add(new SceneSpriteTextStress());
            mScenes.Add(new ScenePrimitiveStress());
            mScenes.Add(new ScenePixelStress());
            mScenes.Add(new SceneEllipseStress());
            mScenes.Add(new SceneOffscreenTest());

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
        /// Switch to previous scene, wrap around at the end
        /// </summary>
        public void PreviousScene()
        {
            int newScene = mCurrentScene - 1;
            if (newScene < 0)
            {
                newScene = mScenes.Count - 1;
            }

            mScenes[mCurrentScene].Exit();
            mCurrentScene = newScene;
            mScenes[mCurrentScene].Enter();
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
