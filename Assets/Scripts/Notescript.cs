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
    void Awake() {
        tr = gameObject.GetComponent<Transform>();
        sr = gameObject.GetComponent<SpriteRenderer>();
    }
    void Start(){
        StartCoroutine(noteGo());
    }
    IEnumerator noteGo(){
        yield return new WaitUntil(()=> BMSdataManager.dManagerScript.loadDone>=1);
        int lineNum = 0;
        if(noteType.Equals(8)){//변속
            yield return new WaitUntil(()=> BMSdataManager.Time.Elapsed.TotalMilliseconds >= time*1000);
            BMSdataManager.dManagerScript.BPM = LNtime;
            BMSdataManager.dManagerScript.BPMchangeTime = time;
            BMSdataManager.dManagerScript.BPMchangescroll = scroll;
        }else if(noteType.Equals(9)){//STOP
            yield return new WaitUntil(()=> BMSdataManager.Time.Elapsed.TotalMilliseconds >= time*1000);
            BMSdataManager.dManagerScript.BPMchangescroll = scroll;
            BMSdataManager.dManagerScript.STOPED = true;
            yield return new WaitUntil(()=> BMSdataManager.Time.Elapsed.TotalMilliseconds >= (time+LNtime)*1000);
            BMSdataManager.dManagerScript.BPMchangeTime = time+LNtime;
            BMSdataManager.dManagerScript.STOPED = false;
        }else if(noteType>10){
            float x;
            float y = 0;
            if(EXtype>0){
                lineNum = noteType-51;
                x = BMSdataManager.noteLoc[noteType-50]+BMSdataManager.playAreaX;
                if(EXtype.Equals(1)){
                    sr.sprite = Notes[BMSdataManager.noteSprite[noteType-10]];//롱노트 시작부분일때
                }else if(EXtype.Equals(2)){
                    sr.sprite = Notes[BMSdataManager.noteSprite[noteType+90]];//롱노트 끝부분일때
                    y = -11;
                }else if(EXtype.Equals(3)){
                    int LNanimPro = 0;//롱노트 애니매이션 과정 저장
                    sr.sortingOrder = 2;
                    sr.sprite = Notes[BMSdataManager.noteSprite[noteType+190]];//롱노트 중앙부분일때
                    tr.localScale = new Vector3(1,LNtime*723,1);
                    while (BMSdataManager.totalScroll < LNtime+scroll){
                        yield return null;
                        if(!lnEndnote.EXtype.Equals(4)){//롱노트 눌리면 4로 바뀔거임
                            tr.position = new Vector3(x,((scroll-BMSdataManager.totalScroll)*723*BMSdataManager.dManagerScript.HISPEED)+357,0);
                            tr.localScale = new Vector3(1,LNtime*723*BMSdataManager.dManagerScript.HISPEED,1);
                            sr.sprite = Notes[BMSdataManager.noteSprite[noteType+190]];
                        }else{
                            if(LNanimPro<20){
                                LNanimPro++;
                                sr.sprite = Notes[BMSdataManager.noteSprite[noteType+190]+3];
                                yield return null;
                            }else if(LNanimPro<40){
                                sr.sprite = Notes[BMSdataManager.noteSprite[noteType+190]+6];
                                LNanimPro++;
                                yield return null;
                            }else if(LNanimPro.Equals(40)){
                                LNanimPro=0;
                            }
                            tr.position = new Vector3(x,357,0);
                            tr.localScale = new Vector3(1,(scroll+LNtime-BMSdataManager.totalScroll)*723*BMSdataManager.dManagerScript.HISPEED,1);
                        }
                    }
                    Destroy(gameObject);
                }
            }else{
                lineNum = noteType-11;
                x = BMSdataManager.noteLoc[noteType-10]+BMSdataManager.playAreaX;
                sr.sprite = Notes[BMSdataManager.noteSprite[noteType-10]];
            }
            if(EXtype!=3){
                while(BMSdataManager.Time.Elapsed.TotalMilliseconds < time*1000+BMSdataManager.judgeTimings[4]){
                    yield return null;
                    tr.position = new Vector3(x,((scroll-BMSdataManager.totalScroll)*723*BMSdataManager.dManagerScript.HISPEED)+357+y,0);
                }
                Destroy(gameObject);
                BMSdataManager.dManagerScript.Notes[lineNum].Dequeue();
            }
        }
    }
}
