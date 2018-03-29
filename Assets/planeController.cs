using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class planeController : MonoBehaviour {
	public float AmbientSpeed = 100.0f;

	public float RotationSpeed = 200.0f;
	private Rigidbody rb;
	private float roll = 0;
	private float pitch = 0;

	void Awake(){
		rb = GetComponent<Rigidbody>();
	}

	void Update()
		{        Quaternion AddRot = Quaternion.identity;
			
			float yaw = 0;
			roll += CrossPlatformInputManager.GetAxis("Mouse X") * (Time.deltaTime * RotationSpeed);
			pitch += CrossPlatformInputManager.GetAxis("Mouse Y") * (Time.deltaTime * RotationSpeed);
			AmbientSpeed += CrossPlatformInputManager.GetAxis("Horizontal");
			yaw = Input.GetAxis("Mouse ScrollWheel") * (Time.deltaTime * RotationSpeed);
			AddRot.eulerAngles = new Vector3(-pitch, yaw, -roll);
			rb.rotation *= AddRot;
			Vector3 AddPos = Vector3.forward;
			AddPos = rb.rotation * AddPos;
			rb.velocity = AddPos * (Time.deltaTime * AmbientSpeed);
			AmbientSpeed-=1;
		}
}
