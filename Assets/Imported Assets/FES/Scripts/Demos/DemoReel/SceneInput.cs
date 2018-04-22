namespace FESDemo
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Demonstrate input
    /// </summary>
    public class SceneInput : SceneDemo
    {
        private Dictionary<string, KeyboardKey> mKeys = new Dictionary<string, KeyboardKey>();

        private string mInputStr;

        private Button mNextButton;

        private float mScrollDelta;
        private float mScrollDeltaAnim;

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneInput()
        {
            InitKeyboard();

            mNextButton = new Button(new Rect2i(550, 334, 87, 23), new Rect2i(550, 334, 87, 23), 3, 2, "Touch here to go\nto the next screen", (KeyCode)555, 0, NextScreenButtonCB);
        }

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            mInputStr = string.Empty;
            mNextButton.Reset();
            base.Enter();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            DemoReel demo = (DemoReel)FES.Game;

            mNextButton.Update();

            ProcessStringInput();

            int color = 1;
            if ((FES.Ticks % 200 > 170 && FES.Ticks % 200 < 180) || (FES.Ticks % 200) > 190)
            {
                color = 5;
            }

            mNextButton.LabelColor = color;

            mScrollDelta = FES.PointerScrollDelta();

            mScrollDeltaAnim = Mathf.Lerp(mScrollDeltaAnim, mScrollDelta, 0.25f);

            if (FES.ButtonPressed(FES.BTN_SYSTEM))
            {
                Application.Quit();
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Clear(1);
            }
            else
            {
                FES.Clear(DemoUtil.IndexToRGB(1));
            }

            string str = DemoUtil.HighlightCode(
                "@C// Test gamepad input for two players\n" +
                "@Kif @N(@MFES@N.ButtonDown(@MFES.@NBTN_A, @MFES@N.PLAYER_ONE) {\n" +
                "   @C// Handle button A down for player one\n@N" +
                "}\n" +
                "@Kif @N(@MFES@N.ButtoPressed(@MFES.@NBTN_LEFT, @MFES@N.PLAYER_TWO) {\n" +
                "   @C// Handle button LEFT transitioning from \"up\" to \"down\"\n@N" +
                "}\n" +
                "@Kif @N(@MFES@N.ButtonReleased(@MFES.@NBTN_MENU, @MFES@N.PLAYER_ANY) {\n" +
                "   @C// Handle button MENU transitioning from \"down\" to \"up\"\n@N" +
                "}");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(4, 4), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(4, 4), DemoUtil.IndexToRGB(5), str);
            }

            DrawGamepad(260, 15, FES.PLAYER_ONE);
            DrawGamepad(400, 15, FES.PLAYER_TWO);
            DrawMouse(540, 8);

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawLine(new Vector2i(16, 85), new Vector2i(FES.DisplaySize.width - 16, 85), 2);
            }
            else
            {
                FES.DrawLine(new Vector2i(16, 85), new Vector2i(FES.DisplaySize.width - 16, 85), DemoUtil.IndexToRGB(2));
            }

            str = DemoUtil.HighlightCode(
                "@C// Test keyboard input\n" +
                "@Kif @N(@MFES@N.KeyDown(@MKeyCode@N.LeftShift)) {\n" +
                "   @C// Handle Left Shift down\n@N" +
                "}\n\n" +
                "@C// Retrieve the string of characters typed since last update\n" +
                "@Kstring@n userInput += @MFES@N.InputString();");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(50, 92), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(50, 92), DemoUtil.IndexToRGB(5), str);
            }

            str = DemoUtil.HighlightCode(
                "@C// Get pointer position (mouse or touch)\n" +
                "@MFES@N.DrawSprite(@L4@N, @MFES@N.PointerPos());\n\n" +
                "@C// Test pointer button input\n" +
                "@Kif @N(@MFES@N.ButtonPressed(@MFES@N.BTN_POINTER_A)) {\n" +
                "   @C// Handle Pointer Button A down\n@N" +
                "}\n");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(350, 92), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(350, 92), DemoUtil.IndexToRGB(5), str);
            }

            DrawKeyboard(14, 194);
            DrawInputString(14, 155);

            mNextButton.Render();

            if (FES.PointerPosValid())
            {
                FES.DrawSprite(4, FES.PointerPos());
            }
        }

        private void NextScreenButtonCB(Button button, object userData)
        {
            DemoReel demo = (DemoReel)FES.Game;
            demo.NextScene();
        }

        private void ProcessStringInput()
        {
            string input = FES.InputString();
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c >= ' ')
                {
                    mInputStr += c;
                }
                else if (c == (char)KeyCode.Backspace && mInputStr.Length > 0)
                {
                    mInputStr = mInputStr.Substring(0, mInputStr.Length - 1);
                }
                else if (c == (char)KeyCode.Return)
                {
                    mInputStr += '\n';
                    int lines = 1;
                    for (int j = 0; j < mInputStr.Length; j++)
                    {
                        if (mInputStr[j] == '\n')
                        {
                            lines++;
                        }
                    }

                    if (lines > 3)
                    {
                        mInputStr = string.Empty;
                    }
                }
            }
        }

        private void InitKeyboard()
        {
            int xStart = 8;
            int yStart = 8;
            int w = 24;
            int h = 18;

            int space = 2;

            int x = xStart;
            int y = yStart;

            // Row 1
            mKeys["Esc"] = new KeyboardKey(new Rect2i(x, y, w, h), "Esc", KeyCode.Escape);
            x += w + (space * 10);
            mKeys["F1"] = new KeyboardKey(new Rect2i(x, y, w, h), "F1", KeyCode.F1);
            x += w + space;
            mKeys["F2"] = new KeyboardKey(new Rect2i(x, y, w, h), "F2", KeyCode.F2);
            x += w + space;
            mKeys["F3"] = new KeyboardKey(new Rect2i(x, y, w, h), "F3", KeyCode.F3);
            x += w + space;
            mKeys["F4"] = new KeyboardKey(new Rect2i(x, y, w, h), "F4", KeyCode.F4);
            x += w + (space * 9);
            mKeys["F5"] = new KeyboardKey(new Rect2i(x, y, w, h), "F5", KeyCode.F5);
            x += w + space;
            mKeys["F6"] = new KeyboardKey(new Rect2i(x, y, w, h), "F6", KeyCode.F6);
            x += w + space;
            mKeys["F7"] = new KeyboardKey(new Rect2i(x, y, w, h), "F7", KeyCode.F7);
            x += w + space;
            mKeys["F8"] = new KeyboardKey(new Rect2i(x, y, w, h), "F8", KeyCode.F8);
            x += w + (space * 9);
            mKeys["F9"] = new KeyboardKey(new Rect2i(x, y, w, h), "F9", KeyCode.F9);
            x += w + space;
            mKeys["F10"] = new KeyboardKey(new Rect2i(x, y, w, h), "F10", KeyCode.F10);
            x += w + space;
            mKeys["F11"] = new KeyboardKey(new Rect2i(x, y, w, h), "F11", KeyCode.F11);
            x += w + space;
            mKeys["F12"] = new KeyboardKey(new Rect2i(x, y, w, h), "F12", KeyCode.F12);
            x += w + (space * 8);
            mKeys["PrintScreen"] = new KeyboardKey(new Rect2i(x, y, w, h), "Prt\nScn", KeyCode.SysReq);
            x += w + space;
            mKeys["ScrollLock"] = new KeyboardKey(new Rect2i(x, y, w, h), "Scr\nLk", KeyCode.ScrollLock);
            x += w + space;
            mKeys["Pause"] = new KeyboardKey(new Rect2i(x, y, w, h), "Paus", KeyCode.Pause);

            x = xStart;
            y += h + (space * 3);

            // Row 2
            mKeys["~"] = new KeyboardKey(new Rect2i(x, y, w, h), "~\n`", KeyCode.BackQuote);
            x += w + space;
            mKeys["1"] = new KeyboardKey(new Rect2i(x, y, w, h), "!\n1", KeyCode.Alpha1, KeyCode.Exclaim);
            x += w + space;
            mKeys["2"] = new KeyboardKey(new Rect2i(x, y, w, h), "@@\n2", KeyCode.Alpha2, KeyCode.At);
            x += w + space;
            mKeys["3"] = new KeyboardKey(new Rect2i(x, y, w, h), "#\n3", KeyCode.Alpha3, KeyCode.Hash);
            x += w + space;
            mKeys["4"] = new KeyboardKey(new Rect2i(x, y, w, h), "$\n4", KeyCode.Alpha4, KeyCode.Dollar);
            x += w + space;
            mKeys["5"] = new KeyboardKey(new Rect2i(x, y, w, h), "%\n5", KeyCode.Alpha5);
            x += w + space;
            mKeys["6"] = new KeyboardKey(new Rect2i(x, y, w, h), "^\n6", KeyCode.Alpha6);
            x += w + space;
            mKeys["7"] = new KeyboardKey(new Rect2i(x, y, w, h), "&\n7", KeyCode.Alpha7, KeyCode.Ampersand);
            x += w + space;
            mKeys["8"] = new KeyboardKey(new Rect2i(x, y, w, h), "*\n8", KeyCode.Alpha8);
            x += w + space;
            mKeys["9"] = new KeyboardKey(new Rect2i(x, y, w, h), "(\n9", KeyCode.Alpha9, KeyCode.LeftParen);
            x += w + space;
            mKeys["0"] = new KeyboardKey(new Rect2i(x, y, w, h), ")\n0", KeyCode.Alpha0, KeyCode.RightParen);
            x += w + space;
            mKeys["Minus"] = new KeyboardKey(new Rect2i(x, y, w, h), "_\n-", KeyCode.Minus, KeyCode.Underscore);
            x += w + space;
            mKeys["Plus"] = new KeyboardKey(new Rect2i(x, y, w, h), "+\n=", KeyCode.Plus, KeyCode.Equals);
            x += w + space;
            mKeys["Backspace"] = new KeyboardKey(new Rect2i(x, y, w * 2, h), "Bkspace", KeyCode.Backspace);
            x += (w * 2) + (space * 8);
            mKeys["Insert"] = new KeyboardKey(new Rect2i(x, y, w, h), "Ins", KeyCode.Insert);
            x += w + space;
            mKeys["Home"] = new KeyboardKey(new Rect2i(x, y, w, h), "Home", KeyCode.Home);
            x += w + space;
            mKeys["PageUp"] = new KeyboardKey(new Rect2i(x, y, w, h), "Pg\nUp", KeyCode.PageUp);
            x += w + (space * 8);
            mKeys["NumLock"] = new KeyboardKey(new Rect2i(x, y, w, h), "Num\nLock", KeyCode.Numlock);
            x += w + space;
            mKeys["/"] = new KeyboardKey(new Rect2i(x, y, w, h), "/", KeyCode.KeypadDivide);
            x += w + space;
            mKeys["*"] = new KeyboardKey(new Rect2i(x, y, w, h), "*", KeyCode.KeypadMultiply);
            x += w + space;
            mKeys["KeypadMinus"] = new KeyboardKey(new Rect2i(x, y, w, h), "-", KeyCode.KeypadMinus);
            x += w + space;

            x = xStart;
            y += h + space;

            // Row 3
            mKeys["Tab"] = new KeyboardKey(new Rect2i(x, y, (int)(w * 1.5f), h), "Tab", KeyCode.Tab);
            x += (int)(w * 1.5f) + space;
            mKeys["Q"] = new KeyboardKey(new Rect2i(x, y, w, h), "Q", KeyCode.Q);
            x += w + space;
            mKeys["W"] = new KeyboardKey(new Rect2i(x, y, w, h), "W", KeyCode.W);
            x += w + space;
            mKeys["E"] = new KeyboardKey(new Rect2i(x, y, w, h), "E", KeyCode.E);
            x += w + space;
            mKeys["R"] = new KeyboardKey(new Rect2i(x, y, w, h), "R", KeyCode.R);
            x += w + space;
            mKeys["T"] = new KeyboardKey(new Rect2i(x, y, w, h), "T", KeyCode.T);
            x += w + space;
            mKeys["Y"] = new KeyboardKey(new Rect2i(x, y, w, h), "Y", KeyCode.Y);
            x += w + space;
            mKeys["U"] = new KeyboardKey(new Rect2i(x, y, w, h), "U", KeyCode.U);
            x += w + space;
            mKeys["I"] = new KeyboardKey(new Rect2i(x, y, w, h), "I", KeyCode.I);
            x += w + space;
            mKeys["O"] = new KeyboardKey(new Rect2i(x, y, w, h), "O", KeyCode.O);
            x += w + space;
            mKeys["P"] = new KeyboardKey(new Rect2i(x, y, w, h), "P", KeyCode.P);
            x += w + space;
            mKeys["{"] = new KeyboardKey(new Rect2i(x, y, w, h), "{\n[", KeyCode.LeftBracket);
            x += w + space;
            mKeys["}"] = new KeyboardKey(new Rect2i(x, y, w, h), "}\n]", KeyCode.RightBracket);
            x += w + space;
            mKeys["|"] = new KeyboardKey(new Rect2i(x, y, (int)(w * 1.5f), h), "|\n\\", KeyCode.Backslash);
            x += (int)(w * 1.5f) + (space * 8);
            mKeys["Delete"] = new KeyboardKey(new Rect2i(x, y, w, h), "Del", KeyCode.Delete);
            x += w + space;
            mKeys["End"] = new KeyboardKey(new Rect2i(x, y, w, h), "End", KeyCode.End);
            x += w + space;
            mKeys["PageDown"] = new KeyboardKey(new Rect2i(x, y, w, h), "Pg\nDown", KeyCode.PageDown);
            x += w + (space * 8);
            mKeys["Keypad7"] = new KeyboardKey(new Rect2i(x, y, w, h), "7", KeyCode.Keypad7);
            x += w + space;
            mKeys["Keypad8"] = new KeyboardKey(new Rect2i(x, y, w, h), "8", KeyCode.Keypad8);
            x += w + space;
            mKeys["Keypad9"] = new KeyboardKey(new Rect2i(x, y, w, h), "9", KeyCode.Keypad9);
            x += w + space;
            mKeys["KeypadPlus"] = new KeyboardKey(new Rect2i(x, y, w, h + space + h), "+", KeyCode.KeypadPlus);
            x += w + space;

            x = xStart;
            y += h + space;

            // Row 4
            mKeys["Caps"] = new KeyboardKey(new Rect2i(x, y, (int)(w * 1.7f), h), "Caps\nLock", KeyCode.CapsLock);
            x += (int)(w * 1.7f) + space;
            mKeys["A"] = new KeyboardKey(new Rect2i(x, y, w, h), "A", KeyCode.A);
            x += w + space;
            mKeys["S"] = new KeyboardKey(new Rect2i(x, y, w, h), "S", KeyCode.S);
            x += w + space;
            mKeys["D"] = new KeyboardKey(new Rect2i(x, y, w, h), "D", KeyCode.D);
            x += w + space;
            mKeys["F"] = new KeyboardKey(new Rect2i(x, y, w, h), "F", KeyCode.F);
            x += w + space;
            mKeys["G"] = new KeyboardKey(new Rect2i(x, y, w, h), "G", KeyCode.G);
            x += w + space;
            mKeys["H"] = new KeyboardKey(new Rect2i(x, y, w, h), "H", KeyCode.H);
            x += w + space;
            mKeys["J"] = new KeyboardKey(new Rect2i(x, y, w, h), "J", KeyCode.J);
            x += w + space;
            mKeys["K"] = new KeyboardKey(new Rect2i(x, y, w, h), "K", KeyCode.K);
            x += w + space;
            mKeys["L"] = new KeyboardKey(new Rect2i(x, y, w, h), "L", KeyCode.L);
            x += w + space;
            mKeys[";"] = new KeyboardKey(new Rect2i(x, y, w, h), ":\n;", KeyCode.Colon, KeyCode.Semicolon);
            x += w + space;
            mKeys["'"] = new KeyboardKey(new Rect2i(x, y, w, h), "\"\n'", KeyCode.DoubleQuote, KeyCode.Quote);
            x += w + space;
            mKeys["Enter"] = new KeyboardKey(new Rect2i(x, y, (int)(w * 2.45f), h), "Enter", KeyCode.Return);
            x += (int)(w * 2.45f) + (space * 2) + (space * 8) + (w * 3) + (space * 8);
            mKeys["Keypad4"] = new KeyboardKey(new Rect2i(x, y, w, h), "4", KeyCode.Keypad4);
            x += w + space;
            mKeys["Keypad5"] = new KeyboardKey(new Rect2i(x, y, w, h), "5", KeyCode.Keypad5);
            x += w + space;
            mKeys["Keypad6"] = new KeyboardKey(new Rect2i(x, y, w, h), "6", KeyCode.Keypad6);

            x = xStart;
            y += h + space;

            // Row 5
            mKeys["LeftShift"] = new KeyboardKey(new Rect2i(x, y, (int)(w * 2.2f), h), "Shift", KeyCode.LeftShift);
            x += (int)(w * 2.2f) + space;
            mKeys["Z"] = new KeyboardKey(new Rect2i(x, y, w, h), "Z", KeyCode.Z);
            x += w + space;
            mKeys["X"] = new KeyboardKey(new Rect2i(x, y, w, h), "X", KeyCode.X);
            x += w + space;
            mKeys["C"] = new KeyboardKey(new Rect2i(x, y, w, h), "C", KeyCode.C);
            x += w + space;
            mKeys["V"] = new KeyboardKey(new Rect2i(x, y, w, h), "V", KeyCode.V);
            x += w + space;
            mKeys["B"] = new KeyboardKey(new Rect2i(x, y, w, h), "B", KeyCode.B);
            x += w + space;
            mKeys["N"] = new KeyboardKey(new Rect2i(x, y, w, h), "N", KeyCode.N);
            x += w + space;
            mKeys["M"] = new KeyboardKey(new Rect2i(x, y, w, h), "M", KeyCode.M);
            x += w + space;
            mKeys["<"] = new KeyboardKey(new Rect2i(x, y, w, h), "<\n,", KeyCode.Less, KeyCode.Comma);
            x += w + space;
            mKeys[">"] = new KeyboardKey(new Rect2i(x, y, w, h), ">\n.", KeyCode.Greater, KeyCode.Period);
            x += w + space;
            mKeys["?"] = new KeyboardKey(new Rect2i(x, y, w, h), "?\n/", KeyCode.Question, KeyCode.Slash);
            x += w + space;
            mKeys["RightShift"] = new KeyboardKey(new Rect2i(x, y, (int)(w * 3.0f), h), "Shift", KeyCode.RightShift);
            x += (int)(w * 3.0f) + (space * 8) + w + space;
            mKeys["Up"] = new KeyboardKey(new Rect2i(x, y, w, h), ((char)127).ToString(), KeyCode.UpArrow);
            x += w + space + w + (space * 8);
            mKeys["Keypad1"] = new KeyboardKey(new Rect2i(x, y, w, h), "1", KeyCode.Keypad1);
            x += w + space;
            mKeys["Keypad2"] = new KeyboardKey(new Rect2i(x, y, w, h), "2", KeyCode.Keypad2);
            x += w + space;
            mKeys["Keypad3"] = new KeyboardKey(new Rect2i(x, y, w, h), "3", KeyCode.Keypad3);
            x += w + space;
            mKeys["KeypadEnter"] = new KeyboardKey(new Rect2i(x, y, w, h + space + h), "Ent", KeyCode.KeypadEnter);
            x += w + space;

            x = xStart;
            y += h + space;

            // Row 6
            mKeys["LeftCtrl"] = new KeyboardKey(new Rect2i(x, y, (int)(w * 1.4f), h), "Ctrl", KeyCode.LeftControl);
            x += (int)(w * 1.4f) + space;
            mKeys["Sys"] = new KeyboardKey(new Rect2i(x, y, (int)(w * 1.4f), h), "Sys", KeyCode.LeftWindows);
            x += (int)(w * 1.4f) + space;
            mKeys["LeftAlt"] = new KeyboardKey(new Rect2i(x, y, (int)(w * 1.4f), h), "Alt", KeyCode.LeftAlt);
            x += (int)(w * 1.4f) + space;
            mKeys["Space"] = new KeyboardKey(new Rect2i(x, y, (int)(w * 7.35f), h), "Space", KeyCode.Space);
            x += (int)(w * 7.35f) + space;
            mKeys["RightAlt"] = new KeyboardKey(new Rect2i(x, y, (int)(w * 1.4f), h), "Alt", KeyCode.RightAlt);
            x += (int)(w * 1.4f) + space;
            mKeys["Menu"] = new KeyboardKey(new Rect2i(x, y, (int)(w * 1.4f), h), "Menu", KeyCode.Menu);
            x += (int)(w * 1.4f) + space;
            mKeys["RightCtrl"] = new KeyboardKey(new Rect2i(x, y, (int)(w * 1.4f), h), "Ctrl", KeyCode.RightControl);
            x += (int)(w * 1.4f) + (space * 8);
            mKeys["Left"] = new KeyboardKey(new Rect2i(x, y, w, h), ((char)129).ToString(), KeyCode.LeftArrow);
            x += w + space;
            mKeys["Down"] = new KeyboardKey(new Rect2i(x, y, w, h), ((char)128).ToString(), KeyCode.DownArrow);
            x += w + space;
            mKeys["Right"] = new KeyboardKey(new Rect2i(x, y, w, h), ((char)130).ToString(), KeyCode.RightArrow);
            x += w + (space * 8);
            mKeys["Keypad0"] = new KeyboardKey(new Rect2i(x, y, (w * 2) + space, h), "0", KeyCode.Keypad0);
            x += (w * 2) + (space * 2);
            mKeys["KeypadPeriod"] = new KeyboardKey(new Rect2i(x, y, w, h), ".", KeyCode.KeypadPeriod);
        }

        private void DrawGamepadABXY(int x, int y, string label, bool pressed, int color, int colorOutline)
        {
            var demo = (DemoReel)FES.Game;

            int faceColor = pressed ? 5 : color;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawEllipseFill(new Vector2i(x, y), new Vector2i(7, 7), faceColor);
                FES.DrawEllipse(new Vector2i(x, y), new Vector2i(7, 7), colorOutline);
                FES.Print(new Vector2i(x - 2, y - 3), colorOutline, label);
            }
            else
            {
                FES.DrawEllipseFill(new Vector2i(x, y), new Vector2i(7, 7), DemoUtil.IndexToRGB(faceColor));
                FES.DrawEllipse(new Vector2i(x, y), new Vector2i(7, 7), DemoUtil.IndexToRGB(colorOutline));
                FES.Print(new Vector2i(x - 2, y - 3), DemoUtil.IndexToRGB(colorOutline), label);
            }
        }

        private void DrawGamepadMenu(int x, int y, bool pressed, int color, int colorOutline)
        {
            var demo = (DemoReel)FES.Game;

            int faceColor = pressed ? 5 : color;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawEllipseFill(new Vector2i(x, y), new Vector2i(7, 7), faceColor);
                FES.DrawEllipseFill(new Vector2i(x + 10, y), new Vector2i(7, 7), faceColor);

                FES.DrawEllipse(new Vector2i(x, y), new Vector2i(7, 7), colorOutline);
                FES.DrawEllipse(new Vector2i(x + 10, y), new Vector2i(7, 7), colorOutline);
                FES.DrawRect(new Rect2i(x, y - 7, 10, 15), colorOutline);

                FES.DrawRectFill(new Rect2i(x, y - 6, 10, 13), faceColor);

                FES.Print(new Vector2i(x - 4, y - 3), colorOutline, "MENU");
            }
            else
            {
                FES.DrawEllipseFill(new Vector2i(x, y), new Vector2i(7, 7), DemoUtil.IndexToRGB(faceColor));
                FES.DrawEllipseFill(new Vector2i(x + 10, y), new Vector2i(7, 7), DemoUtil.IndexToRGB(faceColor));

                FES.DrawEllipse(new Vector2i(x, y), new Vector2i(7, 7), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawEllipse(new Vector2i(x + 10, y), new Vector2i(7, 7), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawRect(new Rect2i(x, y - 7, 10, 15), DemoUtil.IndexToRGB(colorOutline));

                FES.DrawRectFill(new Rect2i(x, y - 6, 10, 13), DemoUtil.IndexToRGB(faceColor));

                FES.Print(new Vector2i(x - 4, y - 3), DemoUtil.IndexToRGB(colorOutline), "MENU");
            }
        }

        private void DrawGamepadSystem(int x, int y, bool pressed, int color, int colorOutline)
        {
            var demo = (DemoReel)FES.Game;

            int faceColor = pressed ? 5 : color;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawEllipseFill(new Vector2i(x, y), new Vector2i(7, 7), faceColor);
                FES.DrawEllipseFill(new Vector2i(x + 6, y), new Vector2i(7, 7), faceColor);

                FES.DrawEllipse(new Vector2i(x, y), new Vector2i(7, 7), colorOutline);
                FES.DrawEllipse(new Vector2i(x + 6, y), new Vector2i(7, 7), colorOutline);
                FES.DrawRect(new Rect2i(x - 1, y - 7, 10, 15), colorOutline);

                FES.DrawRectFill(new Rect2i(x - 1, y - 6, 10, 13), faceColor);

                FES.Print(new Vector2i(x - 4, y - 3), colorOutline, "SYS");
            }
            else
            {
                FES.DrawEllipseFill(new Vector2i(x, y), new Vector2i(7, 7), DemoUtil.IndexToRGB(faceColor));
                FES.DrawEllipseFill(new Vector2i(x + 6, y), new Vector2i(7, 7), DemoUtil.IndexToRGB(faceColor));

                FES.DrawEllipse(new Vector2i(x, y), new Vector2i(7, 7), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawEllipse(new Vector2i(x + 6, y), new Vector2i(7, 7), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawRect(new Rect2i(x - 1, y - 7, 10, 15), DemoUtil.IndexToRGB(colorOutline));

                FES.DrawRectFill(new Rect2i(x - 1, y - 6, 10, 13), DemoUtil.IndexToRGB(faceColor));

                FES.Print(new Vector2i(x - 4, y - 3), DemoUtil.IndexToRGB(colorOutline), "SYS");
            }
        }

        private void DrawGamepadDPad(int x, int y, int pressedMask, int color, int colorOutline)
        {
            var demo = (DemoReel)FES.Game;

            int pressedColor = 5;
            int size = 10;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawRect(new Rect2i(x + size, y, size, size * 3), colorOutline);
                FES.DrawRect(new Rect2i(x, y + size, size * 3, size), colorOutline);

                FES.DrawRectFill(new Rect2i(x + size + 1, y + 1, size - 2, (size * 3) - 2), color);
                FES.DrawRectFill(new Rect2i(x + 1, y + size + 1, (size * 3) - 2, size - 2), color);

                // Up
                FES.DrawRectFill(new Rect2i(x + size + 1, y + 1, size - 2, size), (pressedMask & FES.BTN_UP) != 0 ? pressedColor : color);
                FES.Print(new Vector2i(x + size + (size / 2) - 3, y + 2), colorOutline, ((char)127).ToString());

                // Down
                FES.DrawRectFill(new Rect2i(x + size + 1, y + (size * 2) - 1, size - 2, size), (pressedMask & FES.BTN_DOWN) != 0 ? pressedColor : color);
                FES.Print(new Vector2i(x + size + (size / 2) - 3, y + 2 + (size * 2)), colorOutline, ((char)128).ToString());

                // Left
                FES.DrawRectFill(new Rect2i(x + 1, y + size + 1, size, size - 2), (pressedMask & FES.BTN_LEFT) != 0 ? pressedColor : color);
                FES.Print(new Vector2i(x + 2, y + size + 2), colorOutline, ((char)129).ToString());

                // Right
                FES.DrawRectFill(new Rect2i(x + (size * 2) - 1, y + size + 1, size, size - 2), (pressedMask & FES.BTN_RIGHT) != 0 ? pressedColor : color);
                FES.Print(new Vector2i(x + (size * 2) + 3, y + size + 2), colorOutline, ((char)130).ToString());
            }
            else
            {
                FES.DrawRect(new Rect2i(x + size, y, size, size * 3), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawRect(new Rect2i(x, y + size, size * 3, size), DemoUtil.IndexToRGB(colorOutline));

                FES.DrawRectFill(new Rect2i(x + size + 1, y + 1, size - 2, (size * 3) - 2), DemoUtil.IndexToRGB(color));
                FES.DrawRectFill(new Rect2i(x + 1, y + size + 1, (size * 3) - 2, size - 2), DemoUtil.IndexToRGB(color));

                // Up
                FES.DrawRectFill(new Rect2i(x + size + 1, y + 1, size - 2, size), (pressedMask & FES.BTN_UP) != 0 ? DemoUtil.IndexToRGB(pressedColor) : DemoUtil.IndexToRGB(color));
                FES.Print(new Vector2i(x + size + (size / 2) - 3, y + 2), DemoUtil.IndexToRGB(colorOutline), ((char)127).ToString());

                // Down
                FES.DrawRectFill(new Rect2i(x + size + 1, y + (size * 2) - 1, size - 2, size), (pressedMask & FES.BTN_DOWN) != 0 ? DemoUtil.IndexToRGB(pressedColor) : DemoUtil.IndexToRGB(color));
                FES.Print(new Vector2i(x + size + (size / 2) - 3, y + 2 + (size * 2)), DemoUtil.IndexToRGB(colorOutline), ((char)128).ToString());

                // Left
                FES.DrawRectFill(new Rect2i(x + 1, y + size + 1, size, size - 2), (pressedMask & FES.BTN_LEFT) != 0 ? DemoUtil.IndexToRGB(pressedColor) : DemoUtil.IndexToRGB(color));
                FES.Print(new Vector2i(x + 2, y + size + 2), DemoUtil.IndexToRGB(colorOutline), ((char)129).ToString());

                // Right
                FES.DrawRectFill(new Rect2i(x + (size * 2) - 1, y + size + 1, size, size - 2), (pressedMask & FES.BTN_RIGHT) != 0 ? DemoUtil.IndexToRGB(pressedColor) : DemoUtil.IndexToRGB(color));
                FES.Print(new Vector2i(x + (size * 2) + 3, y + size + 2), DemoUtil.IndexToRGB(colorOutline), ((char)130).ToString());
            }
        }

        private void DrawGamepadShoulder(int x, int y, string label, bool pressed, int color, int colorOutline)
        {
            var demo = (DemoReel)FES.Game;

            int faceColor = pressed ? 5 : color;
            int offset = pressed ? 3 : 0;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawRectFill(new Rect2i(x, y + offset + 1, 16, 12), faceColor);
                FES.DrawRect(new Rect2i(x + 1, y + offset, 14, 1), colorOutline);
                FES.DrawRect(new Rect2i(x, y + offset + 1, 1, 12), colorOutline);
                FES.DrawRect(new Rect2i(x + 15, y + offset + 1, 1, 12), colorOutline);

                FES.Print(new Vector2i(x + 4, y + 2 + offset), colorOutline, label);
            }
            else
            {
                FES.DrawRectFill(new Rect2i(x, y + offset + 1, 16, 12), DemoUtil.IndexToRGB(faceColor));
                FES.DrawRect(new Rect2i(x + 1, y + offset, 14, 1), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawRect(new Rect2i(x, y + offset + 1, 1, 12), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawRect(new Rect2i(x + 15, y + offset + 1, 1, 12), DemoUtil.IndexToRGB(colorOutline));

                FES.Print(new Vector2i(x + 4, y + 2 + offset), DemoUtil.IndexToRGB(colorOutline), label);
            }
        }

        private void DrawGamepad(int x, int y, int playerNum)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y));

            int color1 = playerNum == FES.PLAYER_ONE ? 22 : 15;
            int color2 = playerNum == FES.PLAYER_ONE ? 21 : 14;
            int colorOutline = 0;

            DrawGamepadShoulder(25, -9, "LS", FES.ButtonDown(FES.BTN_LS, playerNum), color2, colorOutline);
            DrawGamepadShoulder(88, -9, "RS", FES.ButtonDown(FES.BTN_RS, playerNum), color2, colorOutline);

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawEllipseFill(new Vector2i(26, 28), new Vector2i(26, 28), color1);
                FES.DrawEllipseFill(new Vector2i(102, 28), new Vector2i(26, 28), color1);

                FES.DrawEllipse(new Vector2i(26, 28), new Vector2i(26, 28), colorOutline);
                FES.DrawEllipse(new Vector2i(102, 28), new Vector2i(26, 28), colorOutline);
                FES.DrawRect(new Rect2i(26, 0, 76, 57), colorOutline);

                FES.DrawRectFill(new Rect2i(26, 1, 76, 55), color1);
            }
            else
            {
                FES.DrawEllipseFill(new Vector2i(26, 28), new Vector2i(26, 28), DemoUtil.IndexToRGB(color1));
                FES.DrawEllipseFill(new Vector2i(102, 28), new Vector2i(26, 28), DemoUtil.IndexToRGB(color1));

                FES.DrawEllipse(new Vector2i(26, 28), new Vector2i(26, 28), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawEllipse(new Vector2i(102, 28), new Vector2i(26, 28), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawRect(new Rect2i(26, 0, 76, 57), DemoUtil.IndexToRGB(colorOutline));

                FES.DrawRectFill(new Rect2i(26, 1, 76, 55), DemoUtil.IndexToRGB(color1));
            }

            DrawGamepadABXY(100, 16, "Y", FES.ButtonDown(FES.BTN_Y, playerNum), color2, colorOutline);
            DrawGamepadABXY(100, 41, "A", FES.ButtonDown(FES.BTN_A, playerNum), color2, colorOutline);
            DrawGamepadABXY(87, 29, "X", FES.ButtonDown(FES.BTN_X, playerNum), color2, colorOutline);
            DrawGamepadABXY(113, 29, "B", FES.ButtonDown(FES.BTN_B, playerNum), color2, colorOutline);

            DrawGamepadMenu(55, 20, FES.ButtonDown(FES.BTN_MENU, playerNum), color2, colorOutline);
            DrawGamepadSystem(57, 37, FES.ButtonDown(FES.BTN_SYSTEM, playerNum), color2, colorOutline);

            DrawGamepadDPad(
                10,
                14,
                (FES.ButtonDown(FES.BTN_UP, playerNum) ? FES.BTN_UP : 0) | (FES.ButtonDown(FES.BTN_DOWN, playerNum) ? FES.BTN_DOWN : 0) | (FES.ButtonDown(FES.BTN_LEFT, playerNum) ? FES.BTN_LEFT : 0) | (FES.ButtonDown(FES.BTN_RIGHT, playerNum) ? FES.BTN_RIGHT : 0),
                color2,
                colorOutline);

            FES.CameraReset();
        }

        private void DrawMouse(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y));

            int color1 = 4;
            int color2 = 3;
            int colorOutline = 0;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawEllipseFill(new Vector2i(26, 15), new Vector2i(26, 15), color1);
                FES.DrawEllipseFill(new Vector2i(26, 54), new Vector2i(26, 20), color1);
                FES.DrawEllipse(new Vector2i(26, 15), new Vector2i(26, 15), colorOutline);
                FES.DrawEllipse(new Vector2i(26, 54), new Vector2i(26, 20), colorOutline);
                FES.DrawRect(new Rect2i(0, 15, 53, 54 - 15), colorOutline);
                FES.DrawRectFill(new Rect2i(1, 15, 53 - 2, 54 - 15), color1);

                FES.DrawLine(new Vector2i(52 / 3, 2), new Vector2i(52 / 3, 30), colorOutline);
                FES.DrawLine(new Vector2i(52 / 3 * 2, 2), new Vector2i(52 / 3 * 2, 30), colorOutline);
                FES.DrawLine(new Vector2i(0, 30), new Vector2i(52, 30), colorOutline);

                if (FES.ButtonDown(FES.BTN_POINTER_A))
                {
                    FES.ClipSet(new Rect2i(-FES.CameraGet().x + 1, -FES.CameraGet().y, (52 / 3) - 1, 30));
                    FES.DrawEllipseFill(new Vector2i(26, 16), new Vector2i(26, 15), 5);
                    FES.DrawEllipse(new Vector2i(26, 15), new Vector2i(26, 15), colorOutline);
                    FES.DrawRectFill(new Rect2i(1, 15, 53 - 2, 54 - 15), 5);
                    FES.ClipReset();
                }

                if (FES.ButtonDown(FES.BTN_POINTER_B))
                {
                    FES.ClipSet(new Rect2i(-FES.CameraGet().x + 35, -FES.CameraGet().y, 52 / 3, 30));
                    FES.DrawEllipseFill(new Vector2i(26, 16), new Vector2i(26, 15), 5);
                    FES.DrawEllipse(new Vector2i(26, 15), new Vector2i(26, 15), colorOutline);
                    FES.DrawRectFill(new Rect2i(1, 15, 53 - 2, 54 - 15), 5);
                    FES.ClipReset();
                }

                if (FES.ButtonDown(FES.BTN_POINTER_C))
                {
                    FES.ClipSet(new Rect2i(-FES.CameraGet().x + 18, -FES.CameraGet().y, (52 / 3) - 1, 30));
                    FES.DrawEllipseFill(new Vector2i(26, 16), new Vector2i(26, 15), 5);
                    FES.DrawEllipse(new Vector2i(26, 15), new Vector2i(26, 15), colorOutline);
                    FES.DrawRectFill(new Rect2i(1, 15, 53 - 2, 54 - 15), 5);
                    FES.ClipReset();
                }

                if (FES.ButtonDown(FES.BTN_POINTER_A))
                {
                    FES.Print(new Vector2i(9, 16), colorOutline, "A");
                }
                else
                {
                    FES.Print(new Vector2i(8, 15), colorOutline, "A");
                }

                if (FES.ButtonDown(FES.BTN_POINTER_C))
                {
                    FES.Print(new Vector2i(25, 16), colorOutline, "C");
                }
                else
                {
                    FES.Print(new Vector2i(24, 15), colorOutline, "C");
                }

                if (FES.ButtonDown(FES.BTN_POINTER_B))
                {
                    FES.Print(new Vector2i(41, 16), colorOutline, "B");
                }
                else
                {
                    FES.Print(new Vector2i(40, 15), colorOutline, "B");
                }

                FES.DrawRectFill(new Rect2i(23, 36, 8, 24), color2);
                int barSize = (int)(6 * mScrollDeltaAnim / 2.0f);
                barSize = Mathf.Clamp(barSize, -12, 12);

                if (barSize > 0)
                {
                    FES.DrawRectFill(new Rect2i(23, 48 - barSize, 8, barSize), 5);
                }
                else if (barSize < 0)
                {
                    FES.DrawRectFill(new Rect2i(23, 48, 8, -barSize), 5);
                }

                FES.DrawRect(new Rect2i(23, 36, 8, 24), colorOutline);
                FES.DrawLine(new Vector2i(24, 48), new Vector2i(29, 48), colorOutline);
            }
            else
            {
                FES.DrawEllipseFill(new Vector2i(26, 15), new Vector2i(26, 15), DemoUtil.IndexToRGB(color1));
                FES.DrawEllipseFill(new Vector2i(26, 54), new Vector2i(26, 20), DemoUtil.IndexToRGB(color1));
                FES.DrawEllipse(new Vector2i(26, 15), new Vector2i(26, 15), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawEllipse(new Vector2i(26, 54), new Vector2i(26, 20), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawRect(new Rect2i(0, 15, 53, 54 - 15), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawRectFill(new Rect2i(1, 15, 53 - 2, 54 - 15), DemoUtil.IndexToRGB(color1));

                FES.DrawLine(new Vector2i(52 / 3, 2), new Vector2i(52 / 3, 30), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawLine(new Vector2i(52 / 3 * 2, 2), new Vector2i(52 / 3 * 2, 30), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawLine(new Vector2i(0, 30), new Vector2i(52, 30), DemoUtil.IndexToRGB(colorOutline));

                if (FES.ButtonDown(FES.BTN_POINTER_A))
                {
                    FES.ClipSet(new Rect2i(-FES.CameraGet().x + 1, -FES.CameraGet().y, (52 / 3) - 1, 30));
                    FES.DrawEllipseFill(new Vector2i(26, 16), new Vector2i(26, 15), DemoUtil.IndexToRGB(5));
                    FES.DrawEllipse(new Vector2i(26, 15), new Vector2i(26, 15), DemoUtil.IndexToRGB(colorOutline));
                    FES.DrawRectFill(new Rect2i(1, 15, 53 - 2, 54 - 15), DemoUtil.IndexToRGB(5));
                    FES.ClipReset();
                }

                if (FES.ButtonDown(FES.BTN_POINTER_B))
                {
                    FES.ClipSet(new Rect2i(-FES.CameraGet().x + 35, -FES.CameraGet().y, 52 / 3, 30));
                    FES.DrawEllipseFill(new Vector2i(26, 16), new Vector2i(26, 15), DemoUtil.IndexToRGB(5));
                    FES.DrawEllipse(new Vector2i(26, 15), new Vector2i(26, 15), DemoUtil.IndexToRGB(colorOutline));
                    FES.DrawRectFill(new Rect2i(1, 15, 53 - 2, 54 - 15), DemoUtil.IndexToRGB(5));
                    FES.ClipReset();
                }

                if (FES.ButtonDown(FES.BTN_POINTER_C))
                {
                    FES.ClipSet(new Rect2i(-FES.CameraGet().x + 18, -FES.CameraGet().y, (52 / 3) - 1, 30));
                    FES.DrawEllipseFill(new Vector2i(26, 16), new Vector2i(26, 15), DemoUtil.IndexToRGB(5));
                    FES.DrawEllipse(new Vector2i(26, 15), new Vector2i(26, 15), DemoUtil.IndexToRGB(colorOutline));
                    FES.DrawRectFill(new Rect2i(1, 15, 53 - 2, 54 - 15), DemoUtil.IndexToRGB(5));
                    FES.ClipReset();
                }

                if (FES.ButtonDown(FES.BTN_POINTER_A))
                {
                    FES.Print(new Vector2i(9, 16), DemoUtil.IndexToRGB(colorOutline), "A");
                }
                else
                {
                    FES.Print(new Vector2i(8, 15), DemoUtil.IndexToRGB(colorOutline), "A");
                }

                if (FES.ButtonDown(FES.BTN_POINTER_C))
                {
                    FES.Print(new Vector2i(25, 16), DemoUtil.IndexToRGB(colorOutline), "C");
                }
                else
                {
                    FES.Print(new Vector2i(24, 15), DemoUtil.IndexToRGB(colorOutline), "C");
                }

                if (FES.ButtonDown(FES.BTN_POINTER_B))
                {
                    FES.Print(new Vector2i(41, 16), DemoUtil.IndexToRGB(colorOutline), "B");
                }
                else
                {
                    FES.Print(new Vector2i(40, 15), DemoUtil.IndexToRGB(colorOutline), "B");
                }

                FES.DrawRectFill(new Rect2i(23, 36, 8, 24), DemoUtil.IndexToRGB(color2));
                int barSize = (int)(6 * mScrollDeltaAnim / 2.0f);
                barSize = Mathf.Clamp(barSize, -12, 12);

                if (barSize > 0)
                {
                    FES.DrawRectFill(new Rect2i(23, 48 - barSize, 8, barSize), DemoUtil.IndexToRGB(5));
                }
                else if (barSize < 0)
                {
                    FES.DrawRectFill(new Rect2i(23, 48, 8, -barSize), DemoUtil.IndexToRGB(5));
                }

                FES.DrawRect(new Rect2i(23, 36, 8, 24), DemoUtil.IndexToRGB(colorOutline));
                FES.DrawLine(new Vector2i(24, 48), new Vector2i(29, 48), DemoUtil.IndexToRGB(colorOutline));
            }

            FES.CameraReset();
        }

        private Rect2i RectBetweenKeys(string key1, string key2)
        {
            Rect2i keyboardRect = mKeys[key1].Rect;
            keyboardRect.width = (mKeys[key2].Rect.x + mKeys[key2].Rect.width) - mKeys[key1].Rect.x;
            keyboardRect.height = (mKeys[key2].Rect.y + mKeys[key2].Rect.height) - mKeys[key1].Rect.y;

            return keyboardRect;
        }

        private void DrawKeyboard(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y));

            Rect2i keyboardRect = RectBetweenKeys("Esc", "KeypadEnter");
            keyboardRect.Expand(8);

            int cornerSize = 8;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawEllipseFill(new Vector2i(keyboardRect.x + cornerSize, keyboardRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), 3);
                FES.DrawEllipse(new Vector2i(keyboardRect.x + cornerSize, keyboardRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), 2);

                FES.DrawEllipseFill(new Vector2i(keyboardRect.x + keyboardRect.width - cornerSize - 1, keyboardRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), 3);
                FES.DrawEllipse(new Vector2i(keyboardRect.x + keyboardRect.width - cornerSize - 1, keyboardRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), 2);

                FES.DrawEllipseFill(new Vector2i(keyboardRect.x + cornerSize, keyboardRect.y + keyboardRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), 3);
                FES.DrawEllipse(new Vector2i(keyboardRect.x + cornerSize, keyboardRect.y + keyboardRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), 2);

                FES.DrawEllipseFill(new Vector2i(keyboardRect.x + keyboardRect.width - cornerSize - 1, keyboardRect.y + keyboardRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), 3);
                FES.DrawEllipse(new Vector2i(keyboardRect.x + keyboardRect.width - cornerSize - 1, keyboardRect.y + keyboardRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), 2);

                FES.DrawRect(new Rect2i(keyboardRect.x + cornerSize, keyboardRect.y, keyboardRect.width - (cornerSize * 2), keyboardRect.height), 2);
                FES.DrawRectFill(new Rect2i(keyboardRect.x + cornerSize, keyboardRect.y + 1, keyboardRect.width - (cornerSize * 2), keyboardRect.height - 2), 3);

                FES.DrawRect(new Rect2i(keyboardRect.x, keyboardRect.y + cornerSize, cornerSize, keyboardRect.height - (cornerSize * 2)), 2);
                FES.DrawRectFill(new Rect2i(keyboardRect.x + 1, keyboardRect.y + cornerSize, cornerSize - 1, keyboardRect.height - (cornerSize * 2)), 3);

                FES.DrawRect(new Rect2i(keyboardRect.x + keyboardRect.width - cornerSize, keyboardRect.y + cornerSize, cornerSize, keyboardRect.height - (cornerSize * 2)), 2);
                FES.DrawRectFill(new Rect2i(keyboardRect.x + keyboardRect.width - cornerSize - 1, keyboardRect.y + cornerSize, cornerSize, keyboardRect.height - (cornerSize * 2)), 3);

                int holeColor = 2;

                Rect2i holeRect = RectBetweenKeys("Esc", "Esc");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, holeColor);

                holeRect = RectBetweenKeys("F1", "F4");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, holeColor);

                holeRect = RectBetweenKeys("F5", "F8");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, holeColor);

                holeRect = RectBetweenKeys("F9", "F12");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, holeColor);

                holeRect = RectBetweenKeys("PrintScreen", "Pause");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, holeColor);

                holeRect = RectBetweenKeys("~", "RightCtrl");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, holeColor);

                holeRect = RectBetweenKeys("Insert", "PageDown");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, holeColor);

                holeRect = RectBetweenKeys("Up", "Up");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, holeColor);

                holeRect = RectBetweenKeys("Left", "Right");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, holeColor);

                holeRect = RectBetweenKeys("NumLock", "KeypadEnter");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, holeColor);
            }
            else
            {
                FES.DrawEllipseFill(new Vector2i(keyboardRect.x + cornerSize, keyboardRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
                FES.DrawEllipse(new Vector2i(keyboardRect.x + cornerSize, keyboardRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

                FES.DrawEllipseFill(new Vector2i(keyboardRect.x + keyboardRect.width - cornerSize - 1, keyboardRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
                FES.DrawEllipse(new Vector2i(keyboardRect.x + keyboardRect.width - cornerSize - 1, keyboardRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

                FES.DrawEllipseFill(new Vector2i(keyboardRect.x + cornerSize, keyboardRect.y + keyboardRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
                FES.DrawEllipse(new Vector2i(keyboardRect.x + cornerSize, keyboardRect.y + keyboardRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

                FES.DrawEllipseFill(new Vector2i(keyboardRect.x + keyboardRect.width - cornerSize - 1, keyboardRect.y + keyboardRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
                FES.DrawEllipse(new Vector2i(keyboardRect.x + keyboardRect.width - cornerSize - 1, keyboardRect.y + keyboardRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

                FES.DrawRect(new Rect2i(keyboardRect.x + cornerSize, keyboardRect.y, keyboardRect.width - (cornerSize * 2), keyboardRect.height), DemoUtil.IndexToRGB(2));
                FES.DrawRectFill(new Rect2i(keyboardRect.x + cornerSize, keyboardRect.y + 1, keyboardRect.width - (cornerSize * 2), keyboardRect.height - 2), DemoUtil.IndexToRGB(3));

                FES.DrawRect(new Rect2i(keyboardRect.x, keyboardRect.y + cornerSize, cornerSize, keyboardRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(2));
                FES.DrawRectFill(new Rect2i(keyboardRect.x + 1, keyboardRect.y + cornerSize, cornerSize - 1, keyboardRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(3));

                FES.DrawRect(new Rect2i(keyboardRect.x + keyboardRect.width - cornerSize, keyboardRect.y + cornerSize, cornerSize, keyboardRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(2));
                FES.DrawRectFill(new Rect2i(keyboardRect.x + keyboardRect.width - cornerSize - 1, keyboardRect.y + cornerSize, cornerSize, keyboardRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(3));

                int holeColor = 2;

                Rect2i holeRect = RectBetweenKeys("Esc", "Esc");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, DemoUtil.IndexToRGB(holeColor));

                holeRect = RectBetweenKeys("F1", "F4");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, DemoUtil.IndexToRGB(holeColor));

                holeRect = RectBetweenKeys("F5", "F8");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, DemoUtil.IndexToRGB(holeColor));

                holeRect = RectBetweenKeys("F9", "F12");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, DemoUtil.IndexToRGB(holeColor));

                holeRect = RectBetweenKeys("PrintScreen", "Pause");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, DemoUtil.IndexToRGB(holeColor));

                holeRect = RectBetweenKeys("~", "RightCtrl");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, DemoUtil.IndexToRGB(holeColor));

                holeRect = RectBetweenKeys("Insert", "PageDown");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, DemoUtil.IndexToRGB(holeColor));

                holeRect = RectBetweenKeys("Up", "Up");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, DemoUtil.IndexToRGB(holeColor));

                holeRect = RectBetweenKeys("Left", "Right");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, DemoUtil.IndexToRGB(holeColor));

                holeRect = RectBetweenKeys("NumLock", "KeypadEnter");
                holeRect.Expand(1);
                FES.DrawRectFill(holeRect, DemoUtil.IndexToRGB(holeColor));
            }

            foreach (KeyValuePair<string, KeyboardKey> key in mKeys)
            {
                KeyboardKey k = key.Value;

                bool pressed = FES.KeyDown(k.Code1) || FES.KeyDown(k.Code2);

                if (pressed)
                {
                    if (demo.ColorMode == FES.ColorMode.Indexed)
                    {
                        FES.DrawRectFill(new Rect2i(k.Rect.x + 2, k.Rect.y + 2, k.Rect.width - 2, k.Rect.height - 2), 5);
                        FES.Print(new Vector2i(k.Rect.x + 3, k.Rect.y + 3), 0, k.Label);
                    }
                    else
                    {
                        FES.DrawRectFill(new Rect2i(k.Rect.x + 2, k.Rect.y + 2, k.Rect.width - 2, k.Rect.height - 2), DemoUtil.IndexToRGB(5));
                        FES.Print(new Vector2i(k.Rect.x + 3, k.Rect.y + 3), DemoUtil.IndexToRGB(0), k.Label);
                    }
                }
                else
                {
                    if (demo.ColorMode == FES.ColorMode.Indexed)
                    {
                        FES.DrawRectFill(k.Rect, 4);
                        FES.Print(new Vector2i(k.Rect.x + 2, k.Rect.y + 2), 0, k.Label);
                        FES.DrawRect(k.Rect, 3);
                    }
                    else
                    {
                        FES.DrawRectFill(k.Rect, DemoUtil.IndexToRGB(4));
                        FES.Print(new Vector2i(k.Rect.x + 2, k.Rect.y + 2), DemoUtil.IndexToRGB(0), k.Label);
                        FES.DrawRect(k.Rect, DemoUtil.IndexToRGB(3));
                    }
                }
            }

            FES.CameraReset();
        }

        private void DrawInputString(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y));

            int width = 612;
            int height = 32;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawRect(new Rect2i(0, 0, width, height), 25);
                FES.DrawRectFill(new Rect2i(1, 1, width - 2, height - 2), 27);
            }
            else
            {
                FES.DrawRect(new Rect2i(0, 0, width, height), DemoUtil.IndexToRGB(25));
                FES.DrawRectFill(new Rect2i(1, 1, width - 2, height - 2), DemoUtil.IndexToRGB(27));
            }

            FES.ClipSet(new Rect2i(x + 4, y + 4, width - 8, height - 2));

            if (FES.Ticks % 30 < 15)
            {
                if (demo.ColorMode == FES.ColorMode.Indexed)
                {
                    FES.Print(new Vector2i(4, 4), 23, mInputStr);
                }
                else
                {
                    FES.Print(new Vector2i(4, 4), DemoUtil.IndexToRGB(23), mInputStr);
                }
            }
            else
            {
                if (demo.ColorMode == FES.ColorMode.Indexed)
                {
                    FES.Print(new Vector2i(4, 4), 23, mInputStr + "|");
                }
                else
                {
                    FES.Print(new Vector2i(4, 4), DemoUtil.IndexToRGB(23), mInputStr + "|");
                }
            }

            FES.CameraReset();

            FES.ClipReset();
        }

        private class KeyboardKey
        {
            public Rect2i Rect;
            public KeyCode Code1;
            public KeyCode Code2;
            public string Label;

            public KeyboardKey(Rect2i rect, string label, KeyCode code1, KeyCode code2 = KeyCode.F15)
            {
                Rect = rect;
                Code1 = code1;

                // Using F15 as an undefined default key, if set then make code2 same as code1
                Code2 = code2 == KeyCode.F15 ? code1 : code2;
                Label = label;
            }
        }
    }
}
