using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShells : MonoBehaviour {

	public int amount = 0;
	public Text display;
	private int displayAmount;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (displayAmount != amount){
			displayAmount += (amount - displayAmount)/Mathf.Abs(amount - displayAmount);
		}
		display.text = displayAmount.ToString();
	}

	void OnCollisionEnter (Collision other) {
	
		if (other.gameObject.tag == "Shell") {
		Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
			}
	
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Shell"){
			//Destroy(other.gameObject);
			amount+=1;
		}
	}
	
}
