using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class planeController : MonoBehaviour {
	public float forwardSpeed = 100.0f;
	public CursorLockMode cursorLock;
	public float RotationSens = 200.0f;
	public float speed;
	public GameObject propellor;
	public GameObject player;
	public GameObject localCamera;
	private Rigidbody rb;
	private float roll = 0;
	private float pitch = 0;
	private float propellorSpeed = 0;

	private planeController self;

	void Awake(){
		rb = GetComponent<Rigidbody>();
		self = GetComponent<planeController>();
	}


	public void Init(){
		localCamera.SetActive(true);
		forwardSpeed=0;
		propellorSpeed=0;
		roll=0;
		pitch=0;
		player.SetActive(false);
		self.enabled=true;
		transform.position=player.transform.position;
		transform.rotation=player.transform.rotation;

	}
	void Update()
		{        
			Quaternion AddRot = Quaternion.identity;
			float yaw = 0;
			roll += CrossPlatformInputManager.GetAxis("Mouse X") * (Time.deltaTime * RotationSens);
			pitch += CrossPlatformInputManager.GetAxis("Mouse Y") * (Time.deltaTime * RotationSens);
			forwardSpeed += CrossPlatformInputManager.GetAxis("Vertical")*speed;
			propellorSpeed += CrossPlatformInputManager.GetAxis("Vertical")*speed;
			yaw = Input.GetAxis("Mouse ScrollWheel") * (Time.deltaTime * RotationSens);
			AddRot.eulerAngles = new Vector3(-pitch, yaw, -roll);
			rb.rotation *= AddRot;
			Vector3 AddPos = Vector3.forward;
			AddPos = rb.rotation * AddPos;
			rb.velocity = AddPos * (Time.deltaTime * forwardSpeed);
			forwardSpeed= Mathf.Max(0,forwardSpeed-speed*rb.drag);
			Cursor.lockState = cursorLock;
			if (CrossPlatformInputManager.GetButtonDown("Jump")) EjectPlayer();

			AddRot.z=propellorSpeed;
			propellor.transform.localEulerAngles= new Vector3(0,0,propellorSpeed);
			//propellorSpeed=Mathf.Abs(propellorSpeed-speed)*propellorSpeed/Mathf.Abs(propellorSpeed);
		
		}

	void OnCollisionEnter (Collision col){
		if (self.enabled && Mathf.Abs(Vector3.Magnitude( rb.velocity))>10) EjectPlayer();
	}

	void EjectPlayer(){
		rb.velocity/=2;
		player.transform.position = transform.position + new Vector3(0,30,0);
		player.GetComponent<Rigidbody>().velocity= new Vector3(0,10,0);
		player.SetActive(true);
		localCamera.SetActive(false);
		GetComponent<planeController>().enabled=false;
	}
}
