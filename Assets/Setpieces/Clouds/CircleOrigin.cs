using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleOrigin : MonoBehaviour {

	public float speed;
	private float timeCounter;
	private float dist;
	// Use this for initialization
	void Start () {
		dist = transform.position.magnitude;
		timeCounter= 2 * Mathf.PI * dist;
	}
	
	// Update is called once per frame
	void Update () {
		timeCounter += Time.deltaTime * speed/dist;

		float x = Mathf.Cos(timeCounter)*dist;
		float y =transform.position.y;
		float z =Mathf.Sin(timeCounter)*dist;

		transform.position= new Vector3(x,y,z);
	}
}
