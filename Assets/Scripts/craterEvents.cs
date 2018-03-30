using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class craterEvents : MonoBehaviour {

	public GameObject noseman;
	private GameObject player;
	private DialogueObject dialogue;
	public BoxCollider col;
	private DialogueSystem dialogueSystem;
	public Camera playerCamera;
	public GameObject ladder;

	public string[] exposition;

	private int stateSwitch = 0;
	
	// Use this for initialization
	void Start () {
		col = GetComponent<BoxCollider>();
		dialogue = GetComponent<DialogueObject>();
	}
	

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player" && player==null){
			player = other.gameObject;
			dialogueSystem = player.GetComponent<DialogueSystem>();
			BoxCollider[] cols = GetComponents<BoxCollider>();
			foreach (BoxCollider c in cols){
				c.enabled=true;
			}
			StartCoroutine(checkPlayer());
		}
	}
	
	 IEnumerator checkPlayer(){
		while (true){
			switch (stateSwitch){
				case 0:
				Ray ray = new Ray(player.transform.position, playerCamera.transform.forward);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, 100)){
					if (hit.collider.gameObject == noseman){
						noseman.GetComponent<Rigidbody>().AddForce((player.transform.position-noseman.transform.position)*20);
						noseman.GetComponent<SphereCollider>().enabled=false;
						dialogueSystem.clearText();
						col.enabled=false;
						dialogue.DialogueLines = exposition;
						col.enabled=true;
						yield return new WaitForSeconds(1f);
						dialogueSystem.priorityText=true;
						stateSwitch++;

						
					}
				}
				break;
				case 1:
				if (!dialogueSystem.priorityText){
					ladder.SetActive(true);
					Destroy(gameObject);

				}
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}
	 }
	
}


	
