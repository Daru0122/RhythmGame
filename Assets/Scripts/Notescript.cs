using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notescript : MonoBehaviour
{
    public Notescript lnEndnote;
    private RectTransform tr;
    private Image sr;
    public float time;
    public float scroll;
    public int snd;
    public int noteType;
    public int EXtype;
    public float LNtime;
    public Sprite[] Notes;
    void Awake() {
        tr = gameObject.GetComponent<RectTransform>();
        sr = gameObject.GetComponent<Image>();
    }
    void Start(){
        StartCoroutine(noteGo());
    }
    IEnumerator noteGo(){
        yield return new WaitUntil(()=> dataManager.loadDone>=1);
        int lineNum = 0;
        if(noteType.Equals(8)){//변속
            yield return new WaitUntil(()=> Player.Time.Elapsed.TotalMilliseconds >= time*1000);
            Player.playScript.BPM = LNtime;
            Player.playScript.BPMchangeTime = time;
            Player.playScript.BPMchangescroll = scroll;
        }else if(noteType.Equals(9)){//STOP
            yield return new WaitUntil(()=> Player.Time.Elapsed.TotalMilliseconds >= time*1000);
            Player.playScript.BPMchangescroll = scroll;
            Player.playScript.STOPED = true;
            yield return new WaitUntil(()=> Player.Time.Elapsed.TotalMilliseconds >= (time+LNtime)*1000);
            Player.playScript.BPMchangeTime = time+LNtime;
            Player.playScript.STOPED = false;
        }else if(noteType>10){
            float x;
            float y = 0;
            if(EXtype>0){
                lineNum = noteType-51;
                x = dataManager.noteLoc[noteType-50]+dataManager.playAreaX;
                if(EXtype.Equals(1)){
                    sr.sprite = Notes[dataManager.noteSprite[noteType-10]];//롱노트 시작부분일때
                    tr.sizeDelta = new Vector2(sr.sprite.bounds.size.x,sr.sprite.bounds.size.y);
                }else if(EXtype.Equals(2)){
                    sr.sprite = Notes[dataManager.noteSprite[noteType+90]];//롱노트 끝부분일때
                    tr.sizeDelta = new Vector2(sr.sprite.bounds.size.x,sr.sprite.bounds.size.y);
                    y = -11;
                }else if(EXtype.Equals(3)){
                    int LNanimPro = 0;//롱노트 애니매이션 과정 저장
                    sr.sprite = Notes[dataManager.noteSprite[noteType+190]];//롱노트 중앙부분일때
                    gameObject.transform.SetAsFirstSibling();
                    gameObject.transform.SetSiblingIndex(tr.GetSiblingIndex()+1);
                    while (Player.totalScroll < LNtime+scroll){
                        yield return null;
                        if(!lnEndnote.EXtype.Equals(4)){//롱노트 눌리면 4로 바뀔거임
                            sr.sprite = Notes[dataManager.noteSprite[noteType+190]];
                            tr.position = new Vector3(x,((scroll-Player.totalScroll)*723*Player.HISPEED)+357,0);
                            tr.sizeDelta = new Vector2(sr.sprite.bounds.size.x,LNtime*723*Player.HISPEED);
                        }else{
                            if(LNanimPro<20){
                                LNanimPro++;
                                sr.sprite = Notes[dataManager.noteSprite[noteType+190]+3];
                            }else if(LNanimPro<40){
                                sr.sprite = Notes[dataManager.noteSprite[noteType+190]+6];
                                LNanimPro++;
                            }else if(LNanimPro>=40){
                                LNanimPro=0;
                            }
                            tr.position = new Vector3(x,357,0);
                            tr.sizeDelta = new Vector2(sr.sprite.bounds.size.x,(scroll+LNtime-Player.totalScroll)*723*Player.HISPEED);
                            yield return null;
                        }
                    }
                    Destroy(gameObject);
                }
            }else{
                lineNum = noteType-11;
                x = dataManager.noteLoc[noteType-10]+dataManager.playAreaX;
                sr.sprite = Notes[dataManager.noteSprite[noteType-10]];
                tr.sizeDelta = new Vector2(sr.sprite.bounds.size.x,sr.sprite.bounds.size.y);
            }
            if(EXtype!=3){
                while(Player.Time.Elapsed.TotalMilliseconds < time*1000+dataManager.judgeTimings[4]){
                    yield return null;
                    tr.position = new Vector3(x,((scroll-Player.totalScroll)*723*Player.HISPEED)+357+y,0);
                }
                Destroy(gameObject);
                Player.playScript.Notes[lineNum].Dequeue();
            }
        }
    }
}
