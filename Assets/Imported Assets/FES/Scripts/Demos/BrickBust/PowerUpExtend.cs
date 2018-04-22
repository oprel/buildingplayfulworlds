using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Extend power up, paddle grows wider
/// </summary>
public class PowerUpExtend : PowerUp
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pos">Position</param>
    public PowerUpExtend(Vector2i pos) : base("E", 1, pos)
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

        paddle.Extend();

        // Make sure all balls stuck to the paddle are released
        var balls = game.Level.Balls;
        foreach (var ball in balls)
        {
            ball.StuckToPaddle = false;
        }
    }
}
