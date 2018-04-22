namespace FESInternal
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Some utility methods used throughout FES
    /// </summary>
    public class FESUtil
    {
#if UNITY_EDITOR
        private static Dictionary<string, int> mSpamCounter = new Dictionary<string, int>();
#endif

        /// <summary>
        /// Wrap an angle to a value between 0-360 degrees
        /// </summary>
        /// <param name="rotation">Angle</param>
        /// <returns>Wrapped angle</returns>
        public static float WrapAngle(float rotation)
        {
            rotation = rotation % 360;

            if (rotation < 0)
            {
                rotation += 360;
            }

            return rotation;
        }

        /// <summary>
        /// Log an error only once, to avoid spamming
        /// </summary>
        /// <param name="str">Error string</param>
        public static void LogErrorOnce(string str)
        {
            LogOnce(1, str);
        }

        /// <summary>
        /// Log info only once, to avoid spamming
        /// </summary>
        /// <param name="str">Error string</param>
        public static void LogInfoOnce(string str)
        {
            LogOnce(0, str);
        }

        /// <summary>
        /// Log a string only once
        /// </summary>
        /// <param name="severity">Severity level</param>
        /// <param name="str">String</param>
        private static void LogOnce(int severity, string str)
        {
#if UNITY_EDITOR
            var sf = new System.Diagnostics.StackTrace(true).GetFrame(1);
            string id = sf.GetFileName() + ":" + sf.GetFileLineNumber();
            if (!mSpamCounter.ContainsKey(id) || mSpamCounter[id] == 0)
            {
                mSpamCounter[id] = 1;
            }
            else
            {
                // Already logged once
                return;
            }
#endif
            if (severity == 1)
            {
                Debug.LogError(str);
            }
            else
            {
                Debug.Log(str);
            }
        }
    }
}
