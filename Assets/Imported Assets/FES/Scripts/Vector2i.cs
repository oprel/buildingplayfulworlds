using System;
using UnityEngine;

/// <summary>
/// Specifies 2D vector with integers
/// </summary>
public struct Vector2i
{
    /// <summary>
    /// Zero size
    /// </summary>
    public static readonly Vector2i zero = new Vector2i(0, 0);

    /// <summary>
    /// X coordinate
    /// </summary>
    public int x;

    /// <summary>
    /// Y coordinate
    /// </summary>
    public int y;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    public Vector2i(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    public Vector2i(float x, float y)
    {
        this.x = (int)x;
        this.y = (int)y;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="size">Size</param>
    public Vector2i(Size2i size)
    {
        this.x = size.width;
        this.y = size.height;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="v2">Vector</param>
    public Vector2i(Vector2 v2)
    {
        this.x = (int)v2.x;
        this.y = (int)v2.y;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="v3">Vector</param>
    public Vector2i(Vector3 v3)
    {
        this.x = (int)v3.x;
        this.y = (int)v3.y;
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
                    return this.x;
                case 1:
                    return this.y;
                default:
                    throw new IndexOutOfRangeException("Invalid Vector2i index!");
            }
        }

        set
        {
            switch (index)
            {
                case 0:
                    this.x = value;
                    break;
                case 1:
                    this.y = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Vector2i index!");
            }
        }
    }

    /// <summary>
    /// Implicit operator
    /// </summary>
    /// <param name="size">Size</param>
    public static implicit operator Vector2i(Size2i size)
    {
        return new Vector2i(size);
    }

    /// <summary>
    /// Implicit operator
    /// </summary>
    /// <param name="vector">Vector</param>
    public static implicit operator Vector2i(Vector2 vector)
    {
        return new Vector2i(vector);
    }

    /// <summary>
    /// Implicit operator
    /// </summary>
    /// <param name="vector">Vector</param>
    public static implicit operator Vector2i(Vector3 vector)
    {
        return new Vector2i(vector);
    }

    /// <summary>
    /// Add two vectors together
    /// </summary>
    /// <param name="a">Left</param>
    /// <param name="b">Right</param>
    /// <returns>Result</returns>
    public static Vector2i operator +(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.x + b.x, a.y + b.y);
    }

    /// <summary>
    /// Subtract two vectors from each other
    /// </summary>
    /// <param name="a">Left</param>
    /// <param name="b">Right</param>
    /// <returns>Result</returns>
    public static Vector2i operator -(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.x - b.x, a.y - b.y);
    }

    /// <summary>
    /// Negate vector
    /// </summary>
    /// <param name="a">Vector</param>
    /// <returns>Result</returns>
    public static Vector2i operator -(Vector2i a)
    {
        return new Vector2i(-a.x, -a.y);
    }

    /// <summary>
    /// Multiply two vectors together
    /// </summary>
    /// <param name="a">Left</param>
    /// <param name="d">Right</param>
    /// <returns>Result</returns>
    public static Vector2i operator *(Vector2i a, int d)
    {
        return new Vector2i(a.x * d, a.y * d);
    }

    /// <summary>
    /// Multiply vector by scalar
    /// </summary>
    /// <param name="scalar">Scalar</param>
    /// <param name="vector">Vector</param>
    /// <returns>Result</returns>
    public static Vector2i operator *(int scalar, Vector2i vector)
    {
        return new Vector2i(vector.x * scalar, vector.y * scalar);
    }

    /// <summary>
    /// Divide vector by scalar
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <param name="scalar">Scalar</param>
    /// <returns>Result</returns>
    public static Vector2i operator /(Vector2i vector, int scalar)
    {
        return new Vector2i(vector.x / scalar, vector.y / scalar);
    }

    /// <summary>
    /// Multiply vector by scalar
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <param name="scalar">Scalar</param>
    /// <returns>Result</returns>
    public static Vector2i operator *(Vector2i vector, float scalar)
    {
        return new Vector2i(vector.x * scalar, vector.y * scalar);
    }

    /// <summary>
    /// Multiply vector by scalar
    /// </summary>
    /// <param name="scalar">Scalar</param>
    /// <param name="vector">Vector</param>
    /// <returns>Result</returns>
    public static Vector2i operator *(float scalar, Vector2i vector)
    {
        return new Vector2i(vector.x * scalar, vector.y * scalar);
    }

    /// <summary>
    /// Divide by scalar
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <param name="scalar">Scalar</param>
    /// <returns>Result</returns>
    public static Vector2i operator /(Vector2i vector, float scalar)
    {
        return new Vector2i(vector.x / scalar, vector.y / scalar);
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="a">Left side</param>
    /// <param name="b">Right side</param>
    /// <returns>True if equal</returns>
    public static bool operator ==(Vector2i a, Vector2i b)
    {
        return a.x == b.x && a.y == b.y;
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="a">Left side</param>
    /// <param name="b">Right side</param>
    /// <returns>True if not equal</returns>
    public static bool operator !=(Vector2i a, Vector2i b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Convert to Vector2
    /// </summary>
    /// <returns>Vector2</returns>
    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <returns>String</returns>
    public override string ToString()
    {
        return string.Format("({0}, {1})", new object[] { this.x, this.y });
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <param name="format">Format</param>
    /// <returns>String</returns>
    public string ToString(string format)
    {
        return string.Format("({0}, {1})", new object[] { this.x.ToString(format), this.y.ToString(format) });
    }

    /// <summary>
    /// Object equality
    /// </summary>
    /// <param name="other">Other</param>
    /// <returns>True if equal</returns>
    public override bool Equals(object other)
    {
        bool result;

        if (!(other is Vector2i))
        {
            result = false;
        }
        else
        {
            Vector2i v = (Vector2i)other;
            result = this.x.Equals(v.x) && this.y.Equals(v.y);
        }

        return result;
    }

    /// <summary>
    /// Get hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
    }
}
