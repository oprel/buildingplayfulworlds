using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Laser powerup, paddle gets two laser turrets
/// </summary>
public class PowerUpLaser : PowerUp
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pos">Position</param>
    public PowerUpLaser(Vector2i pos) : base("L", 4, pos)
    {
    }

    /// <summary>
    /// Activate the power up
    /// </summary>
    protected override void Activate()
    {
        base.Activate();

        BrickBustGame game = (BrickBustGame)FES.Game;
        var paddle = game.Level.Paddle;

        paddle.Laser();

        // Make sure all balls stuck to the paddle are released
        var balls = game.Level.Balls;
        foreach (var ball in balls)
        {
            ball.StuckToPaddle = false;
        }
    }
}
