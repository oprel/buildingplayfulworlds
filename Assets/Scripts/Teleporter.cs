using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using kode80.PixelRender;
using UnityStandardAssets.Characters.FirstPerson;

public class Teleporter : MonoBehaviour {
	public Object scene;
	public Camera cam;

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player"){
			 IrisWipeEffect wipe = cam.GetComponent<IrisWipeEffect>();
			 wipe.target=-1;
			 while (wipe.position>0);
			SceneManager.LoadScene(scene.name);
		}
	}

}
