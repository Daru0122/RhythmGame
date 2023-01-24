using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public bool loadDone;
    public bool LNactive;
    public int secType;
    public float SecTime;
    public string noteBeat;
    public float secBPM;
    public float secScroll;
    public float secScale;
    [SerializeField]  GameObject Note;
    Note N2com;
    BMSdataManager bmsDM;
    void Start(){
        bmsDM = GameObject.Find("DataManager").GetComponent<BMSdataManager>();
        StartCoroutine(noteInstantiate());
    }
    IEnumerator noteInstantiate(){
        if(secType.Equals(1)){//배경음일때
            for(int i = 0; i < noteBeat.Length/2;){
                yield return new WaitUntil(()=> BMSdataManager.tT.ElapsedMilliseconds >= 1000*(SecTime+i*secScale*(240/secBPM)/(noteBeat.Length/2)));
                if(noteBeat.Substring(i*2,2) != "00"){
                    FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[noteBeat.Substring(i*2,2)], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
                }
                i++;
            }
            Destroy(gameObject);
        }else if(secType > 50){//롱노트일때
            for(int i = 0; i < noteBeat.Length/2;){
                if(noteBeat.Substring(i*2,2) != "00"){
                    GameObject N2s = Instantiate(Note);
                    N2com = N2s.GetComponent<Note>();
                    if(bmsDM.LNactive[secType-51]){
                        bmsDM.LNactive[secType-51] = false;
                        N2com.EX = 2;//롱노트 끝 표시
                    }else {
                        N2com.EX = 1;//롱노트 시작 표시
                        bmsDM.LNactive[secType-51] = true;
                        bmsDM.notes[secType-51].Enqueue(N2s);
                    }
                    N2com.type = secType;
                    N2com.scroll = secScroll+secScale*144000/174545*i/(noteBeat.Length/2);
                    N2com.snd = noteBeat.Substring(i*2,2);
                    N2com.time = SecTime+i*secScale*(240/secBPM)/(noteBeat.Length/2);
                }
                i++;
            }
            loadDone = true;
        }else {//배경음이 아닐때(노트일때)
            for(int i = 0; i < noteBeat.Length/2;){
                if(noteBeat.Substring(i*2,2) != "00"){
                    GameObject N2s = Instantiate(Note);
                    N2com = N2s.GetComponent<Note>();
                    N2com.EX = 0;
                    N2com.type = secType;
                    N2com.scroll = secScroll+secScale*144000/174545*i/(noteBeat.Length/2);
                    N2com.snd = noteBeat.Substring(i*2,2);
                    N2com.time = SecTime+i*secScale*(240/secBPM)/(noteBeat.Length/2);
                    bmsDM.notes[secType-11].Enqueue(N2s);
                }
                i++;
            }
            loadDone = true;
        }
    }
}
