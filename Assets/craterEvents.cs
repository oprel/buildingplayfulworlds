using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class craterEvents : MonoBehaviour {

	public GameObject noseman;
	private GameObject player;
	private DialogueObject dialogue;
	private BoxCollider col;
	private DialogueSystem dialogueSystem;
	public Camera playerCamera;
	public GameObject ladder;

	private int stateSwitch = 1;
	
	// Use this for initialization
	void Start () {
		col = GetComponent<BoxCollider>();
		dialogue = GetComponent<DialogueObject>();
	}
	

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player" && player==null){
			player = other.gameObject;
			dialogueSystem = player.GetComponent<DialogueSystem>();
			StartCoroutine(checkPlayer());
		}
	}
	
	 IEnumerator checkPlayer(){
		while (true){
			/*
			float angle = 50;
			if  ( Vector3.Angle(playerCamera.transform.forward, noseman.transform.position - player.transform.position) < angle) {

			}*/

			Ray ray = new Ray(player.transform.position, playerCamera.transform.forward);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 100)){
				if (hit.collider.gameObject == noseman){
					noseman.GetComponent<Rigidbody>().AddForce((player.transform.position-noseman.transform.position)*20);
					noseman.GetComponent<SphereCollider>().enabled=false;
					Destroy(gameObject);
					break;
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
	 }
	
}


	
