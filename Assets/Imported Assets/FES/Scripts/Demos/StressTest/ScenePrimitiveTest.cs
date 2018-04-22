namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate drawing apis
    /// </summary>
    public class ScenePrimitiveStress : SceneStress
    {
        /// <summary>
        /// Run the stress test
        /// </summary>
        protected override void StressTest()
        {
            Random.InitState(0);

            for (int i = 0; i < mStressLevel; i++)
            {
                var wiggle = GetWiggle();

                var randRect = new Rect2i(
                    Random.Range(-FES.SpriteSize(0).width * 0.75f, FES.DisplaySize.width - (FES.SpriteSize(0).width * 0.25f)) + wiggle.x,
                    Random.Range(-FES.SpriteSize(0).height * 0.75f, FES.DisplaySize.height - (FES.SpriteSize(0).height * 0.25f)) + wiggle.y,
                    Random.Range(8, 64),
                    Random.Range(8, 64));

                int type = Random.Range(0, 3);
                if (type == 0)
                {
                    FES.DrawLine(new Vector2i(randRect.x, randRect.y), new Vector2i(randRect.x + randRect.width, randRect.y + randRect.height), Random.Range(1, 32));
                }
                else if (type == 1)
                {
                    FES.DrawRect(randRect, Random.Range(1, 32));
                }
                else if (type == 2)
                {
                    FES.DrawRectFill(randRect, Random.Range(1, 32));
                }
            }
        }

        /// <summary>
        /// Draw information overlay
        /// </summary>
        protected override void Overlay()
        {
            var str = "Primitive Stress Test\n@003Primitives: @005" + mStressLevel + "\n@003FPS: @005" + mFPS.ToString("0.00") +
                "\n\n@005" + (char)127 + " " + (char)128 + " @003Change Stress Level\n" +
                "@005" + (char)129 + " " + (char)130 + " @003Change Test";

            FES.DrawRectFill(new Rect2i(4, 4, 115, 51), 1);
            FES.Print(new Vector2i(6, 6), 5, str);
        }
    }
}
