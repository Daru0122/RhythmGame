using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteJudge : MonoBehaviour
{
    BMSdataManager bmsDM;
    IEnumerator getInput1(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[0].Count > 0){
                yield  return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[0]));
                GameObject nextNote = bmsDM.notes[0].Peek();
                float judgetime = nextNote.GetComponent<Note>().time;
                string snd = nextNote.GetComponent<Note>().snd;
                if(BMSdataManager.tT.ElapsedMilliseconds >= judgetime*1000-BMSdataManager.judgeTimings[0] && BMSdataManager.tT.ElapsedMilliseconds <= judgetime*1000+BMSdataManager.judgeTimings[0]){
                    FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
                    bmsDM.notes[0].Dequeue();
                    Destroy(nextNote);
                }
            }
    }
    IEnumerator getInput2(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[1].Count > 0){
                yield  return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[1]));
                GameObject nextNote = bmsDM.notes[1].Peek();
                float judgetime = nextNote.GetComponent<Note>().time;
                string snd = nextNote.GetComponent<Note>().snd;
                if(BMSdataManager.tT.ElapsedMilliseconds >= judgetime*1000-BMSdataManager.judgeTimings[0] && BMSdataManager.tT.ElapsedMilliseconds <= judgetime*1000+BMSdataManager.judgeTimings[0]){
                    FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
                    bmsDM.notes[1].Dequeue();
                    Destroy(nextNote);
                }
            }
    }
    IEnumerator getInput3(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[2].Count > 0){
                yield  return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[2]));
                GameObject nextNote = bmsDM.notes[2].Peek();
                float judgetime = nextNote.GetComponent<Note>().time;
                string snd = nextNote.GetComponent<Note>().snd;
                if(BMSdataManager.tT.ElapsedMilliseconds >= judgetime*1000-BMSdataManager.judgeTimings[0] && BMSdataManager.tT.ElapsedMilliseconds <= judgetime*1000+BMSdataManager.judgeTimings[0]){
                    FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
                    bmsDM.notes[2].Dequeue();
                    Destroy(nextNote);
                }
            }
    }
    IEnumerator getInput4(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[3].Count > 0){
                yield  return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[3]));
                GameObject nextNote = bmsDM.notes[3].Peek();
                float judgetime = nextNote.GetComponent<Note>().time;
                string snd = nextNote.GetComponent<Note>().snd;
                if(BMSdataManager.tT.ElapsedMilliseconds >= judgetime*1000-BMSdataManager.judgeTimings[0] && BMSdataManager.tT.ElapsedMilliseconds <= judgetime*1000+BMSdataManager.judgeTimings[0]){
                    FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
                    bmsDM.notes[3].Dequeue();
                    Destroy(nextNote);
                }
            }
    }
    IEnumerator getInput5(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[4].Count > 0){
                yield  return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[4]));
                GameObject nextNote = bmsDM.notes[4].Peek();
                float judgetime = nextNote.GetComponent<Note>().time;
                string snd = nextNote.GetComponent<Note>().snd;
                if(BMSdataManager.tT.ElapsedMilliseconds >= judgetime*1000-BMSdataManager.judgeTimings[0] && BMSdataManager.tT.ElapsedMilliseconds <= judgetime*1000+BMSdataManager.judgeTimings[0]){
                    FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
                    bmsDM.notes[4].Dequeue();
                    Destroy(nextNote);
                }
            }
    }
    IEnumerator getInput6(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[5].Count > 0){
                yield  return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[5]));
                GameObject nextNote = bmsDM.notes[5].Peek();
                float judgetime = nextNote.GetComponent<Note>().time;
                string snd = nextNote.GetComponent<Note>().snd;
                if(BMSdataManager.tT.ElapsedMilliseconds >= judgetime*1000-BMSdataManager.judgeTimings[0] && BMSdataManager.tT.ElapsedMilliseconds <= judgetime*1000+BMSdataManager.judgeTimings[0]){
                    FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
                    bmsDM.notes[5].Dequeue();
                    Destroy(nextNote);
                }
            }
    }
    IEnumerator getInput8(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[7].Count > 0){
                yield  return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[7]));
                GameObject nextNote = bmsDM.notes[7].Peek();
                float judgetime = nextNote.GetComponent<Note>().time;
                string snd = nextNote.GetComponent<Note>().snd;
                if(BMSdataManager.tT.ElapsedMilliseconds >= judgetime*1000-BMSdataManager.judgeTimings[0] && BMSdataManager.tT.ElapsedMilliseconds <= judgetime*1000+BMSdataManager.judgeTimings[0]){
                    FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
                    bmsDM.notes[7].Dequeue();
                    Destroy(nextNote);
                }
            }
    }
    IEnumerator getInput9(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[8].Count > 0){
                yield  return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[8]));
                GameObject nextNote = bmsDM.notes[8].Peek();
                float judgetime = nextNote.GetComponent<Note>().time;
                string snd = nextNote.GetComponent<Note>().snd;
                if(BMSdataManager.tT.ElapsedMilliseconds >= judgetime*1000-BMSdataManager.judgeTimings[0] && BMSdataManager.tT.ElapsedMilliseconds <= judgetime*1000+BMSdataManager.judgeTimings[0]){
                    FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
                    bmsDM.notes[8].Dequeue();
                    Destroy(nextNote);
                }
            }
    }
    // Start is called before the first frame update
    void Start()
    {
        bmsDM = gameObject.GetComponent<BMSdataManager>();
        StartCoroutine(getInput1());
        StartCoroutine(getInput2());
        StartCoroutine(getInput3());
        StartCoroutine(getInput4());
        StartCoroutine(getInput5());
        StartCoroutine(getInput6());
        StartCoroutine(getInput8());
        StartCoroutine(getInput9());
    }
}
