namespace FESDemo
{
    /// <summary>
    /// A simple game scene, supporting enter and exit transitions
    /// </summary>
    public class Scene
    {
        private TransitionState mTransitionState = TransitionState.DONE;
        private float mTransitionProgress = 0.0f;

        private enum TransitionState
        {
            DONE = 0,
            ENTERING = 1,
            EXITING = 2
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public virtual bool Initialize()
        {
            return true;
        }

        /// <summary>
        /// Called when the scene is entered
        /// </summary>
        public virtual void Enter()
        {
            mTransitionProgress = 0.0f;
            mTransitionState = TransitionState.ENTERING;
        }

        /// <summary>
        /// Called when the scene is exited
        /// </summary>
        public virtual void Exit()
        {
            mTransitionProgress = 0.0f;
            mTransitionState = TransitionState.EXITING;
        }

        /// <summary>
        /// Check if transition is done
        /// </summary>
        /// <returns>True if transition is done</returns>
        public bool TransitionDone()
        {
            return mTransitionState == TransitionState.DONE;
        }

        /// <summary>
        /// Update
        /// </summary>
        public virtual void Update()
        {
            if (mTransitionState == TransitionState.ENTERING || mTransitionState == TransitionState.EXITING)
            {
                mTransitionProgress += 0.025f;
                if (mTransitionProgress >= 1.0f)
                {
                    mTransitionState = TransitionState.DONE;
                }
            }
        }

        /// <summary>
        /// Render transition effect
        /// </summary>
        public virtual void Render()
        {
            FES.CameraReset();

            if (mTransitionState == TransitionState.ENTERING)
            {
                FES.EffectSet(FES.Effect.Pinhole, 1.0f - mTransitionProgress, new Vector2i(FES.DisplaySize.width / 2, FES.DisplaySize.height / 2), 0);
            }
            else if (mTransitionState == TransitionState.EXITING)
            {
                FES.EffectSet(FES.Effect.Pinhole, mTransitionProgress, new Vector2i(FES.DisplaySize.width / 2, FES.DisplaySize.height / 2), 0);
            }
            else
            {
                FES.EffectSet(FES.Effect.Pinhole, 0);
            }
        }
    }
}
