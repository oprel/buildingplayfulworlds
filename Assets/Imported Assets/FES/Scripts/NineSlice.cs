/// <summary>
/// Defines a nine-slice image
/// </summary>
public struct NineSlice
{
    /// <summary>
    /// Top left corner
    /// </summary>
    public Rect2i TopLeftCornerRect;

    /// <summary>
    /// Top side
    /// </summary>
    public Rect2i TopSideRect;

    /// <summary>
    /// Top right corner
    /// </summary>
    public Rect2i TopRightCornerRect;

    /// <summary>
    /// Left side
    /// </summary>
    public Rect2i LeftSideRect;

    /// <summary>
    /// Middle
    /// </summary>
    public Rect2i MiddleRect;

    /// <summary>
    /// Right side
    /// </summary>
    public Rect2i RightSideRect;

    /// <summary>
    /// Bottom left corner
    /// </summary>
    public Rect2i BottomLeftCornerRect;

    /// <summary>
    /// Bottom side
    /// </summary>
    public Rect2i BottomSideRect;

    /// <summary>
    /// Bottom right corner
    /// </summary>
    public Rect2i BottomRightCornerRect;

    /// <summary>
    /// Top left corner flags
    /// </summary>
    public int FlagsTopLeftCorner;

    /// <summary>
    /// Top flags
    /// </summary>
    public int FlagsTopSide;

    /// <summary>
    /// Top right corner flags
    /// </summary>
    public int FlagsTopRightCorner;

    /// <summary>
    /// Left side flags
    /// </summary>
    public int FlagsLeftSide;

    /// <summary>
    /// Right side flags
    /// </summary>
    public int FlagsRightSide;

    /// <summary>
    /// Bottom left corner flags
    /// </summary>
    public int FlagsBottomLeftCorner;

    /// <summary>
    /// Bottom flags
    /// </summary>
    public int FlagsBottomSide;

    /// <summary>
    /// Bottom right flags
    /// </summary>
    public int FlagsBottomRightCorner;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="topLeftCornerRect">Top left corner</param>
    /// <param name="topSideRect">Top side</param>
    /// <param name="topRightCornerRect">Top right corner</param>
    /// <param name="leftSideRect">Left side</param>
    /// <param name="middleRect">Middle</param>
    /// <param name="rightSideRect">Right side</param>
    /// <param name="bottomLeftCornerRect">Bottom left corner</param>
    /// <param name="bottomSideRect">Bottom side</param>
    /// <param name="bottomRightCornerRect">Bottom right corner</param>
    public NineSlice(
        Rect2i topLeftCornerRect,
        Rect2i topSideRect,
        Rect2i topRightCornerRect,
        Rect2i leftSideRect,
        Rect2i middleRect,
        Rect2i rightSideRect,
        Rect2i bottomLeftCornerRect,
        Rect2i bottomSideRect,
        Rect2i bottomRightCornerRect)
    {
        TopLeftCornerRect = topLeftCornerRect;
        TopSideRect = topSideRect;
        TopRightCornerRect = topRightCornerRect;
        LeftSideRect = leftSideRect;
        MiddleRect = middleRect;
        RightSideRect = rightSideRect;
        BottomLeftCornerRect = bottomLeftCornerRect;
        BottomSideRect = bottomSideRect;
        BottomRightCornerRect = bottomRightCornerRect;

        FlagsTopLeftCorner = 0;
        FlagsTopSide = 0;
        FlagsTopRightCorner = 0;
        FlagsLeftSide = 0;
        FlagsRightSide = 0;
        FlagsBottomLeftCorner = 0;
        FlagsBottomSide = 0;
        FlagsBottomRightCorner = 0;
    }

    /// <summary>
    /// Simple nine-slice constructor, all corners and sides are mirrored and rotated
    /// </summary>
    /// <param name="topLeftCornerRect">Top left corner rect</param>
    /// <param name="topSideRect">Top side rect</param>
    /// <param name="middleRect">Middle rect</param>
    public NineSlice(Rect2i topLeftCornerRect, Rect2i topSideRect, Rect2i middleRect)
    {
        TopLeftCornerRect = topLeftCornerRect;
        TopSideRect = topSideRect;
        TopRightCornerRect = topLeftCornerRect;
        LeftSideRect = topSideRect;
        MiddleRect = middleRect;
        RightSideRect = topSideRect;
        BottomLeftCornerRect = topLeftCornerRect;
        BottomSideRect = topSideRect;
        BottomRightCornerRect = topLeftCornerRect;

        FlagsTopLeftCorner = 0;
        FlagsTopSide = 0;
        FlagsTopRightCorner = FES.FLIP_H;
        FlagsLeftSide = FES.ROT_90_CCW;
        FlagsRightSide = FES.ROT_90_CW;
        FlagsBottomLeftCorner = FES.FLIP_V;
        FlagsBottomSide = FES.FLIP_V;
        FlagsBottomRightCorner = FES.FLIP_H | FES.FLIP_V;
    }
}
