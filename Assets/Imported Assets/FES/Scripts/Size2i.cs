using System;
using UnityEngine;

/// <summary>
/// Specifies 2D dimensions using integers
/// </summary>
public struct Size2i
{
    /// <summary>
    /// Zero size
    /// </summary>
    public static readonly Size2i zero = new Size2i(0, 0);

    /// <summary>
    /// Width
    /// </summary>
    public int width;

    /// <summary>
    /// Height
    /// </summary>
    public int height;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    public Size2i(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    public Size2i(float width, float height)
    {
        this.width = (int)width;
        this.height = (int)height;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="v2i">Vector</param>
    public Size2i(Vector2i v2i)
    {
        this.width = v2i.x;
        this.height = v2i.y;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="v2">Vector</param>
    public Size2i(Vector2 v2)
    {
        this.width = (int)v2.x;
        this.height = (int)v2.y;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="v3">Vector</param>
    public Size2i(Vector3 v3)
    {
        this.width = (int)v3.x;
        this.height = (int)v3.y;
    }

    /// <summary>
    /// Indexed getter/setter
    /// </summary>
    /// <param name="index">Index</param>
    /// <returns>Component value</returns>
    public int this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return this.width;
                case 1:
                    return this.height;
                default:
                    throw new IndexOutOfRangeException("Invalid Size2i index!");
            }
        }

        set
        {
            switch (index)
            {
                case 0:
                    this.width = value;
                    break;
                case 1:
                    this.height = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Size2i index!");
            }
        }
    }

    /// <summary>
    /// Implicit operator
    /// </summary>
    /// <param name="vector">Vector</param>
    public static implicit operator Size2i(Vector2i vector)
    {
        return new Size2i(vector);
    }

    /// <summary>
    /// Implicit operator
    /// </summary>
    /// <param name="vector">Vector</param>
    public static implicit operator Size2i(Vector2 vector)
    {
        return new Size2i(vector);
    }

    /// <summary>
    /// Implicit operator
    /// </summary>
    /// <param name="vector">Vector</param>
    public static implicit operator Size2i(Vector3 vector)
    {
        return new Size2i(vector);
    }

    /// <summary>
    /// Add two sizes together
    /// </summary>
    /// <param name="a">Left</param>
    /// <param name="b">Right</param>
    /// <returns>Result</returns>
    public static Size2i operator +(Size2i a, Size2i b)
    {
        return new Size2i(a.width + b.width, a.height + b.height);
    }

    /// <summary>
    /// Subtract to sizes from each other
    /// </summary>
    /// <param name="a">Left</param>
    /// <param name="b">Right</param>
    /// <returns>Result</returns>
    public static Size2i operator -(Size2i a, Size2i b)
    {
        return new Size2i(a.width - b.width, a.height - b.height);
    }

    /// <summary>
    /// Negate size
    /// </summary>
    /// <param name="a">Size</param>
    /// <returns>Result</returns>
    public static Size2i operator -(Size2i a)
    {
        return new Size2i(-a.width, -a.height);
    }

    /// <summary>
    /// Multiply size by scalar
    /// </summary>
    /// <param name="size">Size</param>
    /// <param name="scalar">Scalar</param>
    /// <returns>Result</returns>
    public static Size2i operator *(Size2i size, int scalar)
    {
        return new Size2i(size.width * scalar, size.height * scalar);
    }

    /// <summary>
    /// Multiply size by scalar
    /// </summary>
    /// <param name="scalar">Scalar</param>
    /// <param name="size">Size</param>
    /// <returns>Result</returns>
    public static Size2i operator *(int scalar, Size2i size)
    {
        return new Size2i(size.width * scalar, size.height * scalar);
    }

    /// <summary>
    /// Divide size by scalar
    /// </summary>
    /// <param name="size">Size</param>
    /// <param name="scalar">Scalar</param>
    /// <returns>Result</returns>
    public static Size2i operator /(Size2i size, int scalar)
    {
        return new Size2i(size.width / scalar, size.height / scalar);
    }

    /// <summary>
    /// Multiply size by scalar
    /// </summary>
    /// <param name="size">Scalar</param>
    /// <param name="scalar">Size</param>
    /// <returns>Result</returns>
    public static Size2i operator *(Size2i size, float scalar)
    {
        return new Size2i(size.width * scalar, size.height * scalar);
    }

    /// <summary>
    /// Multiply size by scalar
    /// </summary>
    /// <param name="scalar">Scalar</param>
    /// <param name="size">Size</param>
    /// <returns>Result</returns>
    public static Size2i operator *(float scalar, Size2i size)
    {
        return new Size2i(size.width * scalar, size.height * scalar);
    }

    /// <summary>
    /// Divide size by scalar
    /// </summary>
    /// <param name="size">Size</param>
    /// <param name="scalar">Scalar</param>
    /// <returns>Result</returns>
    public static Size2i operator /(Size2i size, float scalar)
    {
        return new Size2i(size.width / scalar, size.height / scalar);
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="a">Left side</param>
    /// <param name="b">Right side</param>
    /// <returns>True if equal</returns>
    public static bool operator ==(Size2i a, Size2i b)
    {
        return a.width == b.width && a.height == b.height;
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="a">Left side</param>
    /// <param name="b">Right side</param>
    /// <returns>True if not equal</returns>
    public static bool operator !=(Size2i a, Size2i b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Convert to Vector2
    /// </summary>
    /// <returns>Vector2</returns>
    public Vector2 ToVector2()
    {
        return new Vector2(width, height);
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <returns>String</returns>
    public override string ToString()
    {
        return string.Format("({0}, {1})", new object[] { this.width, this.height });
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <param name="format">Format</param>
    /// <returns>String</returns>
    public string ToString(string format)
    {
        return string.Format("({0}, {1})", new object[] { this.width.ToString(format), this.height.ToString(format) });
    }

    /// <summary>
    /// Object equality
    /// </summary>
    /// <param name="other">Other</param>
    /// <returns>True if equal</returns>
    public override bool Equals(object other)
    {
        bool result;

        if (!(other is Size2i))
        {
            result = false;
        }
        else
        {
            Size2i s = (Size2i)other;
            result = this.width.Equals(s.width) && this.height.Equals(s.height);
        }

        return result;
    }

    /// <summary>
    /// Get hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        return this.width.GetHashCode() ^ this.height.GetHashCode() << 2;
    }
}
