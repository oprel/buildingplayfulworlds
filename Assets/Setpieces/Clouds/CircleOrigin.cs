﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleOrigin : MonoBehaviour {

	public float speed;
	public Vector3 offset;
	private float timeCounter;
	private float dist;
	private Billboard billboard;


	// Use this for initialization
	void Start () {
		dist = transform.position.magnitude;
		timeCounter= 2 * Mathf.PI * dist;
		billboard = GetComponent<Billboard>();
	}

	
	// Update is called once per frame
	void Update () {
		timeCounter += Time.deltaTime * speed/dist;

		float x = Mathf.Cos(timeCounter)*dist+offset.x;
		float y =transform.position.y;
		float z =Mathf.Sin(timeCounter)*dist+offset.z;

		transform.position= new Vector3(x,y,z);
		if (billboard == null)
			transform.rotation = Quaternion.LookRotation (offset - transform.position);
	}
}
