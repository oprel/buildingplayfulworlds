namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// A basic UI button for demo purposes
    /// </summary>
    public class StressButton
    {
        private Rect2i mRect;
        private Rect2i mHotZone;
        private KeyCode mKeyCode;
        private int mFaceColor;
        private int mLabelColor;
        private StressButtonPressedCB mButtonPressedCB;
        private StressButtonReleasedCB mButtonReleasedCB;
        private Rect2i mHitRect;
        private string mLabel;
        private bool mLabelBottomAligned;
        private object mUserData;
        private bool mTouchArmed = false;

        private bool mPressed;
        private bool mTouched;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rect">Rectangular area of button</param>
        /// <param name="hotZone">Interactable hot zone</param>
        /// <param name="faceColor">Color of button face</param>
        /// <param name="labelColor">Color of button label text</param>
        /// <param name="label">Button label</param>
        /// <param name="keyCode">KeyCode to map to</param>
        /// <param name="userData">User data</param>
        /// <param name="buttonPressed">Button pressed delegate</param>
        /// <param name="buttonReleased">Button released delegate</param>
        /// <param name="labelBottomAligned">Is label bottom aligned, or centered?</param>
        public StressButton(Rect2i rect, Rect2i hotZone, int faceColor, int labelColor, string label, KeyCode keyCode, object userData, StressButtonPressedCB buttonPressed = null, StressButtonReleasedCB buttonReleased = null, bool labelBottomAligned = false)
        {
            mRect = rect;
            mHotZone = hotZone;
            mKeyCode = keyCode;
            mFaceColor = faceColor;
            mLabelColor = labelColor;
            mLabel = label;
            mUserData = userData;
            mLabelBottomAligned = labelBottomAligned;

            mButtonPressedCB = buttonPressed;
            mButtonReleasedCB = buttonReleased;
        }

        /// <summary>
        /// Button pressed delegate
        /// </summary>
        /// <param name="button">Button</param>
        /// <param name="userData">User data</param>
        public delegate void StressButtonPressedCB(StressButton button, object userData);

        /// <summary>
        /// Button released delegate
        /// </summary>
        /// <param name="button">Button</param>
        /// <param name="userData">User data</param>
        public delegate void StressButtonReleasedCB(StressButton button, object userData);

        /// <summary>
        /// Get/Set the button label
        /// </summary>
        public string Label
        {
            get { return mLabel; }
            set { mLabel = value; }
        }

        /// <summary>
        /// Get/Set label color
        /// </summary>
        public int LabelColor
        {
            get { return mLabelColor; }
            set { mLabelColor = value; }
        }

        /// <summary>
        /// Reset button state
        /// </summary>
        public void Reset()
        {
            mPressed = mTouched = mTouchArmed = false;
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            if (FES.KeyPressed(mKeyCode))
            {
                if (!mPressed && mButtonPressedCB != null)
                {
                    mButtonPressedCB(this, mUserData);
                }

                mPressed = true;
            }
            else if (FES.KeyReleased(mKeyCode))
            {
                if (mPressed && mButtonReleasedCB != null)
                {
                    mButtonReleasedCB(this, mUserData);
                }

                mPressed = false;
            }

            if (mTouchArmed)
            {
                if (FES.ButtonDown(FES.BTN_POINTER_A) && mHitRect.Contains(FES.PointerPos()))
                {
                    if (!mPressed && mButtonPressedCB != null)
                    {
                        mButtonPressedCB(this, mUserData);
                    }

                    mPressed = true;
                    mTouched = true;
                }
                else if ((!FES.ButtonDown(FES.BTN_POINTER_A) || !mHitRect.Contains(FES.PointerPos())) && mTouched)
                {
                    if (mPressed && mButtonReleasedCB != null)
                    {
                        mButtonReleasedCB(this, mUserData);
                    }

                    mTouched = false;
                    mPressed = false;
                }
            }
            else if (!FES.ButtonDown(FES.BTN_POINTER_A))
            {
                mTouchArmed = true;
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            Size2i labelSize = FES.PrintMeasure(mLabel);

            int yPos;
            if (mLabelBottomAligned)
            {
                yPos = mRect.y + mRect.height - labelSize.height - 4;
            }
            else
            {
                yPos = mRect.y + (mRect.height / 2) - (labelSize.height / 2);
            }

            if (mPressed)
            {
                FES.DrawRectFill(new Rect2i(mRect.x + 2, mRect.y + 2, mRect.width - 2, mRect.height - 2), 5);
                FES.Print(new Vector2i(mRect.x + (mRect.width / 2) - (labelSize.width / 2) + 1, yPos + 1), mLabelColor, mLabel);
            }
            else
            {
                FES.DrawRectFill(mRect, mFaceColor);
                FES.Print(new Vector2i(mRect.x + (mRect.width / 2) - (labelSize.width / 2), yPos), mLabelColor, mLabel);
                FES.DrawRect(mRect, 5);
            }

            mHitRect = new Rect2i(-FES.CameraGet().x + mHotZone.x, -FES.CameraGet().y + mHotZone.y, mHotZone.width, mHotZone.height);
        }
    }
}
