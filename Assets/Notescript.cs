using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notescript : MonoBehaviour
{
    private Transform tr;
    private SpriteRenderer sr;
    public float time;
    public float scroll;
    public int snd;
    public int noteType;
    public int EXtype;
    public float LNtime;
    public Sprite[] Notes;
    public IEnumerator noteGo(){
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
                }
            }else{
                lineNum = noteType-11;
                x = BMSdataManager.noteLoc[noteType-10];
                sr.sprite = Notes[BMSdataManager.noteSprite[noteType-10]];
            }
            while(BMSdataManager.Time.ElapsedMilliseconds < time*1000){
                yield return null;
                tr.position = new Vector3(x,((scroll-BMSdataManager.totalScroll)*723)+357,0);
            }
            if(snd!=0){
                FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
            }
            Destroy(gameObject);
            BMSdataManager.dManagerScript.Notes[lineNum].Dequeue();
        }
    }
}
