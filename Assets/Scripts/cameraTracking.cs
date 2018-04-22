using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraTracking : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		Quaternion a = transform.rotation;
		Quaternion b = transform.localRotation;
		float x = -(Mathf.Abs(a.z) + 9*b.z)/10;
		b.z= Mathf.Lerp(b.z,x,.1f);
		transform.localRotation=b;
	}
}
