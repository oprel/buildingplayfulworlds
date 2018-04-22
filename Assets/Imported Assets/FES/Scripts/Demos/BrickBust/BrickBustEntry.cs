using UnityEngine;

/// <summary>
/// This is your game entry point. You can rename this class to whatever you'd like.
/// </summary>
public class BrickBustEntry : MonoBehaviour
{
    private void Awake()
    {
        // To get going simply call FES.Initialize giving it an instance of your game.
        FES.Initialize(new BrickBustGame());
    }
}
