namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Main game scene
    /// </summary>
    public class SceneGame : Scene
    {
        /// <summary>
        /// The width of the game level, in tiles
        /// </summary>
        public const int LEVEL_WIDTH = 181;

        private EntityPlayer mPlayerOne;
        private EntityPlayer mPlayerTwo;

        private EntityFlag mFlagOne;
        private EntityFlag mFlagTwo;

        private EntityFlagSlot mFlagOneSlot;
        private EntityFlagSlot mFlagTwoSlot;

        private int mWinningPlayer = 0;
        private float mTimeoutUntilReset = 5.0f;

        /// <summary>
        /// Get enemy flag
        /// </summary>
        /// <param name="playerNum">What player to get flag for</param>
        /// <returns>Enemy flag</returns>
        public EntityFlag GetEnemyFlag(int playerNum)
        {
            if (playerNum == FES.PLAYER_ONE)
            {
                return mFlagTwo;
            }
            else if (playerNum == FES.PLAYER_TWO)
            {
                return mFlagOne;
            }

            return null;
        }

        /// <summary>
        /// Get flag slot
        /// </summary>
        /// <param name="playerNum">Whater player to get slot for</param>
        /// <returns>Flag slot</returns>
        public EntityFlagSlot GetFlagSlot(int playerNum)
        {
            if (playerNum == FES.PLAYER_ONE)
            {
                return mFlagOneSlot;
            }
            else if (playerNum == FES.PLAYER_TWO)
            {
                return mFlagTwoSlot;
            }

            return null;
        }

        /// <summary>
        /// Set the winning player
        /// </summary>
        /// <param name="playerNum">Winning player</param>
        public void SetWinner(int playerNum)
        {
            if (mWinningPlayer == 0)
            {
                mWinningPlayer = playerNum;
            }
        }

        /// <summary>
        /// Get the winning palyer
        /// </summary>
        /// <returns>Winning player</returns>
        public int GetWinner()
        {
            return mWinningPlayer;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public override bool Initialize()
        {
            base.Initialize();

            mPlayerOne = new EntityPlayer(new Vector2(FES.SpriteSize().width * 2, 420), FES.PLAYER_ONE);
            mPlayerTwo = new EntityPlayer(new Vector2(((LEVEL_WIDTH - 1) * FES.SpriteSize().width) - (FES.SpriteSize().width * 2), 420), FES.PLAYER_TWO);

            mFlagOne = new EntityFlag(new Vector2(FES.SpriteSize().width, (FES.SpriteSize().height * 25) + 5), FES.PLAYER_ONE);
            mFlagTwo = new EntityFlag(new Vector2(((LEVEL_WIDTH - 2) * FES.SpriteSize().width) - 8, (FES.SpriteSize().height * 25) + 5), FES.PLAYER_TWO);

            mFlagOneSlot = new EntityFlagSlot(new Vector2((FES.SpriteSize().width * 3) - 8, (FES.SpriteSize().height * 25) + 5), FES.PLAYER_ONE);
            mFlagTwoSlot = new EntityFlagSlot(new Vector2((LEVEL_WIDTH - 4) * FES.SpriteSize().width, (FES.SpriteSize().height * 25) + 5), FES.PLAYER_TWO);

            return true;
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();

            mPlayerOne.Update();
            mPlayerTwo.Update();
            mFlagOne.Update();
            mFlagTwo.Update();

            if (mWinningPlayer != 0)
            {
                if (mTimeoutUntilReset > 0)
                {
                    mTimeoutUntilReset -= Time.deltaTime;

                    if (mTimeoutUntilReset <= 0)
                    {
                        SceneTitle scene = new SceneTitle();
                        scene.Initialize();
                        SuperFlagRun game = (SuperFlagRun)FES.Game;
                        game.SwitchScene(scene);
                    }
                }
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            FES.Clear(22);

            // Draw Player One view
            FES.CameraReset();
            FES.ClipSet(new Rect2i(0, 0, FES.DisplaySize.width, FES.DisplaySize.height / 2));

            var cameraOffset = GetCameraOffset(mPlayerOne);

            DrawScrollingSky((int)cameraOffset.x, (int)cameraOffset.y);

            FES.CameraSet(new Vector2i((int)cameraOffset.x, (int)cameraOffset.y));

            FES.DrawMapLayer(SuperFlagRun.MAP_LAYER_BACKGROUND);
            FES.DrawMapLayer(SuperFlagRun.MAP_LAYER_TERRAIN);

            mFlagOneSlot.Render();
            mFlagTwoSlot.Render();

            mFlagOne.Render();
            mFlagTwo.Render();

            mPlayerTwo.Render();
            mPlayerOne.Render();

            // Draw Castles
            FES.DrawCopy(new Rect2i(0, 64, 48, 64), new Vector2i(16, FES.SpriteSize().height * 26));
            FES.PaletteSwapSet(SuperFlagRun.PALETTE_SWAP_PLAYER_TWO);
            FES.DrawCopy(new Rect2i(0, 64, 48, 64), new Vector2i((FES.SpriteSize().width * LEVEL_WIDTH) - 64, FES.SpriteSize().height * 26), 0);
            FES.PaletteSwapSet(0);

            // Draw Player Two view
            FES.ClipSet(new Rect2i(0, FES.DisplaySize.height / 2, FES.DisplaySize.width, FES.DisplaySize.height / 2));

            cameraOffset = GetCameraOffset(mPlayerTwo);

            DrawScrollingSky((int)cameraOffset.x, (int)cameraOffset.y - (FES.DisplaySize.height / 2));

            cameraOffset = GetCameraOffset(mPlayerTwo);
            FES.CameraSet(new Vector2i((int)cameraOffset.x, (int)cameraOffset.y - (FES.DisplaySize.height / 2)));

            FES.DrawMapLayer(SuperFlagRun.MAP_LAYER_BACKGROUND);
            FES.DrawMapLayer(SuperFlagRun.MAP_LAYER_TERRAIN);

            mFlagOneSlot.Render();
            mFlagTwoSlot.Render();

            mFlagOne.Render();
            mFlagTwo.Render();

            mPlayerOne.Render();
            mPlayerTwo.Render();

            // Draw Castles
            FES.DrawCopy(new Rect2i(0, 64, 48, 64), new Vector2i(16, FES.SpriteSize().height * 26));
            FES.PaletteSwapSet(SuperFlagRun.PALETTE_SWAP_PLAYER_TWO);
            FES.DrawCopy(new Rect2i(0, 64, 48, 64), new Vector2i((FES.SpriteSize().width * LEVEL_WIDTH) - 64, FES.SpriteSize().height * 26), 0);
            FES.PaletteSwapSet(0);

            FES.ClipReset();
            FES.CameraReset();

            // Draw divider
            for (int x = 0; x < FES.DisplaySize.width; x += 16)
            {
                FES.DrawSprite(FES.SpriteIndex(0, 0), new Vector2i(x, (FES.DisplaySize.height / 2) - 4));
            }

            if (mWinningPlayer != 0)
            {
                string playerOneStr = "LOSER";
                string playerTwoStr = "WINNER";
                if (mWinningPlayer == FES.PLAYER_ONE)
                {
                    playerOneStr = "WINNER";
                    playerTwoStr = "LOSER";
                }

                int textOffsetX = (int)(Mathf.Cos(Time.realtimeSinceStartup * 6.0f) * 8);
                int textOffsetY = (int)(Mathf.Sin(Time.realtimeSinceStartup * 6.0f) * 5);
                Size2i textSize;
                string text = playerOneStr;
                textSize = FES.PrintMeasure(SuperFlagRun.GAME_FONT, text);
                FES.Print(SuperFlagRun.GAME_FONT, new Vector2i((FES.DisplaySize.width / 2) - (textSize.width / 2) + textOffsetX, (FES.DisplaySize.height / 4) - (textSize.height / 2) + textOffsetY), 0, text);

                text = playerTwoStr;
                textSize = FES.PrintMeasure(SuperFlagRun.GAME_FONT, text);
                FES.Print(SuperFlagRun.GAME_FONT, new Vector2i((FES.DisplaySize.width / 2) - (textSize.width / 2) + textOffsetX, (FES.DisplaySize.height / 4 * 3) - (textSize.height / 2) + textOffsetY), 0, text);
            }

            // Let base render last so it can overlay the scene
            base.Render();
        }

        /// <summary>
        /// Enter the scene
        /// </summary>
        public override void Enter()
        {
            // Title page animates some colors, restore them here
            FES.PaletteColorSet(31, new ColorRGBA(153, 59, 145));
            FES.PaletteColorSet(30, new ColorRGBA(182, 70, 173));

            base.Enter();
        }

        private Vector2 GetCameraOffset(EntityPlayer player)
        {
            // Clip the camera, note we clip first and last column of tiles out because they are special invisible tiles that
            // block movement, they're not meant to be shown
            int cameraX = (int)player.Pos.x - (FES.DisplaySize.width / 2) + (FES.SpriteSize().width / 2);
            if (cameraX < FES.SpriteSize().width)
            {
                cameraX = FES.SpriteSize().width;
            }

            if (cameraX > (FES.SpriteSize().width * LEVEL_WIDTH) - FES.DisplaySize.width - FES.SpriteSize().width)
            {
                cameraX = (FES.SpriteSize().width * LEVEL_WIDTH) - FES.DisplaySize.width - FES.SpriteSize().width;
            }

            int cameraY = (int)player.Pos.y - (int)(FES.SpriteSize().height * 2.5f);
            if (cameraY < FES.SpriteSize().height)
            {
                cameraY = FES.SpriteSize().height;
            }

            SuperFlagRun game = (SuperFlagRun)FES.Game;
            if (cameraY > (FES.SpriteSize().height * game.GameMapSize.height) - (FES.DisplaySize.height / 2) - FES.SpriteSize().height)
            {
                cameraY = (FES.SpriteSize().height * game.GameMapSize.height) - (FES.DisplaySize.height / 2) - FES.SpriteSize().height;
            }

            return new Vector2(cameraX, cameraY);
        }

        private void DrawScrollingSky(int xoffset, int yoffset)
        {
            SuperFlagRun game = (SuperFlagRun)FES.Game;

            int totalMapWidth = game.GameMapSize.width * FES.SpriteSize().width;
            int offset = (int)(Time.realtimeSinceStartup * 25) % totalMapWidth;

            FES.CameraSet(new Vector2i(xoffset + offset, yoffset));
            FES.DrawMapLayer(SuperFlagRun.MAP_LAYER_SKY);

            FES.CameraSet(new Vector2i(xoffset + offset - totalMapWidth, yoffset));
            FES.DrawMapLayer(SuperFlagRun.MAP_LAYER_SKY);
        }
    }
}
