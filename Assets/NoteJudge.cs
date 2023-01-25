using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteJudge : MonoBehaviour
{
    BMSdataManager bmsDM;
    void Judgement(GameObject nextNote, float judgetime, int noteNum){
        string snd = "00";
        if(BMSdataManager.tT.ElapsedMilliseconds >= judgetime*1000-BMSdataManager.judgeTimings[0] && BMSdataManager.tT.ElapsedMilliseconds <= judgetime*1000+BMSdataManager.judgeTimings[0]){
            snd = nextNote.GetComponent<Note>().snd;
            bmsDM.notes[noteNum].Dequeue();
            Destroy(nextNote);
            Debug.Log("PG");
        }else if(BMSdataManager.tT.ElapsedMilliseconds >= judgetime*1000-BMSdataManager.judgeTimings[1] && BMSdataManager.tT.ElapsedMilliseconds <= judgetime*1000+BMSdataManager.judgeTimings[1]){
            snd = nextNote.GetComponent<Note>().snd;
            bmsDM.notes[noteNum].Dequeue();
            Destroy(nextNote);
            Debug.Log("GR");
        }else if(BMSdataManager.tT.ElapsedMilliseconds >= judgetime*1000-BMSdataManager.judgeTimings[2] && BMSdataManager.tT.ElapsedMilliseconds <= judgetime*1000+BMSdataManager.judgeTimings[2]){
            snd = nextNote.GetComponent<Note>().snd;
            bmsDM.notes[noteNum].Dequeue();
            Destroy(nextNote);
            Debug.Log("GD");
        }else if(BMSdataManager.tT.ElapsedMilliseconds >= judgetime*1000-BMSdataManager.judgeTimings[3] && BMSdataManager.tT.ElapsedMilliseconds <= judgetime*1000+BMSdataManager.judgeTimings[3]){
            snd = nextNote.GetComponent<Note>().snd;
            bmsDM.notes[noteNum].Dequeue();
            Destroy(nextNote);
            Debug.Log("BD");
        }
        if(snd != "00"){
            FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
        }
    }
    IEnumerator getInput1(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[0].Count > 0){
            yield return new WaitUntil (()=> Input.GetKey(BMSdataManager.keyBinds[0]));
            GameObject nextNote = bmsDM.notes[0].Peek();
            float judgetime = nextNote.GetComponent<Note>().time;
            Judgement(nextNote, judgetime, 0);
            yield return new WaitUntil (()=> Input.GetKey(BMSdataManager.keyBinds[0]).Equals(false)); 
        }
    }
    IEnumerator getInput2(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[1].Count > 0){
            yield return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[1]));
            GameObject nextNote = bmsDM.notes[1].Peek();
            float judgetime = nextNote.GetComponent<Note>().time;
            Judgement(nextNote, judgetime, 1);
            yield return new WaitUntil (()=> Input.GetKey(BMSdataManager.keyBinds[1]).Equals(false)); 
        }
    }
    IEnumerator getInput3(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[2].Count > 0){
            yield return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[2]));
            GameObject nextNote = bmsDM.notes[2].Peek();
            float judgetime = nextNote.GetComponent<Note>().time;
            Judgement(nextNote, judgetime, 2);
            yield return new WaitUntil (()=> Input.GetKey(BMSdataManager.keyBinds[2]).Equals(false)); 
        }
    }
    IEnumerator getInput4(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[3].Count > 0){
            yield return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[3]));
            GameObject nextNote = bmsDM.notes[3].Peek();
            float judgetime = nextNote.GetComponent<Note>().time;
            Judgement(nextNote, judgetime, 3);
            yield return new WaitUntil (()=> Input.GetKey(BMSdataManager.keyBinds[3]).Equals(false)); 
        }
    }
    IEnumerator getInput5(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[4].Count > 0){
            yield return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[4]));
            GameObject nextNote = bmsDM.notes[4].Peek();
            float judgetime = nextNote.GetComponent<Note>().time;
            Judgement(nextNote, judgetime, 4);
            yield return new WaitUntil (()=> Input.GetKey(BMSdataManager.keyBinds[4]).Equals(false)); 
        }
    }
    IEnumerator getInput6(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[5].Count > 0){
            yield return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[5]));
            GameObject nextNote = bmsDM.notes[5].Peek();
            float judgetime = nextNote.GetComponent<Note>().time;
            Judgement(nextNote, judgetime, 5);
            yield return new WaitUntil (()=> Input.GetKey(BMSdataManager.keyBinds[5]).Equals(false)); 
        }
    }
    IEnumerator getInput8(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[7].Count > 0){
            yield return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[7]));
            GameObject nextNote = bmsDM.notes[7].Peek();
            float judgetime = nextNote.GetComponent<Note>().time;
            Judgement(nextNote, judgetime, 7);
            yield return new WaitUntil (()=> Input.GetKey(BMSdataManager.keyBinds[7]).Equals(false)); 
        }
    }
    IEnumerator getInput9(){
        yield return new WaitUntil(()=>bmsDM.loadDone >= 1);
        while(bmsDM.notes[8].Count > 0){
            yield return new WaitUntil (()=> Input.GetKeyDown(BMSdataManager.keyBinds[8]));
            GameObject nextNote = bmsDM.notes[8].Peek();
            float judgetime = nextNote.GetComponent<Note>().time;
            Judgement(nextNote, judgetime, 8);
            yield return new WaitUntil (()=> Input.GetKey(BMSdataManager.keyBinds[8]).Equals(false)); 
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