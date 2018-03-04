using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ClimbableLadder : MonoBehaviour {

	public bool canClimb;
	public float climbSpeed;
	private Collider player;
	private FirstPersonController controller;
	private float oldGravity;
	
	void OnTriggerEnter(Collider other){
		if (other.tag == "Player" && controller == null){
			player = other;
			canClimb = true;
			controller= other.GetComponent<FirstPersonController>();
			oldGravity = controller.m_GravityMultiplier;
			controller.m_GravityMultiplier = 0;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.tag == "Player" && controller != null){
			canClimb=false;
			player.GetComponent<Rigidbody>().AddForce(Vector3.up * climbSpeed);
			controller.m_GravityMultiplier=oldGravity;
			controller=null;
		}
	}


	void Update () {
		if (canClimb){
			float v = Input.GetAxis("Vertical");
			player.transform.Translate(Vector3.up * v * climbSpeed * Time.deltaTime);
		}
	}
}
