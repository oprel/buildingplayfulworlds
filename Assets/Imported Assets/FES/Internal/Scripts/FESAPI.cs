namespace FESInternal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Internal wrapper class for all the FES subsystems
    /// </summary>
    public class FESAPI : MonoBehaviour
    {
        private FESHW mHW;
        private FESRenderer mRenderer;
        private FESPixelCamera mPixelCamera;
        private FESPalette mPalette;
        private FESFont mFont;
        private FESTilemap mTilemap;
        private FESInput mInput;
        private FESAudio mAudio;
        private FESEffects mEffects;
        private FESResourceBucket mResourceBucket;
        private FESPerf mPerf;

        private ulong mTicks = 0;

        private bool mInitialized = false;

        /// <summary>
        /// Get initialized state
        /// </summary>
        public bool Initialized
        {
            get { return mInitialized; }
        }

        /// <summary>
        /// Get Hardware subsystem
        /// </summary>
        public FESHW HW
        {
            get { return mHW; }
        }

        /// <summary>
        /// Gets Renderer subsystem
        /// </summary>
        public FESRenderer Renderer
        {
            get { return mRenderer; }
        }

        /// <summary>
        /// Gets PixelCamera subsystem
        /// </summary>
        public FESPixelCamera PixelCamera
        {
            get { return mPixelCamera; }
        }

        /// <summary>
        /// Get Palette subsystem
        /// </summary>
        public FESPalette Palette
        {
            get { return mPalette; }
        }

        /// <summary>
        /// Get Font subsystem
        /// </summary>
        public FESFont Font
        {
            get { return mFont; }
        }

        /// <summary>
        /// Get Tilemap subsystem
        /// </summary>
        public FESTilemap Tilemap
        {
            get { return mTilemap; }
        }

        /// <summary>
        /// Get Input subsystem
        /// </summary>
        public FESInput Input
        {
            get { return mInput; }
        }

        /// <summary>
        /// Get Audio subsystem
        /// </summary>
        public FESAudio Audio
        {
            get { return mAudio; }
        }

        /// <summary>
        /// Get Effects subsystem
        /// </summary>
        public FESEffects Effects
        {
            get { return mEffects; }
        }

        /// <summary>
        /// Get FESResourceBucket
        /// </summary>
        public FESResourceBucket ResourceBucket
        {
            get { return mResourceBucket; }
        }

        /// <summary>
        /// Get FESPerf
        /// </summary>
        public FESPerf Perf
        {
            get { return mPerf; }
        }

        /// <summary>
        /// Ticks since startup, or since TicksReset was called
        /// </summary>
        public ulong Ticks
        {
            get { return mTicks; }
        }

        /// <summary>
        /// Reset ticks
        /// </summary>
        public void TicksReset()
        {
            mTicks = 0;
        }

        /// <summary>
        /// Initialize the subsystem wrapper
        /// </summary>
        /// <param name="settings">Hardware settings to initialize with</param>
        /// <returns>True if successful</returns>
        public bool Initialize(FES.HardwareSettings settings)
        {
            var resourceBucketObj = GameObject.Find("ResourceBucket");
            if (resourceBucketObj == null)
            {
                Debug.Log("Can't find ResourceBucket game object");
                return false;
            }

            mResourceBucket = resourceBucketObj.GetComponent<FESResourceBucket>();
            if (mResourceBucket == null)
            {
                return false;
            }

            mHW = new FESHW();
            if (mHW == null || !mHW.Initialize(settings))
            {
                return false;
            }

            var cameraObj = GameObject.Find("FESPixelCamera");
            if (cameraObj == null)
            {
                Debug.Log("Can't find FESPixelCamera game object");
                return false;
            }

            mPixelCamera = cameraObj.GetComponent<FESPixelCamera>();
            if (mPixelCamera == null || !mPixelCamera.Initialize(this))
            {
                return false;
            }

            mRenderer = new FESRenderer();
            if (mRenderer == null || !mRenderer.Initialize(this))
            {
                return false;
            }

            if (HW.ColorMode == FES.ColorMode.Indexed)
            {
                mPalette = new FESPalette();
                if (mPalette == null || !mPalette.Initialize(this))
                {
                    return false;
                }
            }
            else
            {
                mPalette = null;
            }

            mFont = new FESFont();
            if (mFont == null || !mFont.Initialize(this))
            {
                return false;
            }

            mTilemap = new FESTilemap();
            if (mTilemap == null || !mTilemap.Initialize(this))
            {
                return false;
            }

            mInput = new FESInput();
            if (mInput == null || !mInput.Initialize(this))
            {
                return false;
            }

            var audioObj = GameObject.Find("FESAudio");
            if (audioObj == null)
            {
                Debug.Log("Can't find FESAudio game object");
                return false;
            }

            mAudio = audioObj.GetComponent<FESAudio>();
            if (mAudio == null || !mAudio.Initialize(this))
            {
                return false;
            }

            mEffects = new FESEffects();
            if (mEffects == null || !mEffects.Initialize(this))
            {
                return false;
            }

            mPerf = new FESPerf();
            if (mPerf == null || !mPerf.Initialize(this))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Finalize initialization, this is called after the game is initialized
        /// </summary>
        /// <param name="initialized">True if initialized successfuly</param>
        public void FinalizeInitialization(bool initialized)
        {
            mInitialized = initialized;
        }

        private void Start()
        {
#if UNITY_EDITOR
            // Debug.Log("Disabling live recompilation");
            UnityEditor.EditorApplication.LockReloadAssemblies();
#endif
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            // Debug.Log("Enabling live recompilation");
            UnityEditor.EditorApplication.UnlockReloadAssemblies();
#endif
        }

        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            // Debug.Log("Enabling live recompilation");
            UnityEditor.EditorApplication.UnlockReloadAssemblies();
#endif
        }

        // Input string has to be assembled from Update() because FixedUpdate() will probably drop characters
        private void Update()
        {
            if (!mInitialized)
            {
                return;
            }

            if (Input != null)
            {
                Input.AppendInputString(UnityEngine.Input.inputString);
                Input.UpdateScrollWheel();
            }
        }

        /// <summary>
        /// Heart beat of FES. FES runs on a fixed update.
        /// </summary>
        private void FixedUpdate()
        {
            if (!mInitialized)
            {
                return;
            }

            if (Input != null)
            {
                Input.FrameStart();
            }

            var game = FES.Game;
            if (game != null)
            {
                game.Update();
                mTicks++;
            }

            if (Perf != null)
            {
                Perf.UpdateEvent();
            }

            if (Input != null)
            {
                Input.FrameEnd();
            }

            if (Tilemap != null)
            {
                Tilemap.FrameEnd();
            }
        }
    }
}
