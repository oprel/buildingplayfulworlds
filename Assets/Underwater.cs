 using UnityEngine;
 using UnityEngine.UI;
 using System.Collections;
using kode80.PixelRender;

 public class Underwater: MonoBehaviour {
	public float waterHeight;
	public RawImage img;
	public Color underwaterColor;
	
	private bool isUnderwater;
	private Color normalColor;
	private Camera camera;
	private Color targetColor;
	private float targetDensity;
	private float transitionSpeed = 20;
	private Color skyColor;

	
	// Use this for initialization
	void Start () {
		camera = transform.GetComponent<Camera>();
		normalColor = new Color (1f, 1f, 1f, 1f);
		targetColor = normalColor;
		targetDensity=0f;
		skyColor=camera.backgroundColor;
		RenderSettings.fogColor=underwaterColor;
	}
	
	// Update is called once per frame
	void Update () {
		if ((transform.position.y < waterHeight) != isUnderwater) {
			isUnderwater = transform.position.y < waterHeight;
			if (isUnderwater) SetUnderwater ();
			if (!isUnderwater) SetNormal ();
		}
		img.color=Color.Lerp(img.color,targetColor,Time.deltaTime * transitionSpeed);
		//camera.farClipPlane = Mathf.Lerp(camera.farClipPlane,targetClip,Time.deltaTime * transitionSpeed);
		RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity,targetDensity,Time.deltaTime * transitionSpeed);
	}
 
	void SetNormal () {
	//camera.farClipPlane=1000;
	//img.color=normalColor;
	targetDensity=0f;
	targetColor=normalColor;
	gameObject.GetComponent<PixelOutlineEffect>().enabled=true;
	camera.backgroundColor=skyColor;
	
	}
 
	void SetUnderwater () {
		//camera.farClipPlane=10;
		//img.color=underwaterColor;
		targetDensity = .2f;
		targetColor = underwaterColor;
		gameObject.GetComponent<PixelOutlineEffect>().enabled=false;
		camera.backgroundColor=underwaterColor;
	}
 }