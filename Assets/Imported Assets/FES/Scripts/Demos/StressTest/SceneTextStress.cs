﻿namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate drawing apis
    /// </summary>
    public class SceneTextStress : SceneStress
    {
        /// <summary>
        /// Run the stress test
        /// </summary>
        protected override void StressTest()
        {
            Random.InitState(0);

            string testStr = "@005T" + "@007e" + "@015s" + "@023t";

            for (int i = 0; i < mStressLevel; i++)
            {
                var randPos = new Vector2i(Random.Range(-32, FES.DisplaySize.width), Random.Range(-8, FES.DisplaySize.height));

                randPos += GetWiggle();

                FES.Print(randPos, 5, testStr);
            }
        }

        /// <summary>
        /// Draw information overlay
        /// </summary>
        protected override void Overlay()
        {
            var str = "Text Stress Test\n@003Strings: @005" + mStressLevel + "\n@003FPS: @005" + mFPS.ToString("0.00") +
                "\n\n@005" + (char)127 + " " + (char)128 + " @003Change Stress Level\n" +
                "@005" + (char)129 + " " + (char)130 + " @003Change Test";

            FES.DrawRectFill(new Rect2i(4, 4, 115, 51), 1);
            FES.Print(new Vector2i(6, 6), 5, str);
        }
    }
}
