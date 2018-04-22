using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Handle class that encapsulates constant values shared throughout the game
/// </summary>
public class C
{
    /// <summary>
    /// Major version of the game
    /// </summary>
    public const int MAJOR_VER = 1;

    /// <summary>
    /// Minor version of the game
    /// </summary>
    public const int MINOR_VER = 0;

    /// <summary>
    /// Revision version of the game
    /// </summary>
    public const int REV_VER = 0;

    /// <summary>
    /// Width of a brick
    /// </summary>
    public const int BRICK_WIDTH = 20;

    /// <summary>
    /// Height of a brick
    /// </summary>
    public const int BRICK_HEIGHT = 10;

    /// <summary>
    /// Width of the ball
    /// </summary>
    public const int BALL_WIDTH = 6;

    /// <summary>
    /// Height of the ball
    /// </summary>
    public const int BALL_HEIGHT = 6;

    /// <summary>
    /// Palette color swap for gold brick
    /// </summary>
    public const int SWAP_GOLD_BRICK = 0;

    /// <summary>
    /// Palette color swap for blue brick
    /// </summary>
    public const int SWAP_BLUE_BRICK = 1;

    /// <summary>
    /// Palette color swap for green brick
    /// </summary>
    public const int SWAP_GREEN_BRICK = 2;

    /// <summary>
    /// Palette color swap for brown brick
    /// </summary>
    public const int SWAP_BROWN_BRICK = 3;

    /// <summary>
    /// Palette color swap for pink brick
    /// </summary>
    public const int SWAP_PINK_BRICK = 4;

    /// <summary>
    /// Palette color swap for black brick
    /// </summary>
    public const int SWAP_BLACK_BRICK = 5;

    /// <summary>
    /// Palette color swap for a shadow effect (all colors black)
    /// </summary>
    public const int SWAP_SHADOW = 6;

    /// <summary>
    /// Palette color swap for a whiteout effect (all colors white)
    /// </summary>
    public const int SWAP_WHITEOUT = 7;

    /// <summary>
    /// Ball hits brick sound
    /// </summary>
    public const int SOUND_HIT_BRICK = 0;

    /// <summary>
    /// Ball hits wall sound
    /// </summary>
    public const int SOUND_HIT_WALL = 1;

    /// <summary>
    /// Ball "dies" sound
    /// </summary>
    public const int SOUND_DEATH = 2;

    /// <summary>
    /// Brick explodes sound
    /// </summary>
    public const int SOUND_EXPLODE = 3;

    /// <summary>
    /// Game started sound
    /// </summary>
    public const int SOUND_START = 4;

    /// <summary>
    /// Powerup collected sound
    /// </summary>
    public const int SOUND_POWERUP = 5;

    /// <summary>
    /// Laser shot sound
    /// </summary>
    public const int SOUND_LASERSHOT = 6;

    /// <summary>
    /// Laser hit sound
    /// </summary>
    public const int SOUND_LASERHIT = 7;

    /// <summary>
    /// Main menu music
    /// </summary>
    public const int MENU_MUSIC = 0;

    /// <summary>
    /// Level music
    /// </summary>
    public const int LEVEL_MUSIC = 1;

    /// <summary>
    /// Verb used in help text, "CLICK" for desktop, "TAP" for mobile/touchscreen
    /// </summary>
    public static string ACTION_VERB = "CLICK";
}
