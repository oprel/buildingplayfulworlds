using UnityEngine;

/// <summary>
/// Stress Test entry
/// </summary>
public class StressTestEntry : MonoBehaviour
{
    private void Awake()
    {
        // To get started call FES.Initialize with an instance of DemoReel
        FES.Initialize(new FESDemo.StressTest());
    }
}
