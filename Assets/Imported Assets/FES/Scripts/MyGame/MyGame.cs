using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Your game! You can of course rename this class to whatever you'd like.
/// </summary>
public class MyGame : FES.IFESGame
{
    /// <summary>
    /// Query hardware. Here you initialize your retro game hardware.
    /// </summary>
    /// <returns>Hardware settings</returns>
    public FES.HardwareSettings QueryHardware()
    {
        var hw = new FES.HardwareSettings();

        // Set your display size
        hw.DisplaySize = new Size2i(320, 180);

        // Set your preferred color mode, either RGB for a full color mode, or Indexed for an indexed/palettized color mode
        hw.ColorMode = FES.ColorMode.RGB;

        // If your color mode is Indexed you can set the palette file here, recommended format is PNG. Each pixel in the image
        // represents one color in your palette. Color index 0 is the top left pixel, and the last color index is the bottom
        // right pixel. 250 colors maximum.
        // By default a built-in 32 color FES palette will be used.
        //// hw.Palette = "MyColorPalette";

        // Set tilemap maximum size, default is 256, 256. Keep this close to your minimum required size to save on memory
        //// hw.MapSize = new Size2i(256, 256);

        // Set tilemap maximum layers, default is 8. Keep this close to your minimum required size to save on memory
        //// hw.MapLayers = 8;

        return hw;
    }

    /// <summary>
    /// Initialize your game here.
    /// </summary>
    /// <returns>Return true if successful</returns>
    public bool Initialize()
    {
        // You can load a spritesheet here
        FES.SpriteSheetSetup(0, "MyGame/MySprites", new Size2i(16, 16));
        FES.SpriteSheetSet(0);

        return true;
    }

    /// <summary>
    /// Update, your game logic should live here. Update is called at a fixed interval of 60 times per second.
    /// </summary>
    public void Update()
    {
        if (FES.ButtonPressed(FES.BTN_SYSTEM))
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Render, your drawing code should go here.
    /// </summary>
    public void Render()
    {
        FES.Clear(new ColorRGBA(50, 104, 108));

        FES.Print(new Vector2i(4, 4), new ColorRGBA(127, 213, 221), "This game is going to be AMAZING!");
        FES.DrawSprite(0, new Vector2i(4, 14));
    }
}
