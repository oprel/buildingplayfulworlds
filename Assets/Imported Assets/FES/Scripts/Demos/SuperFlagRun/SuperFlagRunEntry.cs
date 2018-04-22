using UnityEngine;

/// <summary>
/// Super Flag Run entry
/// </summary>
public class SuperFlagRunEntry : MonoBehaviour
{
    private void Awake()
    {
        // To get started call FES.Initialize with an instance of SuperFlagRun
        FES.Initialize(new FESDemo.SuperFlagRun());
    }
}
