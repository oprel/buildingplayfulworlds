using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Main menu screen
/// </summary>
public class MainMenu
{
    private List<Blob> mBlobs = new List<Blob>();

    /// <summary>
    /// Constructor
    /// </summary>
    public MainMenu()
    {
        for (int i = 0; i < 32; i++)
        {
            mBlobs.Add(new Blob());
        }
    }

    /// <summary>
    /// Update
    /// </summary>
    public void Update()
    {
        if (FES.ButtonPressed(FES.BTN_POINTER_A))
        {
            var game = (BrickBustGame)FES.Game;
            game.ChangeState(BrickBustGame.GameState.LEVEL);
        }

        foreach (var blob in mBlobs)
        {
            blob.Update();
        }
    }

    /// <summary>
    /// Render
    /// </summary>
    public void Render()
    {
        foreach (var blob in mBlobs)
        {
            blob.Render();
        }

        Rect2i titleRect = new Rect2i(0, 50, 93, 17);
        int bob = (int)(Math.Sin(FES.Ticks / 10.0f) * 6);

        FES.PaletteSwapSet(C.SWAP_SHADOW);
        FES.DrawCopy(titleRect, new Rect2i((FES.DisplaySize.width / 2) - (titleRect.width / 2) + 3, 32 + bob + 3, titleRect.width, titleRect.height));
        FES.PaletteSwapSet(0);

        FES.DrawCopy(titleRect, new Rect2i((FES.DisplaySize.width / 2) - (titleRect.width / 2), 32 + bob, titleRect.width, titleRect.height));

        FES.DrawCopy(new Rect2i(38, 68, 199, 188), new Rect2i(11, 96, 199, 188));

        FES.PaletteSwapSet(C.SWAP_WHITEOUT);
        FES.AlphaSet(128);

        int highlightOffset = (int)((Math.Sin(FES.Ticks / 50.0f) * FES.DisplaySize.width) + (FES.DisplaySize.width / 2));
        int highlightWidth = 20;
        FES.DrawCopy(new Rect2i(38 + (highlightOffset - 11), 68, highlightWidth, 188), new Rect2i(highlightOffset, 96, highlightWidth, 188));
        FES.AlphaSet(255);
        FES.PaletteSwapSet(0);

        string pressStr = C.ACTION_VERB + " TO PLAY!";

        FES.Print(new Rect2i(0, FES.DisplaySize.height - 80 + 1, FES.DisplaySize.width, 100 + 1), 0, FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER, pressStr);
        FES.Print(new Rect2i(0, FES.DisplaySize.height - 80, FES.DisplaySize.width, 100), 4, FES.ALIGN_H_CENTER | FES.ALIGN_V_CENTER, pressStr);
    }

    private class Blob
    {
        private UnityEngine.Vector2 mPos;

        private float[] mWaveLengths;
        private float[] mPhases;
        private int mWaves = 3;
        private int mColorSwap = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public Blob()
        {
            mWaveLengths = new float[mWaves * 2];
            mPhases = new float[mWaves * 2];

            for (int i = 0; i < mWaves * 2; i += 2)
            {
                mWaveLengths[i] = UnityEngine.Random.Range(20.0f, 60.0f);
                mWaveLengths[i + 1] = UnityEngine.Random.Range(20.0f, 60.0f);

                mPhases[i] = UnityEngine.Random.Range(0.0f, (float)(Math.PI * 2));
                mPhases[i + 1] = UnityEngine.Random.Range(0.0f, (float)(Math.PI * 2));
            }

            mColorSwap = UnityEngine.Random.Range(C.SWAP_GOLD_BRICK, C.SWAP_BLACK_BRICK);
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            mPos.x = 0;
            mPos.y = 0;

            // Accumulate waves
            for (int i = 0; i < mWaves * 2; i += 2)
            {
                mPos.x += Wave(mWaveLengths[i], mPhases[i]);
                mPos.y += Wave(mWaveLengths[i + 1], mPhases[i + 1]);
            }

            mPos.x = (mPos.x * (FES.DisplaySize.width / 2)) + (FES.DisplaySize.width / 2);
            mPos.y = (mPos.y * (FES.DisplaySize.height / 2)) + (FES.DisplaySize.height / 2);
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            Rect2i blobRect = new Rect2i(191, 0, 65, 65);
            FES.PaletteSwapSet(mColorSwap);
            FES.DrawCopy(blobRect, new Rect2i((int)mPos.x - (blobRect.width / 2), (int)mPos.y - (blobRect.height / 2), blobRect.width, blobRect.height));
            FES.PaletteSwapSet(0);
        }

        /// <summary>
        /// Wave utility function
        /// </summary>
        /// <param name="waveLength">Wave length</param>
        /// <param name="offset">Wave offset</param>
        /// <returns>Wave value</returns>
        private float Wave(float waveLength, float offset)
        {
            return (float)Math.Sin(((2 * Math.PI / waveLength) * (FES.Ticks / 10.0f)) + offset) * 0.5f;
        }
    }
}
