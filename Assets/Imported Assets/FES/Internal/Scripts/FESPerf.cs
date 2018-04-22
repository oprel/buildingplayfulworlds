namespace FESInternal
{
////#define RENDER_PERF_GRAPH
    using UnityEngine;

    /// <summary>
    /// Performance measuring tools
    /// </summary>
    public class FESPerf
    {
        private const int MAX_SAMPLE_POINTS = 128;
#pragma warning disable 0414 // Unused warning
        private FESAPI mFESAPI = null;
#pragma warning restore 0414
        private float[] mUpdateDeltas = new float[MAX_SAMPLE_POINTS];
        private float[] mRenderDeltas = new float[MAX_SAMPLE_POINTS];

        private int mUpdateDeltaIndex = 0;
        private int mRenderDeltaIndex = 0;

        private float mPreviousUpdateTime = -1;
        private float mPreviousRenderTime = -1;

        /// <summary>
        /// Initialize the subsystem
        /// </summary>
        /// <param name="api">Reference to subsystem wrapper</param>
        /// <returns>True if successful</returns>
        public bool Initialize(FESAPI api)
        {
            for (int i = 0; i < MAX_SAMPLE_POINTS; i++)
            {
                mUpdateDeltas[i] = -1;
            }

            mFESAPI = api;
            return true;
        }

        /// <summary>
        /// Processes an update event, calculates timing
        /// </summary>
        public void UpdateEvent()
        {
            float currentTime = Time.realtimeSinceStartup;

            if (mPreviousUpdateTime > 0)
            {
                mUpdateDeltas[mUpdateDeltaIndex] = currentTime - mPreviousUpdateTime;

                mUpdateDeltaIndex++;
                if (mUpdateDeltaIndex == MAX_SAMPLE_POINTS)
                {
                    mUpdateDeltaIndex = 0;
                }
            }

            mPreviousUpdateTime = currentTime;
        }

        /// <summary>
        /// Processes a render event, calculates timing
        /// </summary>
        public void RenderEvent()
        {
            float currentTime = Time.realtimeSinceStartup;

            if (mPreviousRenderTime > 0)
            {
                mRenderDeltas[mRenderDeltaIndex] = currentTime - mPreviousRenderTime;

                mRenderDeltaIndex++;
                if (mRenderDeltaIndex == MAX_SAMPLE_POINTS)
                {
                    mRenderDeltaIndex = 0;
                }
            }

            mPreviousRenderTime = currentTime;
        }

        /// <summary>
        /// Draw performance data to display
        /// </summary>
        public void Draw()
        {
#if RENDER_PERF_GRAPH
            var displaySize = mFESAPI.HW.DisplaySize;

            var oldCamera = mFESAPI.Renderer.CameraGet();
            mFESAPI.Renderer.CameraSet(0, 0);

            for (int i = 0; i < MAX_SAMPLE_POINTS; i++)
            {
                int val = (int)(displaySize.height - (mUpdateDeltas[i] * 2000)) - 1;
                mFESAPI.Renderer.DrawPixel(displaySize.width - MAX_SAMPLE_POINTS + i, val, mFESAPI.HW.ColorSystemWhite, new Color32(255, 255, 255, 255), true);

                val = (int)(displaySize.height - (mRenderDeltas[i] * 2000)) - 1;
                mFESAPI.Renderer.DrawPixel(displaySize.width - MAX_SAMPLE_POINTS + i, val, mFESAPI.HW.ColorSystemPurple, new Color32(255, 255, 0, 255), true);
            }

            mFESAPI.Renderer.CameraSet(oldCamera.x, oldCamera.y);
#endif
        }
    }
}
