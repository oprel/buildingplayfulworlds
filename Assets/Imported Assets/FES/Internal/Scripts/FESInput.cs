namespace FESInternal
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Input subsystem
    /// </summary>
    public class FESInput
    {
        private const int BUTTON_LAST = FES.BTN_POINTER_C;
        private const int MAX_KEY_CODE = (int)KeyCode.Menu;

#pragma warning disable 0414 // Unused warning
        private FESAPI mFESAPI = null;
#pragma warning restore 0414

        private Dictionary<int, bool>[] mButtonPreviousState = new Dictionary<int, bool>[FESHW.HW_MAX_PLAYERS];
        private Dictionary<int, bool>[] mButtonCurrentState = new Dictionary<int, bool>[FESHW.HW_MAX_PLAYERS];

        private bool[] mKeyPreviousState = new bool[MAX_KEY_CODE + 1];
        private bool[] mKeyCurrentState = new bool[MAX_KEY_CODE + 1];

        private string mInputString = string.Empty;

        private Vector2i mPointerPos;
        private bool mPointerPosValid;
        private float mScrollDelta;

        private FES.InputOverrideMethod mOverrideMethod = null;

        /// <summary>
        /// Initialize subsystem
        /// </summary>
        /// <param name="api">Subsystem wrapper reference</param>
        /// <returns>True if successful</returns>
        public bool Initialize(FESAPI api)
        {
            mFESAPI = api;

            for (int i = 0; i < FESHW.HW_MAX_PLAYERS; i++)
            {
                mButtonCurrentState[i] = new Dictionary<int, bool>();
                mButtonPreviousState[i] = new Dictionary<int, bool>();
            }

            GetAllButtonStates(mButtonCurrentState);

            return true;
        }

        /// <summary>
        /// Check if button is down
        /// </summary>
        /// <param name="button">Button</param>
        /// <param name="player">Player number</param>
        /// <returns>True if down</returns>
        public bool ButtonDown(int button, int player)
        {
            return FetchButtonState(button, player);
        }

        /// <summary>
        /// Check if button was pressed since last update
        /// </summary>
        /// <param name="button">Button</param>
        /// <param name="player">Player number</param>
        /// <returns>True if pressed</returns>
        public bool ButtonPressed(int button, int player)
        {
            if (CheckButtonState(button, player, mButtonCurrentState) && !CheckButtonState(button, player, mButtonPreviousState))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if button was released since last update
        /// </summary>
        /// <param name="button">Button</param>
        /// <param name="player">Player number</param>
        /// <returns>True if released</returns>
        public bool ButtonReleased(int button, int player)
        {
            if (!CheckButtonState(button, player, mButtonCurrentState) && CheckButtonState(button, player, mButtonPreviousState))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if key is down
        /// </summary>
        /// <param name="keyCode">Keycode</param>
        /// <returns>True if down</returns>
        public bool KeyDown(KeyCode keyCode)
        {
            if ((int)keyCode < 0 || (int)keyCode >= mKeyCurrentState.Length)
            {
                return false;
            }

            if ((int)keyCode > MAX_KEY_CODE)
            {
                return false;
            }

            return Input.GetKey(keyCode);
        }

        /// <summary>
        /// Check if key was pressed since last update
        /// </summary>
        /// <param name="keyCode">Keycode</param>
        /// <returns>True if pressed</returns>
        public bool KeyPressed(KeyCode keyCode)
        {
            if ((int)keyCode < 0 || (int)keyCode >= mKeyCurrentState.Length)
            {
                return false;
            }

            if (mKeyCurrentState[(int)keyCode] && !mKeyPreviousState[(int)keyCode])
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if key was released since last update
        /// </summary>
        /// <param name="keyCode">Keycode</param>
        /// <returns>True if released</returns>
        public bool KeyReleased(KeyCode keyCode)
        {
            if ((int)keyCode < 0 || (int)keyCode >= mKeyCurrentState.Length)
            {
                return false;
            }

            if (!mKeyCurrentState[(int)keyCode] && mKeyPreviousState[(int)keyCode])
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get pointer (mouse or touch) position
        /// </summary>
        /// <returns>Position</returns>
        public Vector2i PointerPos()
        {
            return mPointerPos;
        }

        /// <summary>
        /// Check if pointer position is valid. It's not valid if there is no pointer devices, or touch screen is not pressed
        /// </summary>
        /// <returns>True if valid</returns>
        public bool PointerPosValid()
        {
            return mPointerPosValid;
        }

        /// <summary>
        /// Set input override method
        /// </summary>
        /// <param name="overrideMethod">Override method</param>
        public void InputOverride(FES.InputOverrideMethod overrideMethod)
        {
            mOverrideMethod = overrideMethod;
        }

        /// <summary>
        /// New update frame just started, setup input states
        /// </summary>
        public void FrameStart()
        {
            // Copy all previous button values
            for (int p = 0; p < FESHW.HW_MAX_PLAYERS; p++)
            {
                foreach (KeyValuePair<int, bool> state in mButtonCurrentState[p])
                {
                    mButtonPreviousState[p][state.Key] = state.Value;
                }
            }

            // Copy all previous key values
            for (int i = 0; i < MAX_KEY_CODE; i++)
            {
                mKeyPreviousState[i] = mKeyCurrentState[i];
            }

            GetAllKeyStates(mKeyCurrentState);

            // Update current values
            GetAllButtonStates(mButtonCurrentState);

            mPointerPosValid = false;

            // Get current mouse coordinates
            if (!Input.mousePresent && Input.touchCount == 0)
            {
                mPointerPos = Vector2i.zero;
            }
            else
            {
                Vector2 mousePos;

                if (Input.touchCount > 0)
                {
                    mousePos = mFESAPI.PixelCamera.ScreenToViewportPoint(Input.touches[0].position);
                    mPointerPosValid = true;
                }
                else if (Input.mousePresent)
                {
                    mousePos = mFESAPI.PixelCamera.ScreenToViewportPoint(Input.mousePosition);
                    mPointerPosValid = true;
                }
                else
                {
                    mousePos = Vector2.zero;
                }

                mPointerPos.x = (int)mousePos.x;
                mPointerPos.y = (int)mousePos.y;
            }
        }

        /// <summary>
        /// Update frame just ended, reset some parameters
        /// </summary>
        public void FrameEnd()
        {
            mInputString = string.Empty;
            mScrollDelta = 0;
        }

        /// <summary>
        /// Append to input string for the frame
        /// </summary>
        /// <param name="str">String to append with</param>
        public void AppendInputString(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                // Filter out invalid ascii characters
                if ((str[i] >= ' ' && str[i] <= 127) || str[i] == 0x8 || str[i] == 0x9 || str[i] == 0xD || str[i] == 0x1B)
                {
                    mInputString += str[i];
                }
            }
        }

        /// <summary>
        /// Get input string for this frame
        /// </summary>
        /// <returns>Input string</returns>
        public string InputString()
        {
            return mInputString;
        }

        /// <summary>
        /// Update scroll wheel delta
        /// </summary>
        public void UpdateScrollWheel()
        {
            mScrollDelta += Input.mouseScrollDelta.y;
        }

        /// <summary>
        /// Get scroll delta
        /// </summary>
        /// <returns>Scroll delta</returns>
        public float PointerScrollDelta()
        {
            return mScrollDelta;
        }

        private bool CheckButtonState(int button, int player, Dictionary<int, bool>[] states)
        {
            // Check if any buttons are pressed for any of the given players, if so then return true
            for (int p = 0; p < FESHW.HW_MAX_PLAYERS; p++)
            {
                if ((player & (1 << p)) != 0)
                {
                    for (int b = 1; b <= BUTTON_LAST;)
                    {
                        if ((button & b) != 0 && states[p][b])
                        {
                            return true;
                        }

                        b = b << 1;
                    }
                }
            }

            return false;
        }

        private bool FetchButtonState(int button, int player)
        {
            float joystickThreshold = 0.75f;

            // Check if user input overwrite already handles this input
            if (mOverrideMethod != null)
            {
                bool overrideHandled = false;
                bool ret = mOverrideMethod(button, player, out overrideHandled);
                if (overrideHandled)
                {
                    return ret;
                }
            }

            if ((button & FES.BTN_SYSTEM) != 0)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    return true;
                }

                if (Input.GetButton("SYSTEM"))
                {
                    return true;
                }
            }

            if ((player & FES.PLAYER_ONE) != 0)
            {
                if ((button & FES.BTN_UP) != 0)
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        return true;
                    }

                    if (Input.GetAxisRaw("P1_VERTICAL") < -joystickThreshold || Input.GetAxisRaw("P1_VERTICAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_DOWN) != 0)
                {
                    if (Input.GetKey(KeyCode.S))
                    {
                        return true;
                    }

                    if (Input.GetAxisRaw("P1_VERTICAL") > joystickThreshold || Input.GetAxisRaw("P1_VERTICAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_LEFT) != 0)
                {
                    if (Input.GetKey(KeyCode.A))
                    {
                        return true;
                    }

                    if (Input.GetAxisRaw("P1_HORIZONTAL") < -joystickThreshold || Input.GetAxisRaw("P1_HORIZONTAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_RIGHT) != 0)
                {
                    if (Input.GetKey(KeyCode.D))
                    {
                        return true;
                    }

                    if (Input.GetAxisRaw("P1_HORIZONTAL") > joystickThreshold || Input.GetAxisRaw("P1_HORIZONTAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_A) != 0)
                {
                    if (Input.GetKey(KeyCode.B))
                    {
                        return true;
                    }

                    if (Input.GetKey(KeyCode.Space))
                    {
                        return true;
                    }

                    if (Input.GetButton("P1_A"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_B) != 0)
                {
                    if (Input.GetKey(KeyCode.N))
                    {
                        return true;
                    }

                    if (Input.GetButton("P1_B"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_X) != 0)
                {
                    if (Input.GetKey(KeyCode.G))
                    {
                        return true;
                    }

                    if (Input.GetButton("P1_X"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_Y) != 0)
                {
                    if (Input.GetKey(KeyCode.H))
                    {
                        return true;
                    }

                    if (Input.GetButton("P1_Y"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_LS) != 0)
                {
                    if (Input.GetKey(KeyCode.T))
                    {
                        return true;
                    }

                    if (Input.GetButton("P1_LS") || Input.GetAxisRaw("P1_LS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_RS) != 0)
                {
                    if (Input.GetKey(KeyCode.Y))
                    {
                        return true;
                    }

                    if (Input.GetButton("P1_RS") || Input.GetAxisRaw("P1_RS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_MENU) != 0)
                {
                    if (Input.GetKey(KeyCode.Alpha5))
                    {
                        return true;
                    }

                    if (Input.GetButton("P1_MENU"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_POINTER_A) != 0)
                {
                    if (Input.GetMouseButton(0))
                    {
                        return true;
                    }

                    if (Input.touchCount > 0)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_POINTER_B) != 0)
                {
                    if (Input.GetMouseButton(1))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_POINTER_C) != 0)
                {
                    if (Input.GetMouseButton(2))
                    {
                        return true;
                    }
                }
            }

            if ((player & FES.PLAYER_TWO) != 0)
            {
                if ((button & FES.BTN_UP) != 0)
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        return true;
                    }

                    if (Input.GetAxisRaw("P2_VERTICAL") < -joystickThreshold || Input.GetAxisRaw("P2_VERTICAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_DOWN) != 0)
                {
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        return true;
                    }

                    if (Input.GetAxisRaw("P2_VERTICAL") > joystickThreshold || Input.GetAxisRaw("P2_VERTICAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_LEFT) != 0)
                {
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        return true;
                    }

                    if (Input.GetAxisRaw("P2_HORIZONTAL") < -joystickThreshold || Input.GetAxisRaw("P2_HORIZONTAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_RIGHT) != 0)
                {
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        return true;
                    }

                    if (Input.GetAxisRaw("P2_HORIZONTAL") > joystickThreshold || Input.GetAxisRaw("P2_HORIZONTAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_A) != 0)
                {
                    if (Input.GetKey(KeyCode.Semicolon))
                    {
                        return true;
                    }

                    if (Input.GetKey(KeyCode.Keypad1))
                    {
                        return true;
                    }

                    if (Input.GetKey(KeyCode.RightControl))
                    {
                        return true;
                    }

                    if (Input.GetButton("P2_A"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_B) != 0)
                {
                    if (Input.GetKey(KeyCode.Quote))
                    {
                        return true;
                    }

                    if (Input.GetKey(KeyCode.Keypad2))
                    {
                        return true;
                    }

                    if (Input.GetButton("P2_B"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_X) != 0)
                {
                    if (Input.GetKey(KeyCode.P))
                    {
                        return true;
                    }

                    if (Input.GetKey(KeyCode.Keypad4))
                    {
                        return true;
                    }

                    if (Input.GetButton("P2_X"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_Y) != 0)
                {
                    if (Input.GetKey(KeyCode.LeftBracket))
                    {
                        return true;
                    }

                    if (Input.GetKey(KeyCode.Keypad5))
                    {
                        return true;
                    }

                    if (Input.GetButton("P2_Y"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_LS) != 0)
                {
                    if (Input.GetKey(KeyCode.Alpha0))
                    {
                        return true;
                    }

                    if (Input.GetKey(KeyCode.Keypad7))
                    {
                        return true;
                    }

                    if (Input.GetButton("P2_LS") || Input.GetAxisRaw("P2_LS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_RS) != 0)
                {
                    if (Input.GetKey(KeyCode.Minus))
                    {
                        return true;
                    }

                    if (Input.GetKey(KeyCode.Keypad8))
                    {
                        return true;
                    }

                    if (Input.GetButton("P2_RS") || Input.GetAxisRaw("P2_RS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_MENU) != 0)
                {
                    if (Input.GetKey(KeyCode.Backspace))
                    {
                        return true;
                    }

                    if (Input.GetKey(KeyCode.KeypadDivide))
                    {
                        return true;
                    }

                    if (Input.GetButton("P2_MENU"))
                    {
                        return true;
                    }
                }
            }

            if ((player & FES.PLAYER_THREE) != 0)
            {
                if ((button & FES.BTN_UP) != 0)
                {
                    if (Input.GetAxisRaw("P3_VERTICAL") < -joystickThreshold || Input.GetAxisRaw("P3_VERTICAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_DOWN) != 0)
                {
                    if (Input.GetAxisRaw("P3_VERTICAL") > joystickThreshold || Input.GetAxisRaw("P3_VERTICAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_LEFT) != 0)
                {
                    if (Input.GetAxisRaw("P3_HORIZONTAL") < -joystickThreshold || Input.GetAxisRaw("P3_HORIZONTAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_RIGHT) != 0)
                {
                    if (Input.GetAxisRaw("P3_HORIZONTAL") > joystickThreshold || Input.GetAxisRaw("P3_HORIZONTAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_A) != 0)
                {
                    if (Input.GetButton("P3_A"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_B) != 0)
                {
                    if (Input.GetButton("P3_B"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_X) != 0)
                {
                    if (Input.GetButton("P3_X"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_Y) != 0)
                {
                    if (Input.GetButton("P3_Y"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_LS) != 0)
                {
                    if (Input.GetButton("P3_LS") || Input.GetAxisRaw("P3_LS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_RS) != 0)
                {
                    if (Input.GetButton("P3_RS") || Input.GetAxisRaw("P3_RS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_MENU) != 0)
                {
                    if (Input.GetButton("P3_MENU"))
                    {
                        return true;
                    }
                }
            }

            if ((player & FES.PLAYER_FOUR) != 0)
            {
                if ((button & FES.BTN_UP) != 0)
                {
                    if (Input.GetAxisRaw("P4_VERTICAL") < -joystickThreshold || Input.GetAxisRaw("P4_VERTICAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_DOWN) != 0)
                {
                    if (Input.GetAxisRaw("P4_VERTICAL") > joystickThreshold || Input.GetAxisRaw("P4_VERTICAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_LEFT) != 0)
                {
                    if (Input.GetAxisRaw("P4_HORIZONTAL") < -joystickThreshold || Input.GetAxisRaw("P4_HORIZONTAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_RIGHT) != 0)
                {
                    if (Input.GetAxisRaw("P4_HORIZONTAL") > joystickThreshold || Input.GetAxisRaw("P4_HORIZONTAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_A) != 0)
                {
                    if (Input.GetButton("P4_A"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_B) != 0)
                {
                    if (Input.GetButton("P4_B"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_X) != 0)
                {
                    if (Input.GetButton("P4_X"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_Y) != 0)
                {
                    if (Input.GetButton("P4_Y"))
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_LS) != 0)
                {
                    if (Input.GetButton("P4_LS") || Input.GetAxisRaw("P4_LS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_RS) != 0)
                {
                    if (Input.GetButton("P4_RS") || Input.GetAxisRaw("P4_RS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & FES.BTN_MENU) != 0)
                {
                    if (Input.GetButton("P4_MENU"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void GetAllButtonStates(Dictionary<int, bool>[] states)
        {
            for (int p = 0; p < FESHW.HW_MAX_PLAYERS; p++)
            {
                for (int b = 1; b <= BUTTON_LAST;)
                {
                    states[p][b] = FetchButtonState(b, 1 << p);
                    b = b << 1;
                }
            }
        }

        private void GetAllKeyStates(bool[] keyStates)
        {
            for (int i = 0; i < MAX_KEY_CODE; i++)
            {
                keyStates[i] = Input.GetKey((KeyCode)i);
            }
        }
    }
}
