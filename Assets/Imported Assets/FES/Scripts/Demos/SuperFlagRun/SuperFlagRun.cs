namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Simple platformer demo game, with custom physics engine
    /// </summary>
    public class SuperFlagRun : FES.IFESGame
    {
        /// <summary>
        /// Player two palette swap slot
        /// </summary>
        public const int PALETTE_SWAP_PLAYER_TWO = 1;

        /// <summary>
        /// Game font
        /// </summary>
        public const int GAME_FONT = 0;

        /// <summary>
        /// Terrain layer
        /// </summary>
        public const int MAP_LAYER_TERRAIN = 0;

        /// <summary>
        /// Background layer
        /// </summary>
        public const int MAP_LAYER_BACKGROUND = 1;

        /// <summary>
        /// Sky layer
        /// </summary>
        public const int MAP_LAYER_SKY = 2;

        /// <summary>
        /// Title terrain layer
        /// </summary>
        public const int MAP_LAYER_TITLE_TERRAIN = 3;

        /// <summary>
        /// Title deco layer
        /// </summary>
        public const int MAP_LAYER_TITLE_DECO = 4;

        /// <summary>
        /// SpriteSheet sprites
        /// </summary>
        public const int SPRITESHEET_SPRITES = 0;

        /// <summary>
        /// SpriteSheet title
        /// </summary>
        public const int SPRITESHEET_TITLE = 1;

        /// <summary>
        /// SpriteSheet terrain
        /// </summary>
        public const int SPRITESHEET_TERRAIN = 2;

        /// <summary>
        /// SpriteSheet deco
        /// </summary>
        public const int SPRITESHEET_DECO = 3;

        /// <summary>
        /// Jump sound
        /// </summary>
        public const int SOUND_JUMP = 0;

        /// <summary>
        /// Flag pickup sound
        /// </summary>
        public const int SOUND_PICKUP_FLAG = 1;

        /// <summary>
        /// Flag drop sound
        /// </summary>
        public const int SOUND_DROP_FLAG = 2;

        /// <summary>
        /// Start game sound
        /// </summary>
        public const int SOUND_START_GAME = 3;

        /// <summary>
        /// Foot step sound
        /// </summary>
        public const int SOUND_FOOT_STEP = 4;

        private Size2i mGameMapSize;
        private Scene mCurrentScene;
        private Scene mNextScene;

        /// <summary>
        /// Get the size of the game map
        /// </summary>
        public Size2i GameMapSize
        {
            get { return mGameMapSize; }
        }

        /// <summary>
        /// Get the current scene
        /// </summary>
        public Scene CurrentScene
        {
            get { return mCurrentScene; }
        }

        /// <summary>
        /// Query hardware
        /// </summary>
        /// <returns>Hardware settings</returns>
        public FES.HardwareSettings QueryHardware()
        {
            var hw = new FES.HardwareSettings();

            hw.MapSize = new Size2i(200, 32);
            hw.MapLayers = 5;
            hw.DisplaySize = new Size2i(480, 270);
            hw.ColorMode = FES.ColorMode.Indexed;

            return hw;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public bool Initialize()
        {
            FES.SpriteSheetSetup(SPRITESHEET_SPRITES, "Demos/SuperFlagRun/Sprites", new Size2i(16, 16));
            FES.SpriteSheetSetup(SPRITESHEET_TITLE, "Demos/SuperFlagRun/SpritesTitle", new Size2i(16, 16));
            FES.SpriteSheetSetup(SPRITESHEET_TERRAIN, "Demos/SuperFlagRun/TilemapTerrain", new Size2i(16, 16));
            FES.SpriteSheetSetup(SPRITESHEET_DECO, "Demos/SuperFlagRun/TilemapDeco", new Size2i(16, 16));

            FES.SpriteSheetSet(SPRITESHEET_SPRITES);

            int[] colorSwaps = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 27, 13, 25, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 18, 26, 27, 28, 29, 30, 31 };
            FES.PaletteSwapSetup(PALETTE_SWAP_PLAYER_TWO, colorSwaps);

            FES.FontSetup(GAME_FONT, (int)'A', (int)'Z', new Vector2i(0, 130), 0, new Size2i(12, 12), 6, 1, 2, false, true);

            if (!LoadMap())
            {
                return false;
            }

            FES.SoundSetup(SOUND_JUMP, "Demos/SuperFlagRun/Jump");
            FES.SoundSetup(SOUND_PICKUP_FLAG, "Demos/SuperFlagRun/Pickup");
            FES.SoundSetup(SOUND_DROP_FLAG, "Demos/SuperFlagRun/DropFlag");
            FES.SoundSetup(SOUND_START_GAME, "Demos/SuperFlagRun/StartGame");
            FES.SoundSetup(SOUND_FOOT_STEP, "Demos/SuperFlagRun/FootStep");

            FES.MusicSetup(0, "Demos/SuperFlagRun/SuperFlagRunMusic");
            FES.MusicVolumeSet(0.5f);
            FES.MusicPlay(0);

            SceneTitle scene = new SceneTitle();
            scene.Initialize();

            SwitchScene(scene);

            FES.EffectSet(FES.Effect.Scanlines, 0.25f);
            FES.EffectSet(FES.Effect.Noise, 0.05f);

            return true;
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            if (FES.ButtonPressed(FES.BTN_SYSTEM))
            {
                Application.Quit();
            }

            if (mCurrentScene != null)
            {
                mCurrentScene.Update();
            }

            if (mNextScene != null)
            {
                if (mCurrentScene.TransitionDone())
                {
                    mCurrentScene = mNextScene;
                    mCurrentScene.Enter();
                    mNextScene = null;
                }
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            mCurrentScene.Render();
        }

        /// <summary>
        /// Switch to a new scene
        /// </summary>
        /// <param name="newScene">New scene</param>
        public void SwitchScene(Scene newScene)
        {
            if (mCurrentScene == null)
            {
                mCurrentScene = newScene;
                mCurrentScene.Enter();
            }
            else
            {
                mNextScene = newScene;
                mCurrentScene.Exit();
            }
        }

        private bool LoadMap()
        {
            if (!FES.MapLoadTMX("Demos/SuperFlagRun/TitleMap", "Terrain", SuperFlagRun.MAP_LAYER_TITLE_TERRAIN))
            {
                Debug.LogError("Can't load map");
                return false;
            }

            if (!FES.MapLoadTMX("Demos/SuperFlagRun/TitleMap", "Deco", SuperFlagRun.MAP_LAYER_TITLE_DECO))
            {
                Debug.LogError("Can't load map");
                return false;
            }

            if (!FES.MapLoadTMX("Demos/SuperFlagRun/GameMap", "Sky", SuperFlagRun.MAP_LAYER_SKY))
            {
                Debug.LogError("Can't load map");
                return false;
            }

            if (!FES.MapLoadTMX("Demos/SuperFlagRun/GameMap", "Terrain", SuperFlagRun.MAP_LAYER_TERRAIN, out mGameMapSize))
            {
                Debug.LogError("Can't load map");
                return false;
            }

            if (!FES.MapLoadTMX("Demos/SuperFlagRun/GameMap", "Background", SuperFlagRun.MAP_LAYER_BACKGROUND))
            {
                Debug.LogError("Can't load map");
                return false;
            }

            FES.MapLayerSpriteSheetSet(SuperFlagRun.MAP_LAYER_TITLE_TERRAIN, SPRITESHEET_TERRAIN);
            FES.MapLayerSpriteSheetSet(SuperFlagRun.MAP_LAYER_TITLE_DECO, SPRITESHEET_DECO);

            FES.MapLayerSpriteSheetSet(SuperFlagRun.MAP_LAYER_SKY, SPRITESHEET_DECO);
            FES.MapLayerSpriteSheetSet(SuperFlagRun.MAP_LAYER_TERRAIN, SPRITESHEET_TERRAIN);
            FES.MapLayerSpriteSheetSet(SuperFlagRun.MAP_LAYER_BACKGROUND, SPRITESHEET_DECO);

            // Post process map, set tile data for collision detection
            ColliderInfo.ColliderType[] colliderTypes = new ColliderInfo.ColliderType[32 * 32];

            FES.SpriteSheetSet(SuperFlagRun.SPRITESHEET_TERRAIN);

            colliderTypes[FES.SpriteIndex(0, 0)] = ColliderInfo.ColliderType.PLATFORM;
            colliderTypes[FES.SpriteIndex(1, 0)] = ColliderInfo.ColliderType.PLATFORM;
            colliderTypes[FES.SpriteIndex(3, 0)] = ColliderInfo.ColliderType.BLOCK;
            colliderTypes[FES.SpriteIndex(4, 0)] = ColliderInfo.ColliderType.PLATFORM;
            colliderTypes[FES.SpriteIndex(5, 0)] = ColliderInfo.ColliderType.PLATFORM;
            colliderTypes[FES.SpriteIndex(6, 0)] = ColliderInfo.ColliderType.BLOCK;
            colliderTypes[FES.SpriteIndex(7, 0)] = ColliderInfo.ColliderType.BLOCK;

            colliderTypes[FES.SpriteIndex(1, 1)] = ColliderInfo.ColliderType.BLOCK;
            colliderTypes[FES.SpriteIndex(3, 1)] = ColliderInfo.ColliderType.BLOCK;
            colliderTypes[FES.SpriteIndex(6, 1)] = ColliderInfo.ColliderType.BLOCK;
            colliderTypes[FES.SpriteIndex(7, 1)] = ColliderInfo.ColliderType.BLOCK;

            colliderTypes[FES.SpriteIndex(0, 2)] = ColliderInfo.ColliderType.PLATFORM;
            colliderTypes[FES.SpriteIndex(1, 2)] = ColliderInfo.ColliderType.PLATFORM;
            colliderTypes[FES.SpriteIndex(3, 2)] = ColliderInfo.ColliderType.BLOCK;
            colliderTypes[FES.SpriteIndex(4, 2)] = ColliderInfo.ColliderType.PLATFORM;
            colliderTypes[FES.SpriteIndex(6, 2)] = ColliderInfo.ColliderType.BLOCK;
            colliderTypes[FES.SpriteIndex(7, 2)] = ColliderInfo.ColliderType.BLOCK;

            colliderTypes[FES.SpriteIndex(1, 3)] = ColliderInfo.ColliderType.BLOCK;
            colliderTypes[FES.SpriteIndex(3, 3)] = ColliderInfo.ColliderType.BLOCK;

            colliderTypes[FES.SpriteIndex(0, 4)] = ColliderInfo.ColliderType.BLOCK;
            colliderTypes[FES.SpriteIndex(1, 4)] = ColliderInfo.ColliderType.BLOCK;

            for (int x = 0; x < mGameMapSize.width; x++)
            {
                for (int y = 0; y < mGameMapSize.height; y++)
                {
                    int tile = FES.MapSpriteGet(SuperFlagRun.MAP_LAYER_TERRAIN, new Vector2i(x, y));
                    if (tile != FES.SPRITE_EMPTY)
                    {
                        FES.MapDataSet<ColliderInfo.ColliderType>(new Vector2i(x, y), colliderTypes[tile]);
                    }
                }
            }

            return true;
        }
    }
}
