using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Catch powerup, ball sticks to paddle until released
/// </summary>
public class PowerUpCatch : PowerUp
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pos">Position</param>
    public PowerUpCatch(Vector2i pos) : base("C", 3, pos)
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

        paddle.Catch();
    }
}
