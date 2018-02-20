using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAnimation : MonoBehaviour {


	public float amplitude = 1;
	public float speed = 1;
	public float seed = 0;
	private float progress = 0;
	private float parentHeight;
	private float height;

	// Use this for initialization
	void Start () {
		if (seed == 0){
			seed = Random.Range(0.0f, 360.0f);
		}
		progress = seed;
		parentHeight = transform.parent.transform.position.y;
		height = transform.position.y - parentHeight;
	}
	
	// Update is called once per frame
	void Update () {
		parentHeight = transform.parent.transform.position.y;
		progress+=Time.deltaTime * speed;
		Vector3 location = transform.position;
		location.y= parentHeight + height+ Mathf.Sin(progress)*amplitude;
		transform.position=location;
	}
}
