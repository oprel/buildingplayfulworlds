using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryText : MonoBehaviour {

    public Text display;
    public string[] script;
    private string str;
    private float scale;



    void Start(){
        scale = Time.timeScale;
        Time.timeScale = 0f;
        StartCoroutine(ShowText());
    }
	
	// Update is called once per frame
	void Update () {
		display.text = str;
	}

    IEnumerator ShowText(){
        for (int j=0; j<script.Length; j++){
            int i = 0;
            str = "";
            while( i < script[j].Length ){
                str += script[j][i++];
                yield return StartCoroutine(WaitForRealTime(.1f));
            }
            yield return StartCoroutine(WaitForRealTime(str.Length*.05f));
            str="";
        };
        Time.timeScale = scale;
        gameObject.SetActive(false);
    }

    public static IEnumerator WaitForRealTime(float delay){
         while(true){
             float pauseEndTime = Time.realtimeSinceStartup + delay;
             while (Time.realtimeSinceStartup < pauseEndTime){
                 yield return 0;
             }
             break;
         }
     }
}
