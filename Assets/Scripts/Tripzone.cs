using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tripzone : MonoBehaviour {

	public Camera m_camera;

	void OnTriggerEnter(Collider other){
		if (other.tag=="Player")
			m_camera.clearFlags= CameraClearFlags.Depth;
	}
	void OnTriggerExit(Collider other){
		if (other.tag=="Player")
			m_camera.clearFlags= CameraClearFlags.SolidColor;
	}
}
