namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Hello World example program
    /// </summary>
    public class HelloWorld : FES.IFESGame
    {
        private int mStringIndex;

        private string mWelcomeStr =
            "Hello Friend!\n\n" +
            "This is a simple @006Hello World@- program. Have a look at it's source\n" +
            "in @006Assets/FES/Scripts/Demo/HelloWorld@- folder to see how it works.\n" +
            "There are other demos to see, each in their own Scene. Check them\n" +
            "out in the @006Assets/FES/Scenes@- folder.\n" +
            "\n" +
            "Be sure to also take a look at the detailed documentation in\n" +
            "the @006Docs@- folder.\n" +
            "\n" +
            "You can also have a look at the @006FES@- APIs directly by looking at the\n" +
            "@006Assets/FES/Scripts/FES.cs@- source file.\n" +
            "\n" +
            "Thank you for your interest in @006FES@-!";

        /// <summary>
        /// Query hardware
        /// </summary>
        /// <returns>Hardware settings</returns>
        public FES.HardwareSettings QueryHardware()
        {
            var hw = new FES.HardwareSettings();

            hw.DisplaySize = new Size2i(320, 180);

            // For this demo we choose the Indexed Color mode, see the documentation about the difference between
            // Indexed color mode and RGB color mode.
            hw.ColorMode = FES.ColorMode.Indexed;

            return hw;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>Return true if successful</returns>
        public bool Initialize()
        {
            FES.SpriteSheetSetup(0, "Demos/HelloWorld/Sprites", new Size2i(8, 8));
            FES.SpriteSheetSet(0);

            FES.SoundSetup(0, "Demos/HelloWorld/Blip");

            return true;
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            if (FES.ButtonPressed(FES.BTN_SYSTEM))
            {
                Application.Quit();
            }

            // This bit of code increments the string index so we can give the illusion of the text being typed out.
            // We have to be careful to skip over the string escape sequences FES uses to do inline color changes.
            // See more about inline color changes in the documentation.
            mStringIndex++;
            if (mStringIndex >= mWelcomeStr.Length)
            {
                mStringIndex = mWelcomeStr.Length - 1;
            }

            if (mWelcomeStr[mStringIndex] == '@')
            {
                if (mWelcomeStr[mStringIndex + 1] == '-')
                {
                    mStringIndex += 2;
                }
                else
                {
                    mStringIndex += 4;
                }
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            FES.Clear(1);

            // Print the welcome string up to "mStringIndex" characters
            int length = Mathf.Min(mStringIndex + 1, mWelcomeStr.Length);
            FES.Print(new Vector2i(4, 4), 3, mWelcomeStr.Substring(0, length));

            // If the text gets past the Hello Friend! part of the string then draw the happy face sprite
            if (length > "Hello Friend!".Length)
            {
                // Alternate between sprite 0 & 1 for a simple wink animation
                int spriteIndex = (FES.Ticks % 100) > 80 ? 1 : 0;
                FES.DrawSprite(spriteIndex, new Vector2i(64, 3));
            }

            // Make a little blip sound every 4 characters printed, with randomized volume & pitch variance
            if (length % 4 == 0)
            {
                FES.SoundPlay(0, Random.Range(0.5f, 0.75f), Random.Range(0.9f, 1.1f));
            }
        }
    }
}
