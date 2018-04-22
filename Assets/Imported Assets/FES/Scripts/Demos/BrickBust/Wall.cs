using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Simple wall, ball bounces off it, thats pretty much it
/// </summary>
public class Wall : Collidable
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rect">Rect that encloses the wall</param>
    public Wall(Rect2i rect)
    {
        Rect = rect;
    }

    /// <summary>
    /// Handle the hit
    /// </summary>
    /// <param name="collider">Who hit us</param>
    /// <param name="pos">Position if impact</param>
    /// <param name="velocity">Velocity at impact</param>
    public override void Hit(Collidable collider, Vector2i pos, Vector2 velocity)
    {
        base.Hit(collider, pos, velocity);

        BrickBustGame game = (BrickBustGame)FES.Game;

        game.Level.Particles.Impact(pos, velocity, C.SWAP_BLACK_BRICK);

        FES.SoundPlay(C.SOUND_HIT_WALL, 1, UnityEngine.Random.Range(0.9f, 1.1f));
    }
}
