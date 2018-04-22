namespace FESInternal
{
    using UnityEngine;

    /// <summary>
    /// Pixel Camera subsystem that renders to an offscreen "pixel surface" that has the pixel dimensions of an old videogame console
    /// </summary>
    public sealed class FESPixelScreen : MonoBehaviour
    {
        private const float SHAKE_INTERVAL = 0.01f;

        private Material mPresentMaterial;
        private Material mPresentRetroMaterial;
        private Material mPresentRetroNoiseMaterial;
        private Material mCurrentPresentMaterial;

        private UnityEngine.Camera mCamera;

        private Size2i mPreviousScreenSize = new Size2i(-1, -1);

        private FESAPI mFESAPI = null;

        private Size2i mPresentSize = Size2i.zero;
        private Vector2i mPresentOffset = Vector2i.zero;

        private Vector2i mLastShakeOffset = Vector2i.zero;
        private float mShakeDelay = 0;

		private RenderTexture screen;

        /// <summary>
        /// Initialize subsystem
        /// </summary>
        /// <param name="api">Subsystem wrapper reference</param>
        /// <returns>True if successful</returns>
        public bool Initialize(FESAPI api)
        {
            mFESAPI = api;

            mCamera = Camera.main;

            if (FES.DisplaySize.width <= 0 || FES.DisplaySize.height <= 0)
            {
                return false;
            }

            var material = mFESAPI.ResourceBucket.LoadMaterial("PresentMaterial");
            mPresentMaterial = new Material(material);

            material = mFESAPI.ResourceBucket.LoadMaterial("PresentRetroMaterial");
            mPresentRetroMaterial = new Material(material);

            material = mFESAPI.ResourceBucket.LoadMaterial("PresentRetroNoiseMaterial");
            mPresentRetroNoiseMaterial = new Material(material);

            mCurrentPresentMaterial = mPresentMaterial;

            /* Dummy camera exists only to keep Unity Editor IDE from complaining that nothing is rendering,
             * if this is not an editor build then we can destroy this camera */
#if !UNITY_EDITOR
            var dummyCamera = GameObject.Find("FESDummyCamera");
            if (dummyCamera != null)
            {
                Destroy(dummyCamera);
            }
#endif

            return true;
        }

        /// <summary>
        /// Get Unity camera
        /// </summary>
        /// <returns>Camera</returns>
        public UnityEngine.Camera GetCamera()
        {
            return mCamera;
        }

        /// <summary>
        /// Convert screen point to viewport point
        /// </summary>
        /// <param name="p">Point</param>
        /// <returns>Converted position</returns>
        public Vector3 ScreenToViewportPoint(Vector3 p)
        {
            if (mPresentSize.width < 1 || mPresentSize.height < 1)
            {
                return new Vector3(0, 0, 0);
            }

            p.z = 0;

            p.x -= mPresentOffset.x;
            p.y += mPresentOffset.y;
            p.y = Screen.height - p.y;

            p = mCamera.ScreenToViewportPoint(p);

            p.x /= (Screen.width - (mPresentOffset.x * 2)) / mCamera.pixelRect.width;
            p.y /= (Screen.height - (mPresentOffset.y * 2)) / mCamera.pixelRect.height;

            p.x *= mCamera.pixelRect.width;
            p.y *= mCamera.pixelRect.height;

            return p;
        }

        /// <summary>
        /// Get the current render target
        /// </summary>
        /// <returns>Current render target</returns>
        public RenderTexture GetRenderTarget()
        {
            return mCamera.targetTexture;
        }

        /// <summary>
        /// Set the current render target
        /// </summary>
        /// <param name="renderTarget">Render target</param>
        public void SetRenderTarget(RenderTexture renderTarget)
        {
            mCamera.targetTexture = renderTarget;
            WindowResize();
        }

        /// <summary>
        /// Setup all shader global variable, there is a bunch, most are tried to post processing effects
        /// </summary>
        /// <param name="effectParams">Array of all current effects</param>
        /// <param name="pixelTexture">Reference to the pixel texture being rendered</param>
        private void SetShaderGlobals(FESEffects.EffectParams[] effectParams, RenderTexture pixelTexture)
        {
            var customShader = mFESAPI.Renderer.ShaderGetMaterial(effectParams[(int)FESEffects.TOTAL_EFFECTS].ColorIndex);

            if (customShader != null)
            {
                mCurrentPresentMaterial = customShader;

                mCurrentPresentMaterial.SetTexture("_PixelTexture", pixelTexture);
                mCurrentPresentMaterial.SetVector("_PixelTextureSize", new Vector2(FES.DisplaySize.width, FES.DisplaySize.height));
                mCurrentPresentMaterial.SetVector("_PresentSize", new Vector2(Screen.width, Screen.height));

                FilterMode filterMode = (FilterMode)effectParams[(int)FESEffects.TOTAL_EFFECTS + 1].ColorIndex;
                pixelTexture.filterMode = filterMode;
            }
            else
            {
                var scanlineParams = effectParams[(int)FES.Effect.Scanlines];
                var noiseParams = effectParams[(int)FES.Effect.Noise];
                var desatParams = effectParams[(int)FES.Effect.Desaturation];
                var curvParams = effectParams[(int)FES.Effect.Curvature];
                var fizzleParams = effectParams[(int)FES.Effect.Fizzle];
                var zoomParams = effectParams[(int)FES.Effect.Zoom];
                var pixelateParams = effectParams[(int)FES.Effect.Pixelate];
                var colorTintParams = effectParams[(int)FES.Effect.ColorTint];
                var colorFadeParams = effectParams[(int)FES.Effect.ColorFade];
                float negativeIntensity = effectParams[(int)FES.Effect.Negative].Intensity;
                float pixelateIntensity = effectParams[(int)FES.Effect.Pixelate].Intensity;

                if (noiseParams.Intensity > 0 || fizzleParams.Intensity > 0 || scanlineParams.Intensity > 0)
                {
                    mCurrentPresentMaterial = mPresentRetroNoiseMaterial;
                }
                else if (scanlineParams.Intensity > 0 || desatParams.Intensity > 0 || curvParams.Intensity > 0)
                {
                    mCurrentPresentMaterial = mPresentRetroMaterial;
                }
                else
                {
                    mCurrentPresentMaterial = mPresentMaterial;
                }

                mCurrentPresentMaterial.SetTexture("_PixelTexture", pixelTexture);
                mCurrentPresentMaterial.SetVector("_PixelTextureSize", new Vector2(FES.DisplaySize.width, FES.DisplaySize.height));
                mCurrentPresentMaterial.SetVector("_PresentSize", new Vector2(Screen.width, Screen.height));

                if (noiseParams.Intensity > 0 || fizzleParams.Intensity > 0 || scanlineParams.Intensity > 0)
                {
                    mCurrentPresentMaterial.SetTexture("_SystemTexture", mFESAPI.Renderer.SystemTexture);
                }

                float sampleFactor = 0;

                if (pixelateParams.Intensity == 0)
                {
                    if (mPresentSize.width % FES.DisplaySize.width != 0 || mPresentSize.height % FES.DisplaySize.height != 0 ||
                        curvParams.Intensity != 0)
                    {
                        sampleFactor = 1.0f / (((float)Screen.width / FES.DisplaySize.width) * 2.5f);
                        if (zoomParams.Intensity != 1)
                        {
                            sampleFactor /= zoomParams.Intensity;
                        }
                    }
                }

                mCurrentPresentMaterial.SetFloat("_SampleFactor", sampleFactor);

                // Apply retroness
                mCurrentPresentMaterial.SetFloat("_ScanlineIntensity", scanlineParams.Intensity);
                if (scanlineParams.Intensity > 0)
                {
                    int offset;
                    int length;
                    float pixelSize = mPresentSize.height / (float)FES.DisplaySize.height;
                    mFESAPI.Renderer.GetScanlineOffsetLength(pixelSize, out offset, out length);

                    mCurrentPresentMaterial.SetFloat("_ScanlineOffset", offset);
                    mCurrentPresentMaterial.SetFloat("_ScanlineLength", length);

                    // Scanlines look horrible if the screen is not evenly divisible by pixel size. Pass the scanline shader a
                    // screensize thats the nearest height evenly divisible by pixel size.
                    int nearestEvenScanHeight = 0;
                    if (mPresentSize.height > 0 && FES.DisplaySize.height > 0)
                    {
                        nearestEvenScanHeight = Mathf.FloorToInt(mPresentSize.height / (float)length);
                    }

                    mCurrentPresentMaterial.SetFloat("_NearestEvenScanHeight", nearestEvenScanHeight);
                }

                mCurrentPresentMaterial.SetFloat("_DesaturationIntensity", desatParams.Intensity);
                if (noiseParams.Intensity > 0)
                {
                    mCurrentPresentMaterial.SetFloat("_NoiseIntensity", noiseParams.Intensity);

                    var oldRandState = Random.state;
                    Random.InitState((int)FES.Ticks);

                    mCurrentPresentMaterial.SetVector("_NoiseSeed", new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));

                    Random.state = oldRandState;
                }
                else
                {
                    mCurrentPresentMaterial.SetFloat("_NoiseIntensity", 0);
                }

                mCurrentPresentMaterial.SetFloat("_CurvatureIntensity", curvParams.Intensity);

                // Color Fade
                ColorRGBA colorFade;
                if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
                {
                    colorFade = mFESAPI.Palette.ColorAtIndex(colorFadeParams.ColorIndex);
                }
                else
                {
                    colorFade = colorFadeParams.Color;
                }

                mCurrentPresentMaterial.SetVector("_ColorFade", new Vector3(colorFade.r / 255.0f, colorFade.g / 255.0f, colorFade.b / 255.0f));
                mCurrentPresentMaterial.SetFloat("_ColorFadeIntensity", colorFadeParams.Intensity);

                // Color Tint
                ColorRGBA colorTint;
                if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
                {
                    colorTint = mFESAPI.Palette.ColorAtIndex(colorTintParams.ColorIndex);
                }
                else
                {
                    colorTint = colorTintParams.Color;
                }

                mCurrentPresentMaterial.SetVector("_ColorTint", new Vector3(colorTint.r / 255.0f, colorTint.g / 255.0f, colorTint.b / 255.0f));
                mCurrentPresentMaterial.SetFloat("_ColorTintIntensity", colorTintParams.Intensity);

                // Fizzle
                ColorRGBA colorFizzle;
                if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
                {
                    colorFizzle = mFESAPI.Palette.ColorAtIndex(fizzleParams.ColorIndex);
                }
                else
                {
                    colorFizzle = fizzleParams.Color;
                }

                mCurrentPresentMaterial.SetVector("_FizzleColor", new Vector3(colorFizzle.r / 255.0f, colorFizzle.g / 255.0f, colorFizzle.b / 255.0f));
                mCurrentPresentMaterial.SetFloat("_FizzleIntensity", fizzleParams.Intensity);

                mCurrentPresentMaterial.SetFloat("_NegativeIntensity", negativeIntensity);

                mCurrentPresentMaterial.SetFloat("_PixelateIntensity", pixelateIntensity);
            }
        }

        private void WindowResize()
        {
            if (mCamera.targetTexture == null)
            {
                return;
            }

            mCamera.orthographicSize = mCamera.targetTexture.height / 2.0f;
            mCamera.rect = new Rect(0, 0, mCamera.targetTexture.width, mCamera.targetTexture.height);
            mCamera.transform.position = new Vector3(0, 0, -10);
            mCamera.transform.localScale = new Vector3(1, 1, 1);
        }

        /// <summary>
        /// Good place to update window size, and shader variables
        /// </summary>
        private void Update()
        {
            if (mCamera == null || mCamera.targetTexture == null)
            {
                return;
            }

            // Check for window resize
            if (mPreviousScreenSize.height != Screen.height ||
                mPreviousScreenSize.width != Screen.width)
            {
                mPreviousScreenSize.width = Screen.width;
                mPreviousScreenSize.height = Screen.height;

                WindowResize();
            }
        }

        private void RenderPixelSurfaces()
        {
            int usedBuffers = 0;
            var frontBuffers = mFESAPI.Renderer.GetFrontBuffer().GetBuffers(out usedBuffers);

            for (int bufferIndex = 0; bufferIndex < usedBuffers; bufferIndex++)
            {
                var buffer = frontBuffers[bufferIndex];
                var effectParams = buffer.effectParams;

                SetShaderGlobals(effectParams, buffer.tex);

                Size2i displaySize = FES.DisplaySize;
                if (mFESAPI.HW.PixelStyle == FES.PixelStyle.Wide)
                {
                    displaySize.width *= 2;
                }
                else if (mFESAPI.HW.PixelStyle == FES.PixelStyle.Tall)
                {
                    displaySize.height *= 2;
                }

                mPresentSize.width = Screen.width;
                mPresentSize.height = (int)(Screen.width * ((float)displaySize.height / (float)displaySize.width));
                if (mPresentSize.height > Screen.height)
                {
                    mPresentSize.width = (int)(Screen.height * ((float)displaySize.width / (float)displaySize.height));
                    mPresentSize.height = Screen.height;
                }

                // Round up present size to the next multiple of scanline pattern length. Without this we can get bad repetition
                // patterns in the scanline effect
                // At most this will cut off a part of 1 pixel.
                int offset;
                int length;
                float pixelSize = mPresentSize.height / (float)displaySize.height;
                mFESAPI.Renderer.GetScanlineOffsetLength(pixelSize, out offset, out length);

                if (mPresentSize.width % length > 0)
                {
                    mPresentSize.width += length - (mPresentSize.width % length);
                }

                if (mPresentSize.height % length > 0)
                {
                    mPresentSize.height += length - (mPresentSize.height % length);
                }

                mPresentOffset.x = (int)((Screen.width - mPresentSize.width) / 2.0f);
                mPresentOffset.y = (int)((Screen.height - mPresentSize.height) / 2.0f);

                var clipRect = new Rect(mPresentOffset.x, mPresentOffset.y, mPresentSize.width, mPresentSize.height);

                // Slide effect
                Vector2i slideOffset = effectParams[(int)FES.Effect.Slide].Vector;
                Vector2 slideOffsetf = new Vector2(
                    ((float)slideOffset.x / (float)FES.DisplaySize.width) * mPresentSize.width,
                    ((float)slideOffset.y / (float)FES.DisplaySize.height) * mPresentSize.height);
                mPresentOffset += slideOffsetf;

                // Clear, but only on first buffer
                if (bufferIndex == 0)
                {
                    GL.Clear(true, true, new Color32(0, 0, 0, 255));
                }

                // Wipe effect
                Rect destRect = new Rect(mPresentOffset.x, mPresentOffset.y, mPresentSize.width, mPresentSize.height);
                Rect srcRect = new Rect(0, 0, 1, 1);

                Vector2i wipe = effectParams[(int)FES.Effect.Wipe].Vector;
                Vector2 wipef = new Vector2((float)wipe.x / (float)FES.DisplaySize.width, (float)wipe.y / (float)FES.DisplaySize.height);

                if (wipe.x > 0)
                {
                    destRect.x = mPresentOffset.x + (mPresentSize.width * wipef.x);
                    destRect.width = mPresentSize.width - (mPresentSize.width * wipef.x);
                    srcRect.x = wipef.x;
                    srcRect.width = 1f - wipef.x;
                }
                else if (wipe.x < 0)
                {
                    destRect.x = mPresentOffset.x;
                    destRect.width = mPresentSize.width - (mPresentSize.width * (-wipef.x));
                    srcRect.x = 0;
                    srcRect.width = 1f - (-wipef.x);
                }

                if (wipe.y > 0)
                {
                    destRect.y = mPresentOffset.y + (mPresentSize.height * wipef.y);
                    destRect.height = mPresentSize.height - (mPresentSize.height * wipef.y);
                    srcRect.y = 0;
                    srcRect.height = 1f - wipef.y;
                }
                else if (wipe.y < 0)
                {
                    destRect.y = mPresentOffset.y;
                    destRect.height = mPresentSize.height - (mPresentSize.height * (-wipef.y));
                    srcRect.y = -wipef.y;
                    srcRect.height = 1f - (-wipef.y);
                }

                // Shake
                float shake = effectParams[(int)FES.Effect.Shake].Intensity;
                if (shake > 0)
                {
                    // Don't shake every frame, shake at a set interval, and decay the shake offset
                    // between intervals
                    if (mShakeDelay <= 0)
                    {
                        var oldRandState = Random.state;
                        Random.InitState((int)FES.Ticks);

                        float maxMag = mPresentSize.width * 0.05f;
                        destRect.x += maxMag * shake * Random.Range(-1.0f, 1.0f);
                        destRect.y += maxMag * shake * Random.Range(-1.0f, 1.0f);

                        mLastShakeOffset = new Vector2i(destRect.x, destRect.y);
                        mShakeDelay = SHAKE_INTERVAL;

                        Random.state = oldRandState;
                    }
                    else
                    {
                        destRect.x = mLastShakeOffset.x;
                        destRect.y = mLastShakeOffset.y;
                        mLastShakeOffset *= 0.75f;
                        mShakeDelay -= mFESAPI.HW.UpdateInterval;
                    }
                }
                else
                {
                    mLastShakeOffset = Vector2i.zero;
                    mShakeDelay = 0;
                }

                // Zoom
                float zoom = effectParams[(int)FES.Effect.Zoom].Intensity;
                if (zoom != 1)
                {
                    destRect.width *= zoom;
                    destRect.height *= zoom;
                    destRect.x += (mPresentSize.width - destRect.width) / 2f;
                    destRect.y += (mPresentSize.height - destRect.height) / 2f;
                }

                GL.PushMatrix();

                float rotation = effectParams[(int)FES.Effect.Rotation].Intensity;

                if (rotation != 0)
                {
                    GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);
                    GL.MultMatrix(
                        Matrix4x4.Translate(new Vector3(Screen.width / 2, Screen.height / 2, 0)) *
                        Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 0, rotation))) *
                        Matrix4x4.Translate(new Vector3(-Screen.width / 2, -Screen.height / 2, 0)));
                }
                else
                {
                    GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);
                }

                // Do not try to render at all if currently offscreen (eg due to slide effect)
                if (!(destRect.x + destRect.width < clipRect.x ||
                    destRect.x >= clipRect.x + clipRect.width ||
                    destRect.y + destRect.height < clipRect.y ||
                    destRect.y >= clipRect.y + clipRect.height))
                {
                    if ((int)destRect.x < (int)clipRect.x)
                    {
                        var correction = clipRect.x - destRect.x;
                        srcRect.x += correction / destRect.width;
                        srcRect.width -= correction / destRect.width;
                        destRect.x = clipRect.x;
                        destRect.width -= correction;
                    }
                    else if ((int)(destRect.x + destRect.width) > (int)(clipRect.x + clipRect.width))
                    {
                        var correction = (destRect.x + destRect.width) - (clipRect.x + clipRect.width);
                        srcRect.width = (destRect.width - correction) / destRect.width;
                        destRect.width -= correction;
                    }

                    if ((int)destRect.y < (int)clipRect.y)
                    {
                        var correction = clipRect.y - destRect.y;
                        srcRect.height = (destRect.height - correction) / destRect.height;
                        destRect.y = clipRect.y;
                        destRect.height -= correction;
                    }
                    else if ((int)(destRect.y + destRect.height) > (int)(clipRect.y + clipRect.height))
                    {
                        var correction = (destRect.y + destRect.height) - (clipRect.y + clipRect.height);
                        srcRect.y += correction / destRect.height;
                        srcRect.height -= correction / destRect.height;
                        destRect.height -= correction;
                    }

                    Graphics.DrawTexture(destRect, mCamera.targetTexture, srcRect, 0, 0, 0, 0, mCurrentPresentMaterial);
                }

                GL.PopMatrix();

                buffer.tex.filterMode = FilterMode.Point;
            }
        }

        /// <summary>
        /// This is where we actually render to screen. Various presentation effects are applied here as well.
        /// </summary>
        private void OnGUI()
        {
            if (mCamera == null || mCamera.targetTexture == null)
            {
                return;
            }

            if (Event.current.type.Equals(EventType.Repaint))
            {
                RenderPixelSurfaces();
            }
        }

        /// <summary>
        /// This is where we call the game Render method to let the user do their rendering
        /// </summary>
        private void OnPostRender()
        {
            if (mCamera == null || mCamera.targetTexture == null)
            {
                return;
            }

            if (mFESAPI == null || mFESAPI.Renderer == null)
            {
                return;
            }

            if (!mFESAPI.Initialized)
            {
                return;
            }

            mFESAPI.Renderer.RenderEnabled = true;

            mFESAPI.Renderer.StartRender();

            var game = FES.Game;

            if (game != null)
            {
                game.Render();
            }

            if (mFESAPI.Perf != null)
            {
                mFESAPI.Perf.RenderEvent();
            }

            mFESAPI.Perf.Draw();

            mFESAPI.Renderer.FrameEnd();

            mFESAPI.Renderer.RenderEnabled = false;
        }
    }
}
