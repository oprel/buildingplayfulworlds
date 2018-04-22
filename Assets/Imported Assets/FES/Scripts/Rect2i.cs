using System;
using UnityEngine;

/// <summary>
/// Defines a rectangular area using integers
/// </summary>
public struct Rect2i
{
    /// <summary>
    /// Zero sized rectangle
    /// </summary>
    public static readonly Rect2i zero = new Rect2i(0, 0, 0, 0);

    /// <summary>
    /// X coordinate of top left corner
    /// </summary>
    public int x;

    /// <summary>
    /// Y coordinate of top left corner
    /// </summary>
    public int y;

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
    /// <param name="x">X coordinate of top left corner</param>
    /// <param name="y">Y coordinate of top left corner</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    public Rect2i(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x">X coordinate of top left corner</param>
    /// <param name="y">Y coordinate of top left corner</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    public Rect2i(float x, float y, float width, float height)
    {
        this.x = (int)x;
        this.y = (int)y;
        this.width = (int)width;
        this.height = (int)height;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pos">Top left corner</param>
    /// <param name="size">Dimensions</param>
    public Rect2i(Vector2i pos, Size2i size)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.width = size.width;
        this.height = size.height;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rect">Rect</param>
    public Rect2i(Rect rect)
    {
        this.x = (int)rect.x;
        this.y = (int)rect.y;
        this.width = (int)rect.width;
        this.height = (int)rect.height;
    }

    /// <summary>
    /// Gets the position of the minimum corner of the rectangle.
    /// </summary>
    public Vector2i min
    {
        get
        {
            return new Vector2i(x, y);
        }
    }

    /// <summary>
    /// Gets the position of the maximum corner of the rectangle.
    /// </summary>
    public Vector2i max
    {
        get
        {
            return new Vector2i(x + width, y + height);
        }
    }

    /// <summary>
    /// Gets the center point of the rectangle
    /// </summary>
    public Vector2i center
    {
        get
        {
            return new Vector2i(x + (width / 2), y + (height / 2));
        }
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
                case 2:
                    return this.width;
                case 3:
                    return this.height;
                default:
                    throw new IndexOutOfRangeException("Invalid Rect2i index!");
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
                case 2:
                    this.width = value;
                    break;
                case 3:
                    this.height = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Rect2i index!");
            }
        }
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="a">Left side</param>
    /// <param name="b">Right side</param>
    /// <returns>True if equal</returns>
    public static bool operator ==(Rect2i a, Rect2i b)
    {
        return a.x == b.x && a.y == b.y && a.width == b.width && a.height == b.height;
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="a">Left side</param>
    /// <param name="b">Right side</param>
    /// <returns>True if not equal</returns>
    public static bool operator !=(Rect2i a, Rect2i b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Implicit operator
    /// </summary>
    /// <param name="rect">Rect</param>
    public static implicit operator Rect2i(Rect rect)
    {
        return new Rect2i(rect);
    }

    /// <summary>
    /// Convert to Rect
    /// </summary>
    /// <returns>Rect</returns>
    public Rect ToRect()
    {
        return new Rect(x, y, width, height);
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <returns>String</returns>
    public override string ToString()
    {
        return string.Format("({0}, {1}, {2}, {3})", new object[] { this.x, this.y, this.width, this.height });
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <param name="format">Format</param>
    /// <returns>String</returns>
    public string ToString(string format)
    {
        return string.Format("({0}, {1}, {2}, {3})", new object[] { this.x.ToString(format), this.y.ToString(format), this.width.ToString(format), this.height.ToString(format) });
    }

    /// <summary>
    /// Object equality
    /// </summary>
    /// <param name="other">Other</param>
    /// <returns>True if equal</returns>
    public override bool Equals(object other)
    {
        bool result;

        if (!(other is Rect2i))
        {
            result = false;
        }
        else
        {
            Rect2i r = (Rect2i)other;
            result = this.x.Equals(r.x) && this.y.Equals(r.y) && this.width.Equals(r.width) && this.height.Equals(r.height);
        }

        return result;
    }

    /// <summary>
    /// Get hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.width.GetHashCode() >> 2 ^ this.height.GetHashCode() << 4;
    }

    /// <summary>
    /// Expand the rectangular area vertically and horizontally by the given amount. Negative values effectively shrink the rectangular area.
    /// </summary>
    /// <param name="amount">Amount to expand by</param>
    public void Expand(int amount)
    {
        Expand(amount, amount);
    }

    /// <summary>
    /// Expand the rectangular area by the given width and height. Negative values effectively shrink the rectangular area.
    /// </summary>
    /// <param name="widthAmount">Width to expand by</param>
    /// <param name="heightAmount">Height to expand by</param>
    public void Expand(int widthAmount, int heightAmount)
    {
        x -= widthAmount;
        width += widthAmount * 2;
        y -= heightAmount;
        height += heightAmount * 2;
    }

    /// <summary>
    /// Return true if the given point is contained within the rectangle, or on its boundary.
    /// </summary>
    /// <param name="point">Point</param>
    /// <returns>True if cotained within or on rectangle boundary</returns>
    public bool Contains(Vector2i point)
    {
        if (point.x >= x && point.y >= y && point.x <= x + width && point.y <= y + height)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if there is an intersections with another rectagle
    /// </summary>
    /// <param name="rect">Other rectangle</param>
    /// <returns>True if there is an intersection, false otherwise.</returns>
    public bool Intersects(Rect2i rect)
    {
        if (rect.x + rect.width < x || rect.y + rect.height < y || rect.x > x + width || rect.y > y + height)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Calculates how far another rectangle intersects into this rectangle. In other words the result of this method is the minimal amount that this rectangle has
    /// to be offset by to ensure it no longer collides. This method is useful in collision resolution.
    /// </summary>
    /// <param name="rect">Other rect</param>
    /// <returns>Intersection depth</returns>
    public Vector2 IntersectionDepth(Rect2i rect)
    {
        Vector2 depth;

        // Calculate half sizes.
        float halfWidthA = ((int)max.x - (int)min.x + 1) / 2;
        float halfHeightA = ((int)max.y - (int)min.y + 1) / 2;
        float halfWidthB = ((int)rect.max.x - (int)rect.min.x + 1) / 2;
        float halfHeightB = ((int)rect.max.y - (int)rect.min.y + 1) / 2;

        // Calculate centers.
        Vector2 centerA, centerB;
        centerA = new Vector2((int)min.x + halfWidthA, (int)min.y + halfHeightA);
        centerB = new Vector2((int)rect.min.x + halfWidthB, (int)rect.min.y + halfHeightB);

        // Calculate current and minimum-non-intersecting distances between centers.
        float distanceX = centerA.x - centerB.x;
        float distanceY = centerA.y - centerB.y;
        float minDistanceX = halfWidthA + halfWidthB;
        float minDistanceY = halfHeightA + halfHeightB;

        // If we are not intersecting at all, return (0, 0).
        // NOTE: Potential loss of precision here, might want to create a specialized template for doubles
        if (Mathf.Abs((float)distanceX) >= minDistanceX || Mathf.Abs((float)distanceY) >= minDistanceY)
        {
            depth.x = 0;
            depth.y = 0;
            return depth;
        }

        // Calculate and return intersection depths.
        float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
        float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
        depth.x = depthX;
        depth.y = depthY;

        return depth;
    }

    /// <summary>
    /// Get a rect representing the intersection area with another rectangle
    /// </summary>
    /// <param name="rect">Rectangle to intersect with</param>
    /// <returns>Intersection area</returns>
    public Rect2i Intersect(Rect2i rect)
    {
        int x0 = Math.Max(x, rect.x);
        int x1 = Math.Min(x + width, rect.x + rect.width);
        int y0 = Math.Max(y, rect.y);
        int y1 = Math.Min(y + height, rect.y + rect.height);

        if ((x1 >= x0) && (y1 >= y))
        {
            return new Rect2i(x0, y0, x1 - x0, y1 - y0);
        }

        return new Rect2i();
    }

    /// <summary>
    /// Get a copy of the rect offset by given position. Width and height are unaffected, only x and y are offset.
    /// </summary>
    /// <param name="pos">Position to offset by</param>
    /// <returns>An offset rect</returns>
    public Rect2i Offset(Vector2i pos)
    {
        Rect2i offsetRect = this;
        offsetRect.x += pos.x;
        offsetRect.y += pos.y;

        return offsetRect;
    }
}
