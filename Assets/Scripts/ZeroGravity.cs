using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravity : MonoBehaviour {

private Rigidbody rb;

void OnTriggerEnter(Collider other){
	rb = other.GetComponent<Rigidbody>();
	if (rb)
		rb.useGravity=false;
	}

void OnTriggerExit(Collider other){
	rb = other.GetComponent<Rigidbody>();
	if (rb)
		rb.useGravity=true;
	}

}
