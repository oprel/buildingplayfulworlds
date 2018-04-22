using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Game HUD, displays score, lives left, and game logo
/// </summary>
public class GameHud
{
    private bool mShowHelp = true;
    private bool mShowGameOver = false;
    private bool mShowWin = false;
    private int mInfoFade = 255;
    private float mLastTapTime;

    /// <summary>
    /// Update
    /// </summary>
    public void Update()
    {
        bool doubleTapped = false;
        if (FES.ButtonPressed(FES.BTN_POINTER_A))
        {
            if (UnityEngine.Time.realtimeSinceStartup - mLastTapTime < 0.5f)
            {
                doubleTapped = true;
            }

            mLastTapTime = UnityEngine.Time.realtimeSinceStartup;
        }

        if (!mShowHelp && !mShowGameOver && !mShowWin)
        {
            mInfoFade = Math.Max(0, mInfoFade - 8);
        }
        else if (mShowGameOver)
        {
            mInfoFade = Math.Min(255, mInfoFade + 8);

            if (doubleTapped)
            {
                var game = (BrickBustGame)FES.Game;
                game.ChangeState(BrickBustGame.GameState.MAIN_MENU);
            }
        }
        else if (mShowWin)
        {
            mInfoFade = Math.Min(255, mInfoFade + 8);

            if (doubleTapped)
            {
                var game = (BrickBustGame)FES.Game;
                game.ChangeState(BrickBustGame.GameState.LEVEL);
            }
        }
    }

    /// <summary>
    /// Render
    /// </summary>
    public void Render()
    {
        var game = (BrickBustGame)FES.Game;
        var level = game.Level;

        for (int i = 0; i < level.Lives; i++)
        {
            FES.DrawCopy(new Rect2i(0, 40, 15, 7), new Rect2i(10 + (i * 17), FES.DisplaySize.height - 10, 15, 7));
        }

        Rect2i titleRect = new Rect2i(0, 50, 93, 17);
        FES.DrawCopy(titleRect, new Rect2i((FES.DisplaySize.width / 2) - (titleRect.width / 2), 2, titleRect.width, titleRect.height));

        FES.Print(new Rect2i(0, 0, FES.DisplaySize.width / 4, FES.SpriteSize(0).height * 2), 4, FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER, "@015SCORE@-\n" + level.Score);
        FES.Print(new Rect2i(FES.DisplaySize.width - (FES.DisplaySize.width / 4), 0, FES.DisplaySize.width / 4, FES.SpriteSize(0).height * 2), 4, FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER, "@015HISCORE@-\n" + level.HiScore);

        string infoStr = "DRAG TO MOVE PADDLE\n\nDOUBLE " + C.ACTION_VERB + " TO RELEASE THE BALL!";
        if (mShowGameOver)
        {
            infoStr = "GAME OVER!\n\nDOUBLE " + C.ACTION_VERB + " TO EXIT!";
        }
        else if (mShowWin)
        {
            infoStr = "LEVEL CLEARED!\n\nDOUBLE " + C.ACTION_VERB + " TO PROCEED!";
        }

        if (mInfoFade > 0)
        {
            int bob = (int)(Math.Sin(FES.Ticks / 10.0f) * 5);

            FES.AlphaSet((byte)mInfoFade);
            FES.Print(new Rect2i(0 + 2, 260 + 2 + bob, FES.DisplaySize.width, 40), 0, FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER, infoStr);
            FES.Print(new Rect2i(0, 260 + bob, FES.DisplaySize.width, 40), 4, FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER, infoStr);
            FES.AlphaSet(255);
        }
    }

    /// <summary>
    /// Hide help text
    /// </summary>
    public void HideHelp()
    {
        mShowHelp = false;
    }

    /// <summary>
    /// Show game over text
    /// </summary>
    public void ShowGameOver()
    {
        mShowGameOver = true;
    }

    /// <summary>
    /// Show Won text
    /// </summary>
    public void ShowWin()
    {
        mShowWin = true;
    }
}
