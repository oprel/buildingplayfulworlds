using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour {

    public Text display;
    private string str;
    private Vector2 location;
    private Vector2 displayLocation;
    private string[] currentScript;


    void Start(){
        location = display.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
		display.text = str;
        display.transform.position= displayLocation;
	}

    void OnTriggerEnter(Collider other) {
        DialogueObject obj = other.GetComponent<DialogueObject>();
         if (obj && obj.DialogueLines != currentScript) {
             StartCoroutine( Textlines(obj.DialogueLines) );
         }
     }
    IEnumerator Textlines(string[] lines){
        currentScript = lines;
        for (int i=0; i<lines.Length; i++){
            StartCoroutine(ShowText(lines[i]) );
            do{
                yield return new WaitForSeconds(1);
            } while (str!="");
        };
        Array.Clear(currentScript,0,currentScript.Length);
    }

    IEnumerator ShowText(string strComplete){
        int i = 0;
        str = "";
        while( i < strComplete.Length ){
            str += strComplete[i++];
            yield return new WaitForSeconds(0.1f);
            displayLocation = location + new Vector2(UnityEngine.Random.Range(-2,2),UnityEngine.Random.Range(-2,2));
        }
        yield return new WaitForSeconds(str.Length*0.1f);
        str="";
    }
}
