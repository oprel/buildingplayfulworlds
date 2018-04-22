using System;
using UnityEngine;

/// <summary>
/// Specifies a color using Red, Green and Blue elements
/// </summary>
public struct ColorRGBA
{
    /// <summary>
    /// White (255, 255, 255)
    /// </summary>
    public static readonly ColorRGBA white = new ColorRGBA(255, 255, 255);

    /// <summary>
    /// Black (0, 0, 0)
    /// </summary>
    public static readonly ColorRGBA black = new ColorRGBA(0, 0, 0);

    /// <summary>
    /// Red (255, 0, 0)
    /// </summary>
    public static readonly ColorRGBA red = new ColorRGBA(255, 0, 0);

    /// <summary>
    /// Green (0, 255, 0)
    /// </summary>
    public static readonly ColorRGBA green = new ColorRGBA(0, 255, 0);

    /// <summary>
    /// Blue (0, 0, 255)
    /// </summary>
    public static readonly ColorRGBA blue = new ColorRGBA(0, 0, 255);

    /// <summary>
    /// Yellow (255, 255, 0)
    /// </summary>
    public static readonly ColorRGBA yellow = new ColorRGBA(255, 255, 0);

    /// <summary>
    /// Purple (255, 0, 255)
    /// </summary>
    public static readonly ColorRGBA purple = new ColorRGBA(255, 0, 255);

    /// <summary>
    /// Cyan (0, 255, 255)
    /// </summary>
    public static readonly ColorRGBA cyan = new ColorRGBA(0, 255, 255);

    /// <summary>
    /// Light Gray (192, 192, 192)
    /// </summary>
    public static readonly ColorRGBA lightgray = new ColorRGBA(192, 192, 192);

    /// <summary>
    /// Gray (128, 128, 128)
    /// </summary>
    public static readonly ColorRGBA gray = new ColorRGBA(128, 128, 128);

    /// <summary>
    /// DarkGray (64, 64, 64)
    /// </summary>
    public static readonly ColorRGBA darkgray = new ColorRGBA(64, 64, 64);

    /// <summary>
    /// Transparent (0, 0, 0, 0)
    /// </summary>
    public static readonly ColorRGBA transparent = new ColorRGBA(0, 0, 0, 0);

    /// <summary>
    /// Red component
    /// </summary>
    public byte r;

    /// <summary>
    /// Green component
    /// </summary>
    public byte g;

    /// <summary>
    /// Blue component
    /// </summary>
    public byte b;

    /// <summary>
    /// Alpha component
    /// </summary>
    public byte a;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="r">Red component</param>
    /// <param name="g">Green component</param>
    /// <param name="b">Blue component</param>
    /// <param name="a">Alpha component</param>
    public ColorRGBA(byte r, byte g, byte b, byte a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="r">Red component</param>
    /// <param name="g">Green component</param>
    /// <param name="b">Blue component</param>
    /// <param name="a">Alpha component</param>
    public ColorRGBA(int r, int g, int b, int a)
    {
        this.r = (byte)r;
        this.g = (byte)g;
        this.b = (byte)b;
        this.a = (byte)a;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="r">Red component</param>
    /// <param name="g">Green component</param>
    /// <param name="b">Blue component</param>
    public ColorRGBA(byte r, byte g, byte b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = 255;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="r">Red component</param>
    /// <param name="g">Green component</param>
    /// <param name="b">Blue component</param>
    public ColorRGBA(int r, int g, int b)
    {
        this.r = (byte)r;
        this.g = (byte)g;
        this.b = (byte)b;
        this.a = 255;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="color">Color</param>
    public ColorRGBA(Color32 color)
    {
        this.r = color.r;
        this.g = color.g;
        this.b = color.b;
        this.a = color.a;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="color">Color</param>
    public ColorRGBA(Color color)
    {
        this.r = (byte)(color.r * 255);
        this.g = (byte)(color.g * 255);
        this.b = (byte)(color.b * 255);
        this.a = (byte)(color.a * 255);
    }

    /// <summary>
    /// Indexed getter/setter
    /// </summary>
    /// <param name="index">Index</param>
    /// <returns>Component value</returns>
    public byte this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return this.r;
                case 1:
                    return this.g;
                case 2:
                    return this.b;
                case 3:
                    return this.a;
                default:
                    throw new IndexOutOfRangeException("Invalid ColorRGBA index!");
            }
        }

        set
        {
            switch (index)
            {
                case 0:
                    this.r = value;
                    break;
                case 1:
                    this.g = value;
                    break;
                case 2:
                    this.b = value;
                    break;
                case 3:
                    this.a = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid ColorRGBA index!");
            }
        }
    }

    /// <summary>
    /// Implicit operator
    /// </summary>
    /// <param name="color">Color</param>
    public static implicit operator ColorRGBA(Color32 color)
    {
        return new ColorRGBA(color);
    }

    /// <summary>
    /// Implicit operator
    /// </summary>
    /// <param name="color">Color</param>
    public static implicit operator ColorRGBA(Color color)
    {
        return new ColorRGBA(color);
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="a">Left side</param>
    /// <param name="b">Right side</param>
    /// <returns>True if equal</returns>
    public static bool operator ==(ColorRGBA a, ColorRGBA b)
    {
        return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="a">Left side</param>
    /// <param name="b">Right side</param>
    /// <returns>True if not equal</returns>
    public static bool operator !=(ColorRGBA a, ColorRGBA b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Convert to Color32. Alpha is set to 255.
    /// </summary>
    /// <returns>Color</returns>
    public Color32 ToColor32()
    {
        return new Color32(r, g, b, a);
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <returns>String</returns>
    public override string ToString()
    {
        return string.Format("({0}, {1}, {2}, {3})", new object[] { this.r, this.g, this.b, this.a });
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <param name="format">Format</param>
    /// <returns>String</returns>
    public string ToString(string format)
    {
        return string.Format("({0}, {1}, {2}, {3})", new object[] { this.r.ToString(format), this.g.ToString(format), this.b.ToString(format), this.a.ToString(format) });
    }

    /// <summary>
    /// Object equality
    /// </summary>
    /// <param name="other">Other</param>
    /// <returns>True if equal</returns>
    public override bool Equals(object other)
    {
        bool result;

        if (!(other is ColorRGBA))
        {
            result = false;
        }
        else
        {
            ColorRGBA c = (ColorRGBA)other;
            result = this.r.Equals(c.g) && this.r.Equals(c.g) && this.b.Equals(c.b) && this.a.Equals(c.a);
        }

        return result;
    }

    /// <summary>
    /// Get hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        return this.r.GetHashCode() ^ this.g.GetHashCode() << 2 ^ this.b.GetHashCode() >> 2 ^ this.a.GetHashCode() << 1;
    }
}
