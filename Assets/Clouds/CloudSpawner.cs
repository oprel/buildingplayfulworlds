using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour {
	public int amount;
	public GameObject Prefab;
	public Texture[] images;
	

	// Use this for initialization
	void Awake () {

		for (int i=0; i <= amount; i++){
			Vector3 start;
			do {
				start= new Vector3(Random.Range(300,800),Random.Range(10,700),0);
			}
			while (start.magnitude > 600);
			GameObject c = Instantiate(Prefab,start, transform.rotation);
			c.transform.parent = gameObject.transform;
			c.transform.localScale*=Random.Range(.1f,4);
			c.GetComponent<Renderer>().material.mainTexture=images[Random.Range(0,images.Length)];
			Vector2 orientation = new Vector2(Random.Range(-1,1),Random.Range(-1,1));
			c.GetComponent<Renderer>().material.SetTextureScale("_MainTex", orientation);
			c.GetComponent<CircleOrigin>().speed = Random.Range(8,10);
		}
	}
	
}
