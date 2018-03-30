using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using kode80.PixelRender;
using UnityStandardAssets.Characters.FirstPerson;

public class Teleporter : MonoBehaviour {
	public string scene;
	public Camera pixelCam;

	private IrisWipeEffect wipe;
	private bool porting;

	void OnTriggerEnter(Collider other) {

		if (other.tag == "Player" && !porting){
			porting=true;
			if (!pixelCam)
				return;
			 wipe = pixelCam.GetComponent<IrisWipeEffect>();
			 wipe.target=-1;
			StartCoroutine(CheckIris());
		}
	}

	IEnumerator CheckIris(){
		int i = 0;
		while (i < 60){
		 if (wipe.position<0)
			SceneManager.LoadScene(scene);
			yield return new WaitForSeconds(1f);
		}

	}
}
