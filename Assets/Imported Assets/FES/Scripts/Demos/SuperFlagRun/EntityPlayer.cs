namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Player entity
    /// </summary>
    public class EntityPlayer : EntityMovable
    {
        private bool mJumpKeyDown = false;
        private int mPlayerNum;
        private int mDirX = 1;
        private float mFrameIndex = 0;
        private float mRunAnimSpeed = 0.125f;
        private int[] mRunFrames = new int[] { 0, 1, 0, 2 };
        private int mLastFrameIndex = 0;

        private EntityFlag mCarriedFlag = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="playerNum">Player number of this player</param>
        public EntityPlayer(Vector2 pos, int playerNum) : base(pos)
        {
            mPlayerNum = playerNum;
            mColliderInfo.Rect = new Rect2i(2, 3, FES.SpriteSize().width - 6, (FES.SpriteSize().height * 2) - 4);

            if (playerNum == FES.PLAYER_ONE)
            {
                mDirX = 1;
            }
            else
            {
                mDirX = -1;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();
            if (Mathf.Abs(mPhysics.Velocity.x) > 0)
            {
                mFrameIndex += Mathf.Abs(mPhysics.Velocity.x) * mRunAnimSpeed;
                if (mFrameIndex >= mRunFrames.Length)
                {
                    mFrameIndex = 0;
                }
            }
            else
            {
                mFrameIndex = 0;
            }

            if (!mPhysics.IsOnGround)
            {
                mFrameIndex = 0;
            }

            int newFrameIndex = (int)mFrameIndex;
            if (mLastFrameIndex != newFrameIndex && (newFrameIndex == 1 || newFrameIndex == 3))
            {
                FES.SoundPlay(SuperFlagRun.SOUND_FOOT_STEP, 0.35f, Random.Range(0.7f, 1.3f));
            }

            mLastFrameIndex = newFrameIndex;

            SuperFlagRun game = (SuperFlagRun)FES.Game;
            SceneGame scene = (SceneGame)game.CurrentScene;

            if (mCarriedFlag == null && scene.GetWinner() == 0)
            {
                EntityFlag flag = scene.GetEnemyFlag(mPlayerNum);

                if (flag.ColliderInfo.Rect.Offset(flag.Pos).Intersects(ColliderInfo.Rect.Offset(Pos)))
                {
                    mCarriedFlag = flag;
                    FES.SoundPlay(SuperFlagRun.SOUND_PICKUP_FLAG);
                }
            }
            else if (mCarriedFlag != null)
            {
                if ((int)mFrameIndex == 1 || (int)mFrameIndex == 2)
                {
                    mCarriedFlag.Pos = new Vector2(Pos.x, Pos.y - 10);
                }
                else
                {
                    mCarriedFlag.Pos = new Vector2(Pos.x, Pos.y - 11);
                }

                if (scene.GetWinner() == 0)
                {
                    EntityFlagSlot flagSlot = scene.GetFlagSlot(mPlayerNum);
                    if (flagSlot.ColliderInfo.Rect.Offset(flagSlot.Pos).Intersects(ColliderInfo.Rect.Offset(Pos)))
                    {
                        FES.SoundPlay(SuperFlagRun.SOUND_DROP_FLAG);
                        scene.SetWinner(mPlayerNum);
                        mCarriedFlag.Pos = flagSlot.Pos;
                        mCarriedFlag = null;
                    }
                }
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            int flip = mDirX == 1 ? 0 : FES.FLIP_H;

            int frame = 0;

            if (mPhysics.IsOnGround)
            {
                frame = mRunFrames[(int)mFrameIndex];
            }
            else
            {
                frame = 3;
            }

            if (mPlayerNum == FES.PLAYER_ONE)
            {
                FES.DrawSprite(FES.SpriteIndex(frame, 2), Pos, flip);
                FES.DrawSprite(FES.SpriteIndex(frame, 3), new Vector2i(Pos.x, Pos.y + 16), flip);
            }
            else
            {
                FES.PaletteSwapSet(SuperFlagRun.PALETTE_SWAP_PLAYER_TWO);
                FES.DrawSprite(FES.SpriteIndex(frame, 2), Pos, flip);
                FES.DrawSprite(FES.SpriteIndex(frame, 3), new Vector2i(Pos.x, Pos.y + 16), flip);
                FES.PaletteSwapSet(0);
            }

            base.Render();
        }

        /// <summary>
        /// Update velocity
        /// </summary>
        protected override void UpdateVelocity()
        {
            float movementSpeed = 0.01f;
            Vector2 movementForce = new Vector2();

            SuperFlagRun game = (SuperFlagRun)FES.Game;
            SceneGame scene = (SceneGame)game.CurrentScene;

            if (scene.GetWinner() == 0)
            {
                if (FES.ButtonDown(FES.BTN_LEFT, mPlayerNum))
                {
                    movementForce.x -= movementSpeed;
                    mDirX = -1;
                }
                else if (FES.ButtonDown(FES.BTN_RIGHT, mPlayerNum))
                {
                    mDirX = 1;
                    movementForce.x += movementSpeed;
                }

                if (FES.ButtonDown(FES.BTN_ABXY, mPlayerNum))
                {
                    if (!mJumpKeyDown && mPhysics.IsOnGround)
                    {
                        mPhysics.Jump();
                        mJumpKeyDown = true;
                    }
                }

                if (!FES.ButtonDown(FES.BTN_ABXY, mPlayerNum))
                {
                    mJumpKeyDown = false;
                }
            }

            if (!mJumpKeyDown)
            {
                mPhysics.Velocity = new Vector2(mPhysics.Velocity.x, Mathf.Max(-1, mPhysics.Velocity.y));
            }

            mPhysics.AddForce(new Vector2(0, PlatformPhysics.GRAVITY));

            mPhysics.AddMovementForce(movementForce * mPhysics.MoveAccel);
        }
    }
}
