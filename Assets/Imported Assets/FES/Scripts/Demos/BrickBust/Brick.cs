using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Standard issue brick, can be of varying colors, each color having different amount of hits required to
/// destroy it
/// </summary>
public class Brick : Collidable
{
    /// <summary>
    /// Hits lefts before this brick is destroyed
    /// </summary>
    public int Life = 1;

    /// <summary>
    /// Maximum hits the brick has
    /// </summary>
    public int MaxLife = 1;

    /// <summary>
    /// Score points awarded for destroying this brick
    /// </summary>
    public int Score;

    /// <summary>
    /// What type/color this brick is
    /// </summary>
    public BrickType Type;

    private float mAlpha;
    private int ColorSwap;
    private Rect2i LastSpriteRect;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pos">Position of the brick</param>
    /// <param name="type">Type of the brick</param>
    public Brick(Vector2i pos, BrickType type)
    {
        Rect = new Rect2i(pos.x, pos.y, C.BRICK_WIDTH, C.BRICK_HEIGHT);

        Type = type;

        switch (Type)
        {
            case BrickType.Blue:
                ColorSwap = C.SWAP_BLUE_BRICK;
                Life = 2;
                Score = 20;
                RequiredForWin = true;
                break;

            case BrickType.Green:
                ColorSwap = C.SWAP_GREEN_BRICK;
                Life = 1;
                Score = 10;
                RequiredForWin = true;
                break;

            case BrickType.Gold:
                ColorSwap = C.SWAP_GOLD_BRICK;
                Life = 5;
                Score = 50;
                RequiredForWin = true;
                break;

            case BrickType.Brown:
                ColorSwap = C.SWAP_BROWN_BRICK;
                Life = 3;
                Score = 30;
                RequiredForWin = true;
                break;

            case BrickType.Pink:
                ColorSwap = C.SWAP_PINK_BRICK;
                Life = 4;
                Score = 40;
                RequiredForWin = true;
                break;

            case BrickType.Black:
                ColorSwap = C.SWAP_BLACK_BRICK;
                Life = 6;
                Score = 60;
                RequiredForWin = true;
                break;

            case BrickType.Block:
                ColorSwap = 0;
                Life = int.MaxValue;
                Score = 0;
                RequiredForWin = false;
                break;
        }

        MaxLife = Life;

        mAlpha = UnityEngine.Random.Range(-0.5f, 0.0f);
    }

    /// <summary>
    /// Type of brick. We could instead create a Brick base class and inherit brick colors from it,
    /// but Bricks don't differ from each other much, and it would just create too many classes
    /// </summary>
    public enum BrickType
    {
        /// <summary>
        /// No brick at all
        /// </summary>
        None,

        /// <summary>
        /// Blue brick
        /// </summary>
        Blue,

        /// <summary>
        /// Green brick
        /// </summary>
        Green,

        /// <summary>
        /// Gold brick
        /// </summary>
        Gold,

        /// <summary>
        /// Brown brick
        /// </summary>
        Brown,

        /// <summary>
        /// Pink brick
        /// </summary>
        Pink,

        /// <summary>
        /// Black brick
        /// </summary>
        Black,

        /// <summary>
        /// Block brick
        /// </summary>
        Block
    }

    /// <summary>
    /// Update
    /// </summary>
    public override void Update()
    {
        base.Update();

        mAlpha += 0.02f;
        if (mAlpha > 1.0f)
        {
            mAlpha = 1.0f;
        }
    }

    /// <summary>
    /// Render
    /// </summary>
    public void Render()
    {
        if (mAlpha <= 0)
        {
            return;
        }

        if (FES.PaletteSwapGet() != C.SWAP_SHADOW)
        {
            FES.PaletteSwapSet(ColorSwap);
        }

        byte prevAlpha = FES.AlphaGet();
        FES.AlphaSet((byte)(mAlpha * prevAlpha));

        int LifeFrame = 5 - (int)(((float)Life / (float)MaxLife) * 5);

        if (Type != BrickType.Block)
        {
            LastSpriteRect = new Rect2i(C.BRICK_WIDTH * LifeFrame, 0, Rect.width, Rect.height);
        }
        else
        {
            LastSpriteRect = new Rect2i(120, 0, 20, 10);
        }

        FES.DrawCopy(LastSpriteRect, Rect);

        for (int i = 0; i < FlashFrame; i++)
        {
            FES.DrawRect(new Rect2i(Rect.x + i, Rect.y + i, Rect.width - (i * 2), Rect.height - (i * 2)), 4);
        }

        if (FES.PaletteSwapGet() != C.SWAP_SHADOW)
        {
            FES.PaletteSwapSet(0);
        }

        FES.AlphaSet(prevAlpha);
    }

    /// <summary>
    /// Handle collision
    /// </summary>
    /// <param name="collider">Who hit us</param>
    /// <param name="pos">Position at impact</param>
    /// <param name="velocity">Velocity at impact</param>
    public override void Hit(Collidable collider, Vector2i pos, Vector2 velocity)
    {
        base.Hit(collider, pos, velocity);

        Life = Math.Max(0, Life - 1);

        if (collider is Ball)
        {
            FES.SoundPlay(C.SOUND_HIT_BRICK, 1, UnityEngine.Random.Range(0.9f, 1.1f));
        }

        BrickBustGame game = (BrickBustGame)FES.Game;

        game.Level.Particles.Impact(pos, velocity, C.SWAP_BLACK_BRICK);

        if (Life <= 0)
        {
            game.Level.Score += Score;
            game.Shake(0.3f);
            game.Level.Particles.Explode(LastSpriteRect, new Vector2i(Rect.x, Rect.y), ColorSwap);

            FES.SoundPlay(C.SOUND_EXPLODE, 1, UnityEngine.Random.Range(0.9f, 1.1f));
        }
    }
}
