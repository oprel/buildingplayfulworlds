using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Extra life powerup
/// </summary>
public class PowerUpExtraLife : PowerUp
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pos">Position</param>
    public PowerUpExtraLife(Vector2i pos) : base("P", 5, pos)
    {
    }

    /// <summary>
    /// Activate the power up
    /// </summary>
    protected override void Activate()
    {
        base.Activate();

        BrickBustGame game = (BrickBustGame)FES.Game;
        var level = game.Level;

        level.Lives++;
    }
}
