namespace FESDemo
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Demonstrate sound and music
    /// </summary>
    public class SceneSound : SceneDemo
    {
        private const float PIANO_C_PITCH = 261.626f;

        private bool mMusicPlaying = false;
        private int mMusicTicks = 0;
        private int mMusicTurnSpeed = 0;
        private List<FES.SoundReference> mFadeSounds = new List<FES.SoundReference>();

        private Rect2i mPianoRect;

        private Button[] mEffectButtons;
        private Button[] mPianoButtons;

        private Button mMusicPlayButton;

        private Button mNextButton;

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneSound()
        {
            FES.MusicSetup(0, "Demos/DemoReel/Music");

            FES.SoundSetup(0, "Demos/DemoReel/Coin");
            FES.SoundSetup(1, "Demos/DemoReel/Explosion");
            FES.SoundSetup(2, "Demos/DemoReel/Jump");
            FES.SoundSetup(3, "Demos/DemoReel/Laser");
            FES.SoundSetup(4, "Demos/DemoReel/C5Note");

            InitPiano();
            InitNoises();
            InitMusic();

            mNextButton = new Button(new Rect2i(550, 334, 87, 23), new Rect2i(550, 334, 87, 23), 3, 2, "Touch here to go\nto the next screen", (KeyCode)555, 0, NextScreenButtonCB);
        }

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            mMusicTicks = 0;
            mMusicPlaying = false;
            mMusicTurnSpeed = 0;
            mNextButton.Reset();
            mMusicPlayButton.Reset();
            foreach (var button in mPianoButtons)
            {
                button.Reset();
            }

            foreach (var button in mEffectButtons)
            {
                button.Reset();
            }

            UpdateMusicButtonLabel();

            FES.OffscreenSetup(0, FES.DisplaySize);

            base.Enter();
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            FES.MusicStop();
            base.Exit();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            for (int i = mFadeSounds.Count - 1; i >= 0; i--)
            {
                FES.SoundReference soundRef = mFadeSounds[i];
                FES.SoundVolumeSet(soundRef, FES.SoundVolumeGet(soundRef) * 0.75f);

                if (FES.SoundVolumeGet(soundRef) < 0.01f)
                {
                    mFadeSounds.RemoveAt(i);
                }
            }

            if (!mMusicPlaying)
            {
                mMusicTurnSpeed--;
                if (mMusicTurnSpeed < 0)
                {
                    mMusicTurnSpeed = 0;
                }
            }

            mMusicTicks += mMusicTurnSpeed;

            foreach (var button in mEffectButtons)
            {
                button.Update();
            }

            foreach (var button in mPianoButtons)
            {
                button.Update();
            }

            mMusicPlayButton.Update();

            mNextButton.Update();

            int color = 1;
            if ((FES.Ticks % 200 > 170 && FES.Ticks % 200 < 180) || (FES.Ticks % 200) > 190)
            {
                color = 5;
            }

            mNextButton.LabelColor = color;

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

            DrawNoisePad(4, 4);
            DrawPiano(4, 200);
            DrawMusicPlayer(350, 4);

            mNextButton.Render();

            if (FES.PointerPosValid())
            {
                FES.DrawSprite(4, FES.PointerPos());
            }
        }

        private void EffectButtonPressedCB(Button button, object userData)
        {
            FES.SoundPlay((int)userData);
        }

        private void PianoButtonPressedCB(Button button, object userData)
        {
            var info = (PianoKeyInfo)userData;
            info.SoundRef = PlayNote(info.Pitch);
        }

        private void PianoButtonReleasedCB(Button button, object userData)
        {
            var info = (PianoKeyInfo)userData;
            mFadeSounds.Add(info.SoundRef);
        }

        private void MusicButtonPressedCB(Button button, object userData)
        {
            if (button == mMusicPlayButton)
            {
                if (!mMusicPlaying)
                {
                    FES.MusicPlay(0);
                    mMusicPlaying = true;
                    mMusicTurnSpeed = 50;
                }
                else
                {
                    FES.MusicStop();
                    mMusicPlaying = false;
                }
            }

            UpdateMusicButtonLabel();
        }

        private void NextScreenButtonCB(Button button, object userData)
        {
            DemoReel demo = (DemoReel)FES.Game;
            demo.NextScene();
        }

        private FES.SoundReference PlayNote(float pitch)
        {
            float basePitch = PIANO_C_PITCH;
            float finalPitch = 1.0f + ((pitch - basePitch) / basePitch);

            return FES.SoundPlay(4, 1.0f, finalPitch);
        }

        private void InitPiano()
        {
            int xStart = 8;
            int yStart = 8;
            int w = 30;
            int h = 102;
            int bw = 22;
            int bh = 60;

            int space = 2;

            int x = xStart;
            int y = yStart;

            mPianoButtons = new Button[12];
            mPianoButtons[0] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "R", KeyCode.R, new PianoKeyInfo(261.626f), PianoButtonPressedCB, PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[1] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "T", KeyCode.T, new PianoKeyInfo(293.665f), PianoButtonPressedCB, PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[2] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "Y", KeyCode.Y, new PianoKeyInfo(329.628f), PianoButtonPressedCB, PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[3] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "U", KeyCode.U, new PianoKeyInfo(349.228f), PianoButtonPressedCB, PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[4] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "I", KeyCode.I, new PianoKeyInfo(391.995f), PianoButtonPressedCB, PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[5] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "O", KeyCode.O, new PianoKeyInfo(440.000f), PianoButtonPressedCB, PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[6] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "P", KeyCode.P, new PianoKeyInfo(493.883f), PianoButtonPressedCB, PianoButtonReleasedCB, true);

            x = xStart;
            y = yStart;

            x += (w / 2) + (space * 2);
            mPianoButtons[7] = new Button(new Rect2i(x, y, bw, bh), new Rect2i(x, y, bw, bh), 1, 4, "5", KeyCode.Alpha5, new PianoKeyInfo(277.182f), PianoButtonPressedCB, PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[8] = new Button(new Rect2i(x, y, bw, bh), new Rect2i(x, y, bw, bh), 1, 4, "6", KeyCode.Alpha6, new PianoKeyInfo(311.127f), PianoButtonPressedCB, PianoButtonReleasedCB, true);
            x += w + space;
            x += w + space;
            mPianoButtons[9] = new Button(new Rect2i(x, y, bw, bh), new Rect2i(x, y, bw, bh), 1, 4, "7", KeyCode.Alpha7, new PianoKeyInfo(369.994f), PianoButtonPressedCB, PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[10] = new Button(new Rect2i(x, y, bw, bh), new Rect2i(x, y, bw, bh), 1, 4, "8", KeyCode.Alpha8, new PianoKeyInfo(415.305f), PianoButtonPressedCB, PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[11] = new Button(new Rect2i(x, y, bw, bh), new Rect2i(x, y, bw, bh), 1, 4, "9", KeyCode.Alpha9, new PianoKeyInfo(466.164f), PianoButtonPressedCB, PianoButtonReleasedCB, true);
            x += w + space;

            mPianoRect = new Rect2i(xStart, yStart, (7 * (w + space)) - space, h);
        }

        private void InitNoises()
        {
            int w = 70;
            int h = 28;
            int x = 0;
            int y = 0;

            mEffectButtons = new Button[4];
            mEffectButtons[0] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y, w, h), 4, 0, "1 - Coin", KeyCode.Alpha1, 0, EffectButtonPressedCB);
            x += w + 2;
            mEffectButtons[1] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y, w, h), 4, 0, "2 - Explosion", KeyCode.Alpha2, 1, EffectButtonPressedCB);
            y += h + 2;
            x = 0;
            mEffectButtons[2] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y, w, h), 4, 0, "3 - Jump", KeyCode.Alpha3, 2, EffectButtonPressedCB);
            x += w + 2;
            mEffectButtons[3] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y, w, h), 4, 0, "4 - Laser", KeyCode.Alpha4, 3, EffectButtonPressedCB);
        }

        private void InitMusic()
        {
            mMusicPlayButton = new Button(new Rect2i(100, 115, 80, 36), new Rect2i(100, 115, 80, 36), 4, 0, "Play", KeyCode.H, 0, MusicButtonPressedCB);
        }

        private void DrawPiano(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y));

            string str = DemoUtil.HighlightCode(
                "@C// Play sound at specific volume and pitch\n" +
                "@MFES@N.SoundSetup(@L4@N, @S\"Demos/Demo/C5Note\"@N);\n" +
                "@MFES@N.SoundPlay(@L4@N, @L0.5f@N, @L1.2f@N);\n");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(0, 0), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(0, 0), DemoUtil.IndexToRGB(5), str);
            }

            FES.CameraSet(new Vector2i(-x, -y - 35));

            Rect2i pianoRect = mPianoRect;
            Rect2i holeRect = pianoRect;
            pianoRect.Expand(8);
            holeRect.Expand(2);

            int cornerSize = 8;
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawEllipseFill(new Vector2i(pianoRect.x + cornerSize, pianoRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), 3);
                FES.DrawEllipse(new Vector2i(pianoRect.x + cornerSize, pianoRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), 2);

                FES.DrawEllipseFill(new Vector2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), 3);
                FES.DrawEllipse(new Vector2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), 2);

                FES.DrawEllipseFill(new Vector2i(pianoRect.x + cornerSize, pianoRect.y + pianoRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), 3);
                FES.DrawEllipse(new Vector2i(pianoRect.x + cornerSize, pianoRect.y + pianoRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), 2);

                FES.DrawEllipseFill(new Vector2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + pianoRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), 3);
                FES.DrawEllipse(new Vector2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + pianoRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), 2);

                FES.DrawRect(new Rect2i(pianoRect.x + cornerSize, pianoRect.y, pianoRect.width - (cornerSize * 2), pianoRect.height), 2);
                FES.DrawRectFill(new Rect2i(pianoRect.x + cornerSize, pianoRect.y + 1, pianoRect.width - (cornerSize * 2), pianoRect.height - 2), 3);

                FES.DrawRect(new Rect2i(pianoRect.x, pianoRect.y + cornerSize, cornerSize, pianoRect.height - (cornerSize * 2)), 2);
                FES.DrawRectFill(new Rect2i(pianoRect.x + 1, pianoRect.y + cornerSize, cornerSize - 1, pianoRect.height - (cornerSize * 2)), 3);

                FES.DrawRect(new Rect2i(pianoRect.x + pianoRect.width - cornerSize, pianoRect.y + cornerSize, cornerSize, pianoRect.height - (cornerSize * 2)), 2);
                FES.DrawRectFill(new Rect2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + cornerSize, cornerSize, pianoRect.height - (cornerSize * 2)), 3);

                FES.DrawRectFill(holeRect, 2);
            }
            else
            {
                FES.DrawEllipseFill(new Vector2i(pianoRect.x + cornerSize, pianoRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
                FES.DrawEllipse(new Vector2i(pianoRect.x + cornerSize, pianoRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

                FES.DrawEllipseFill(new Vector2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
                FES.DrawEllipse(new Vector2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

                FES.DrawEllipseFill(new Vector2i(pianoRect.x + cornerSize, pianoRect.y + pianoRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
                FES.DrawEllipse(new Vector2i(pianoRect.x + cornerSize, pianoRect.y + pianoRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

                FES.DrawEllipseFill(new Vector2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + pianoRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
                FES.DrawEllipse(new Vector2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + pianoRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

                FES.DrawRect(new Rect2i(pianoRect.x + cornerSize, pianoRect.y, pianoRect.width - (cornerSize * 2), pianoRect.height), DemoUtil.IndexToRGB(2));
                FES.DrawRectFill(new Rect2i(pianoRect.x + cornerSize, pianoRect.y + 1, pianoRect.width - (cornerSize * 2), pianoRect.height - 2), DemoUtil.IndexToRGB(3));

                FES.DrawRect(new Rect2i(pianoRect.x, pianoRect.y + cornerSize, cornerSize, pianoRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(2));
                FES.DrawRectFill(new Rect2i(pianoRect.x + 1, pianoRect.y + cornerSize, cornerSize - 1, pianoRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(3));

                FES.DrawRect(new Rect2i(pianoRect.x + pianoRect.width - cornerSize, pianoRect.y + cornerSize, cornerSize, pianoRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(2));
                FES.DrawRectFill(new Rect2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + cornerSize, cornerSize, pianoRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(3));

                FES.DrawRectFill(holeRect, DemoUtil.IndexToRGB(2));
            }

            foreach (var button in mPianoButtons)
            {
                button.Render();
            }

            FES.CameraReset();
        }

        private void DrawNoisePad(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            FES.CameraSet(new Vector2i(-x, -y));

            string str = DemoUtil.HighlightCode(
                "@C// Load sounds into sound slots and play them\n" +
                "@MFES@N.SoundSetup(@L0@N, @S\"Demos/Demo/Coin\"@N);\n" +
                "@MFES@N.SoundSetup(@L1@N, @S\"Demos/Demo/Explosion\"@N);\n" +
                "@MFES@N.SoundSetup(@L2@N, @S\"Demos/Demo/Jump\"@N);\n" +
                "@MFES@N.SoundSetup(@L3@N, @S\"Demos/Demo/Laser\"@N);\n" +
                "\n" +
                "@Kif@N (@MFES@N.KeyboardPressed(@MKeyCode@N.Alpha1) {\n" +
                "   @MFES@N.SoundPlay(@L0@N);\n" +
                "} @Kelse if@N (@MFES@N.KeyboardPressed(@MKeyCode@N.Alpha2) {\n" +
                "   @MFES@N.SoundPlay(@L1@N);\n" +
                "} @Kelse if@N (@MFES@N.KeyboardPressed(@MKeyCode@N.Alpha3) {\n" +
                "   @MFES@N.SoundPlay(@L2@N);\n" +
                "} @Kelse if@N (@MFES@N.KeyboardPressed(@MKeyCode@N.Alpha4) {\n" +
                "   @MFES@N.SoundPlay(@L3@N);\n" +
                "}");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(0, 0), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(0, 0), DemoUtil.IndexToRGB(5), str);
            }

            FES.CameraSet(new Vector2i(-x, -y - 123));
            foreach (var button in mEffectButtons)
            {
                button.Render();
            }

            FES.CameraReset();
        }

        private void DrawSpinner(int x, int y, int spinnerSize)
        {
            var demo = (DemoReel)FES.Game;

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawEllipseFill(new Vector2i(x + spinnerSize, y + spinnerSize), new Vector2i(spinnerSize, spinnerSize), 1);
                FES.DrawEllipseFill(new Vector2i(x + spinnerSize, y + spinnerSize), new Vector2i(spinnerSize - 6, spinnerSize - 6), 4);
                FES.DrawEllipseFill(new Vector2i(x + spinnerSize, y + spinnerSize), new Vector2i(8, 8), 1);
                FES.DrawEllipse(new Vector2i(x + spinnerSize, y + spinnerSize), new Vector2i(spinnerSize, spinnerSize), 4);
            }
            else
            {
                FES.DrawEllipseFill(new Vector2i(x + spinnerSize, y + spinnerSize), new Vector2i(spinnerSize, spinnerSize), DemoUtil.IndexToRGB(1));
                FES.DrawEllipseFill(new Vector2i(x + spinnerSize, y + spinnerSize), new Vector2i(spinnerSize - 6, spinnerSize - 6), DemoUtil.IndexToRGB(4));
                FES.DrawEllipseFill(new Vector2i(x + spinnerSize, y + spinnerSize), new Vector2i(8, 8), DemoUtil.IndexToRGB(1));
                FES.DrawEllipse(new Vector2i(x + spinnerSize, y + spinnerSize), new Vector2i(spinnerSize, spinnerSize), DemoUtil.IndexToRGB(4));
            }

            FES.DrawCopyOffscreen(0, new Rect2i(0, 0, (spinnerSize * 2) + 1, (spinnerSize * 2) + 1), new Rect2i(x, y, (spinnerSize * 2) + 1, (spinnerSize * 2) + 1), new Vector2i(spinnerSize, spinnerSize), mMusicTicks / 50);
        }

        private void DrawMusicPlayer(int x, int y)
        {
            var demo = (DemoReel)FES.Game;

            FES.Offscreen();
            FES.OffscreenClear();

            int spinnerSize = 60;
            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawRectFill(new Rect2i(spinnerSize / 4, spinnerSize - 2, spinnerSize / 2, 5), 1);
                FES.DrawRectFill(new Rect2i(spinnerSize + (spinnerSize / 4) + 1, spinnerSize - 2, spinnerSize / 2, 5), 1);
                FES.DrawRectFill(new Rect2i(spinnerSize - 2, spinnerSize / 4, 5, spinnerSize / 2), 1);
                FES.DrawRectFill(new Rect2i(spinnerSize - 2, spinnerSize + (spinnerSize / 4) + 1, 5, spinnerSize / 2), 1);
            }
            else
            {
                FES.DrawRectFill(new Rect2i(spinnerSize / 4, spinnerSize - 2, spinnerSize / 2, 5), DemoUtil.IndexToRGB(1));
                FES.DrawRectFill(new Rect2i(spinnerSize + (spinnerSize / 4) + 1, spinnerSize - 2, spinnerSize / 2, 5), DemoUtil.IndexToRGB(1));
                FES.DrawRectFill(new Rect2i(spinnerSize - 2, spinnerSize / 4, 5, spinnerSize / 2), DemoUtil.IndexToRGB(1));
                FES.DrawRectFill(new Rect2i(spinnerSize - 2, spinnerSize + (spinnerSize / 4) + 1, 5, spinnerSize / 2), DemoUtil.IndexToRGB(1));
            }

            FES.Onscreen();

            FES.CameraSet(new Vector2i(-x, -y));

            string str = DemoUtil.HighlightCode(
                "@C// Load music into music slot and play it\n" +
                "@MFES@N.MusicSetup(@L0@N, @S\"Demos/Demo/Music\"@N);\n" +
                "\n" +
                "@Kif@N (@MFES@N.KeyboardPressed(@MKeyCode@N.H) {\n" +
                "   @MFES@N.MusicPlay(@L0@N);\n" +
                "} @Kelse if@N (@MFES@N.KeyboardPressed(@MKeyCode@N.J) {\n" +
                "   @MFES@N.MusicStop();\n" +
                "}");

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.Print(new Vector2i(0, 0), 5, str);
            }
            else
            {
                FES.Print(new Vector2i(0, 0), DemoUtil.IndexToRGB(5), str);
            }

            FES.CameraSet(new Vector2i(-x, -y - 80));

            int cornerSize = 8;
            var deckRect = new Rect2i(40, 40, 200, 125);

            if (demo.ColorMode == FES.ColorMode.Indexed)
            {
                FES.DrawEllipseFill(new Vector2i(deckRect.x + cornerSize, deckRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), 3);
                FES.DrawEllipse(new Vector2i(deckRect.x + cornerSize, deckRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), 2);

                FES.DrawEllipseFill(new Vector2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), 3);
                FES.DrawEllipse(new Vector2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), 2);

                FES.DrawEllipseFill(new Vector2i(deckRect.x + cornerSize, deckRect.y + deckRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), 3);
                FES.DrawEllipse(new Vector2i(deckRect.x + cornerSize, deckRect.y + deckRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), 2);

                FES.DrawEllipseFill(new Vector2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + deckRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), 3);
                FES.DrawEllipse(new Vector2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + deckRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), 2);

                FES.DrawRect(new Rect2i(deckRect.x + cornerSize, deckRect.y, deckRect.width - (cornerSize * 2), deckRect.height), 2);
                FES.DrawRectFill(new Rect2i(deckRect.x + cornerSize, deckRect.y + 1, deckRect.width - (cornerSize * 2), deckRect.height - 2), 3);

                FES.DrawRect(new Rect2i(deckRect.x, deckRect.y + cornerSize, cornerSize, deckRect.height - (cornerSize * 2)), 2);
                FES.DrawRectFill(new Rect2i(deckRect.x + 1, deckRect.y + cornerSize, cornerSize - 1, deckRect.height - (cornerSize * 2)), 3);

                FES.DrawRect(new Rect2i(deckRect.x + deckRect.width - cornerSize, deckRect.y + cornerSize, cornerSize, deckRect.height - (cornerSize * 2)), 2);
                FES.DrawRectFill(new Rect2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + cornerSize, cornerSize, deckRect.height - (cornerSize * 2)), 3);

                DrawSpinner(0, 0, spinnerSize);
                DrawSpinner(155, 0, spinnerSize);

                mMusicPlayButton.Render();
            }
            else
            {
                FES.DrawEllipseFill(new Vector2i(deckRect.x + cornerSize, deckRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
                FES.DrawEllipse(new Vector2i(deckRect.x + cornerSize, deckRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

                FES.DrawEllipseFill(new Vector2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
                FES.DrawEllipse(new Vector2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

                FES.DrawEllipseFill(new Vector2i(deckRect.x + cornerSize, deckRect.y + deckRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
                FES.DrawEllipse(new Vector2i(deckRect.x + cornerSize, deckRect.y + deckRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

                FES.DrawEllipseFill(new Vector2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + deckRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
                FES.DrawEllipse(new Vector2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + deckRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

                FES.DrawRect(new Rect2i(deckRect.x + cornerSize, deckRect.y, deckRect.width - (cornerSize * 2), deckRect.height), DemoUtil.IndexToRGB(2));
                FES.DrawRectFill(new Rect2i(deckRect.x + cornerSize, deckRect.y + 1, deckRect.width - (cornerSize * 2), deckRect.height - 2), DemoUtil.IndexToRGB(3));

                FES.DrawRect(new Rect2i(deckRect.x, deckRect.y + cornerSize, cornerSize, deckRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(2));
                FES.DrawRectFill(new Rect2i(deckRect.x + 1, deckRect.y + cornerSize, cornerSize - 1, deckRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(3));

                FES.DrawRect(new Rect2i(deckRect.x + deckRect.width - cornerSize, deckRect.y + cornerSize, cornerSize, deckRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(2));
                FES.DrawRectFill(new Rect2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + cornerSize, cornerSize, deckRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(3));

                DrawSpinner(0, 0, spinnerSize);
                DrawSpinner(155, 0, spinnerSize);

                mMusicPlayButton.Render();
            }

            FES.CameraReset();
        }

        private void UpdateMusicButtonLabel()
        {
            mMusicPlayButton.Label = mMusicPlaying ? "H - Stop" : "H - Play";
        }

        private class PianoKeyInfo
        {
            public float Pitch;
            public FES.SoundReference SoundRef;

            public PianoKeyInfo(float pitch)
            {
                Pitch = pitch;
            }
        }
    }
}
