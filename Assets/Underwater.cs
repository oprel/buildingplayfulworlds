 using UnityEngine;
 using UnityEngine.UI;
 using System.Collections;
 
 public class Underwater: MonoBehaviour {
	public float waterHeight;
	public RawImage img;
	
	private bool isUnderwater;
	private Color normalColor;
	private Color underwaterColor;
	private Camera camera;
	private Color targetColor;
	private float targetClip;
	private float transitionSpeed = 10;

	
	// Use this for initialization
	void Start () {
		camera = transform.GetComponent<Camera>();
		normalColor = new Color (1f, 1f, 1f, 1f);
		underwaterColor = new Color32(0x63,0x73,0xB4,0xFF);
		targetColor = normalColor;
		targetClip=1000;
	}
	
	// Update is called once per frame
	void Update () {
		if ((transform.position.y < waterHeight) != isUnderwater) {
			isUnderwater = transform.position.y < waterHeight;
			if (isUnderwater) SetUnderwater ();
			if (!isUnderwater) SetNormal ();
		}
		img.color=Color.Lerp(img.color,targetColor,Time.deltaTime * transitionSpeed);
		camera.farClipPlane = Mathf.Lerp(camera.farClipPlane,targetClip,Time.deltaTime * transitionSpeed);
	}
 
	void SetNormal () {
	//camera.farClipPlane=1000;
	//img.color=normalColor;
	targetClip=1000;
	targetColor=normalColor;
	
	}
 
	void SetUnderwater () {
		//camera.farClipPlane=10;
		//img.color=underwaterColor;
		targetClip = 20;
		targetColor = underwaterColor;
	
	}
 }