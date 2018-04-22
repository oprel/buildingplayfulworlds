namespace FESDemo
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Demonstrate post processing effects
    /// </summary>
    public class SceneEffects : SceneDemo
    {
        private static int mCloudTicks = 0;

        private Size2i mMapSize;

        private FES.Effect mEffect = 0;
        private string mParamsText;

        private Dictionary<FES.Effect, string> mNames = new Dictionary<FES.Effect, string>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="effect">Which effect to demonstrate</param>
        public SceneEffects(FES.Effect effect)
        {
            mEffect = effect;
        }

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            FES.EffectReset();

            FES.MapLoadTMX("Demos/DemoReel/Tilemap.xml", "Clouds", 1, out mMapSize);
            FES.MapLoadTMX("Demos/DemoReel/Tilemap.xml", "Decoration", 2);
            FES.MapLoadTMX("Demos/DemoReel/Tilemap.xml", "Terrain", 3);

            mNames[FES.Effect.Scanlines] = "Scanlines";
            mNames[FES.Effect.Noise] = "Noise";
            mNames[FES.Effect.Desaturation] = "Desaturation";
            mNames[FES.Effect.Curvature] = "Curvature";
            mNames[FES.Effect.Slide] = "Slide";
            mNames[FES.Effect.Wipe] = "Wipe";
            mNames[FES.Effect.Shake] = "Shake";
            mNames[FES.Effect.Zoom] = "Zoom";
            mNames[FES.Effect.Rotation] = "Rotation";
            mNames[FES.Effect.ColorFade] = "ColorFade";
            mNames[FES.Effect.ColorTint] = "ColorTint";
            mNames[FES.Effect.Negative] = "Negative";
            mNames[FES.Effect.Pixelate] = "Pixelate";
            mNames[FES.Effect.Pinhole] = "Pinhole";
            mNames[FES.Effect.InvertedPinhole] = "InvertedPinhole";
            mNames[FES.Effect.Fizzle] = "Fizzle";
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            FES.EffectReset();
            FES.MapClear();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            mCloudTicks++;
            ApplyEffect();

            base.Update();
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            var demo = (DemoReel)FES.Game;

            int cloudScrollPos = -mCloudTicks % (mMapSize.width * FES.SpriteSize().width);

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Clear(22);
            }
            else
            {
                FES.Clear(DemoUtil.IndexToRGB(22));
            }

            FES.CameraSet(new Vector2i(mMapSize.width * FES.SpriteSize().width / 4, 0));
            FES.DrawMapLayer(1, new Vector2i(cloudScrollPos, 0));
            FES.DrawMapLayer(1, new Vector2i(cloudScrollPos + (mMapSize.width * FES.SpriteSize().width), 0));
            FES.DrawMapLayer(2);
            FES.DrawMapLayer(3);
            FES.CameraReset();

            string text = mNames[(FES.Effect)mEffect] + "\n" + DemoUtil.IndexToColorString(2) +
                "FES.EffectSet(FES.Effect." + mNames[(FES.Effect)mEffect].ToUpper() +
                mParamsText + ");";

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i((FES.DisplaySize.width / 2) - 80, (FES.DisplaySize.height / 2) - 10), 0, text);
            }
            else
            {
                FES.Print(new Vector2i((FES.DisplaySize.width / 2) - 120, (FES.DisplaySize.height / 2) - 10), DemoUtil.IndexToRGB(0), text);
            }
        }

        private void ApplyEffect()
        {
            var demo = (DemoReel)FES.Game;

            float progress = Mathf.Sin((FES.Ticks % 250) / 150f * Mathf.PI);
            progress = Mathf.Clamp(progress, 0, 1);

            switch (mEffect)
            {
                case FES.Effect.Scanlines:
                case FES.Effect.Noise:
                case FES.Effect.Desaturation:
                case FES.Effect.Curvature:
                case FES.Effect.Negative:
                case FES.Effect.Pixelate:
                    FES.EffectSet(mEffect, progress);
                    mParamsText = ", " + progress.ToString("0.00");
                    break;

                case FES.Effect.Shake:
                    FES.EffectSet(mEffect, progress * 0.1f);
                    mParamsText = ", " + (progress * 0.1f).ToString("0.00");
                    break;

                case FES.Effect.Zoom:
                    FES.EffectSet(mEffect, (progress * 5.0f) + 0.5f);
                    mParamsText = ", " + ((progress * 5.0f) + 0.5f).ToString("0.00");
                    break;

                case FES.Effect.Slide:
                case FES.Effect.Wipe:
                    Vector2i slide = new Vector2i(progress * FES.DisplaySize.width, 0);
                    FES.EffectSet(mEffect, slide);
                    mParamsText = ", new Vector2i(" + slide.x.ToString("0") + ", " + slide.y.ToString("0") + ")";
                    break;

                case FES.Effect.Rotation:
                    FES.EffectSet(mEffect, progress * 360.0f);
                    mParamsText = ", " + (progress * 360.0f).ToString("0");
                    break;

                case FES.Effect.ColorFade:
                    if (demo.ColorMode == FES.ColorMode.Indexed)
                    {
                        FES.EffectSet(mEffect, progress, Vector2i.zero, 20);
                        mParamsText = ", " + progress.ToString("0.00") + ", Vector2i.zero, 20";
                    }
                    else
                    {
                        FES.EffectSet(mEffect, progress, Vector2i.zero, DemoUtil.IndexToRGB(20));
                        ColorRGBA rgb = DemoUtil.IndexToRGB(20);
                        mParamsText = ", " + progress.ToString("0.00") + ", Vector2i.zero, new ColorRGBA(" + rgb.r + ", " + rgb.g + ", " + rgb.b + ")";
                    }

                    break;

                case FES.Effect.ColorTint:
                    if (demo.ColorMode == FES.ColorMode.Indexed)
                    {
                        FES.EffectSet(mEffect, progress, Vector2i.zero, 31);
                        mParamsText = ", " + progress.ToString("0.00") + ", Vector2i.zero, 31";
                    }
                    else
                    {
                        FES.EffectSet(mEffect, progress, Vector2i.zero, DemoUtil.IndexToRGB(31));
                        ColorRGBA rgb = DemoUtil.IndexToRGB(31);
                        mParamsText = ", " + progress.ToString("0.00") + ", Vector2i.zero, new ColorRGBA(" + rgb.r + ", " + rgb.g + ", " + rgb.b + ")";
                    }

                    break;

                case FES.Effect.Fizzle:
                    if (demo.ColorMode == FES.ColorMode.Indexed)
                    {
                        FES.EffectSet(mEffect, progress, Vector2i.zero, 11);
                        mParamsText = ", " + progress.ToString("0.00") + ", Vector2i.zero, 11";
                    }
                    else
                    {
                        FES.EffectSet(mEffect, progress, Vector2i.zero, DemoUtil.IndexToRGB(11));
                        ColorRGBA rgb = DemoUtil.IndexToRGB(11);
                        mParamsText = ", " + progress.ToString("0.00") + ", Vector2i.zero, new ColorRGBA(" + rgb.r + ", " + rgb.g + ", " + rgb.b + ")";
                    }

                    break;

                case FES.Effect.Pinhole:
                case FES.Effect.InvertedPinhole:
                    Vector2i pos =
                        new Vector2i(
                            (Mathf.Sin(progress * 8) * (FES.DisplaySize.width / 6.0f)) + (FES.DisplaySize.width / 6.0f),
                            (Mathf.Cos(progress * 8) * (FES.DisplaySize.width / 6.0f)) + (FES.DisplaySize.width / 6.0f));

                    if (demo.ColorMode == FES.ColorMode.Indexed)
                    {
                        FES.EffectSet(mEffect, progress, pos, 0);
                        mParamsText = ", " + progress.ToString("0.00") + ", new Vector2i(" + pos.x.ToString("0") + ", " + pos.y.ToString("0") + "), 0";
                    }
                    else
                    {
                        FES.EffectSet(mEffect, progress, pos, DemoUtil.IndexToRGB(0));
                        ColorRGBA rgb = DemoUtil.IndexToRGB(0);
                        mParamsText = ", " + progress.ToString("0.00") + ", new Vector2i(" + pos.x.ToString("0") + ", " + pos.y.ToString("0") + "), new ColorRGBA(" + rgb.r + ", " + rgb.g + ", " + rgb.b + ")";
                    }

                    break;
            }
        }
    }
}
