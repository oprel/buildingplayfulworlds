using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShove : MonoBehaviour {
	private bool shoving = false;
	public Quaternion targetRot;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire1") && !shoving){
			
			//StartCoroutine(Shove());
			do {
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime);
			}while(Quaternion.Angle(transform.rotation,targetRot)>1);
		}else{
			transform.rotation = Quaternion.Slerp( targetRot, transform.rotation, Time.deltaTime);
		}
	}

	

}
