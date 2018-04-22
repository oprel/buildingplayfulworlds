namespace FESDemo
{
    using UnityEngine;

    /// <summary>
    /// Parent class of all DemoReel scenes
    /// </summary>
    public class SceneDemo
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public virtual bool Initialize()
        {
            return true;
        }

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public virtual void Enter()
        {
            FES.TicksReset();
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public virtual void Exit()
        {
        }

        /// <summary>
        /// Update, handles switching to next scene, and quitting the demo
        /// </summary>
        public virtual void Update()
        {
            var demo = (DemoReel)FES.Game;

            if (FES.KeyPressed(KeyCode.Return) || FES.ButtonPressed(FES.BTN_POINTER_A) || FES.ButtonPressed(FES.BTN_A))
            {
                demo.NextScene();
            }

            if (FES.ButtonPressed(FES.BTN_SYSTEM))
            {
                Application.Quit();
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public virtual void Render()
        {
        }
    }
}
