using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FESInternal;

public class getPixelScreen : MonoBehaviour {

	public GameObject FESPixelCamera;
	private Material display;

	void Update () {
		
		display = GetComponent<Renderer>().material;
		display.SetColor("_Color",Color.white);
		display.mainTexture = FESPixelCamera.GetComponent<FESPixelCamera>().screen;
	}
}
