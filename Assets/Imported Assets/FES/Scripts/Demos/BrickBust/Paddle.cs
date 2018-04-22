using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// The paddle, otherwise known as the player
/// </summary>
public class Paddle : Collidable
{
    private Vector2i mPrevPointerPos = Vector2i.zero;
    private bool mDragging = false;
    private Vector2i mPos;
    private Rect2i mBaseRect;
    private Rect2i mExtendedRect;
    private List<NineSlice> mNSFrame = new List<NineSlice>();
    private float mLaserOffset = 1.0f;
    private int mLaserTurn = 0;

    // Powers
    private bool mExtended = false;
    private bool mCatch = false;
    private bool mLaser = false;

    /// <summary>
    /// Constructor
    /// </summary>
    public Paddle()
    {
        // If mobile then we want to leave a bigger gap on the bottom of the screen as a touch space
        if (UnityEngine.Application.isMobilePlatform)
        {
            Rect = new Rect2i((FES.DisplaySize.width / 2) - 12, FES.DisplaySize.height - (FES.SpriteSize(0).height * 6), FES.SpriteSize(0).width * 3, FES.SpriteSize(0).height);
        }
        else
        {
            Rect = new Rect2i((FES.DisplaySize.width / 2) - 12, FES.DisplaySize.height - (FES.SpriteSize(0).height * 3), FES.SpriteSize(0).width * 3, FES.SpriteSize(0).height);
        }

        mBaseRect = Rect;
        mExtendedRect = new Rect2i(Rect.x - 10, Rect.y, Rect.width + 20, Rect.height);

        mPos = Rect.center;

        int frameOffset = 0;

        mNSFrame.Add(new NineSlice(new Rect2i(0 + frameOffset, 10, 6, 10), new Rect2i(6 + frameOffset, 10, 18, 10), new Rect2i(24 + frameOffset, 10, 6, 10), Rect2i.zero, Rect2i.zero, Rect2i.zero, Rect2i.zero, Rect2i.zero, Rect2i.zero));
        frameOffset += 30;
        mNSFrame.Add(new NineSlice(new Rect2i(0 + frameOffset, 10, 6, 10), new Rect2i(6 + frameOffset, 10, 18, 10), new Rect2i(24 + frameOffset, 10, 6, 10), Rect2i.zero, Rect2i.zero, Rect2i.zero, Rect2i.zero, Rect2i.zero, Rect2i.zero));
        frameOffset += 30;
        mNSFrame.Add(new NineSlice(new Rect2i(0 + frameOffset, 10, 6, 10), new Rect2i(6 + frameOffset, 10, 18, 10), new Rect2i(24 + frameOffset, 10, 6, 10), Rect2i.zero, Rect2i.zero, Rect2i.zero, Rect2i.zero, Rect2i.zero, Rect2i.zero));
        frameOffset += 30;
        mNSFrame.Add(new NineSlice(new Rect2i(0 + frameOffset, 10, 6, 10), new Rect2i(6 + frameOffset, 10, 18, 10), new Rect2i(24 + frameOffset, 10, 6, 10), Rect2i.zero, Rect2i.zero, Rect2i.zero, Rect2i.zero, Rect2i.zero, Rect2i.zero));
    }

    /// <summary>
    ///  Collision rect for the paddle
    /// </summary>
    public override Rect2i CollideRect
    {
        // Let the ball sink into a paddle a little bit, by 4px
        get { return new Rect2i(Rect.x, Rect.y + 4, Rect.width, Rect.height - 4); }
    }

    /// <summary>
    /// Update
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (FES.ButtonDown(FES.BTN_POINTER_A))
        {
            if (mDragging)
            {
                int dragDelta = FES.PointerPos().x - mPrevPointerPos.x;
                mPos.x += dragDelta;

                Rect.x = mPos.x - (Rect.width / 2);

                int minMargin = FES.SpriteSize(0).width / 2;
                if (Rect.x < minMargin)
                {
                    Rect.x = minMargin;
                }
                else if (Rect.x + Rect.width > FES.DisplaySize.width - minMargin)
                {
                    Rect.x = FES.DisplaySize.width - minMargin - Rect.width;
                }
            }

            mPrevPointerPos = FES.PointerPos();
            mDragging = true;
        }
        else
        {
            mDragging = false;
        }

