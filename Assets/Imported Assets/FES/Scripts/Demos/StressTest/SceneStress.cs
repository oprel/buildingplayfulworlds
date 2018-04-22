namespace FESDemo
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Demonstrate drawing apis
    /// </summary>
    public class SceneStress : SceneDemo
    {
        /// <summary>
        /// Level of the stress test, the higher the more is done
        /// </summary>
        protected int mStressLevel = 500;

        /// <summary>
        /// Test stress increments
        /// </summary>
        protected int mStressIncrement = 500;

        /// <summary>
        /// Calculated FPS
        /// </summary>
        protected float mFPS = 0;

        private const int FPS_SLOTS = 32;

        /// <summary>
        /// Random number generator with a seed independant of Unity.Random
        /// </summary>
        private System.Random mRandom = new System.Random();

        private int mFPSSlot = 0;
        private float[] mFPSSlots = new float[FPS_SLOTS];

        private float mPreviousFrame = 0;
        private float mFrameDelta = 0;

        private StressButton mStressUpButton;
        private StressButton mStressDownButton;

        private StressButton mNextButton;
        private StressButton mPreviousButton;

        private List<StressButton> mButtons = new List<StressButton>();

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneStress()
        {
            Rect2i buttonRect;

            Vector2i pos = new Vector2i(12, FES.DisplaySize.height - 32);
            buttonRect = new Rect2i(pos.x, pos.y, 87, 23);
            mPreviousButton = new StressButton(buttonRect, buttonRect, 3, 2, "Previous Test", (KeyCode)555, 0, PreviousScreenButtonCB);

            pos.x += 100;
            buttonRect = new Rect2i(pos.x, pos.y, 87, 23);
            mNextButton = new StressButton(buttonRect, buttonRect, 3, 2, "Next Test", (KeyCode)555, 0, NextScreenButtonCB);

            pos.x += 100;
            buttonRect = new Rect2i(pos.x, pos.y, 87, 23);
            mStressDownButton = new StressButton(buttonRect, buttonRect, 3, 2, "Stress Down", (KeyCode)555, 0, StressDownButtonCB);

            pos.x += 100;
            buttonRect = new Rect2i(pos.x, pos.y, 87, 23);
            mStressUpButton = new StressButton(buttonRect, buttonRect, 3, 2, "Stress Up", (KeyCode)555, 0, StressUpButtonCB);

            mButtons.Add(mNextButton);
            mButtons.Add(mPreviousButton);
            mButtons.Add(mStressDownButton);
            mButtons.Add(mStressUpButton);
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public override bool Initialize()
        {
            base.Initialize();

            return true;
        }

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            foreach (var button in mButtons)
            {
                button.Reset();
            }

            mStressLevel = 500;
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            if (FES.ButtonPressed(FES.BTN_SYSTEM))
            {
                Application.Quit();
            }

            if (FES.ButtonPressed(FES.BTN_UP, FES.PLAYER_ANY))
            {
                StressUp();
            }
            else if (FES.ButtonPressed(FES.BTN_DOWN, FES.PLAYER_ANY))
            {
                StressDown();
            }

            var demo = (StressTest)FES.Game;

            if (FES.ButtonPressed(FES.BTN_LEFT, FES.PLAYER_ANY))
            {
                demo.PreviousScene();
            }
            else if (FES.ButtonPressed(FES.BTN_RIGHT, FES.PLAYER_ANY))
            {
                demo.NextScene();
            }

            foreach (var button in mButtons)
            {
                button.Update();
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            FES.Clear(0);

            StressTest();
            Overlay();

            foreach (var button in mButtons)
            {
                button.Render();
            }

            mFrameDelta = Time.realtimeSinceStartup - mPreviousFrame;
            mPreviousFrame = Time.realtimeSinceStartup;

            if (mFrameDelta > 0)
            {
                mFPSSlots[mFPSSlot] = 1.0f / mFrameDelta;

                mFPSSlot++;

                if (mFPSSlot >= mFPSSlots.Length)
                {
                    mFPSSlot = 0;

                    float fpsTotal = 0;
                    for (int i = 0; i < mFPSSlots.Length; i++)
                    {
                        fpsTotal += mFPSSlots[i];
                    }

                    mFPS = fpsTotal / mFPSSlots.Length;
                }
            }
        }

        /// <summary>
        /// Run the stress test
        /// </summary>
        protected virtual void StressTest()
        {
        }

        /// <summary>
        /// Show informative overlay
        /// </summary>
        protected virtual void Overlay()
        {
        }

        /// <summary>
        /// Get a little wiggle offset
        /// </summary>
        /// <returns>Offset</returns>
        protected Vector2i GetWiggle()
        {
            return new Vector2i((mRandom.Next() % 3) - 1, (mRandom.Next() % 3) - 1);
        }

        private void StressUp()
        {
            mStressLevel += mStressIncrement;
        }

        private void StressDown()
        {
            if (mStressLevel > mStressIncrement)
            {
                mStressLevel -= mStressIncrement;
            }
        }

        private void NextScreenButtonCB(StressButton button, object userData)
        {
            var demo = (StressTest)FES.Game;
            demo.NextScene();
        }

        private void PreviousScreenButtonCB(StressButton button, object userData)
        {
            var demo = (StressTest)FES.Game;
            demo.PreviousScene();
        }

        private void StressUpButtonCB(StressButton button, object userData)
        {
            StressUp();
        }

        private void StressDownButtonCB(StressButton button, object userData)
        {
            StressDown();
        }
    }
}
