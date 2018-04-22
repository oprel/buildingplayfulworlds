using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Slow powerup, slows down all balls in play to base speed
/// </summary>
public class PowerUpSlow : PowerUp
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pos">Position</param>
    public PowerUpSlow(Vector2i pos) : base("S", 0, pos)
    {
    }

    /// <summary>
    /// Activate the power up
    /// </summary>
    protected override void Activate()
    {
        base.Activate();

        BrickBustGame game = (BrickBustGame)FES.Game;
        var balls = game.Level.Balls;

        foreach (var ball in balls)
        {
            ball.ResetSpeed();
        }

        game.Level.Paddle.CancelPowerups();
    }
}
