using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activatePC : MonoBehaviour {

	public GameObject FESgame;
	
	// Update is called once per frame
	void OnTriggerEnter(Collider other){
		if (other.tag == "Player"){
			GetComponent<ParticleSystem>().Stop();
			FESgame.SetActive(true);
		}
	}
}
