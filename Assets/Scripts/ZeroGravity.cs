using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravity : MonoBehaviour {

void OnTriggerEnter(Collider other){
	Rigidbody rb = other.GetComponent<Rigidbody>();
	if (rb != null)
		rb.useGravity=false;
		rb.velocity*= -.5f;
		rb.angularVelocity = Vector3.zero;
	}

void OnTriggerExit(Collider other){
	Rigidbody rb = other.GetComponent<Rigidbody>();
	if (rb != null)
		rb.useGravity=true;
	}

}
