﻿ using UnityEngine;
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
	private float defaultDensity;
	private float targetDensity;
	private float transitionSpeed = 20;
	private Color skyColor;
	private float defaultClip;

	
	// Use this for initialization
	void Start () {
		camera = transform.GetComponent<Camera>();
		normalColor = new Color (1f, 1f, 1f, 1f);
		targetColor = normalColor;
		targetDensity=0f;
		skyColor=camera.backgroundColor;
		RenderSettings.fogColor=underwaterColor;
		defaultClip = camera.farClipPlane;
		defaultDensity = RenderSettings.fogDensity;
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
	camera.farClipPlane=defaultClip;
	//img.color=normalColor;
	targetDensity=defaultDensity;
	targetColor=normalColor;
	gameObject.GetComponent<PixelOutlineEffect>().enabled=true;
	camera.backgroundColor=skyColor;
	
	}
 
	void SetUnderwater () {
		camera.farClipPlane=100;
		//img.color=underwaterColor;
		targetDensity = .1f;
		targetColor = underwaterColor;
		gameObject.GetComponent<PixelOutlineEffect>().enabled=false;
		camera.backgroundColor=underwaterColor*1.33f;
	}
 }