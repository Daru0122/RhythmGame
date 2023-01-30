using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notescript : MonoBehaviour
{
    public Notescript lnEndnote;
    private Transform tr;
    private SpriteRenderer sr;
    public float time;
    public float scroll;
    public int snd;
    public int noteType;
    public int EXtype;
    public float LNtime;
    public Sprite[] Notes;
    void Start(){
        StartCoroutine(noteGo());
    }
    IEnumerator noteGo(){
        yield return new WaitUntil(()=> BMSdataManager.dManagerScript.loadDone>=1);
        int lineNum = 0;
        tr = gameObject.GetComponent<Transform>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        if(noteType.Equals(1)){
            yield return new WaitUntil(()=> BMSdataManager.Time.ElapsedMilliseconds >= time*1000);
            FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
        }else if(noteType>10){
            float x;
            if(EXtype>0){
                lineNum = noteType-51;
                x = BMSdataManager.noteLoc[noteType-50];
                if(EXtype.Equals(1)){
                    sr.sprite = Notes[BMSdataManager.noteSprite[noteType-10]];//롱노트 시작부분일때
                }else if(EXtype.Equals(2)){
                    sr.sprite = Notes[BMSdataManager.noteSprite[noteType+90]];//롱노트 끝부분일때
                }else if(EXtype.Equals(3)){
                    sr.sortingOrder = 2;
                    sr.sprite = Notes[BMSdataManager.noteSprite[noteType+190]];//롱노트 중앙부분일때
                    tr.localScale = new Vector3(1,LNtime*723,1);
                    while (BMSdataManager.totalScroll < LNtime+scroll){
                        yield return null;
                        if(!lnEndnote.EXtype.Equals(4)){//롱노트 눌리면 4로 바뀔거임
                            tr.position = new Vector3(x,((scroll-BMSdataManager.totalScroll)*723)+357,0);
                        }else{
                            tr.position = new Vector3(x,357,0);
                            tr.localScale = new Vector3(1,(scroll+LNtime-BMSdataManager.totalScroll)*723,1);
                        }
                    }
                    Destroy(gameObject);
                }
            }else{
                lineNum = noteType-11;
                x = BMSdataManager.noteLoc[noteType-10];
                sr.sprite = Notes[BMSdataManager.noteSprite[noteType-10]];
            }
            if(EXtype!=3){
                while(BMSdataManager.Time.ElapsedMilliseconds < time*1000+BMSdataManager.judgeTimings[4]){
                    tr.position = new Vector3(x,((scroll-BMSdataManager.totalScroll)*723)+357,0);
                    yield return null;
                }
                Destroy(gameObject);
                BMSdataManager.dManagerScript.Notes[lineNum].Dequeue();
            }
        }
    }
}
