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
    private Coroutine previousLines;
    private Vector3 hitLocation;
    private Color defaultColor;
    private bool globalText;
   // [HideInInspector]
    public bool priorityText;


    void Start(){
        location = display.transform.position;
        currentScript= new string[] {""};
        defaultColor = display.color;
    }
	
    public void clearText(){
        if (currentScript[0] != ""){StopCoroutine( previousLines);};
        currentScript[0]="";
        str="";
    }

	// Update is called once per frame
	void Update () {
		display.text = str;
        display.transform.position= displayLocation;
        if (!globalText && Vector3.Distance(hitLocation,transform.position)>5)
            clearText();
        if (priorityText) priorityText = (currentScript[0] !="");

	}

    void OnTriggerEnter(Collider other) {
        DialogueObject obj = other.GetComponent<DialogueObject>();
         if (!priorityText && obj && obj.DialogueLines[0] != currentScript[0]) {
            globalText = obj.global;
            if (currentScript[0] != ""){StopCoroutine( previousLines);};
            previousLines = StartCoroutine( ShowText(obj.DialogueLines, obj.textColor) );
            hitLocation = transform.position;
         }
     }


    IEnumerator ShowText(string[] lines, Color[] textColors){
        currentScript = (string[]) lines.Clone();
        for (int j=0; j<lines.Length; j++){
            int i = 0;
            str = "";
            if (textColors.Length >0){
                display.color = textColors[Mathf.Min(textColors.Length-1,j)];
            }else{
                display.color= defaultColor;
            }
            while( i < lines[j].Length ){
                str += lines[j][i++];
                yield return new WaitForSeconds(0.1f);
                displayLocation = location + new Vector2(Random.Range(-2,2),Random.Range(-2,2));
            }
            yield return new WaitForSeconds(str.Length*0.1f);
            str="";
        };
        priorityText=false;
        currentScript[0]="";
    }
}
