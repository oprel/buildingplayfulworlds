namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Title scene
    /// </summary>
    public class SceneTitle : Scene
    {
        private EntityFlag mFlagOne;
        private EntityFlag mFlagTwo;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public override bool Initialize()
        {
            base.Initialize();

            mFlagOne = new EntityFlag(new Vector2((FES.SpriteSize().width * 2) - 1, (FES.SpriteSize().height * 3) + 8), FES.PLAYER_ONE);
            mFlagTwo = new EntityFlag(new Vector2(FES.DisplaySize.width - FES.SpriteSize().width - 8, (FES.SpriteSize().height * 3) + 8), FES.PLAYER_TWO);

            return true;
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();

            float c0 = Mathf.Sin(Time.realtimeSinceStartup * 2) + 1.0f;
            c0 /= 2.0f;
            c0 = (c0 * (255 - 125)) + 125;

            byte r = (byte)c0;
            byte g = (byte)c0;
            byte b = (byte)(c0 / 4);

            FES.PaletteColorSet(31, new ColorRGBA(r, g, b));
            r = (byte)(r / 1.5f);
            g = (byte)(g / 1.5f);
            b = (byte)(b / 1.5f);
            FES.PaletteColorSet(30, new ColorRGBA(r, g, b));

            if (!TransitionDone())
            {
                return;
            }

            if (FES.ButtonPressed(FES.BTN_ABXY, FES.PLAYER_ANY) || FES.ButtonPressed(FES.BTN_POINTER_ABC, FES.PLAYER_ANY))
            {
                SceneGame scene = new SceneGame();
                scene.Initialize();
                SuperFlagRun game = (SuperFlagRun)FES.Game;
                game.SwitchScene(scene);

                FES.SoundPlay(SuperFlagRun.SOUND_START_GAME);
            }

            mFlagOne.Update();
            mFlagTwo.Update();
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            FES.Clear(22);

            FES.CameraReset();

            DrawScrollingSky();

            FES.CameraSet(new Vector2i(FES.SpriteSize().width, -FES.SpriteSize().height * 7));

            FES.DrawMapLayer(SuperFlagRun.MAP_LAYER_TITLE_DECO);
            FES.DrawMapLayer(SuperFlagRun.MAP_LAYER_TITLE_TERRAIN);

            // Draw Flags
            mFlagOne.Render();
            mFlagTwo.Render();

            // Draw Players
            int x = (FES.SpriteSize().width * 3) + 8;
            int y = FES.SpriteSize().height * 3;
            FES.DrawSprite(FES.SpriteIndex(0, 2), new Vector2i(x, y), 0);
            FES.DrawSprite(FES.SpriteIndex(0, 3), new Vector2i(x, y + FES.SpriteSize().height), 0);

            x = FES.DisplaySize.width - (FES.SpriteSize().width * 2) - 8;
            FES.PaletteSwapSet(SuperFlagRun.PALETTE_SWAP_PLAYER_TWO);
            FES.DrawSprite(FES.SpriteIndex(0, 2), new Vector2i(x, y), FES.FLIP_H);
            FES.DrawSprite(FES.SpriteIndex(0, 3), new Vector2i(x, y + FES.SpriteSize().height), FES.FLIP_H);
            FES.PaletteSwapSet(0);

            // Draw Castles
            FES.DrawCopy(new Rect2i(0, 64, 48, 64), new Vector2i(FES.SpriteSize().width * 2, FES.SpriteSize().height * 4));
            FES.PaletteSwapSet(SuperFlagRun.PALETTE_SWAP_PLAYER_TWO);
            FES.DrawCopy(new Rect2i(0, 64, 48, 64), new Vector2i(FES.DisplaySize.width - (FES.SpriteSize().width * 3), FES.SpriteSize().height * 4), 0);
            FES.PaletteSwapSet(0);

            // Draw Title
            FES.CameraReset();
            FES.SpriteSheetSet(SuperFlagRun.SPRITESHEET_TITLE);
            FES.DrawCopy(new Rect2i(0, 0, 323, 118), new Vector2i((FES.DisplaySize.width / 2) - (323 / 2), (int)(Mathf.Sin(Time.realtimeSinceStartup * 2) * 6) + 15));
            FES.SpriteSheetSet(SuperFlagRun.SPRITESHEET_SPRITES);

            // Draw Press Any Button
            string str = "PRESS ANY BUTTON";
            Size2i textSize = FES.PrintMeasure(SuperFlagRun.GAME_FONT, str);
            FES.Print(SuperFlagRun.GAME_FONT, new Vector2i((FES.DisplaySize.width / 2) - (textSize.width / 2), (int)(FES.DisplaySize.height * 0.55f)), 0, str);

            FES.Print(new Vector2i(2, FES.DisplaySize.height - 9), 0, "FES technical demo game");

            // Let base render last so it can overlay the scene
            base.Render();
        }

        private void DrawScrollingSky()
        {
            SuperFlagRun game = (SuperFlagRun)FES.Game;

            int totalMapWidth = game.GameMapSize.width * FES.SpriteSize().width;
            int offset = (int)(Time.realtimeSinceStartup * 25) % totalMapWidth;

            FES.CameraSet(new Vector2i(offset, 0));
            FES.DrawMapLayer(SuperFlagRun.MAP_LAYER_SKY);

            FES.CameraSet(new Vector2i(offset - totalMapWidth, 0));
            FES.DrawMapLayer(SuperFlagRun.MAP_LAYER_SKY);
        }
    }
}
