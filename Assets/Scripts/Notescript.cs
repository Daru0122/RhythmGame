using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notescript : MonoBehaviour
{
    private RectTransform tr;
    private Image sr;
    public float scroll;
    public int noteType;
    public int noteLine;
    public int noteNumber;
    public Sprite[] Notes;
    private float x;
    private float y;
    private float LNleng;
    void Awake() {
        tr = gameObject.GetComponent<RectTransform>();
        sr = gameObject.GetComponent<Image>();
    }
    void Start(){
        if(noteType!=3){
            noteType=Player.playScript.Notes[noteLine][noteNumber].Type;
            if(!noteLine.Equals(null)&&!noteNumber.Equals(null)){
                y=0;
                if(noteType.Equals(2)){
                    sr.sprite = Notes[dataManager.noteSprite[noteLine+140]];
                    x = dataManager.noteLoc[noteLine]+dataManager.playAreaX;
                    y = -4;
                }else if(noteType.Equals(1)){
                    sr.sprite = Notes[dataManager.noteSprite[noteLine+40]];
                    x = dataManager.noteLoc[noteLine]+dataManager.playAreaX;
                }else if(noteType.Equals(0)){
                    sr.sprite = Notes[dataManager.noteSprite[noteLine]];
                    x = dataManager.noteLoc[noteLine]+dataManager.playAreaX;
                }
                tr.sizeDelta = new Vector2(sr.sprite.bounds.size.x,sr.sprite.bounds.size.y);
                scroll=Player.playScript.Notes[noteLine][noteNumber].scroll;
            }
        }else{
            gameObject.transform.SetSiblingIndex(1);
            sr.sprite = Notes[dataManager.noteSprite[noteLine+240]];
            x = dataManager.noteLoc[noteLine]+dataManager.playAreaX;
            y=0;
            scroll=Player.playScript.Notes[noteLine][noteNumber].scroll;
            LNleng=Player.playScript.Notes[noteLine][noteNumber].scroll-Player.playScript.Notes[noteLine][noteNumber-1].scroll;
        }
    }
    void Update(){
        if(Player.playScript.Notes[noteLine][noteNumber].proced){
            Destroy(gameObject);
        }else{
            if(noteType.Equals(3)){
                if(Player.playScript.Notes[noteLine][noteNumber].Type==3){
                    tr.position = new Vector3(x,357+y,0);
                    sr.sprite = Notes[dataManager.noteSprite[noteLine+240]+3];
                    tr.sizeDelta = new Vector2(sr.sprite.bounds.size.x,(scroll-Player.totalScroll)*723*Player.HISPEED);
                }else{
                    sr.sprite = Notes[dataManager.noteSprite[noteLine+240]];
                    tr.position = new Vector3(x,((scroll-Player.totalScroll-LNleng)*723*Player.HISPEED)+357+y,0);
                    tr.sizeDelta = new Vector2(sr.sprite.bounds.size.x,LNleng*723*Player.HISPEED);
                }
            }else{
                tr.position = new Vector3(x,((scroll-Player.totalScroll)*723*Player.HISPEED)+357+y,0);
            }
        }
    }
}
