namespace FESInternal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Effects subsystem
    /// </summary>
    public class FESEffects
    {
        /// <summary>
        /// Total count of effects, not including custom shader
        /// </summary>
        public static int TOTAL_EFFECTS = System.Enum.GetNames(typeof(FES.Effect)).Length;

        private EffectParams[] mParams = new EffectParams[System.Enum.GetNames(typeof(FES.Effect)).Length + 2]; // +2 for custom shader and filter type
#pragma warning disable 0414 // Unused warning
        private FESAPI mFESAPI = null;
#pragma warning restore 0414

        /// <summary>
        /// Initialize the subsystem
        /// </summary>
        /// <param name="api">Reference to subsystem wrapper</param>
        /// <returns>True if successful</returns>
        public bool Initialize(FESAPI api)
        {
            mFESAPI = api;

            for (int i = 0; i < mParams.Length; i++)
            {
                mParams[i] = new EffectParams();
            }

            EffectReset();

            return true;
        }

        /// <summary>
        /// Get parameters for effect
        /// </summary>
        /// <param name="effect">Effect</param>
        /// <returns>Parameters</returns>
        public EffectParams ParamsGet(FES.Effect effect)
        {
            return mParams[(int)effect];
        }

        /// <summary>
        /// Set parameters for effect
        /// </summary>
        /// <param name="type">Effect</param>
        /// <param name="intensity">Intensity</param>
        /// <param name="vec">Vector data</param>
        /// <param name="colorIndex">Indexed color</param>
        /// <param name="color">RGB color</param>
        public void EffectSet(FES.Effect type, float intensity, Vector2i vec, int colorIndex, ColorRGBA color)
        {
            // Special case for shaders
            if ((int)type == TOTAL_EFFECTS)
            {
                ParamsGet(type).ColorIndex = colorIndex;
            }

            switch (type)
            {
                case FES.Effect.Noise:
                case FES.Effect.Desaturation:
                case FES.Effect.Curvature:
                case FES.Effect.Shake:
                case FES.Effect.Negative:
                case FES.Effect.Pixelate:
                    ParamsGet(type).Intensity = Mathf.Clamp01(intensity);
                    break;

                case FES.Effect.Scanlines:
                    ParamsGet(type).Intensity = Mathf.Clamp01(intensity);
                    break;

                case FES.Effect.Zoom:
                    ParamsGet(type).Intensity = Mathf.Clamp(intensity, 0, 10000.0f);
                    break;

                case FES.Effect.Slide:
                case FES.Effect.Wipe:
                    ParamsGet(type).Vector = new Vector2i(
                        Mathf.Clamp(vec.x, -FES.DisplaySize.width, FES.DisplaySize.width),
                        Mathf.Clamp(vec.y, -FES.DisplaySize.height, FES.DisplaySize.height));
                    break;

                case FES.Effect.Rotation:
                    ParamsGet(type).Intensity = FESUtil.WrapAngle(intensity);
                    break;

                case FES.Effect.ColorFade:
                case FES.Effect.ColorTint:
                    ParamsGet(type).Intensity = Mathf.Clamp01(intensity);
                    ParamsGet(type).Color = color;
                    ParamsGet(type).ColorIndex = colorIndex;
                    break;

                case FES.Effect.Fizzle:
                    // Increase intensity by 1% to ensure full pixel coverage
                    ParamsGet(type).Intensity = Mathf.Clamp01(intensity) * 1.01f;
                    ParamsGet(type).Color = color;
                    ParamsGet(type).ColorIndex = colorIndex;
                    break;

                case FES.Effect.Pinhole:
                case FES.Effect.InvertedPinhole:
                    ParamsGet(type).Intensity = Mathf.Clamp01(intensity);
                    ParamsGet(type).Vector = new Vector2i((int)Mathf.Clamp(vec.x, 0, FES.DisplaySize.width - 1), (int)Mathf.Clamp(vec.y, 0, FES.DisplaySize.height - 1));
                    ParamsGet(type).Color = color;
                    ParamsGet(type).ColorIndex = colorIndex;
                    break;
            }
        }

        /// <summary>
        /// Set a custom shader effect
        /// </summary>
        /// <param name="shaderIndex">Shader index to use</param>
        public void EffectShaderSet(int shaderIndex)
        {
            ParamsGet((FES.Effect)TOTAL_EFFECTS).ColorIndex = shaderIndex;
        }

        /// <summary>
        /// Set texture filter to use with custom shader effect
        /// </summary>
        /// <param name="filterMode">Filter</param>
        public void EffectFilterSet(FilterMode filterMode)
        {
            ParamsGet((FES.Effect)TOTAL_EFFECTS + 1).ColorIndex = (int)filterMode;
        }

        /// <summary>
        /// Get a copy of the current effect states
        /// </summary>
        /// <returns>Effects</returns>
        public EffectParams[] CopyState()
        {
            EffectParams[] paramsCopy = new EffectParams[mParams.Length];

            for (int i = 0; i < paramsCopy.Length; i++)
            {
                paramsCopy[i] = mParams[i].ShallowCopy();
            }

            return paramsCopy;
        }

        /// <summary>
        /// Apply render time post processing effects, these are just drawing operations and must
        /// be ran before the other shader-time effects are applied
        /// </summary>
        public void ApplyRenderTimeEffects()
        {
            var renderer = mFESAPI.Renderer;

            // Pinhole effect
            if (mFESAPI.Effects.ParamsGet(FES.Effect.Pinhole).Intensity > 0)
            {
                var p = mFESAPI.Effects.ParamsGet(FES.Effect.Pinhole);

                Vector2i c = new Vector2i((int)(p.Vector.x + 0.5f), (int)(p.Vector.y + 0.5f));

                int r = (int)((1.0f - p.Intensity) * renderer.MaxCircleRadiusForCenter(c));

                renderer.DrawEllipseFill(c, new Vector2i(r, r), p.ColorIndex, p.Color.ToColor32(), true);
                if (c.x < FES.DisplaySize.width)
                {
                    renderer.DrawRectFill(new Rect2i(c.x + r + 1, c.y - r, FES.DisplaySize.width - c.x - r - 1, (r * 2) + 1), p.ColorIndex, p.Color.ToColor32(), Vector2i.zero);
                }

                if (c.x > 0)
                {
                    renderer.DrawRectFill(new Rect2i(0, c.y - r, c.x - r, (r * 2) + 1), p.ColorIndex, p.Color.ToColor32(), Vector2i.zero);
                }

                if (c.y > 0)
                {
                    renderer.DrawRectFill(new Rect2i(0, 0, FES.DisplaySize.width, c.y - r), p.ColorIndex, p.Color.ToColor32(), Vector2i.zero);
                }

                if (c.y < FES.DisplaySize.height)
                {
                    renderer.DrawRectFill(new Rect2i(0, c.y + r + 1, FES.DisplaySize.width, FES.DisplaySize.height - (c.y + r + 1)), p.ColorIndex, p.Color.ToColor32(), Vector2i.zero);
                }
            }
            else if (mFESAPI.Effects.ParamsGet(FES.Effect.InvertedPinhole).Intensity > 0)
            {
                var p = mFESAPI.Effects.ParamsGet(FES.Effect.InvertedPinhole);

                Vector2i c = new Vector2i((int)(p.Vector.x + 0.5f), (int)(p.Vector.y + 0.5f));
                int r = (int)(p.Intensity * renderer.MaxCircleRadiusForCenter(c));

                renderer.DrawEllipseFill(c, new Vector2i(r, r), p.ColorIndex, p.Color.ToColor32(), false);
            }

            renderer.DrawClipRegions();
        }

        /// <summary>
        /// Reset all effects back to default/off states
        /// </summary>
        public void EffectReset()
        {
            foreach (var p in mParams)
            {
                p.Color = ColorRGBA.white;
                p.ColorIndex = 0;
                p.Intensity = 0.0f;
                p.Vector = Vector2i.zero;
            }

            mParams[(int)FES.Effect.Zoom].Intensity = 1.0f;
            mParams[TOTAL_EFFECTS].ColorIndex = -1;
            ParamsGet((FES.Effect)TOTAL_EFFECTS + 1).ColorIndex = (int)FilterMode.Point;
        }

        /// <summary>
        /// Effect parameters
        /// </summary>
        public class EffectParams
        {
            /// <summary>
            /// Intensity of effect, usually 0.0 to 1.0
            /// </summary>
            public float Intensity;

            /// <summary>
            /// Generic vector data
            /// </summary>
            public Vector2i Vector;

            /// <summary>
            /// RGB color
            /// </summary>
            public ColorRGBA Color;

            /// <summary>
            /// Indexed color
            /// </summary>
            public int ColorIndex;

            /// <summary>
            /// Shallow copy of an effect
            /// </summary>
            /// <returns>A copy of the effect</returns>
            public EffectParams ShallowCopy()
            {
                EffectParams copy = (EffectParams)this.MemberwiseClone();

                return copy;
            }
        }
    }
}
