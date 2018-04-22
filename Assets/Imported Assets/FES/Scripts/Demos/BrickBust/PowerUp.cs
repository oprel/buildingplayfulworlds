using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Base powerup class
/// </summary>
public class PowerUp
{
    private bool mDead = false;
    private Vector2 mPos;
    private string mLabel;
    private int mColorSwap;
    private Rect2i mRect;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="label">Label letter that will appear on the powerup</param>
    /// <param name="colorSwap">Palette color swap</param>
    /// <param name="pos">Initial position</param>
    public PowerUp(string label, int colorSwap, Vector2i pos)
    {
        mLabel = label;
        mColorSwap = colorSwap;
        mPos = new Vector2(pos.x, pos.y);
    }

    /// <summary>
    /// Dead flag
    /// </summary>
    public bool Dead
    {
        get { return mDead; }
    }

    /// <summary>
    /// Update
    /// </summary>
    public virtual void Update()
    {
        if (mDead)
        {
            return;
        }

        mPos.y += 1.0f;
        mRect = new Rect2i(mPos.x, mPos.y, 19, 10);

        if (mPos.y > FES.DisplaySize.height)
        {
            mDead = true;
        }

        BrickBustGame game = (BrickBustGame)FES.Game;
        var level = game.Level;
        var paddleRect = level.Paddle.Rect;
        if (paddleRect.Intersects(mRect))
        {
            level.Particles.Explode(new Rect2i(40, 20, mRect.width, mRect.height), new Vector2i(mPos.x, mPos.y), mColorSwap);
            Activate();
            mDead = true;
        }
    }

    /// <summary>
    /// Render
    /// </summary>
    public virtual void Render()
    {
        int frame = (int)((FES.Ticks % 24) / 8);

        if (FES.PaletteSwapGet() != C.SWAP_SHADOW)
        {
            FES.PaletteSwapSet(mColorSwap);
        }

        FES.DrawCopy(new Rect2i(40 + (20 * frame), 20, mRect.width, mRect.height), mRect);

        if (FES.PaletteSwapGet() != C.SWAP_SHADOW)
        {
            int scroll = (int)((FES.Ticks % 100) / 10);

            FES.ClipSet(mRect);
            FES.Print(new Rect2i(mPos.x, mPos.y - 10 + scroll, mRect.width, mRect.height), 4, FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER, mLabel);
            FES.Print(new Rect2i(mPos.x, mPos.y + scroll, mRect.width, mRect.height), 4, FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER, mLabel);
            FES.ClipReset();

            FES.PaletteSwapSet(0);
        }
    }

    /// <summary>
    /// Activate/Pickup the power up
    /// </summary>
    protected virtual void Activate()
    {
        FES.SoundPlay(C.SOUND_POWERUP);

        BrickBustGame game = (BrickBustGame)FES.Game;
        var level = game.Level;
        level.Score += 10;
    }
}
