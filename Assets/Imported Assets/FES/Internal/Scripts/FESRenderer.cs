namespace FESInternal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Renderer subsystem
    /// </summary>
    public sealed class FESRenderer
    {
        private const int MAX_QUADS_PER_MESH = 512;
        private const int MAX_INDECIES_PER_MESH = 6 * MAX_QUADS_PER_MESH;
        private const int MAX_VERTEX_PER_MESH = 4 * MAX_QUADS_PER_MESH;

        private const int MAX_ELLIPSE_RADIUS = 10000;

        private FrontBuffer mFrontBuffer = new FrontBuffer();
        private OffscreenSurface[] mOffscreenTargets = new OffscreenSurface[FESHW.HW_RENDER_TARGETS];
        private RenderTexture mCurrentRenderTexture = null;

        private MeshStorage mMeshStorage;

        private Material mCurrentDrawMaterial;
        private Material mDrawMaterialIndexed;
        private Material mDrawMaterialRGB;

        private SpriteSheet[] mSpriteSheets = new SpriteSheet[FESHW.HW_MAX_SPRITESHEETS];
        private int mCurrentSpritesIndex = 0;
        private int mCurrentBatchSpriteIndex = -1;

        private int mPreviousDrawOffscreenIndex = -1;
        private Texture mPreviousTexture;

        private Texture2D mSystemTexture;
        private Color32[] mSystemTexturePixels;
        private Color32[] mSystemTextureSubPixels;
        private Rect mSystemTextureDirtyRegion = new Rect(0, 0, 0, 0);

        private FESShader[] mShaders = new FESShader[FESHW.HW_MAX_SHADERS];
        private int mCurrentShaderIndex = -1;

        private bool mSpritesTextureIsValid = false;

        private ClipRegion mClipRegion;
        private Rect2i mClip;
        private bool mClipDebug;
        private int mClipDebugColorIndex;
        private ColorRGBA mClipDebugColor;
        private List<DebugClipRegion> mDebugClipRegions = new List<DebugClipRegion>();

        private ArrayList mPendingTextureCopyBlocks = new ArrayList();

        private Vector2i mCameraPos;

        private FESAPI mFESAPI = null;

        private int[] mEllipseFillLookup = new int[FESHW.HW_MAX_DISPLAY_DIMENSION];

        private byte mAlpha = 255;
        private Color32 mCurrentColor = new Color32(255, 255, 255, 255);
        private int mCurrentPaletteSwapIndex;

        private bool mRenderEnabled = false;

        private Vector2i[] mPoints = new Vector2i[FESHW.HW_MAX_POLY_POINTS];
        private Vector3[] mPointsF = new Vector3[FESHW.HW_MAX_POLY_POINTS];

        private bool mShowFlushDebug = false;
        private int mFlushDebugFontColorIndex = 1;
        private int mFlushDebugBackgroundColorIndex = 0;
        private ColorRGBA mFlushDebugFontColor = ColorRGBA.white;
        private ColorRGBA mFlushDebugBackgroundColor = ColorRGBA.black;

        private FlushInfo[] mFlushInfo = new FlushInfo[]
        {
            new FlushInfo() { Reason = "Batch Full", Count = 0 },
            new FlushInfo() { Reason = "Spritesheet Change", Count = 0 },
            new FlushInfo() { Reason = "Tilemap Chunk", Count = 0 },
            new FlushInfo() { Reason = "Frame End", Count = 0 },
            new FlushInfo() { Reason = "Clip Change", Count = 0 },
            new FlushInfo() { Reason = "Offscreen Change", Count = 0 },
            new FlushInfo() { Reason = "Present/Effect Apply", Count = 0 },
            new FlushInfo() { Reason = "Shader Apply", Count = 0 },
            new FlushInfo() { Reason = "Shader Reset", Count = 0 },
            new FlushInfo() { Reason = "Set Material", Count = 0 },
            new FlushInfo() { Reason = "Set Texture", Count = 0 },
        };

        private enum FlushReason
        {
            BATCH_FULL,
            SPRITESHEET_CHANGE,
            TILEMAP_CHUNK,
            FRAME_END,
            CLIP_CHANGE,
            OFFSCREEN_CHANGE,
            EFFECT_APPLY,
            SHADER_APPLY,
            SHADER_RESET,
            SET_MATERIAL,
            SET_TEXTURE,
        }

        /// <summary>
        /// Get system texture
        /// </summary>
        public Texture2D SystemTexture
        {
            get { return mSystemTexture; }
        }

        /// <summary>
        /// Get sprite sheet texture
        /// </summary>
        public SpriteSheet CurrentSpriteSheet
        {
            get
            {
                return mSpriteSheets[mCurrentSpritesIndex];
            }
        }

        /// <summary>
        /// Get current spritesheet index
        /// </summary>
        public int CurrentSpriteSheetIndex
        {
            get
            {
                return mCurrentSpritesIndex;
            }
        }

        /// <summary>
        /// Get sprite sheet textures
        /// </summary>
        public SpriteSheet[] SpriteSheets
        {
            get
            {
                return mSpriteSheets;
            }
        }

        /// <summary>
        /// Get pixels of system texture
        /// </summary>
        public Color32[] SystemTexturePixels
        {
            get
            {
                return mSystemTexturePixels;
            }
        }

        /// <summary>
        /// Check if sprite sheet is valid
        /// </summary>
        public bool SpritesValid
        {
            get { return mSpritesTextureIsValid; }
        }

        /// <summary>
        /// Get/Set renderer enabled state
        /// </summary>
        public bool RenderEnabled
        {
            get { return mRenderEnabled; }
            set { mRenderEnabled = value; }
        }

        /// <summary>
        /// Initialize the subsystem
        /// </summary>
        /// <param name="api">Subsystem wrapper reference</param>
        /// <returns>True if successful</returns>
        public bool Initialize(FESAPI api)
        {
            if (api == null)
            {
                return false;
            }

            mFESAPI = api;

            if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
            {
                mDrawMaterialIndexed = mFESAPI.ResourceBucket.LoadMaterial("DrawMaterialIndexed");
                if (mDrawMaterialIndexed == null)
                {
                    return false;
                }
            }
            else
            {
                mDrawMaterialRGB = mFESAPI.ResourceBucket.LoadMaterial("DrawMaterialRGB");
                if (mDrawMaterialRGB == null)
                {
                    return false;
                }
            }

            mMeshStorage = new MeshStorage();

            if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
            {
                SetCurrentMaterial(mDrawMaterialIndexed);
            }
            else
            {
                SetCurrentMaterial(mDrawMaterialRGB);
            }

            if (!GenerateTexture())
            {
                return false;
            }

            if (mCurrentSpritesIndex != -1)
            {
                SetCurrentTexture(mSpriteSheets[mCurrentSpritesIndex].texture);
            }
            else
            {
                SetCurrentTexture(null);
            }

            return DisplayModeSet(FES.DisplaySize, mFESAPI.HW.PixelStyle);
        }

        /// <summary>
        /// Set display mode to given resolution and pixel style. Note that this sets only the FES pixel resolution, and does not affect the native
        /// window size. To change the native window size you can use the Unity Screen.SetResolution() API.
        /// </summary>
        /// <param name="resolution">Resolution</param>
        /// <param name="pixelStyle">Pixel style</param>
        /// <returns>True if mode was successfuly set, false otherwise</returns>
        public bool DisplayModeSet(Size2i resolution, FES.PixelStyle pixelStyle)
        {
            // Create main render target, and offscreen
            if (!mFrontBuffer.Resize(resolution, mFESAPI))
            {
                return false;
            }

            Onscreen();

            mFESAPI.HW.DisplaySize = resolution;
            mFESAPI.HW.PixelStyle = pixelStyle;

            return true;
        }

        /// <summary>
        /// Clear the display
        /// </summary>
        /// <param name="colorIndex">Color index</param>
        /// <param name="color">RGB color</param>
        public void Clear(int colorIndex, Color32 color)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            Color32 clearColor;
            if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
            {
                clearColor = mFESAPI.Palette.ColorAtIndex(colorIndex);
                clearColor.a = 255;
            }
            else
            {
                clearColor = color;
                clearColor.a = 255;
            }

            RenderTexture rt = UnityEngine.RenderTexture.active;
            UnityEngine.RenderTexture.active = mCurrentRenderTexture;
            GL.Clear(true, true, clearColor);
            UnityEngine.RenderTexture.active = rt;

            // Drop whatever we may have been rendering before
            ResetMesh();
        }

        /// <summary>
        /// Clear the offscreen surface
        /// </summary>
        /// <param name="offscreenIndex">Offscreen to clear</param>
        public void OffscreenClear(int offscreenIndex)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            Color32 clearColor = new Color32(0, 0, 0, 0);

            RenderTexture rt = UnityEngine.RenderTexture.active;
            UnityEngine.RenderTexture.active = mOffscreenTargets[offscreenIndex].tex;
            GL.Clear(true, true, clearColor);
            UnityEngine.RenderTexture.active = rt;

            // Drop whatever we may have been rendering on offscreen surface before
            if (mCurrentRenderTexture == mOffscreenTargets[offscreenIndex].tex)
            {
                ResetMesh();
            }
        }

        /// <summary>
        /// Draw a texture at given position, rotation
        /// </summary>
        /// <param name="srcRect">Source rectangle</param>
        /// <param name="destRect">Destination rectangle</param>
        /// <param name="pivot">Rotation pivot point</param>
        /// <param name="rotation">Rotation in degrees</param>
        /// <param name="flags">Flags</param>
        /// <param name="fromSystemTexture">Is this from system texture, or sprite sheet?</param>
        /// <param name="offscreenIndex">Offscreen index</param>
        public void DrawTexture(Rect2i srcRect, Rect2i destRect, Vector2i pivot, float rotation, int flags, bool fromSystemTexture, int offscreenIndex = -1)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            RenderTexture offscreenTexture = null;
            if (offscreenIndex >= 0)
            {
                offscreenTexture = mOffscreenTargets[offscreenIndex].tex;
            }

            CheckFlush(6);

            int swapIndex = mCurrentPaletteSwapIndex;

            int sx0 = srcRect.x;
            int sy0 = srcRect.y;
            int sx1 = sx0 + srcRect.width;
            int sy1 = sy0 + srcRect.height;

            int dx0 = 0;
            int dy0 = 0;
            int dx1;
            int dy1;

            if ((flags & FES.ROT_90_CW) == 0)
            {
                dx1 = dx0 + destRect.width;
                dy1 = dy0 + destRect.height;
            }
            else
            {
                dx1 = dx0 + destRect.height;
                dy1 = dy0 + destRect.width;
            }

            // Wrap the angle first to values between 0 and 360
            if (rotation != 0)
            {
                rotation = FESUtil.WrapAngle(rotation);
            }

            // First row is just color indecies, for solid shape rendering, skip it
            swapIndex++;

            float ux0, uy0, ux1, uy1;
            float ux0raw, uy0raw, ux1raw, uy1raw;

            if (fromSystemTexture)
            {
                ux0raw = sx0 / (float)FESHW.HW_SYSTEM_TEXTURE_WIDTH;
                uy0raw = 1.0f - (sy0 / ((float)FESHW.HW_SYSTEM_TEXTURE_HEIGHT));
                ux1raw = sx1 / (float)FESHW.HW_SYSTEM_TEXTURE_WIDTH;
                uy1raw = 1.0f - (sy1 / ((float)FESHW.HW_SYSTEM_TEXTURE_HEIGHT));
            }
            else if (offscreenTexture != null)
            {
                ux0raw = sx0 / (float)offscreenTexture.width;
                uy0raw = 1.0f - (sy0 / ((float)offscreenTexture.height));
                ux1raw = sx1 / (float)offscreenTexture.width;
                uy1raw = 1.0f - (sy1 / ((float)offscreenTexture.height));
            }
            else
            {
                ux0raw = sx0 / (float)mFESAPI.Renderer.CurrentSpriteSheet.textureSize.width;
                uy0raw = 1.0f - (sy0 / ((float)mFESAPI.Renderer.CurrentSpriteSheet.textureSize.height));
                ux1raw = sx1 / (float)mFESAPI.Renderer.CurrentSpriteSheet.textureSize.width;
                uy1raw = 1.0f - (sy1 / ((float)mFESAPI.Renderer.CurrentSpriteSheet.textureSize.height));
            }

            if ((flags & FES.FLIP_H) == 0)
            {
                ux0 = ux0raw;
                ux1 = ux1raw;
            }
            else
            {
                ux0 = ux1raw;
                ux1 = ux0raw;
            }

            if ((flags & FES.FLIP_V) == 0)
            {
                uy0 = uy0raw;
                uy1 = uy1raw;
            }
            else
            {
                uy0 = uy1raw;
                uy1 = uy0raw;
            }

            Color32 color = mCurrentColor;
            Vector2 vertFlags;

            if (fromSystemTexture)
            {
                vertFlags = new Vector2(0, 0);
            }
            else if (offscreenTexture != null)
            {
                vertFlags = new Vector2(1, 1);
            }
            else
            {
                vertFlags = new Vector2(1, 0);
            }

            Vector3 p1, p2, p3, p4;

            if ((flags & FES.ROT_90_CW) == 0)
            {
                p1 = new Vector3(dx0, dy0, 0);
                p2 = new Vector3(dx1, dy0, 0);
                p3 = new Vector3(dx1, dy1, 0);
                p4 = new Vector3(dx0, dy1, 0);
            }
            else
            {
                p1 = new Vector3(dx1, dy0, 0);
                p2 = new Vector3(dx1, dy1, 0);
                p3 = new Vector3(dx0, dy1, 0);
                p4 = new Vector3(dx0, dy0, 0);
            }

            if (rotation != 0)
            {
                Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation)) * Matrix4x4.Translate(new Vector3(-pivot.x, -pivot.y, 0));

                p1 = matrix.MultiplyPoint3x4(p1);
                p2 = matrix.MultiplyPoint3x4(p2);
                p3 = matrix.MultiplyPoint3x4(p3);
                p4 = matrix.MultiplyPoint3x4(p4);

                p1.x += pivot.x;
                p1.y += pivot.y;

                p2.x += pivot.x;
                p2.y += pivot.y;

                p3.x += pivot.x;
                p3.y += pivot.y;

                p4.x += pivot.x;
                p4.y += pivot.y;
            }

            p1.x -= mCameraPos.x - destRect.x;
            p1.y -= mCameraPos.y - destRect.y;

            p2.x -= mCameraPos.x - destRect.x;
            p2.y -= mCameraPos.y - destRect.y;

            p3.x -= mCameraPos.x - destRect.x;
            p3.y -= mCameraPos.y - destRect.y;

            p4.x -= mCameraPos.x - destRect.x;
            p4.y -= mCameraPos.y - destRect.y;

            // Early clip test
            if (p1.x < mClipRegion.x0 && p2.x < mClipRegion.x0 && p3.x < mClipRegion.x0 && p4.x < mClipRegion.x0)
            {
                return;
            }
            else if (p1.x > mClipRegion.x1 && p2.x > mClipRegion.x1 && p3.x > mClipRegion.x1 && p4.x > mClipRegion.x1)
            {
                return;
            }
            else if (p1.y < mClipRegion.y0 && p2.y < mClipRegion.y0 && p3.y < mClipRegion.y0 && p4.y < mClipRegion.y0)
            {
                // Note that Y axis is inverted by this point, have to invert it back before checking against clip
                return;
            }
            else if (p1.y > mClipRegion.y1 && p2.y > mClipRegion.y1 && p3.y > mClipRegion.y1 && p4.y > mClipRegion.y1)
            {
                return;
            }

            if (offscreenTexture != null)
            {
                SetCurrentTexture(offscreenTexture);
            }
            else
            {
                if (mCurrentSpritesIndex != -1)
                {
                    SetCurrentTexture(mSpriteSheets[mCurrentSpritesIndex].texture);
                }
                else
                {
                    SetCurrentTexture(null);
                }
            }

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            p1.z = p2.z = p3.z = p4.z = swapIndex;

            mMeshStorage.Verticies[i] = p1;
            mMeshStorage.Uvs[i].x = ux0;
            mMeshStorage.Uvs[i].y = uy0;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            i++;

            mMeshStorage.Verticies[i] = p2;
            mMeshStorage.Uvs[i].x = ux1;
            mMeshStorage.Uvs[i].y = uy0;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            i++;

            mMeshStorage.Verticies[i] = p3;
            mMeshStorage.Uvs[i].x = ux1;
            mMeshStorage.Uvs[i].y = uy1;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            i++;

            mMeshStorage.Verticies[i] = p4;
            mMeshStorage.Uvs[i].x = ux0;
            mMeshStorage.Uvs[i].y = uy1;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            mMeshStorage.Indecies[j++] = i - 3;
            mMeshStorage.Indecies[j++] = i - 2;
            mMeshStorage.Indecies[j++] = i - 1;

            mMeshStorage.Indecies[j++] = i - 1;
            mMeshStorage.Indecies[j++] = i - 0;
            mMeshStorage.Indecies[j++] = i - 3;

            i++;

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;

            mPreviousDrawOffscreenIndex = offscreenIndex;
        }

        /// <summary>
        /// Draw nine-slice sprite.
        /// </summary>
        /// <param name="destRect">Destination rectangle</param>
        /// <param name="srcTopLeftCorner">Source rectangle of the top left corner</param>
        /// <param name="flagsTopLeftCorner">Render flags for top left corner</param>
        /// <param name="srcTopSide">Source rectangle of the top side</param>
        /// <param name="flagsTopSide">Render flags for top side</param>
        /// <param name="srcTopRightCorner">Source rectangle of the top right corner</param>
        /// <param name="flagsTopRightCorner">Render flaps for top right corner</param>
        /// <param name="srcLeftSide">Source rectangle of the left side</param>
        /// <param name="flagsLeftSide">Render flags for left side</param>
        /// <param name="srcMiddle">Source rectangle of the middle</param>
        /// <param name="srcRightSide">Render flags for right side</param>
        /// <param name="flagsRightSide">Source rectangle of the right side</param>
        /// <param name="srcBottomLeftCorner">Render flags for bottom left corner</param>
        /// <param name="flagsBottomLeftCorner">Source rectangle of the bottom left corner</param>
        /// <param name="srcBottomSide">Render flags for bottom side</param>
        /// <param name="flagsBottomSide">Source rectangle of the bottom side</param>
        /// <param name="srcBottomRightCorner">Render flags for bottom right corner</param>
        /// <param name="flagsBottomRightCorner">Source rectangle of the bottom right corner</param>
        public void DrawNineSlice(
            Rect2i destRect,
            Rect2i srcTopLeftCorner,
            int flagsTopLeftCorner,
            Rect2i srcTopSide,
            int flagsTopSide,
            Rect2i srcTopRightCorner,
            int flagsTopRightCorner,
            Rect2i srcLeftSide,
            int flagsLeftSide,
            Rect2i srcMiddle,
            Rect2i srcRightSide,
            int flagsRightSide,
            Rect2i srcBottomLeftCorner,
            int flagsBottomLeftCorner,
            Rect2i srcBottomSide,
            int flagsBottomSide,
            Rect2i srcBottomRightCorner,
            int flagsBottomRightCorner)
        {
            if (destRect.width < srcTopLeftCorner.width + srcBottomRightCorner.width ||
                destRect.height < srcTopLeftCorner.height + srcBottomRightCorner.height)
            {
                return;
            }

            int bottomOffset = destRect.height - srcBottomLeftCorner.height;
            int rightOffset = destRect.width - srcTopRightCorner.width;

            int xOffset = srcTopLeftCorner.width;
            while (xOffset < rightOffset && srcTopSide.width > 0)
            {
                int remainingWidth = rightOffset - xOffset;
                int width = Mathf.Min(remainingWidth, srcTopSide.width);

                // Top & Bottom horizontal
                DrawTexture(new Rect2i(srcTopSide.x, srcTopSide.y, width, srcTopSide.height), new Rect2i(destRect.x + xOffset, destRect.y, width, srcTopSide.height), Vector2i.zero, 0, flagsTopSide, false);
                DrawTexture(new Rect2i(srcBottomSide.x, srcBottomSide.y, width, srcBottomSide.height), new Rect2i(destRect.x + xOffset, destRect.y + bottomOffset, width, srcBottomSide.height), Vector2i.zero, 0, flagsBottomSide, false);

                xOffset += srcTopSide.width;
            }

            int yOffset = srcTopLeftCorner.height;
            while (yOffset < bottomOffset && srcLeftSide.height > 0)
            {
                int remainingHeight = bottomOffset - yOffset;
                int height = Mathf.Min(remainingHeight, srcLeftSide.height);

                // Left & Right verticals
                if ((flagsLeftSide & FES.ROT_90_CW) != 0)
                {
                    DrawTexture(new Rect2i(srcLeftSide.x, srcLeftSide.y, height, srcLeftSide.height), new Rect2i(destRect.x, destRect.y + yOffset, height, srcLeftSide.height), Vector2i.zero, 0, flagsLeftSide, false);
                    DrawTexture(new Rect2i(srcRightSide.x, srcRightSide.y, height, srcRightSide.height), new Rect2i(destRect.x + rightOffset, destRect.y + yOffset, height, srcRightSide.height), Vector2i.zero, 0, flagsRightSide, false);
                }
                else
                {
                    DrawTexture(new Rect2i(srcLeftSide.x, srcLeftSide.y, srcLeftSide.width, height), new Rect2i(destRect.x, destRect.y + yOffset, srcLeftSide.width, height), Vector2i.zero, 0, flagsLeftSide, false);
                    DrawTexture(new Rect2i(srcRightSide.x, srcRightSide.y, srcRightSide.width, height), new Rect2i(destRect.x + rightOffset, destRect.y + yOffset, srcRightSide.width, height), Vector2i.zero, 0, flagsRightSide, false);
                }

                yOffset += srcLeftSide.height;
            }

            yOffset = srcTopLeftCorner.height;
            while (yOffset < bottomOffset && srcMiddle.height > 0)
            {
                int remainingHeight = bottomOffset - yOffset;
                int height = Mathf.Min(remainingHeight, srcMiddle.height);

                xOffset = srcTopLeftCorner.width;
                while (xOffset < rightOffset)
                {
                    int remainingWidth = rightOffset - xOffset;
                    int width = Mathf.Min(remainingWidth, srcMiddle.width);

                    // Center
                    DrawTexture(new Rect2i(srcMiddle.x, srcMiddle.y, width, height), new Rect2i(destRect.x + xOffset, destRect.y + yOffset, width, height), Vector2i.zero, 0, 0, false);

                    xOffset += srcMiddle.width;
                }

                yOffset += srcMiddle.height;
            }

            /* Top left corner */
            DrawTexture(srcTopLeftCorner, new Rect2i(destRect.x, destRect.y, srcTopLeftCorner.width, srcTopLeftCorner.height), Vector2i.zero, 0, flagsTopLeftCorner, false);

            /* Bottom left corner */
            DrawTexture(srcBottomLeftCorner, new Rect2i(destRect.x, destRect.y + bottomOffset, srcBottomLeftCorner.width, srcBottomLeftCorner.height), Vector2i.zero, 0, flagsBottomLeftCorner, false);

            /* Top right corner */
            DrawTexture(srcTopRightCorner, new Rect2i(destRect.x + rightOffset, destRect.y, srcTopRightCorner.width, srcTopRightCorner.height), Vector2i.zero, 0, flagsTopRightCorner, false);

            /* Bottom right corner */
            DrawTexture(srcBottomRightCorner, new Rect2i(destRect.x + rightOffset, destRect.y + bottomOffset, srcBottomRightCorner.width, srcBottomRightCorner.height), Vector2i.zero, 0, flagsBottomRightCorner, false);
        }

        /// <summary>
        /// Draw a single pixel
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="colorIndex">color index</param>
        /// <param name="color">RGB color</param>
        /// <param name="cameraApply">Apply camera offset</param>
        public void DrawPixel(int x, int y, int colorIndex, Color32 color, bool cameraApply = true)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            CheckFlush(3);

            if (cameraApply)
            {
                x -= mCameraPos.x;
                y -= mCameraPos.y;
            }

            if (x < mClipRegion.x0 || x > mClipRegion.x1 || y < mClipRegion.y0 || y > mClipRegion.y1)
            {
                return;
            }

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
            {
                color = mCurrentColor;
            }
            else
            {
                if (mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
                {
                    color = (Color)color * (Color)mCurrentColor;
                }
            }

            int swapIndex = mCurrentPaletteSwapIndex;

            // First row is just color indecies, for solid shape rendering, skip it
            swapIndex++;

            Vector2 vertFlags = new Vector2(0, 0);

            // Draw pixel with just one triangle, make sure it passes through the middle of the pixel,
            // by extending its sides a bit. This should gurantee that it gets rasterized
            mMeshStorage.Verticies[i].x = x;
            mMeshStorage.Verticies[i].y = y;
            mMeshStorage.Verticies[i].z = swapIndex;
            mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * colorIndex) + 0.0001f;
            mMeshStorage.Uvs[i].y = 0.999999f;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            i++;

            mMeshStorage.Verticies[i].x = x + 1.2f;
            mMeshStorage.Verticies[i].y = y;
            mMeshStorage.Verticies[i].z = swapIndex;
            mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * (colorIndex + 1)) - 0.0001f;
            mMeshStorage.Uvs[i].y = 0.999999f;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            i++;

            mMeshStorage.Verticies[i].x = x;
            mMeshStorage.Verticies[i].y = y + 1.2f;
            mMeshStorage.Verticies[i].z = swapIndex;
            mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * (colorIndex + 1)) - 0.0001f;
            mMeshStorage.Uvs[i].y = 1.0f;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            i++;

            mMeshStorage.Indecies[j++] = i - 3;
            mMeshStorage.Indecies[j++] = i - 2;
            mMeshStorage.Indecies[j++] = i - 1;

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        /// <summary>
        /// Draw a rectangle outline
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="colorIndex">Color index</param>
        /// <param name="color">RGB color</param>
        /// <param name="pivot">Rotation pivot point</param>
        /// <param name="rotation">Rotation in degrees</param>
        public void DrawRect(Rect2i rect, int colorIndex, Color32 color, Vector2i pivot, float rotation = 0)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            int x = rect.x;
            int y = rect.y;
            int w = rect.width;
            int h = rect.height;

            if (w < 0 || h < 0)
            {
                return;
            }

            if (w <= 2 || h <= 2)
            {
                DrawRectFill(rect, colorIndex, color, Vector2i.zero);
            }
            else
            {
                // Wrap the angle first to values between 0 and 360
                if (rotation != 0)
                {
                    rotation = FESUtil.WrapAngle(rotation);
                }

                // Simple case, use rect fill to draw straight lines
                if (rotation == 0)
                {
                    DrawRectFill(new Rect2i(x, y, w, 1), colorIndex, color, Vector2i.zero);
                    DrawRectFill(new Rect2i(x, y + 1, 1, h - 2), colorIndex, color, Vector2i.zero);
                    DrawRectFill(new Rect2i(x + w - 1, y + 1, 1, h - 2), colorIndex, color, Vector2i.zero);
                    DrawRectFill(new Rect2i(x, y + h - 1, w, 1), colorIndex, color, Vector2i.zero);
                }
                else
                {
                    Vector3 p1, p2, p3, p4;

                    p1 = new Vector3(0, 0, 0);
                    p2 = new Vector3(w, 0, 0);
                    p3 = new Vector3(w, -h, 0);
                    p4 = new Vector3(0, -h, 0);

                    Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -rotation)) * Matrix4x4.Translate(new Vector3(-pivot.x, pivot.y, 0));

                    p1 = matrix.MultiplyPoint3x4(p1);
                    p2 = matrix.MultiplyPoint3x4(p2);
                    p3 = matrix.MultiplyPoint3x4(p3);
                    p4 = matrix.MultiplyPoint3x4(p4);

                    p1.x += pivot.x + x;
                    p1.y -= pivot.y + y;

                    p2.x += pivot.x + x;
                    p2.y -= pivot.y + y;

                    p3.x += pivot.x + x;
                    p3.y -= pivot.y + y;

                    p4.x += pivot.x + x;
                    p4.y -= pivot.y + y;

                    p1.y = -p1.y;
                    p2.y = -p2.y;
                    p3.y = -p3.y;
                    p4.y = -p4.y;

                    DrawLine(p1, p2, colorIndex, color, Vector2i.zero, 0);
                    DrawLine(p2, p3, colorIndex, color, Vector2i.zero, 0);
                    DrawLine(p3, p4, colorIndex, color, Vector2i.zero, 0);
                    DrawLine(p4, p1, colorIndex, color, Vector2i.zero, 0);
                }
            }
        }

        /// <summary>
        /// Draw a filled rectangle
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="colorIndex">Color index</param>
        /// <param name="color">RGB color</param>
        /// <param name="pivot">Rotation pivot point</param>
        /// <param name="rotation">Rotation in degrees</param>
        public void DrawRectFill(Rect2i rect, int colorIndex, Color32 color, Vector2i pivot, float rotation = 0)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            if (rect.width <= 0 || rect.height <= 0)
            {
                return;
            }

            // If width or height is 1 then we're better off drawing ortho line because its made of just 1 triangle
            if ((rect.width == 1 || rect.height == 1) && rotation == 0)
            {
                DrawOrthoLine(new Vector2i(rect.x, rect.y), new Vector2i(rect.x + rect.width - 1, rect.y + rect.height - 1), colorIndex, color, true);
                return;
            }

            CheckFlush(6);

            int swapIndex = mCurrentPaletteSwapIndex;

            int x = rect.x;
            int y = rect.y;
            int w = rect.width;
            int h = rect.height;

            int dx0 = 0;
            int dy0 = 0;
            int dx1 = dx0 + w;
            int dy1 = dy0 + h;

            // Wrap the angle first to values between 0 and 360
            if (rotation != 0)
            {
                rotation = FESUtil.WrapAngle(rotation);
            }

            // First row is just color indecies, for solid shape rendering, skip it
            swapIndex++;

            if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
            {
                color = mCurrentColor;
            }
            else
            {
                if (mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
                {
                    color = (Color)color * (Color)mCurrentColor;
                }
            }

            Vector2 vertFlags = new Vector2(0, 0);

            Vector3 p1, p2, p3, p4;

            p1 = new Vector3(dx0, dy0, 0);
            p2 = new Vector3(dx1, dy0, 0);
            p3 = new Vector3(dx1, dy1, 0);
            p4 = new Vector3(dx0, dy1, 0);

            if (rotation != 0)
            {
                Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation)) * Matrix4x4.Translate(new Vector3(-pivot.x, -pivot.y, 0));

                p1 = matrix.MultiplyPoint3x4(p1);
                p2 = matrix.MultiplyPoint3x4(p2);
                p3 = matrix.MultiplyPoint3x4(p3);
                p4 = matrix.MultiplyPoint3x4(p4);

                p1.x += pivot.x;
                p1.y += pivot.y;

                p2.x += pivot.x;
                p2.y += pivot.y;

                p3.x += pivot.x;
                p3.y += pivot.y;

                p4.x += pivot.x;
                p4.y += pivot.y;
            }

            p1.x -= mCameraPos.x - x;
            p1.y -= mCameraPos.y - y;

            p2.x -= mCameraPos.x - x;
            p2.y -= mCameraPos.y - y;

            p3.x -= mCameraPos.x - x;
            p3.y -= mCameraPos.y - y;

            p4.x -= mCameraPos.x - x;
            p4.y -= mCameraPos.y - y;

            // Early clip test
            if (p1.x < mClipRegion.x0 && p2.x < mClipRegion.x0 && p3.x < mClipRegion.x0 && p4.x < mClipRegion.x0)
            {
                return;
            }
            else if (p1.x > mClipRegion.x1 && p2.x > mClipRegion.x1 && p3.x > mClipRegion.x1 && p4.x > mClipRegion.x1)
            {
                return;
            }
            else if (p1.y < mClipRegion.y0 && p2.y < mClipRegion.y0 && p3.y < mClipRegion.y0 && p4.y < mClipRegion.y0)
            {
                // Note that Y axis is inverted by this point, have to invert it back before checking against clip
                return;
            }
            else if (p1.y > mClipRegion.y1 && p2.y > mClipRegion.y1 && p3.y > mClipRegion.y1 && p4.y > mClipRegion.y1)
            {
                return;
            }

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            p1.z = p2.z = p3.z = p4.z = swapIndex;

            mMeshStorage.Verticies[i] = p1;
            mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * colorIndex) + 0.0001f;
            mMeshStorage.Uvs[i].y = 0.999999f;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            i++;

            mMeshStorage.Verticies[i] = p2;
            mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * (colorIndex + 1)) - 0.0001f;
            mMeshStorage.Uvs[i].y = 0.999999f;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            i++;

            mMeshStorage.Verticies[i] = p3;
            mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * (colorIndex + 1)) - 0.0001f;
            mMeshStorage.Uvs[i].y = 1.0f;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            i++;

            mMeshStorage.Verticies[i] = p4;
            mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * colorIndex) + 0.0001f;
            mMeshStorage.Uvs[i].y = 1.0f;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            mMeshStorage.Indecies[j++] = i - 3;
            mMeshStorage.Indecies[j++] = i - 2;
            mMeshStorage.Indecies[j++] = i - 1;

            mMeshStorage.Indecies[j++] = i - 1;
            mMeshStorage.Indecies[j++] = i - 0;
            mMeshStorage.Indecies[j++] = i - 3;

            i++;

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        /// <summary>
        /// Draw a straight orthogonal line
        /// </summary>
        /// <param name="p0">Start point</param>
        /// <param name="p1">End point</param>
        /// <param name="colorIndex">Color index</param>
        /// <param name="color">RGB color</param>
        /// <param name="cameraApply">True if camera offset should be applied</param>
        public void DrawOrthoLine(Vector2i p0, Vector2i p1, int colorIndex, Color32 color, bool cameraApply)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            CheckFlush(3);

            // Make sure p0 is before p1
            if (p0.x > p1.x || p0.y > p1.y)
            {
                Vector2i tp = p0;
                p0 = p1;
                p1 = tp;
            }

            bool horizontal = false;
            if (p0.y == p1.y)
            {
                horizontal = true;
            }

            if (cameraApply)
            {
                p0 -= mCameraPos;
                p1 -= mCameraPos;
            }

            if ((p0.x < mClipRegion.x0 && p1.x < mClipRegion.x0) || (p0.x > mClipRegion.x1 && p1.x > mClipRegion.x1))
            {
                return;
            }

            if ((p0.y < mClipRegion.y0 && p1.y < mClipRegion.y0) || (p0.y > mClipRegion.y1 && p1.y > mClipRegion.y1))
            {
                return;
            }

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
            {
                color = mCurrentColor;
            }
            else
            {
                if (mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
                {
                    color = (Color)color * (Color)mCurrentColor;
                }
            }

            int swapIndex = mCurrentPaletteSwapIndex;

            // First row is just color indecies, for solid shape rendering, skip it
            swapIndex++;

            Vector2 vertFlags = new Vector2(0, 0);

            // Draw the line just one triangle, make sure it passes through the middle of the pixel
            if (horizontal)
            {
                mMeshStorage.Verticies[i].x = p0.x - 0.1f;
                mMeshStorage.Verticies[i].y = p0.y - 0.1f;
                mMeshStorage.Verticies[i].z = swapIndex;
                mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * colorIndex) + 0.0001f;
                mMeshStorage.Uvs[i].y = 0.999999f;
                mMeshStorage.Flags[i] = vertFlags;
                mMeshStorage.Colors[i] = color;

                i++;

                mMeshStorage.Verticies[i].x = p1.x + 1.1f;
                mMeshStorage.Verticies[i].y = p1.y + 0.5f;
                mMeshStorage.Verticies[i].z = swapIndex;
                mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * (colorIndex + 1)) - 0.0001f;
                mMeshStorage.Uvs[i].y = 0.999999f;
                mMeshStorage.Flags[i] = vertFlags;
                mMeshStorage.Colors[i] = color;

                i++;

                mMeshStorage.Verticies[i].x = p0.x - 0.1f;
                mMeshStorage.Verticies[i].y = p0.y + 1.1f;
                mMeshStorage.Verticies[i].z = swapIndex;
                mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * (colorIndex + 1)) - 0.0001f;
                mMeshStorage.Uvs[i].y = 1.0f;
                mMeshStorage.Flags[i] = vertFlags;
                mMeshStorage.Colors[i] = color;

                i++;
            }
            else
            {
                mMeshStorage.Verticies[i].x = p0.x - 0.1f;
                mMeshStorage.Verticies[i].y = p0.y - 0.1f;
                mMeshStorage.Verticies[i].z = swapIndex;
                mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * colorIndex) + 0.0001f;
                mMeshStorage.Uvs[i].y = 0.999999f;
                mMeshStorage.Flags[i] = vertFlags;
                mMeshStorage.Colors[i] = color;

                i++;

                mMeshStorage.Verticies[i].x = p0.x + 1.1f;
                mMeshStorage.Verticies[i].y = p0.y - 0.1f;
                mMeshStorage.Verticies[i].z = swapIndex;
                mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * (colorIndex + 1)) - 0.0001f;
                mMeshStorage.Uvs[i].y = 1.0f;
                mMeshStorage.Flags[i] = vertFlags;
                mMeshStorage.Colors[i] = color;

                i++;

                mMeshStorage.Verticies[i].x = p1.x + 0.5f;
                mMeshStorage.Verticies[i].y = p1.y + 1.1f;
                mMeshStorage.Verticies[i].z = swapIndex;
                mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * (colorIndex + 1)) - 0.0001f;
                mMeshStorage.Uvs[i].y = 0.999999f;
                mMeshStorage.Flags[i] = vertFlags;
                mMeshStorage.Colors[i] = color;

                i++;
            }

            mMeshStorage.Indecies[j++] = i - 3;
            mMeshStorage.Indecies[j++] = i - 2;
            mMeshStorage.Indecies[j++] = i - 1;

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        /// <summary>
        /// Draw line from lp0 to lp1
        /// </summary>
        /// <param name="lp0">Start point</param>
        /// <param name="lp1">End point</param>
        /// <param name="colorIndex">Color index</param>
        /// <param name="color">RGB color</param>
        /// <param name="pivot">Rotation pivot point</param>
        /// <param name="rotation">Rotation in degrees</param>
        /// <param name="cameraApply">Apply camera offset?</param>
        /// <param name="startPixel">Render the first pixel in the line</param>
        /// <param name="endPixel">Render the last pixel in the line</param>
        public void DrawLine(Vector2i lp0, Vector2i lp1, int colorIndex, Color32 color, Vector2i pivot, float rotation = 0, bool cameraApply = true, bool startPixel = true, bool endPixel = true)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            CheckFlush(6);

            int swapIndex = mCurrentPaletteSwapIndex;

            // First row is just color indecies, for solid shape rendering, skip it
            swapIndex++;

            if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
            {
                color = mCurrentColor;
            }
            else
            {
                if (mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
                {
                    color = (Color)color * (Color)mCurrentColor;
                }
            }

            // Wrap the angle first to values between 0 and 360
            if (rotation != 0)
            {
                rotation = FESUtil.WrapAngle(rotation);
            }

            // Trivial straight lines, use rect, faster
            if (rotation == 0)
            {
                if (lp0.x == lp1.x || lp0.y == lp1.y)
                {
                    DrawOrthoLine(lp0, lp1, colorIndex, color, cameraApply);
                    return;
                }
            }

            Vector3 lp0f = lp0.ToVector2();
            Vector3 lp1f = lp1.ToVector2();

            if (rotation != 0)
            {
                Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation)) * Matrix4x4.Translate(new Vector3(-pivot.x, -pivot.y, 0));

                lp0f = matrix.MultiplyPoint3x4(lp0f);
                lp1f = matrix.MultiplyPoint3x4(lp1f);

                lp0f.x += pivot.x;
                lp0f.y += pivot.y;

                lp1f.x += pivot.x;
                lp1f.y += pivot.y;
            }

            lp0f.x -= mCameraPos.x;
            lp0f.y -= mCameraPos.y;

            lp1f.x -= mCameraPos.x;
            lp1f.y -= mCameraPos.y;

            // Early clip test
            if (lp0f.x < mClipRegion.x0 && lp1f.x < mClipRegion.x0)
            {
                return;
            }
            else if (lp0f.x > mClipRegion.x1 && lp1f.x > mClipRegion.x1)
            {
                return;
            }
            else if (lp0f.y < mClipRegion.y0 && lp1f.y < mClipRegion.y0)
            {
                return;
            }
            else if (lp0f.y > mClipRegion.y1 && lp1f.y > mClipRegion.y1)
            {
                return;
            }

            float angle = 0;
            int quadrant = 0;

            Vector2 dir = lp1f - lp0f;

            angle = Vector2.Angle(new Vector2(0, 1), new Vector2(dir.x, -dir.y));

            /* Figure out which quadrant the angle is in
             *
             * \      0     /
             *   \        /
             *     \    /
             *       \/
             * 3     /\     1
             *     /    \
             *   /        \
             * /      2     \
             */

            if (dir.x > 0)
            {
                if (angle <= 45)
                {
                    quadrant = 0;
                }
                else if (angle <= 135)
                {
                    quadrant = 1;
                }
                else
                {
                    quadrant = 2;
                }
            }
            else
            {
                if (angle <= 45)
                {
                    quadrant = 0;
                }
                else if (angle <= 135)
                {
                    quadrant = 3;
                }
                else
                {
                    quadrant = 2;
                }
            }

            Vector3 p0, p1, p2, p3;

            if (quadrant == 0)
            {
                p0 = lp0f;
                p1 = lp1f;

                p0.y += 0.5f;
                p1.y += 0.5f;
                p2 = new Vector2(p1.x + 1.0f, p1.y);
                p3 = new Vector2(p0.x + 1.0f, p0.y);

                var sideDir = p1 - p0;
                sideDir.Normalize();

                if (startPixel)
                {
                    p0 += sideDir * -0.5f;
                    p3 += sideDir * -0.5f;
                }
                else
                {
                    p0 += sideDir * 0.5f;
                    p3 += sideDir * 0.5f;
                }

                if (endPixel)
                {
                    p1 += sideDir * 0.5f;
                    p2 += sideDir * 0.5f;
                }
                else
                {
                    p1 += sideDir * -0.5f;
                    p2 += sideDir * -0.5f;
                }
            }
            else if (quadrant == 2)
            {
                p1 = lp0f;
                p0 = lp1f;

                p0.y += 0.5f;
                p1.y += 0.5f;
                p2 = new Vector2(p1.x + 1.0f, p1.y);
                p3 = new Vector2(p0.x + 1.0f, p0.y);

                var sideDir = p1 - p0;
                sideDir.Normalize();

                if (startPixel)
                {
                    p1 += sideDir * 0.5f;
                    p2 += sideDir * 0.5f;
                }
                else
                {
                    p1 += sideDir * -0.5f;
                    p2 += sideDir * -0.5f;
                }

                if (endPixel)
                {
                    p0 += sideDir * -0.5f;
                    p3 += sideDir * -0.5f;
                }
                else
                {
                    p0 += sideDir * 0.5f;
                    p3 += sideDir * 0.5f;
                }
            }
            else if (quadrant == 1)
            {
                p0 = lp0f;
                p1 = lp1f;

                p0.x += 0.5f;
                p1.x += 0.5f;
                p2 = new Vector2(p1.x, p1.y + 1.0f);
                p3 = new Vector2(p0.x, p0.y + 1.0f);

                var sideDir = p1 - p0;
                sideDir.Normalize();

                if (startPixel)
                {
                    p0 += sideDir * -0.5f;
                    p3 += sideDir * -0.5f;
                }
                else
                {
                    p0 += sideDir * 0.5f;
                    p3 += sideDir * 0.5f;
                }

                if (endPixel)
                {
                    p1 += sideDir * 0.5f;
                    p2 += sideDir * 0.5f;
                }
                else
                {
                    p1 += sideDir * -0.5f;
                    p2 += sideDir * -0.5f;
                }
            }
            else
            {
                p1 = lp0f;
                p0 = lp1f;

                p0.x += 0.5f;
                p1.x += 0.5f;
                p2 = new Vector2(p1.x, p1.y + 1.0f);
                p3 = new Vector2(p0.x, p0.y + 1.0f);

                var sideDir = p1 - p0;
                sideDir.Normalize();

                if (startPixel)
                {
                    p1 += sideDir * 0.5f;
                    p2 += sideDir * 0.5f;
                }
                else
                {
                    p1 += sideDir * -0.5f;
                    p2 += sideDir * -0.5f;
                }

                if (endPixel)
                {
                    p0 += sideDir * -0.5f;
                    p3 += sideDir * -0.5f;
                }
                else
                {
                    p0 += sideDir * 0.5f;
                    p3 += sideDir * 0.5f;
                }
            }

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            Vector2 vertFlags = new Vector2(0, 0);

            p0.z = p1.z = p2.z = p3.z = swapIndex;

            mMeshStorage.Verticies[i] = p0;
            mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * colorIndex) + 0.0001f;
            mMeshStorage.Uvs[i].y = 0.999999f;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            i++;

            mMeshStorage.Verticies[i] = p1;
            mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * (colorIndex + 1)) - 0.0001f;
            mMeshStorage.Uvs[i].y = 0.999999f;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            i++;

            mMeshStorage.Verticies[i] = p2;
            mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * (colorIndex + 1)) - 0.0001f;
            mMeshStorage.Uvs[i].y = 1.0f;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            i++;

            mMeshStorage.Verticies[i] = p3;
            mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * colorIndex) + 0.0001f;
            mMeshStorage.Uvs[i].y = 1.0f;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            mMeshStorage.Indecies[j++] = i - 3;
            mMeshStorage.Indecies[j++] = i - 2;
            mMeshStorage.Indecies[j++] = i - 1;

            mMeshStorage.Indecies[j++] = i - 1;
            mMeshStorage.Indecies[j++] = i - 0;
            mMeshStorage.Indecies[j++] = i - 3;

            i++;

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        /// <summary>
        /// Draw ellipse
        /// </summary>
        /// <param name="center">Center of ellipse</param>
        /// <param name="radius">Radius</param>
        /// <param name="colorIndex">Color index</param>
        /// <param name="color">RGB color</param>
        public void DrawEllipse(Vector2i center, Vector2i radius, int colorIndex, Color32 color)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            Rect2i userClipRegion = FES.ClipGet();
            Rect2i bounds = new Rect2i(center.x - radius.x - 1, center.y - radius.y - 1, (radius.x * 2) + 2, (radius.y * 2) + 2);
            var cameraPos = FES.CameraGet();
            bounds.x -= cameraPos.x;
            bounds.y -= cameraPos.y;
            if (!userClipRegion.Intersects(bounds))
            {
                return;
            }

            if (radius.x == 0 || radius.y == 0)
            {
                DrawEllipseSlow(center, radius, colorIndex, color);
                return;
            }

            if (radius.x < 12 && radius.y < 12)
            {
                DrawEllipseSlow(center, radius, colorIndex, color);
                return;
            }

            // Calculate points in line based on circumference, round it up to nearest 4
            int maxRadius = Mathf.Max(radius.x, radius.y);
            int count = (int)Mathf.Sqrt(maxRadius) * 4;
            if (count < 8)
            {
                count = 8;
            }

            if (count > FESHW.HW_MAX_POLY_POINTS)
            {
                count = FESHW.HW_MAX_POLY_POINTS;
            }

            for (int i = 0; i < count + 1; i++)
            {
                float t = (i / (float)count) * (Mathf.PI * 2);

                mPoints[i].x = Mathf.RoundToInt(center.x + (Mathf.Sin(t) * radius.x));
                mPoints[i].y = Mathf.RoundToInt(center.y + (Mathf.Cos(t) * radius.y));
            }

            count++;

            DrawLineStrip(mPoints, count, colorIndex, color, new Vector2i(0, 0), 0, true);
        }

        /// <summary>
        /// Draw filled ellipse
        /// </summary>
        /// <param name="center">Center of ellipse</param>
        /// <param name="radius">Radius</param>
        /// <param name="colorIndex">Color index</param>
        /// <param name="color">RGB color</param>
        /// <param name="inverse">Do an inverted fill?</param>
        public void DrawEllipseFill(Vector2i center, Vector2i radius, int colorIndex, Color32 color, bool inverse)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            Rect2i userClipRegion = FES.ClipGet();
            Rect2i bounds = new Rect2i(center.x - radius.x - 1, center.y - radius.y - 1, (radius.x * 2) + 2, (radius.y * 2) + 2);
            var cameraPos = FES.CameraGet();
            bounds.x -= cameraPos.x;
            bounds.y -= cameraPos.y;
            if (!userClipRegion.Intersects(bounds))
            {
                return;
            }

            if (radius.x == 0 || radius.y == 0)
            {
                DrawEllipseFillSlow(center, radius, colorIndex, color, inverse);
                return;
            }

            if (radius.x < 12 && radius.y < 12)
            {
                DrawEllipseFillSlow(center, radius, colorIndex, color, inverse);
                return;
            }

            center.x -= mCameraPos.x;
            center.y -= mCameraPos.y;

            // Calculate points in line based on circumference, round it up to nearest 4
            int maxRadius = Mathf.Max(radius.x, radius.y);
            int count = (int)Mathf.Sqrt(maxRadius) * 4;
            if (count < 8)
            {
                count = 8;
            }

            if (count > FESHW.HW_MAX_POLY_POINTS)
            {
                count = FESHW.HW_MAX_POLY_POINTS;
            }

            if (!inverse)
            {
                for (int i = 0; i < count + 1; i++)
                {
                    float t = (i / (float)count) * (Mathf.PI * 2);

                    mPointsF[i].x = center.x + 0.5f + (Mathf.Sin(t) * radius.x);
                    mPointsF[i].y = center.y + 0.5f + (Mathf.Cos(t) * radius.y);
                }

                count++;

                DrawConvexPolygon(center, mPointsF, count, colorIndex, color, new Vector2i(0, 0), 0, true);
            }
            else
            {
                // Top Right
                int j = count;
                for (int i = 0; i < count + 1; i++)
                {
                    float t = (j / (float)count) * (Mathf.PI / 2);
                    j--;

                    mPointsF[i].x = center.x + 0.5f + (Mathf.Sin(t) * radius.x);
                    mPointsF[i].y = center.y + 0.5f + (Mathf.Cos(t) * radius.y);
                }

                DrawConvexPolygon(new Vector2i(center.x + radius.x + 1, center.y + radius.y + 1), mPointsF, count + 1, colorIndex, color, new Vector2i(0, 0), 0, true);

                // Bottom Right
                j = 0;
                for (int i = 0; i < count + 1; i++)
                {
                    float t = (j / (float)count) * (Mathf.PI / 2);
                    j++;

                    mPointsF[i].x = center.x + 0.5f + (Mathf.Sin(t) * radius.x);
                    mPointsF[i].y = center.y + 0.5f + (Mathf.Cos(t) * -radius.y);
                }

                DrawConvexPolygon(new Vector2i(center.x + radius.x + 1, center.y - radius.y), mPointsF, count + 1, colorIndex, color, new Vector2i(0, 0), 0, true);

                // Bottom Left
                j = count;
                for (int i = 0; i < count + 1; i++)
                {
                    float t = (j / (float)count) * (Mathf.PI / 2);
                    j--;

                    mPointsF[i].x = center.x + 0.5f + (Mathf.Sin(t) * -radius.x);
                    mPointsF[i].y = center.y + 0.5f + (Mathf.Cos(t) * -radius.y);
                }

                DrawConvexPolygon(new Vector2i(center.x - radius.x, center.y - radius.y), mPointsF, count + 1, colorIndex, color, new Vector2i(0, 0), 0, true);

                // Top Right
                j = 0;
                for (int i = 0; i < count + 1; i++)
                {
                    float t = (j / (float)count) * (Mathf.PI / 2);
                    j++;

                    mPointsF[i].x = center.x + 0.5f + (Mathf.Sin(t) * -radius.x);
                    mPointsF[i].y = center.y + 0.5f + (Mathf.Cos(t) * radius.y);
                }

                DrawConvexPolygon(new Vector2i(center.x - radius.x, center.y + radius.y + 1), mPointsF, count + 1, colorIndex, color, new Vector2i(0, 0), 0, true);
            }
        }

        /// <summary>
        /// Draw a prepared mesh to screen
        /// </summary>
        /// <param name="mesh">Mesh to draw</param>
        /// <param name="rect">Rect to check against clip region</param>
        /// <param name="translateToCamera">Apply camera offset</param>
        /// <param name="texture">Texture to render the mesh with</param>
        public void DrawPreparedMesh(Mesh mesh, Rect2i rect, bool translateToCamera, Texture texture)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            if (mesh == null)
            {
                return;
            }

            // Early clip test
            var clipRect = new Rect2i(mClipRegion.x0, mClipRegion.y0, mClipRegion.x1 - mClipRegion.x0, mClipRegion.y1 - mClipRegion.y0);

            if (!rect.Intersects(clipRect))
            {
                return;
            }

            Flush(FlushReason.TILEMAP_CHUNK);

            SetShaderValues();
            SetShaderGlobalAlpha((float)mAlpha / 255.0f);

            Graphics.SetRenderTarget(mCurrentRenderTexture);
            SetCurrentTexture(texture);

            GL.PushMatrix();
            GL.LoadPixelMatrix(0, mCurrentRenderTexture.width, mCurrentRenderTexture.height, 0);

            Vector3 posFinal;
            if (translateToCamera)
            {
                posFinal = new Vector3(-mCameraPos.x, -mCameraPos.y, 0);
            }
            else
            {
                posFinal = Vector3.zero;
            }

            for (int pass = 0; pass < mCurrentDrawMaterial.passCount; pass++)
            {
                mFlushInfo[(int)FlushReason.TILEMAP_CHUNK].Count++;
                mCurrentDrawMaterial.SetPass(pass);
                Graphics.DrawMeshNow(mesh, Matrix4x4.TRS(posFinal, Quaternion.identity, new Vector3(1, 1, 1)));
            }

            if (mCurrentSpritesIndex != -1)
            {
                SetCurrentTexture(mSpriteSheets[mCurrentSpritesIndex].texture);
            }
            else
            {
                SetCurrentTexture(null);
            }

            GL.PopMatrix();
        }

        /// <summary>
        /// Set camera position
        /// </summary>
        /// <param name="pos">Camera position</param>
        public void CameraSet(Vector2i pos)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            mCameraPos = pos;
        }

        /// <summary>
        /// Get camera position
        /// </summary>
        /// <returns>Camera position</returns>
        public Vector2i CameraGet()
        {
            return mCameraPos;
        }

        /// <summary>
        /// Start renderer for the frame
        /// </summary>
        public void StartRender()
        {
            if (!mRenderEnabled)
            {
                return;
            }

            mFrontBuffer.Reset();
            ShaderReset();

            mPreviousTexture = null;

            mDebugClipRegions.Clear();

            mPreviousDrawOffscreenIndex = -1;

            Onscreen();
            CameraSet(Vector2i.zero);
            AlphaSet(255);
            TintColorSet(new Color32(255, 255, 255, 255));
            PaletteSwapSet(0);
            FES.ClipReset();
            ResetMesh();
        }

        /// <summary>
        /// End renderer for the frame. This also applies some renderer based post-processing effects by drawing
        /// on top of anything else the user may have drawn
        /// </summary>
        public void FrameEnd()
        {
            if (!mRenderEnabled)
            {
                return;
            }

            Flush(FlushReason.FRAME_END);

            var drawState = StoreState();

            Onscreen();
            CameraSet(Vector2i.zero);
            AlphaSet(255);
            TintColorSet(new Color32(255, 255, 255, 255));
            PaletteSwapSet(0);
            FES.ClipReset();
            ShaderReset();

            mFESAPI.Effects.ApplyRenderTimeEffects();

            mFrontBuffer.FrameEnd(mFESAPI);

            if (mShowFlushDebug)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                int lineCount = 0;
                for (int i = 0; i < mFlushInfo.Length; i++)
                {
                    if (mFlushInfo[i].Count > 0)
                    {
                        if (lineCount > 0)
                        {
                            sb.Append("\n");
                        }

                        sb.Append(mFlushInfo[i].Reason);
                        sb.Append(": ");
                        sb.Append(mFlushInfo[i].Count);

                        mFlushInfo[i].Count = 0;

                        lineCount++;
                    }
                }

                string flushString = sb.ToString();
                var flushStringSize = FES.PrintMeasure(flushString);
                if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
                {
                    FES.DrawRectFill(new Rect2i(0, 0, flushStringSize.width + 8, flushStringSize.height + 8), mFlushDebugBackgroundColorIndex);
                    FES.Print(new Vector2i(4, 4), mFlushDebugFontColorIndex, sb.ToString());
                }
                else
                {
                    FES.DrawRectFill(new Rect2i(0, 0, flushStringSize.width + 8, flushStringSize.height + 8), mFlushDebugBackgroundColor);
                    FES.Print(new Vector2i(4, 4), mFlushDebugFontColor, sb.ToString());
                }
            }

            Flush(FlushReason.FRAME_END);

            RestoreState(drawState);

            mFlushInfo[(int)FlushReason.EFFECT_APPLY].Count++;
        }

        /// <summary>
        /// Draw all clip regions
        /// </summary>
        public void DrawClipRegions()
        {
            foreach (var clipRegion in mDebugClipRegions)
            {
                DrawRect(clipRegion.region, clipRegion.colorIndex, clipRegion.color.ToColor32(), Vector2i.zero);
            }
        }

        /// <summary>
        /// Mark the whole system texture as dirty
        /// </summary>
        public void MarkSystemTextureDirty()
        {
            mSystemTextureDirtyRegion = new Rect(0, 0, FESHW.HW_SYSTEM_TEXTURE_WIDTH, FESHW.HW_SYSTEM_TEXTURE_HEIGHT);
        }

        /// <summary>
        /// Add to the dirty texture area
        /// </summary>
        /// <param name="region">Dirty region</param>
        public void MarkSystemTextureDirty(Rect region)
        {
            if (mSystemTextureDirtyRegion.width == 0 && mSystemTextureDirtyRegion.height == 0)
            {
                mSystemTextureDirtyRegion = region;
                return;
            }

            if (region.x < mSystemTextureDirtyRegion.x)
            {
                mSystemTextureDirtyRegion.x = region.x;
            }

            if (region.y < mSystemTextureDirtyRegion.y)
            {
                mSystemTextureDirtyRegion.y = region.y;
            }

            if (region.x + region.width > mSystemTextureDirtyRegion.x + mSystemTextureDirtyRegion.width)
            {
                mSystemTextureDirtyRegion.width = region.x + region.width - mSystemTextureDirtyRegion.x;
            }

            if (region.y + region.height > mSystemTextureDirtyRegion.y + mSystemTextureDirtyRegion.height)
            {
                mSystemTextureDirtyRegion.height = region.y + region.height - mSystemTextureDirtyRegion.y;
            }
        }

        /// <summary>
        /// Add pending texture update copy block
        /// </summary>
        /// <param name="cb">Copy block</param>
        public void AddPendingTextureCopyBlock(TextureCopyBlock cb)
        {
            for (int i = 0; i < mPendingTextureCopyBlocks.Count; i++)
            {
                var copyblockOld = (TextureCopyBlock)mPendingTextureCopyBlocks[i];
                if (copyblockOld.colors == cb.colors && copyblockOld.rect.x == cb.rect.x && copyblockOld.rect.y == cb.rect.y && copyblockOld.rect.width == cb.rect.width && copyblockOld.rect.height == cb.rect.height)
                {
                    return;
                }
            }

            mPendingTextureCopyBlocks.Add(cb);
        }

        /// <summary>
        /// Set clip region
        /// </summary>
        /// <param name="rect">Region</param>
        public void ClipSet(Rect2i rect)
        {
            Rect2i origRect = rect;

            if (!mRenderEnabled)
            {
                return;
            }

            if (rect.width < 0 || rect.height < 0)
            {
                return;
            }

            int x0 = rect.x;
            int y0 = rect.y;
            int x1 = x0 + rect.width - 1;
            int y1 = y0 + rect.height - 1;

            if (x0 != mClipRegion.x0 || x1 != mClipRegion.x1 || y0 != mClipRegion.y0 || y1 != mClipRegion.y1)
            {
                Flush(FlushReason.CLIP_CHANGE);

                mClipRegion.x0 = x0;
                mClipRegion.y0 = y0;
                mClipRegion.x1 = x1;
                mClipRegion.y1 = y1;
            }

            mClip = rect;

            if (mClipDebug)
            {
                DebugClipRegion region;
                region.region = origRect;
                region.colorIndex = mClipDebugColorIndex;
                region.color = mClipDebugColor;

                mDebugClipRegions.Add(region);
            }
        }

        /// <summary>
        /// Get clip region
        /// </summary>
        /// <returns>Clip region</returns>
        public Rect2i ClipGet()
        {
            return mClip;
        }

        /// <summary>
        /// Set clip debug state
        /// </summary>
        /// <param name="enabled">Enable/Disabled flag</param>
        /// <param name="colorIndex">Color index</param>
        /// <param name="color">RGBA color</param>
        public void ClipDebugSet(bool enabled, int colorIndex, ColorRGBA color)
        {
            mClipDebug = enabled;
            mClipDebugColor = color;
            mClipDebugColorIndex = colorIndex;
        }

        /// <summary>
        /// Set flush debug state
        /// </summary>
        /// <param name="enabled">Enabled/Disabled flag</param>
        /// <param name="fontColorIndex">Font color index</param>
        /// <param name="backgroundColorIndex">Background color index</param>
        /// <param name="fontColor">Font RGBA color</param>
        /// <param name="backgroundColor">Background RGBA color</param>
        public void FlashDebugSet(bool enabled, int fontColorIndex, int backgroundColorIndex, ColorRGBA fontColor, ColorRGBA backgroundColor)
        {
            mFlushDebugBackgroundColor = backgroundColor;
            mFlushDebugBackgroundColorIndex = backgroundColorIndex;
            mFlushDebugFontColor = fontColor;
            mFlushDebugFontColorIndex = fontColorIndex;
            mShowFlushDebug = enabled;
        }

        /// <summary>
        /// Set alpha transparency
        /// </summary>
        /// <param name="a">Alpha value</param>
        public void AlphaSet(byte a)
        {
            mAlpha = a;
            mCurrentColor.a = a;
        }

        /// <summary>
        /// Get alpha tansparency value
        /// </summary>
        /// <returns>Alpha value</returns>
        public byte AlphaGet()
        {
            return mAlpha;
        }

        /// <summary>
        /// Set Tint color to apply to drawing
        /// </summary>
        /// <param name="tintColor">Tint color</param>
        public void TintColorSet(Color32 tintColor)
        {
            if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
            {
                mCurrentColor = new Color32(255, 255, 255, mAlpha);
            }
            else
            {
                mCurrentColor = new Color32(tintColor.r, tintColor.g, tintColor.b, mAlpha);
            }
        }

        /// <summary>
        /// Get current tint color
        /// </summary>
        /// <returns>Tint color</returns>
        public Color32 TintColorGet()
        {
            return mCurrentColor;
        }

        /// <summary>
        /// Set the current palette swap
        /// </summary>
        /// <param name="swapIndex">Swap index</param>
        public void PaletteSwapSet(int swapIndex)
        {
            swapIndex = FESInternal.FESHW.HW_PALETTE_SWAPS - swapIndex;
            mCurrentPaletteSwapIndex = swapIndex;
        }

        /// <summary>
        /// Set the current palette swap without calculating its offset in system texture
        /// </summary>
        /// <param name="swapIndex">Swap index</param>
        public void PaletteSwapDirect(int swapIndex)
        {
            mCurrentPaletteSwapIndex = swapIndex;
        }

        /// <summary>
        /// Get current swap index
        /// </summary>
        /// <returns>Swap index</returns>
        public int PaletteSwapGet()
        {
            return mCurrentPaletteSwapIndex;
        }

        /// <summary>
        /// Create render texture
        /// </summary>
        /// <param name="size">Dimensions</param>
        /// <returns>Render texture</returns>
        public RenderTexture RenderTextureCreate(Size2i size)
        {
            RenderTexture tex = new RenderTexture(size.width, size.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            if (tex == null)
            {
                return null;
            }

            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.anisoLevel = 0;
            tex.antiAliasing = 1;

            tex.autoGenerateMips = false;
            tex.depth = 0;
            tex.useMipMap = false;

            tex.Create();

            return tex;
        }

        /// <summary>
        /// Create offscreen surface
        /// </summary>
        /// <param name="offscreenIndex">Offscreen index</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <returns>True if successful</returns>
        public bool OffscreenCreate(int offscreenIndex, int width, int height)
        {
            if (offscreenIndex >= FESHW.HW_RENDER_TARGETS)
            {
                return false;
            }

            if (width < 0 || height < 0)
            {
                return false;
            }

            bool wasActive = false;
            if (UnityEngine.RenderTexture.active == mOffscreenTargets[offscreenIndex].tex)
            {
                UnityEngine.RenderTexture.active = null;
                wasActive = true;
            }

            bool wasCameraRenderTarget = false;
            if (mFESAPI.PixelCamera.GetRenderTarget() == mOffscreenTargets[offscreenIndex].tex)
            {
                mFESAPI.PixelCamera.SetRenderTarget(null);
                wasCameraRenderTarget = true;
            }

            if (width == 0 || height == 0)
            {
                if (mOffscreenTargets[offscreenIndex].tex != null)
                {
                    mOffscreenTargets[offscreenIndex].tex.Release();
                    mOffscreenTargets[offscreenIndex].tex = null;
                }

                return true;
            }

            if (mOffscreenTargets[offscreenIndex].tex != null)
            {
                // If existing texture is already the same size then do nothing
                if (mOffscreenTargets[offscreenIndex].tex.width == width && mOffscreenTargets[offscreenIndex].tex.height == height)
                {
                    return true;
                }

                // Release existing texture
                mOffscreenTargets[offscreenIndex].tex.Release();
                mOffscreenTargets[offscreenIndex].tex = null;
            }

            RenderTexture tex = RenderTextureCreate(new Size2i(width, height));
            if (tex == null)
            {
                return false;
            }

            tex.name = "Offscreen_" + offscreenIndex;

            mOffscreenTargets[offscreenIndex].tex = tex;
            mOffscreenTargets[offscreenIndex].needsClear = true;

            if (wasCameraRenderTarget)
            {
                mFESAPI.PixelCamera.SetRenderTarget(mOffscreenTargets[offscreenIndex].tex);
            }

            if (wasActive)
            {
                UnityEngine.RenderTexture.active = mOffscreenTargets[offscreenIndex].tex;
            }

            return true;
        }

        /// <summary>
        /// Check if given offscreen is valid
        /// </summary>
        /// <param name="offscreenIndex">Offscreen index</param>
        /// <returns>True if valid</returns>
        public bool OffscreenValid(int offscreenIndex)
        {
            if (offscreenIndex >= FESHW.HW_RENDER_TARGETS)
            {
                return false;
            }

            if (mOffscreenTargets[offscreenIndex].tex != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set the offscreen target by target index, also resets the clipping region to cover the new render target
        /// </summary>
        /// <param name="offscreenIndex">Offscreen index</param>
        public void OffscreenTarget(int offscreenIndex)
        {
            if (mCurrentRenderTexture != mOffscreenTargets[offscreenIndex].tex)
            {
                Flush(FlushReason.OFFSCREEN_CHANGE);
            }

            if (mOffscreenTargets[offscreenIndex].needsClear)
            {
                OffscreenClear(offscreenIndex);
                mOffscreenTargets[offscreenIndex].needsClear = false;
            }

            mCurrentRenderTexture = mOffscreenTargets[offscreenIndex].tex;
            FES.ClipReset();

            mFESAPI.PixelCamera.SetRenderTarget(mCurrentRenderTexture);
        }

        /// <summary>
        /// Get current render texture
        /// </summary>
        /// <returns>Render texture</returns>
        public Texture CurrentRenderTexture()
        {
            return mCurrentRenderTexture;
        }

        /// <summary>
        /// Get texture of the given offscreen
        /// </summary>
        /// <param name="offscreenIndex">Offscreen index</param>
        /// <returns>Texture</returns>
        public Texture OffscreenGetTexture(int offscreenIndex)
        {
            if (offscreenIndex < 0 || offscreenIndex >= mOffscreenTargets.Length)
            {
                return null;
            }

            return mOffscreenTargets[offscreenIndex].tex;
        }

        /// <summary>
        /// Set the current render target to the display, also resets the clipping region to cover the display
        /// </summary>
        public void Onscreen()
        {
            if (mCurrentRenderTexture != mFrontBuffer.Texture)
            {
                Flush(FlushReason.OFFSCREEN_CHANGE);
            }

            mCurrentRenderTexture = mFrontBuffer.Texture;
            FES.ClipReset();

            mFESAPI.PixelCamera.SetRenderTarget(mCurrentRenderTexture);
        }

        /// <summary>
        /// Get scanline effect info
        /// </summary>
        /// <param name="pixelSize">Size of the game pixels in native display pixel size</param>
        /// <param name="offset">Offset in system texture</param>
        /// <param name="length">Length of scanline in system texture</param>
        public void GetScanlineOffsetLength(float pixelSize, out int offset, out int length)
        {
            if (pixelSize < 1)
            {
                pixelSize = 1;
            }

            offset = length = 0;
            offset = (int)pixelSize;
            length = offset;
        }

        /// <summary>
        /// Setup a sprite sheet
        /// </summary>
        /// <param name="index">Index of the sprite sheet</param>
        /// <param name="filename">Filename</param>
        /// <param name="spriteSize">Sprite size</param>
        /// <returns>True if successful</returns>
        public bool SpriteSheetSetup(int index, string filename, Size2i spriteSize)
        {
            if (index < 0 || index >= FESHW.HW_MAX_SPRITESHEETS)
            {
                return false;
            }

            if (filename == null)
            {
                // No filename indicates the user wants to delete texture, just set to null and return
                mSpriteSheets[index].texture = null;
                mSpriteSheets[index].spriteSize = new Size2i(0, 0);
                mSpriteSheets[index].textureSize = new Size2i(0, 0);
                mSpriteSheets[index].columns = 0;
                mSpriteSheets[index].rows = 0;
                return true;
            }

            var spritesTextureOriginal = (Texture2D)Resources.Load(filename);
            if (spritesTextureOriginal == null)
            {
                Debug.Log("Could not load sprite sheet from " + filename + ", make sure the resource is placed somehwere in Assets/Resources folder");
                mSpritesTextureIsValid = false;
                mSpriteSheets = null;
                return false;
            }

            Texture2D newTexture;

            newTexture = new Texture2D(spritesTextureOriginal.width, spritesTextureOriginal.height, TextureFormat.ARGB32, false);

            newTexture.filterMode = FilterMode.Point;
            newTexture.wrapMode = TextureWrapMode.Mirror;
            if (spritesTextureOriginal != null)
            {
                newTexture.SetPixels32(spritesTextureOriginal.GetPixels32());
            }

            newTexture.Apply();

            mSpriteSheets[index].texture = newTexture;
            mSpriteSheets[index].spriteSize = spriteSize;
            mSpriteSheets[index].textureSize = new Size2i(newTexture.width, newTexture.height);
            mSpriteSheets[index].columns = mSpriteSheets[index].textureSize.width / mSpriteSheets[index].spriteSize.width;
            mSpriteSheets[index].rows = mSpriteSheets[index].textureSize.height / mSpriteSheets[index].spriteSize.height;

            if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
            {
                mFESAPI.Palette.PalettizeSprites(newTexture);
            }

            // If there is no spritesheet set then set this one as the current one
            if (mCurrentSpritesIndex == -1)
            {
                SpriteSheetSet(index);
            }

            return true;
        }

        /// <summary>
        /// Set the current sprite sheet to use
        /// </summary>
        /// <param name="index">Index of the sprite sheet</param>
        public void SpriteSheetSet(int index)
        {
            if (index < 0 || index >= FESHW.HW_MAX_SPRITESHEETS || mSpriteSheets[index].texture == null)
            {
                return;
            }

            // Flush if changing textures
            if (mCurrentBatchSpriteIndex != index || mPreviousDrawOffscreenIndex != -1)
            {
                Flush(FlushReason.SPRITESHEET_CHANGE);
                mPreviousDrawOffscreenIndex = -1;
            }

            mCurrentSpritesIndex = index;
            if (mCurrentSpritesIndex != -1)
            {
                SetCurrentTexture(mSpriteSheets[mCurrentSpritesIndex].texture);
            }
            else
            {
                SetCurrentTexture(null);
            }
        }

        /// <summary>
        /// Save the current effects state at this point, and move to next front buffer.
        /// </summary>
        public void EffectApplyNow()
        {
            var drawState = StoreState();

            bool wasOnFrontBuffer = false;
            if (mCurrentRenderTexture == mFrontBuffer.Texture)
            {
                wasOnFrontBuffer = true;
            }

            Onscreen();
            CameraSet(Vector2i.zero);
            AlphaSet(255);
            TintColorSet(new Color32(255, 255, 255, 255));
            PaletteSwapSet(0);
            FES.ClipReset();
            ShaderReset();

            mFESAPI.Effects.ApplyRenderTimeEffects();

            Flush(FlushReason.EFFECT_APPLY);

            mFrontBuffer.NextBuffer(mFESAPI);

            RestoreState(drawState);

            if (wasOnFrontBuffer)
            {
                mCurrentRenderTexture = mFrontBuffer.Texture;
            }

            ClearTransparent(mFrontBuffer.Texture);
        }

        /// <summary>
        /// Maximum radius of a circle
        /// </summary>
        /// <param name="center">Center of circle</param>
        /// <returns>Max radius</returns>
        public int MaxCircleRadiusForCenter(Vector2i center)
        {
            int maxEdgeDistance = 0;
            if (center.x > maxEdgeDistance)
            {
                maxEdgeDistance = center.x;
            }

            if (center.y > maxEdgeDistance)
            {
                maxEdgeDistance = center.y;
            }

            if (FES.DisplaySize.width - center.x > maxEdgeDistance)
            {
                maxEdgeDistance = FES.DisplaySize.width - center.x;
            }

            if (FES.DisplaySize.height - center.y > maxEdgeDistance)
            {
                maxEdgeDistance = FES.DisplaySize.height - center.y;
            }

            int maxRadius = (int)Mathf.Sqrt(2 * maxEdgeDistance * maxEdgeDistance) + 1;

            return maxRadius;
        }

        /// <summary>
        /// Get current front buffer
        /// </summary>
        /// <returns>Front buffer</returns>
        public FrontBuffer GetFrontBuffer()
        {
            return mFrontBuffer;
        }

        /// <summary>
        /// Clear the given RenderTexture to a transparent color
        /// </summary>
        /// <param name="texture">Texture to clear</param>
        public void ClearTransparent(RenderTexture texture)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            Color32 clearColor = new Color32(0, 0, 0, 0);

            RenderTexture rt = UnityEngine.RenderTexture.active;
            UnityEngine.RenderTexture.active = texture;
            GL.Clear(true, true, clearColor);
            UnityEngine.RenderTexture.active = rt;

            ResetMesh();
        }

        /// <summary>
        /// Load a shader from the given file
        /// </summary>
        /// <param name="index">Shader index to load into</param>
        /// <param name="filename">Shader filename</param>
        /// <returns>True if successful</returns>
        public bool ShaderSetup(int index, string filename)
        {
            if (index < 0 || index >= FESHW.HW_MAX_SHADERS)
            {
                return false;
            }

            if (filename == null)
            {
                // No filename indicates the user wants to delete the shader, just set to null and return
                mShaders[index] = null;
                return true;
            }

            var shader = (Shader)Resources.Load(filename);
            if (shader == null)
            {
                Debug.Log("Could not load shader from " + filename + ", make sure the resource is placed somehwere in Assets/Resources folder");
                return false;
            }

            var material = new FESShader(shader);
            mShaders[index] = material;

            return true;
        }

        /// <summary>
        /// Set the current shader
        /// </summary>
        /// <param name="index">Shader index</param>
        public void ShaderSet(int index)
        {
            if (index < 0 || index >= FESHW.HW_MAX_SHADERS)
            {
                return;
            }

            if (mShaders[index] == null)
            {
                ShaderReset();
            }
            else
            {
                SetCurrentMaterial(mShaders[index]);
                mCurrentShaderIndex = index;

                if (mCurrentSpritesIndex == -1)
                {
                    SetCurrentTexture(null);
                }
                else
                {
                    SetCurrentTexture(mSpriteSheets[mCurrentSpritesIndex].texture);
                }
            }
        }

        /// <summary>
        /// Apply the shader now, by flushing
        /// </summary>
        public void ShaderApplyNow()
        {
            Flush(FlushReason.SHADER_APPLY);
        }

        /// <summary>
        /// Reset the shader to default
        /// </summary>
        public void ShaderReset()
        {
            if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
            {
                SetCurrentMaterial(mDrawMaterialIndexed);
            }
            else
            {
                SetCurrentMaterial(mDrawMaterialRGB);
            }

            Flush(FlushReason.SHADER_RESET);

            mCurrentShaderIndex = -1;
        }

        /// <summary>
        /// Get the shader Material
        /// </summary>
        /// <param name="index">Shader index</param>
        /// <returns>Material</returns>
        public Material ShaderGetMaterial(int index)
        {
            if (index < 0 || index >= mShaders.Length)
            {
                return null;
            }

            return mShaders[index];
        }

        /// <summary>
        /// Get the shader parameters
        /// </summary>
        /// <param name="shaderIndex">Shader index</param>
        /// <returns>Shader parameters</returns>
        public FESShader ShaderParameters(int shaderIndex)
        {
            return mShaders[shaderIndex];
        }

        private DrawState StoreState()
        {
            DrawState state = new DrawState();

            state.Alpha = AlphaGet();
            state.CameraPos = CameraGet();
            state.Clip = ClipGet();
            state.CurrentRenderTexture = mCurrentRenderTexture;
            state.PaletteSwap = PaletteSwapGet();
            state.TintColor = TintColorGet();
            state.CurrentMaterial = mCurrentDrawMaterial;

            return state;
        }

        private void RestoreState(DrawState state)
        {
            AlphaSet(state.Alpha);
            CameraSet(state.CameraPos);
            ClipSet(state.Clip);
            mCurrentRenderTexture = state.CurrentRenderTexture;
            PaletteSwapSet(state.PaletteSwap);
            TintColorSet(state.TintColor);
            SetCurrentMaterial(state.CurrentMaterial);
        }

        /// <summary>
        /// Draw ellipse slow pixel by pixel
        /// </summary>
        /// <param name="center">Center of ellipse</param>
        /// <param name="radius">Radius</param>
        /// <param name="colorIndex">Color index</param>
        /// <param name="color">RGB color</param>
        private void DrawEllipseSlow(Vector2i center, Vector2i radius, int colorIndex, Color32 color)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            int rx = radius.x;
            int ry = radius.y;
            int cx = center.x;
            int cy = center.y;

            if (rx < 0 || ry < 0)
            {
                return;
            }

            if (rx == 0 || ry == 0)
            {
                DrawRectFill(new Rect2i(cx - rx, cy - ry, (rx * 2) + 1, (ry * 2) + 1), colorIndex, color, Vector2i.zero);
                return;
            }

            if (rx > MAX_ELLIPSE_RADIUS || ry > MAX_ELLIPSE_RADIUS)
            {
                return;
            }

            float radiusXSq = rx * rx;
            float radiusYSq = ry * ry;
            int x = 0, y = ry;
            float p;
            float px = 0, py = 2 * radiusXSq * y;

            EllipsePlotPoints(cx, cy, x, y, colorIndex, color);

            p = radiusYSq - (radiusXSq * ry) + (0.25f * radiusXSq);
            while (px < py)
            {
                x++;
                px = px + (2 * radiusYSq);

                if (p < 0)
                {
                    p = p + radiusYSq + px;
                }
                else
                {
                    y--;
                    py = py - (2 * radiusXSq);
                    p = p + radiusYSq + px - py;
                }

                EllipsePlotPoints(cx, cy, x, y, colorIndex, color);
            }

            p = (radiusYSq * (x + 0.5f) * (x + 0.5f)) + (radiusXSq * (y - 1) * (y - 1)) - (radiusXSq * radiusYSq);
            while (y > 0)
            {
                y--;
                py = py - (2 * radiusXSq);
                if (p > 0)
                {
                    p = p + radiusXSq - py;
                }
                else
                {
                    x++;
                    px = px + (2 * radiusYSq);
                    p = p + radiusXSq - py + px;
                }

                EllipsePlotPoints(cx, cy, x, y, colorIndex, color);
            }
        }

        /// <summary>
        /// Draw filled ellipse, slow, column by column
        /// </summary>
        /// <param name="center">Center of ellipse</param>
        /// <param name="radius">Radius</param>
        /// <param name="colorIndex">Color index</param>
        /// <param name="color">RGB color</param>
        /// <param name="inverse">Do an inverted fill?</param>
        private void DrawEllipseFillSlow(Vector2i center, Vector2i radius, int colorIndex, Color32 color, bool inverse)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            int rx = radius.x;
            int ry = radius.y;
            int cx = center.x;
            int cy = center.y;

            if (rx < 0 || ry < 0)
            {
                return;
            }

            if (rx == 0 || ry == 0)
            {
                DrawRectFill(new Rect2i(cx - rx, cy - ry, (rx * 2) + 1, (ry * 2) + 1), colorIndex, color, Vector2i.zero);
                return;
            }

            if (rx > MAX_ELLIPSE_RADIUS || ry > MAX_ELLIPSE_RADIUS)
            {
                return;
            }

            int cameraOffset = -mCameraPos.x;
            int lookupX0 = Mathf.Max(0, cx - rx + cameraOffset);
            int lookupX1 = Mathf.Min(mFESAPI.HW.DisplaySize.width, cx + rx + cameraOffset);

            for (int i = lookupX0; i <= lookupX1; i++)
            {
                mEllipseFillLookup[i] = int.MaxValue;
            }

            float radiusXSq = rx * rx;
            float radiusYSq = ry * ry;
            int x = 0, y = ry;
            float p;
            float px = 0, py = 2 * radiusXSq * y;

            EllipseCheckLowestPoints(cx - rx, cx, cy, x, y, mEllipseFillLookup);

            p = radiusYSq - (radiusXSq * ry) + (0.25f * radiusXSq);
            while (px < py)
            {
                x++;
                px = px + (2 * radiusYSq);

                if (p < 0)
                {
                    p = p + radiusYSq + px;
                }
                else
                {
                    y--;
                    py = py - (2 * radiusXSq);
                    p = p + radiusYSq + px - py;
                }

                EllipseCheckLowestPoints(cx - rx, cx, cy, x, y, mEllipseFillLookup);
            }

            p = (radiusYSq * (x + 0.5f) * (x + 0.5f)) + (radiusXSq * (y - 1) * (y - 1)) - (radiusXSq * radiusYSq);
            while (y > 0)
            {
                y--;
                py = py - (2 * radiusXSq);
                if (p > 0)
                {
                    p = p + radiusXSq - py;
                }
                else
                {
                    x++;
                    px = px + (2 * radiusYSq);
                    p = p + radiusXSq - py + px;
                }

                EllipseCheckLowestPoints(cx - rx, cx, cy, x, y, mEllipseFillLookup);
            }

            if (!inverse)
            {
                for (int i = lookupX0; i <= lookupX1; i++)
                {
                    int coly = mEllipseFillLookup[i];
                    int colh = ((cy - coly) * 2) + 1;

                    DrawRectFill(new Rect2i(i - cameraOffset, coly, 1, colh), colorIndex, color, Vector2i.zero);
                }
            }
            else
            {
                for (int i = lookupX0; i <= lookupX1; i++)
                {
                    int coly = mEllipseFillLookup[i];
                    int colh = ((cy - coly) * 2) + 1;
                    int y0 = cy - ry;
                    int y1 = coly;
                    int y2 = coly + colh;
                    int y3 = cy + ry + 1;

                    DrawRectFill(new Rect2i(i - cameraOffset, y0, 1, y1 - y0), colorIndex, color, Vector2i.zero);
                    DrawRectFill(new Rect2i(i - cameraOffset, y2, 1, y3 - y2), colorIndex, color, Vector2i.zero);
                }
            }
        }

        private void DrawLineStrip(Vector2i[] points, int pointCount, int colorIndex, Color32 color, Vector2i pivot, float rotation = 0, bool cameraApply = true)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            if (pointCount < 2 || pointCount > points.Length)
            {
                return;
            }

            int i = 1;
            for (; i < pointCount - 1; i++)
            {
                if (points[i] != points[i - 1])
                {
                    DrawLine(points[i - 1], points[i], colorIndex, color, pivot, rotation, cameraApply, true, false);
                }
            }

            if (points[i].x != points[0].x || points[i].y != points[0].y)
            {
                DrawLine(points[i - 1], points[i], colorIndex, color, pivot, rotation, cameraApply, true, true);
            }
            else
            {
                DrawLine(points[i - 1], points[i], colorIndex, color, pivot, rotation, cameraApply, true, false);
            }
        }

        private void DrawConvexPolygon(Vector2i center, Vector3[] points, int pointCount, int colorIndex, Color32 color, Vector2i pivot, float rotation = 0, bool cameraApply = true)
        {
            if (pointCount < 2 || pointCount > points.Length)
            {
                return;
            }

            // First row is just color indecies, for solid shape rendering, skip it
            int swapIndex = mCurrentPaletteSwapIndex;
            swapIndex++;

            if (mFESAPI.HW.ColorMode == FES.ColorMode.Indexed)
            {
                color = mCurrentColor;
            }
            else
            {
                if (mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
                {
                    color = (Color)color * (Color)mCurrentColor;
                }
            }

            CheckFlush(1 + (pointCount * 2));

            Vector2 vertFlags = new Vector2(0, 0);

            int i = mMeshStorage.CurrentVertex;
            int ci = i;
            int j = mMeshStorage.CurrentIndex;

            CheckFlush(2);

            mMeshStorage.Verticies[i] = new Vector3(center.x, center.y, swapIndex);
            mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * colorIndex) + 0.0001f;
            mMeshStorage.Uvs[i].y = 0.999999f;
            mMeshStorage.Flags[i] = vertFlags;
            mMeshStorage.Colors[i] = color;

            i++;

            for (int k = 0; k < pointCount - 1; k++)
            {
                CheckFlush(2);

                float x = points[k].x;
                float y = points[k].y;

                mMeshStorage.Verticies[i] = new Vector3(x, y, swapIndex);
                mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * (colorIndex + 1)) - 0.0001f;
                mMeshStorage.Uvs[i].y = 0.999999f;
                mMeshStorage.Flags[i] = vertFlags;
                mMeshStorage.Colors[i] = color;

                i++;

                x = points[k + 1].x;
                y = points[k + 1].y;

                mMeshStorage.Verticies[i] = new Vector3(x, y, swapIndex);
                mMeshStorage.Uvs[i].x = ((1.0f / FESHW.HW_SYSTEM_TEXTURE_WIDTH) * (colorIndex + 1)) - 0.0001f;
                mMeshStorage.Uvs[i].y = 1.0f;
                mMeshStorage.Flags[i] = vertFlags;
                mMeshStorage.Colors[i] = color;

                i++;

                mMeshStorage.Indecies[j++] = ci;
                mMeshStorage.Indecies[j++] = i - 1;
                mMeshStorage.Indecies[j++] = i - 2;
            }

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        private void EllipsePlotPoints(int xcenter, int ycenter, int x, int y, int colorIndex, Color32 color)
        {
            DrawPixel(xcenter + x, ycenter + y, colorIndex, color);
            DrawPixel(xcenter - x, ycenter + y, colorIndex, color);
            DrawPixel(xcenter + x, ycenter - y, colorIndex, color);
            DrawPixel(xcenter - x, ycenter - y, colorIndex, color);
        }

        private void EllipseCheckLowestPoints(int xoffset, int xcenter, int ycenter, int x, int y, int[] lowestPoints)
        {
            int i;
            int miny = ycenter - y;

            int cameraOffset = -mCameraPos.x;

            i = x + xcenter + cameraOffset;
            if (i >= 0 && i < mFESAPI.HW.DisplaySize.width)
            {
                if (lowestPoints[i] > miny)
                {
                    lowestPoints[i] = miny;
                }
            }

            i = -x + xcenter + cameraOffset;
            if (i >= 0 && i < mFESAPI.HW.DisplaySize.width)
            {
                if (lowestPoints[i] > miny)
                {
                    lowestPoints[i] = miny;
                }
            }
        }

        private void CheckFlush(int minimumVerticesRequired)
        {
            if (mMeshStorage.CurrentVertex == 0)
            {
                // If start of batch then set the current spritesheet index to this one
                mCurrentBatchSpriteIndex = mCurrentSpritesIndex;
            }
            else if (mMeshStorage.MaxVertices - mMeshStorage.CurrentVertex < minimumVerticesRequired)
            {
                Flush(FlushReason.BATCH_FULL);
            }
        }

        private void SetCurrentMaterial(Material material)
        {
            if (material == mCurrentDrawMaterial)
            {
                return;
            }

            Flush(FlushReason.SET_MATERIAL);

            mCurrentDrawMaterial = material;
            SetShaderValues();
        }

        private void SetCurrentTexture(Texture texture)
        {
            if (mPreviousTexture == texture)
            {
                return;
            }

            Flush(FlushReason.SET_TEXTURE);

            mCurrentDrawMaterial.SetTexture("_SpritesTexture", texture);

            mPreviousTexture = texture;
        }

        private void SetShaderValues()
        {
            if (mCurrentRenderTexture == null)
            {
                return;
            }

            int x = mClipRegion.x0;
            int y = mClipRegion.y0;
            int w = mClipRegion.x1 - mClipRegion.x0 + 1;
            int h = mClipRegion.y1 - mClipRegion.y0 + 1;

            int x0 = x;
            int y0 = mCurrentRenderTexture.height - (y + h);
            int x1 = x0 + w;
            int y1 = y0 + h;

            mCurrentDrawMaterial.SetVector("_Clip", new Vector4(x0, y0, x1, y1));
            mCurrentDrawMaterial.SetVector("_SystemTextureSize", new Vector4(FESHW.HW_SYSTEM_TEXTURE_WIDTH, FESHW.HW_SYSTEM_TEXTURE_HEIGHT));
            SetShaderGlobalAlpha(1.0f);
        }

        private void SetShaderGlobalAlpha(float alpha)
        {
            mCurrentDrawMaterial.SetFloat("_GlobalAlpha", alpha);
        }

        /// <summary>
        /// Flush vertex buffer to screen
        /// </summary>
        /// <param name="reason">Reason for flushing</param>
        private void Flush(FlushReason reason)
        {
            if (!mRenderEnabled)
            {
                return;
            }

            // First check if texture is dirty and needs an update
            if ((mSystemTextureDirtyRegion.width != 0 && mSystemTextureDirtyRegion.height != 0) || mPendingTextureCopyBlocks.Count > 0)
            {
                UpdateTexture();
            }

            if (mMeshStorage.CurrentIndex == 0)
            {
                // Nothing to flush
                return;
            }

            SetShaderValues();

            if (mMeshStorage.CurrentVertex > 0)
            {
                // Always set system texture
                mCurrentDrawMaterial.SetTexture("_SystemTexture", mSystemTexture);

                var mesh = mMeshStorage.ReduceAndUploadMesh(mMeshStorage);
                if (mesh == null)
                {
                    // Could not get mesh, will not be able to draw, drop vertices instead
                    mMeshStorage.CurrentVertex = 0;
                    mMeshStorage.CurrentIndex = 0;
                    return;
                }

                Graphics.SetRenderTarget(mCurrentRenderTexture);

                GL.PushMatrix();
                GL.LoadPixelMatrix(0, mCurrentRenderTexture.width, mCurrentRenderTexture.height, 0);

                // If we're using a custom shader then apply chosen filters to all offscreen surfaces
                if (mCurrentShaderIndex != -1)
                {
                    FESShader shader = mShaders[mCurrentShaderIndex];
                    var filters = shader.GetOffscreenFilters();
                    foreach (var filter in filters)
                    {
                        var tex = OffscreenGetTexture(filter.Key);
                        if (tex != null)
                        {
                            tex.filterMode = filter.Value;
                        }
                    }
                }

                for (int pass = 0; pass < mCurrentDrawMaterial.passCount; pass++)
                {
                    mFlushInfo[(int)reason].Count++;
                    mCurrentDrawMaterial.SetPass(pass);
                    Graphics.DrawMeshNow(mesh.Mesh, Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(1, 1, 1)));
                }

                // Revert any filter changes
                if (mCurrentShaderIndex != -1)
                {
                    FESShader shader = mShaders[mCurrentShaderIndex];
                    var filters = shader.GetOffscreenFilters();
                    foreach (var filter in filters)
                    {
                        var tex = OffscreenGetTexture(filter.Key);
                        if (tex != null)
                        {
                            tex.filterMode = FilterMode.Point;
                        }
                    }
                }

                GL.PopMatrix();

                mMeshStorage.CurrentVertex = 0;
                mMeshStorage.CurrentIndex = 0;
            }
        }

        private void ResetMesh()
        {
            mMeshStorage.CurrentVertex = 0;
            mMeshStorage.CurrentIndex = 0;
        }

        /// <summary>
        /// Generate system texture
        /// </summary>
        /// <returns>True if successful</returns>
        private bool GenerateTexture()
        {
            mSystemTexture = new Texture2D(FESHW.HW_SYSTEM_TEXTURE_WIDTH, FESHW.HW_SYSTEM_TEXTURE_HEIGHT, TextureFormat.ARGB32, false);
            mSystemTexture.filterMode = FilterMode.Point;
            mSystemTexture.wrapMode = TextureWrapMode.Mirror;

            mSystemTexturePixels = new Color32[FESHW.HW_SYSTEM_TEXTURE_WIDTH * FESHW.HW_SYSTEM_TEXTURE_HEIGHT];
            mSystemTextureSubPixels = new Color32[FESHW.HW_SYSTEM_TEXTURE_WIDTH * FESHW.HW_SYSTEM_TEXTURE_HEIGHT];

            for (int i = 0; i < mSystemTexturePixels.Length; i++)
            {
                mSystemTexturePixels[i] = new Color32(0, 0, 0, 0);
            }

            // For RGB mode fill the first row of pixels with solid white, we'll use this to render
            // solid color geometry
            if (mFESAPI.HW.ColorMode == FES.ColorMode.RGB)
            {
                for (int i = 0; i < FESHW.HW_SYSTEM_TEXTURE_WIDTH; i++)
                {
                    mSystemTexturePixels[i + (mSystemTexturePixels.Length - FESHW.HW_SYSTEM_TEXTURE_WIDTH)] = new Color32(255, 255, 255, 255);
                }
            }

            var oldRandState = Random.state;
            Random.InitState(0xDEAD);

            // Create a noise texture that will be sampled for the noise retroness effect
            for (int x = FESHW.HW_SYSTEM_TEXTURE_WIDTH - 128; x < FESHW.HW_SYSTEM_TEXTURE_WIDTH; x++)
            {
                for (int y = FESHW.HW_SYSTEM_TEXTURE_HEIGHT - 128; y < FESHW.HW_SYSTEM_TEXTURE_HEIGHT; y++)
                {
                    int i = x + (y * FESHW.HW_SYSTEM_TEXTURE_WIDTH);
                    byte c = (byte)Random.Range(0, 256);
                    mSystemTexturePixels[i] = new Color32(c, c, c, 255);
                }
            }

            Random.state = oldRandState;

            // Create a gradient texture used for scanline effect
            int col = 0;
            for (int x = FESHW.HW_SYSTEM_TEXTURE_WIDTH - 256; x < FESHW.HW_SYSTEM_TEXTURE_WIDTH - 128; x++)
            {
                int scanHeight = col;

                int row = scanHeight - 1;
                for (int y = FESHW.HW_SYSTEM_TEXTURE_HEIGHT - scanHeight; y < FESHW.HW_SYSTEM_TEXTURE_HEIGHT; y++)
                {
                    int i = x + (y * FESHW.HW_SYSTEM_TEXTURE_WIDTH);

                    byte c = 255;
                    if (scanHeight > 16)
                    {
                        if (row == 0)
                        {
                            c = 0;
                        }
                        else if (row == 1)
                        {
                            c = 64;
                        }
                        else if (row == 2)
                        {
                            c = 128;
                        }
                        else if (row == 3)
                        {
                            c = 196;
                        }
                    }
                    else if (scanHeight > 8)
                    {
                        if (row == 0)
                        {
                            c = 0;
                        }
                        else if (row == 1)
                        {
                            c = 85;
                        }
                        else if (row == 2)
                        {
                            c = 170;
                        }
                    }
                    else if (scanHeight > 3)
                    {
                        if (row == 0)
                        {
                            c = 0;
                        }
                        else if (row == 1)
                        {
                            c = 128;
                        }
                    }
                    else if (scanHeight > 2)
                    {
                        if (row == 0)
                        {
                            c = 0;
                        }
                    }
                    else if (scanHeight > 1)
                    {
                        if (row == 0)
                        {
                            c = 128;
                        }
                    }

                    mSystemTexturePixels[i] = new Color32(c, c, c, 255);

                    row--;
                }

                col++;
            }

            MarkSystemTextureDirty();

            return true;
        }

        /// <summary>
        /// Update system texture for dirty region
        /// </summary>
        private void UpdateTexture()
        {
            if (mSystemTextureDirtyRegion.x == 0 && mSystemTextureDirtyRegion.y == 0 &&
                mSystemTextureDirtyRegion.width == mSystemTexture.width && mSystemTextureDirtyRegion.height == mSystemTexture.height)
            {
                // If the whole region is dirty then update the whole texture
                mSystemTexture.SetPixels32(mSystemTexturePixels);
            }
            else if (mSystemTextureDirtyRegion.width > 0 && mSystemTextureDirtyRegion.height > 0)
            {
                // Otherwise update only part of the texture
                int i = 0;
                int j = 0;
                i = ((int)mSystemTextureDirtyRegion.y * mSystemTexture.width) + (int)mSystemTextureDirtyRegion.x;

                // Prepare the colors buffer, this is a bit wasteful, but no way around it without having individual buffers for each
                // update
                for (int y = 0; y < (int)mSystemTextureDirtyRegion.height; y++)
                {
                    for (int x = 0; x < (int)mSystemTextureDirtyRegion.width; x++)
                    {
                        mSystemTextureSubPixels[j] = mSystemTexturePixels[i];
                        i++;
                        j++;
                    }

                    i += mSystemTexture.width - (int)mSystemTextureDirtyRegion.width;
                }

                mSystemTexture.SetPixels32((int)mSystemTextureDirtyRegion.x, (int)mSystemTextureDirtyRegion.y, (int)mSystemTextureDirtyRegion.width, (int)mSystemTextureDirtyRegion.height, mSystemTextureSubPixels);
            }

            mSystemTextureDirtyRegion = new Rect(0, 0, 0, 0);

            // Copy all texture copy blocks
            bool updatePixelsArray = false;
            for (int i = 0; i < mPendingTextureCopyBlocks.Count; i++)
            {
                var cb = (TextureCopyBlock)mPendingTextureCopyBlocks[i];
                mSystemTexture.SetPixels32(cb.rect.x, cb.rect.y, cb.rect.width, cb.rect.height, cb.colors);
                if (cb.updatePixelsArray)
                {
                    updatePixelsArray = true;
                }
            }

            mPendingTextureCopyBlocks.Clear();

            mSystemTexture.Apply();

            if (updatePixelsArray)
            {
                mSystemTexturePixels = mSystemTexture.GetPixels32();
            }

#if UNITY_EDITOR && false
            byte[] bytes = mSystemTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/../SavedSystemTexture.png", bytes);
#endif
        }

        /// <summary>
        /// Texture copy block, defines the area to copy, and the color data
        /// </summary>
        public struct TextureCopyBlock
        {
            /// <summary>
            /// Texture region
            /// </summary>
            public Rect2i rect;

            /// <summary>
            /// Pixel colors to update with
            /// </summary>
            public Color32[] colors;

            /// <summary>
            /// True if pixels array should be updated too
            /// </summary>
            public bool updatePixelsArray;
        }

        /// <summary>
        /// Defines a single sprite sheet
        /// </summary>
        public struct SpriteSheet
        {
            /// <summary>
            /// The texture for the spritesheet
            /// </summary>
            public Texture2D texture;

            /// <summary>
            /// Size of the texture for quick lookup
            /// </summary>
            public Size2i textureSize;

            /// <summary>
            /// Size of sprites in the texture
            /// </summary>
            public Size2i spriteSize;

            /// <summary>
            /// Sprite columns in the texture
            /// </summary>
            public int columns;

            /// <summary>
            /// Sprite rows in the texture
            /// </summary>
            public int rows;
        }

        private struct FlushInfo
        {
            public string Reason;
            public int Count;
        }

        private struct ClipRegion
        {
            public int x0, y0, x1, y1;
        }

        private struct DebugClipRegion
        {
            public Rect2i region;
            public int colorIndex;
            public ColorRGBA color;
        }

        private struct OffscreenSurface
        {
            public RenderTexture tex;
            public bool needsClear;
        }

        private struct DrawState
        {
            public RenderTexture CurrentRenderTexture;
            public Vector2i CameraPos;
            public byte Alpha;
            public Color32 TintColor;
            public int PaletteSwap;
            public Rect2i Clip;
            public Material CurrentMaterial;
        }

        /// <summary>
        /// Wrapper for Unity Material, adding an extra API for tracking offscreen texture filters
        /// </summary>
        public class FESShader : Material
        {
            private Dictionary<int, FilterMode> mShaderFilter = new Dictionary<int, FilterMode>();

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="shader">Shader</param>
            public FESShader(Shader shader) : base(shader)
            {
            }

            /// <summary>
            /// Set texture filter
            /// </summary>
            /// <param name="offscreenIndex">Offscreen index</param>
            /// <param name="filterMode">Filter mode</param>
            public void SetOffscreenFilter(int offscreenIndex, FilterMode filterMode)
            {
                // Point is the default filter type, don't need to store it
                if (filterMode == FilterMode.Point)
                {
                    mShaderFilter.Remove(offscreenIndex);
                }
                else
                {
                    mShaderFilter[offscreenIndex] = filterMode;
                }
            }

            /// <summary>
            /// Get all offscreen filters
            /// </summary>
            /// <returns>Filters</returns>
            public Dictionary<int, FilterMode> GetOffscreenFilters()
            {
                return mShaderFilter;
            }
        }

        /// <summary>
        /// A collection of front buffers
        /// </summary>
        public class FrontBuffer
        {
            /// <summary>
            /// Size of front buffers, they are all the same size
            /// </summary>
            public Size2i Size;

            private List<BufferState> mBuffers = new List<BufferState>();
            private int mCurrentBufferIndex = -1;

            /// <summary>
            /// Get current front buffer texture
            /// </summary>
            public RenderTexture Texture
            {
                get
                {
                    if (mCurrentBufferIndex < 0)
                    {
                        return null;
                    }

                    return mBuffers[mCurrentBufferIndex].tex;
                }
            }

            /// <summary>
            /// Resize all front buffers
            /// </summary>
            /// <param name="size">New size</param>
            /// <param name="api">Reference to FESAPI</param>
            /// <returns>True if successful</returns>
            public bool Resize(Size2i size, FESAPI api)
            {
                if (size.width < 0 || size.height < 0)
                {
                    return false;
                }

                if (mBuffers.Count == 0)
                {
                    var tex = api.Renderer.RenderTextureCreate(size);
                    if (tex != null)
                    {
                        tex.name = "FrontBuffer_0";

                        mBuffers.Add(new BufferState(tex));
                        Size = size;
                        mCurrentBufferIndex = 0;
                        return true;
                    }
                }
                else
                {
                    // Same size, nothing to do
                    if (size == Size)
                    {
                        return true;
                    }

                    // Resize all existing buffers
                    for (int i = 0; i < mBuffers.Count; i++)
                    {
                        bool wasActive = false;
                        if (UnityEngine.RenderTexture.active == mBuffers[i].tex)
                        {
                            UnityEngine.RenderTexture.active = null;
                            wasActive = true;
                        }

                        bool wasCameraRenderTarget = false;
                        if (api.PixelCamera.GetRenderTarget() == mBuffers[i].tex)
                        {
                            api.PixelCamera.SetRenderTarget(null);
                            wasCameraRenderTarget = true;
                        }

                        if (size.width == 0 || size.height == 0)
                        {
                            if (mBuffers[i].tex != null)
                            {
                                mBuffers[i].tex.Release();
                                mBuffers[i].tex = null;
                            }
                        }

                        if (mBuffers[i].tex != null)
                        {
                            // Release existing texture
                            mBuffers[i].tex.Release();
                            mBuffers[i].tex = null;
                        }

                        RenderTexture tex = api.Renderer.RenderTextureCreate(size);
                        if (tex == null)
                        {
                            return false;
                        }

                        tex.name = "FrontBuffer_" + i;

                        mBuffers[i] = new BufferState(tex);

                        if (wasCameraRenderTarget)
                        {
                            api.PixelCamera.SetRenderTarget(mBuffers[i].tex);
                        }

                        if (wasActive)
                        {
                            UnityEngine.RenderTexture.active = mBuffers[i].tex;
                        }
                    }

                    Size = size;
                }

                return true;
            }

            /// <summary>
            /// Get next front buffer texture, one will be created if necessary
            /// </summary>
            /// <param name="api">Reference to the FESAPI</param>
            /// <returns>Next front buffer texture</returns>
            public bool NextBuffer(FESAPI api)
            {
                int index = mCurrentBufferIndex + 1;

                if (index >= mBuffers.Count)
                {
                    RenderTexture tex = api.Renderer.RenderTextureCreate(FES.DisplaySize);
                    if (tex == null)
                    {
                        return false;
                    }

                    tex.name = "FrontBuffer_" + index;

                    mBuffers.Add(new BufferState(tex));
                }

                if (mCurrentBufferIndex >= 0)
                {
                    mBuffers[mCurrentBufferIndex].effectParams = api.Effects.CopyState();
                }

                mCurrentBufferIndex = index;

                return true;
            }

            /// <summary>
            /// Notify that render frame has ended, this stores the effects state at the end of the frame
            /// for later post-process rendering stage
            /// </summary>
            /// <param name="api">Reference to the FESAPI</param>
            public void FrameEnd(FESAPI api)
            {
                if (mCurrentBufferIndex >= 0)
                {
                    mBuffers[mCurrentBufferIndex].effectParams = api.Effects.CopyState();
                }
            }

            /// <summary>
            /// Reset back to the first frame buffer
            /// </summary>
            public void Reset()
            {
                if (mBuffers.Count > 0)
                {
                    mCurrentBufferIndex = 0;
                }
                else
                {
                    mCurrentBufferIndex = -1;
                }
            }

            /// <summary>
            /// Get all the frame buffers, and the count of used ones
            /// </summary>
            /// <param name="usedBuffers">Count of currently used buffers in the last frame</param>
            /// <returns>List of all the front buffers</returns>
            public List<BufferState> GetBuffers(out int usedBuffers)
            {
                usedBuffers = mCurrentBufferIndex + 1;
                return mBuffers;
            }

            /// <summary>
            /// Contains information about a single frame buffer
            /// </summary>
            public class BufferState
            {
                /// <summary>
                /// Texture
                /// </summary>
                public RenderTexture tex;

                /// <summary>
                /// Copy of all the effects to be applied to this frame buffer
                /// </summary>
                public FESEffects.EffectParams[] effectParams;

                /// <summary>
                /// Constructor
                /// </summary>
                /// <param name="tex">Texture</param>
                public BufferState(RenderTexture tex)
                {
                    this.tex = tex;
                    this.effectParams = null;
                }
            }
        }

        private class MeshStorage
        {
            public int CurrentVertex = 0;
            public int CurrentIndex = 0;
            public int MaxVertices = MAX_VERTEX_PER_MESH;

            public Vector3[] Verticies;
            public Vector2[] Uvs;
            public Vector2[] Flags;
            public Color32[] Colors;
            public int[] Indecies;

            private List<MeshDef> MeshBucket = new List<MeshDef>();

            public MeshStorage()
            {
                int maxVerts = 4;
                while (true)
                {
                    MeshBucket.Add(new MeshDef(maxVerts));

                    if (maxVerts >= MAX_VERTEX_PER_MESH)
                    {
                        break;
                    }

                    maxVerts *= 2;

                    if (maxVerts > MAX_VERTEX_PER_MESH)
                    {
                        maxVerts = MAX_VERTEX_PER_MESH;
                    }
                }

                // Instead of allocating its own arrays the MeshStorage can share arrays with the biggest mesh
                var biggestMesh = MeshBucket[MeshBucket.Count - 1];
                biggestMesh.SharedArrays = true;

                Verticies = biggestMesh.Verticies;
                Uvs = biggestMesh.Uvs;
                Flags = biggestMesh.Flags;
                Colors = biggestMesh.Colors;
                Indecies = biggestMesh.Indecies;
            }

            public MeshDef ReduceAndUploadMesh(MeshStorage meshStorage)
            {
                MeshDef meshDef = null;

                for (int i = 0; i < MeshBucket.Count; i++)
                {
                    meshDef = MeshBucket[i];
                    if (meshDef.MaxVerts >= CurrentVertex)
                    {
                        break;
                    }
                }

                if (meshDef != null)
                {
                    if (!meshDef.SharedArrays)
                    {
                        System.Array.Copy(Verticies, meshDef.Verticies, CurrentVertex);
                        System.Array.Copy(Uvs, meshDef.Uvs, CurrentVertex);
                        System.Array.Copy(Flags, meshDef.Flags, CurrentVertex);
                        System.Array.Copy(Colors, meshDef.Colors, CurrentVertex);

                        System.Buffer.BlockCopy(Indecies, 0, meshDef.Indecies, 0, CurrentIndex * sizeof(int));
                    }

                    // Clear the remainder of the indecies array to create degenerate triangles for unused
                    // indecies. Only clear if last time mesh was used it had more indecies than this time
                    if (meshDef.LastIndeciesLength > CurrentIndex)
                    {
                        System.Array.Clear(meshDef.Indecies, CurrentIndex, meshDef.LastIndeciesLength - CurrentIndex);
                    }

                    meshDef.LastIndeciesLength = CurrentIndex;

                    meshDef.Upload();
                }

                return meshDef;
            }

            public class MeshDef
            {
                public Mesh Mesh = new Mesh();
                public int MaxVerts;
                public int MaxIndecies;

                public Vector3[] Verticies;
                public Vector2[] Uvs;
                public Vector2[] Flags;
                public Color32[] Colors;
                public int[] Indecies;

                public int LastIndeciesLength = 0;

                public bool SharedArrays = false;

                public MeshDef(int maxVerts)
                {
                    MaxVerts = maxVerts;
                    MaxIndecies = maxVerts / 4 * 6;

                    Verticies = new Vector3[MaxVerts];
                    Uvs = new Vector2[MaxVerts];
                    Flags = new Vector2[MaxVerts];
                    Colors = new Color32[MaxVerts];
                    Indecies = new int[MaxIndecies];

                    Mesh.MarkDynamic();

                    // Pre-populate the mesh object at startup so there are no hiccups later
                    Mesh.vertices = Verticies;
                    Mesh.uv = Uvs;
                    Mesh.uv2 = Flags;
                    Mesh.colors32 = Colors;
                    Mesh.SetIndices(Indecies, MeshTopology.Triangles, 0, false);
                    Mesh.UploadMeshData(false);

                    LastIndeciesLength = 0;
                }

                public void Upload()
                {
                    // Upload meshes
                    Mesh.vertices = Verticies;
                    Mesh.uv = Uvs;
                    Mesh.uv2 = Flags;
                    Mesh.colors32 = Colors;
                    Mesh.SetIndices(Indecies, MeshTopology.Triangles, 0, false);
                    Mesh.UploadMeshData(false);
                }
            }
        }
    }
}
