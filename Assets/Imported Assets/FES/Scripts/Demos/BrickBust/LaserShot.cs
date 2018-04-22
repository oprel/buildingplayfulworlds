using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Laser shot
/// </summary>
public class LaserShot : Collidable
{
    /// <summary>
    /// Dead flag
    /// </summary>
    public bool Dead;

    private int mAlpha;
    private Vector2 mPos;
    private Vector2 mVelocity;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pos">Position</param>
    public LaserShot(Vector2i pos)
    {
        Rect = new Rect2i(pos.x - (C.BALL_WIDTH / 2), pos.y - (C.BALL_HEIGHT / 2), C.BALL_WIDTH, C.BALL_HEIGHT);
        mPos = new Vector2(Rect.x, Rect.y);
        mVelocity = new Vector2(0, -3);
    }

    /// <summary>
    /// Update
    /// </summary>
    public override void Update()
    {
        base.Update();

        mPos.y += mVelocity.y;
        Rect.x = (int)mPos.x;
        Rect.y = (int)mPos.y;

        DoPhysics();

        // Animate fade-in
        mAlpha = Math.Min(255, mAlpha + 25);
    }

    /// <summary>
    /// Render
    /// </summary>
    public void Render()
    {
        byte prevAlpha = FES.AlphaGet();

        // Be sure to account for shadow render pass here
        FES.AlphaSet((byte)((mAlpha / 255.0f) * prevAlpha));
        FES.DrawCopy(new Rect2i(127, 10, 5, 8), new Rect2i((int)mPos.x, (int)mPos.y, 5, 8));
        FES.AlphaSet(prevAlpha);
    }

    /// <summary>
    /// Handle a hit
    /// </summary>
    /// <param name="collider">Who hit us</param>
    /// <param name="pos">Position of impact</param>
    /// <param name="velocity">Velocity at impact</param>
    public override void Hit(Collidable collider, Vector2i pos, Vector2 velocity)
    {
        FES.SoundPlay(C.SOUND_LASERHIT, 1, UnityEngine.Random.Range(0.9f, 1.1f));
    }

    /// <summary>
    /// Perform physics
    /// </summary>
    private void DoPhysics()
    {
        var game = (BrickBustGame)FES.Game;

        foreach (var brick in game.Level.Bricks)
        {
            CheckCollision(brick);
        }

        foreach (var wall in game.Level.Walls)
        {
            CheckCollision(wall);
        }
    }

    /// <summary>
    /// Check if laser shot collided with the given collidable
    /// </summary>
    /// <param name="collidable">Collidable</param>
    private void CheckCollision(Collidable collidable)
    {
        var game = (BrickBustGame)FES.Game;

        // Laser shots don't need fancy collision handling, they just need to see if they've intersected with a brick.
        // Laser shots also travel at a rate which is smaller than the size of a brick so we also don't have to worry
        // about a laser shot skipping over a brick
        if (collidable.Rect.Intersects(Rect))
        {
            var pos = new Vector2i(Rect.x + 3, Rect.y);

            collidable.Hit(this, new Vector2i(Rect.x + 3, Rect.y), mVelocity);
            Hit(collidable, pos, mVelocity);

            game.Level.Particles.Impact(new Vector2i(Rect.x + 3, Rect.y + 5), new Vector2(0, -1), C.SWAP_GREEN_BRICK);
            Dead = true;
        }
    }
}
