namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Flag slot that enemy flag can be placed into
    /// </summary>
    public class EntityFlagSlot : Entity
    {
        private int mPlayerNum;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="playerNum">Which player the slot belongs to</param>
        public EntityFlagSlot(Vector2 pos, int playerNum) : base(pos)
        {
            mPlayerNum = playerNum;
            mColliderInfo.Rect = new Rect2i(2, 3, FES.SpriteSize().width - 6, (FES.SpriteSize().height * 2) - 4);
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            FES.PaletteSwapSet(mPlayerNum == FES.PLAYER_ONE ? SuperFlagRun.PALETTE_SWAP_PLAYER_TWO : 0);
            FES.DrawSprite(FES.SpriteIndex(4, 6), new Vector2i((int)Pos.x + 4, (int)Pos.y + (int)(Mathf.Sin(Time.realtimeSinceStartup * 8) * 3) - 1), 0);
            FES.PaletteSwapSet(0);
        }
    }
}
