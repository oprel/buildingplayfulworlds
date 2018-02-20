using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShove : MonoBehaviour {
	private bool shoving = false;
	public Quaternion targetRot;
	public float returnSpeed = .1f;
	public float shoveSpeed  = .1f;

	private Quaternion parent;
	
	// Update is called once per frame
	void Update () {
		parent = transform.parent.transform.rotation;
		if (Input.GetButton("Fire1") && !shoving){
			//Debug.Log("Updating arms");
			transform.rotation = Quaternion.Slerp(transform.rotation, parent * targetRot, Time.deltaTime * shoveSpeed);
		}else{
			transform.rotation = Quaternion.Slerp(transform.rotation, parent, Time.deltaTime * returnSpeed);
		}
	}

	

}
