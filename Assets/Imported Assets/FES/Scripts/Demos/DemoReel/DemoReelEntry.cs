using UnityEngine;

/// <summary>
/// Demo Reel entry
/// </summary>
public class DemoReelEntry : MonoBehaviour
{
    /// <summary>
    /// Color mode that the demo will run in. Either Indexed, or RGB.
    /// </summary>
    public FES.ColorMode ColorMode;

    private void Awake()
    {
        // To get started call FES.Initialize with an instance of DemoReel
        FES.Initialize(new FESDemo.DemoReel(ColorMode));
    }
}