        if (mExtended)
        {
            if (Rect.width < mExtendedRect.width)
            {
                Rect.x--;
                Rect.width += 2;
            }
        }
        else
        {
            if (Rect.width > mBaseRect.width)
            {
                Rect.x++;
                Rect.width -= 2;
            }
        }

        if (mLaser)
        {
            mLaserOffset -= 0.1f;
            if (mLaserOffset < 0)
            {
                mLaserOffset = 0;
            }

            if (FES.ButtonPressed(FES.BTN_POINTER_A))
            {
                Shoot();
            }
        }
        else
        {
            mLaserOffset += 0.1f;
            if (mLaserOffset > 1)
            {
                mLaserOffset = 1;
            }
        }

        // Rest mPos after collision rect corrections
        mPos = Rect.center;
    }

    /// <summary>
    /// Render
    /// </summary>
    public void Render()
    {
        FES.DrawNineSlice(Rect, mNSFrame[FlashFrame]);

        if (mLaserOffset < 1.0f)
        {
            Vector2i offset;
            offset.x = (int)(16 * mLaserOffset);
            offset.y = (int)(8 * mLaserOffset);

            byte prevAlpha = FES.AlphaGet();

            FES.AlphaSet((byte)((1 - mLaserOffset) * prevAlpha));
            FES.DrawCopy(new Rect2i(120, 10, 7, 10), new Rect2i(Rect.x - 3 - offset.x, Rect.y - 2 - offset.y, 7, 10));
            FES.DrawCopy(new Rect2i(120, 10, 7, 10), new Rect2i(Rect.x + Rect.width - 4 + offset.x, Rect.y - 2 - offset.y, 7, 10));
            FES.AlphaSet(prevAlpha);
        }
    }

    /// <summary>
    /// Handle hit
    /// </summary>
    /// <param name="collider">Who hit us</param>
    /// <param name="pos">Position of impact</param>
    /// <param name="velocity">Velocity at impact</param>
    public override void Hit(Collidable collider, Vector2i pos, Vector2 velocity)
    {
        base.Hit(collider, pos, velocity);

        FES.SoundPlay(C.SOUND_HIT_WALL, 1, UnityEngine.Random.Range(0.9f, 1.1f));

        if (mCatch)
        {
            if (collider is Ball)
            {
                var ball = (Ball)collider;
                ball.StuckToPaddle = true;
            }
        }

        if (collider is Ball)
        {
            var ball = (Ball)collider;
            ball.SpeedUp();
        }
    }

    /// <summary>
    /// Apply extend power up, cancelling other powerups as needed
    /// </summary>
    public void Extend()
    {
        CancelPowerups();
        mExtended = true;
    }

    /// <summary>
    /// Apply catch power up, cancelling other powerups as needed
    /// </summary>
    public void Catch()
    {
        CancelPowerups();
        mCatch = true;
    }

    /// <summary>
    /// Apply laser power up, cancelling other powerups as needed
    /// </summary>
    public void Laser()
    {
        CancelPowerups();
        mLaser = true;
    }

    /// <summary>
    /// Cancel powerups
    /// </summary>
    public void CancelPowerups()
    {
        mExtended = false;
        mCatch = false;
        mLaser = false;
    }

    /// <summary>
    /// Shoot if we have the laser powerup
    /// </summary>
    private void Shoot()
    {
        if (!mLaser)
        {
            return;
        }

        BrickBustGame game = (BrickBustGame)FES.Game;
        var level = game.Level;

        LaserShot shot;

        if (mLaserTurn == 0)
        {
            shot = new LaserShot(new Vector2i(Rect.x + 1, Rect.y - 8));
            level.AddShot(shot);
        }
        else
        {
            shot = new LaserShot(new Vector2i(Rect.x + Rect.width, Rect.y - 8));
            level.AddShot(shot);
        }

        level.Particles.Impact(new Vector2i(shot.Rect.x + 3, shot.Rect.y + 5), new Vector2(0, -1), C.SWAP_GREEN_BRICK);

        mLaserTurn = (mLaserTurn == 0) ? 1 : 0;

        FES.SoundPlay(C.SOUND_LASERSHOT, 1, UnityEngine.Random.Range(0.9f, 1.1f));
    }
}
