﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMove : MonoBehaviour {

	public Vector3 direction;

	void Update () {

	 	transform.localPosition += transform.rotation * direction * Time.deltaTime;	
	}
}