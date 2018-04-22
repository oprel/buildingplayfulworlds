using UnityEngine;

/// <summary>
/// HelloWorld entry
/// </summary>
public class HelloWorldEntry : MonoBehaviour
{
    private void Awake()
    {
        // To get started call FES.Initialize with an instance of HelloWorld
        FES.Initialize(new FESDemo.HelloWorld());
    }
}
