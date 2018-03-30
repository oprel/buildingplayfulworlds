using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShove : MonoBehaviour {
	private bool shoving = false;
	public Quaternion targetRot;
	public float returnSpeed = .1f;
	public float shoveSpeed  = .1f;
	public Vector3 impactVector;
	private Collider[] colliders;

	private Quaternion parent;
	
	
	void Start(){
		colliders = transform.GetComponents<Collider>();
	}

	void Update () {
		parent = transform.parent.transform.rotation;
		if (Input.GetButton("Fire1") && !shoving){
			//Debug.Log("Updating arms");
			transform.rotation = Quaternion.Slerp(transform.rotation, parent * targetRot, Time.deltaTime * shoveSpeed);
			foreach (Collider col in colliders){
				col.enabled = true;
			} 

		}else{
			transform.rotation = Quaternion.Slerp(transform.rotation, parent, Time.deltaTime * returnSpeed);
			foreach (Collider col in colliders){
				col.enabled = false;
			} 
		}
		
	}
	void OnTriggerEnter(Collider other){
		Rigidbody rb = other.GetComponent<Rigidbody>();
		if (rb){
			rb.AddForceAtPosition(impactVector, transform.position);

			

		}

		//plane enter
		if (other.transform.parent && other.transform.parent.transform.parent){
			planeController plane = other.transform.parent.transform.parent.GetComponent<planeController>();
			if (plane) plane.Init();
		}
	}


	
	

}
