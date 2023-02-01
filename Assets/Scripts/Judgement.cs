using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Windows.Input;
public class Judgement : MonoBehaviour
{
    private void OnLine1(){
        if(BMSdataManager.dManagerScript.Notes[0].Count > 0){
            Judge(1);
        }
    }
    private void OnLine2(){
        if(BMSdataManager.dManagerScript.Notes[1].Count > 0){
            Judge(2);
        }
    }
    private void OnLine3(){
        if(BMSdataManager.dManagerScript.Notes[2].Count > 0){
            Judge(3);
        }
    }
    private void OnLine4(){
        if(BMSdataManager.dManagerScript.Notes[3].Count > 0){
            Judge(4);
        }
    }
    private void OnLine5(){
        if(BMSdataManager.dManagerScript.Notes[4].Count > 0){
            Judge(5);
        }
    }
    private void OnLine6(){
        if(BMSdataManager.dManagerScript.Notes[5].Count > 0){
            Judge(6);
        }
    }
    private void OnLine8(){
        if(BMSdataManager.dManagerScript.Notes[7].Count > 0){
            Judge(8);
        }
    }
    private void OnLine9(){
        if(BMSdataManager.dManagerScript.Notes[8].Count > 0){
            Judge(9);
        }
    }
    private int[] sndPerKey = new int[9];
    public IEnumerator getFirstKeysound(){//첫키음을 받아냄
        yield return new WaitUntil(()=> BMSdataManager.dManagerScript.loadDone >= 0);
        sndPerKey[0] = BMSdataManager.dManagerScript.Notes[0].Peek().GetComponent<Notescript>().snd;
        sndPerKey[1] = BMSdataManager.dManagerScript.Notes[1].Peek().GetComponent<Notescript>().snd;
        sndPerKey[2] = BMSdataManager.dManagerScript.Notes[2].Peek().GetComponent<Notescript>().snd;
        sndPerKey[3] = BMSdataManager.dManagerScript.Notes[3].Peek().GetComponent<Notescript>().snd;
        sndPerKey[4] = BMSdataManager.dManagerScript.Notes[4].Peek().GetComponent<Notescript>().snd;
        sndPerKey[5] = BMSdataManager.dManagerScript.Notes[5].Peek().GetComponent<Notescript>().snd;
        sndPerKey[7] = BMSdataManager.dManagerScript.Notes[7].Peek().GetComponent<Notescript>().snd;
        sndPerKey[8] = BMSdataManager.dManagerScript.Notes[8].Peek().GetComponent<Notescript>().snd;
    }
    void Judge(int noteNum){
        GameObject note = BMSdataManager.dManagerScript.Notes[noteNum-1].Peek();
        Notescript noteScript = note.GetComponent<Notescript>();
        if(noteScript.noteType<50){
            if(noteScript.time*1000 <= BMSdataManager.Time.ElapsedMilliseconds+BMSdataManager.judgeTimings[1] && noteScript.time*1000 >= BMSdataManager.Time.ElapsedMilliseconds-BMSdataManager.judgeTimings[1]){
                sndPerKey[noteNum-1] = noteScript.snd;
                Destroy(note);
                BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
            }
            if(noteScript.snd!=0){
                FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[sndPerKey[noteNum-1]], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
            }
        }
    }
}
