using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stepTrigger : MonoBehaviour {
	
	void OnTriggerEnter (Collider col){
		GetComponent<Rigidbody>().useGravity=true;
	}
}
