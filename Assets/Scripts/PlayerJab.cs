using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJab : MonoBehaviour {

	public GameObject armL;
	public GameObject armR;
	public Vector3 jabLength;
	private Vector3 defaultL;
	private Vector3 defaultR;


	// Use this for initialization
	void Start () {
		defaultL = armL.transform.position - transform.parent.transform.position;
		defaultR = armR.transform.position - transform.parent.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire1")){
			jab(armL);
		}
		if (Input.GetButton("Fire2")){
			jab(armR);
		}
		armL.transform.position = Vector3.Slerp(armL.transform.position,defaultL, Time.deltaTime);
		armR.transform.position = Vector3.Slerp(armR.transform.position,defaultR, Time.deltaTime);
	}

	void jab(GameObject arm){
		for (int i =0; i <7;i++){
			arm.transform.position = Vector3.Slerp(arm.transform.position,arm.transform.position+jabLength, Time.deltaTime);
		}
		
	}
}
