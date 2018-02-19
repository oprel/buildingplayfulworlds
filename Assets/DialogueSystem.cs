using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour {

    public Text display;
    private string str;
    private Vector2 location;
    private Vector2 displayLocation;


    void Start(){
        StartCoroutine( ShowText("Welcome, player!") );
        location = display.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
		display.text = str;
        display.transform.position= displayLocation;
	}

    IEnumerator ShowText(string strComplete){
        int i = 0;
        str = "";
        while( i < strComplete.Length ){
            str += strComplete[i++];
            yield return new WaitForSeconds(0.1f);
            displayLocation = location + new Vector2(Random.Range(-2,2),Random.Range(-2,2));
        }
        yield return new WaitForSeconds(str.Length*0.15f);
        str="";
    }
}
