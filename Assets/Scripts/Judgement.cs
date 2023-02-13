using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Judgement : MonoBehaviour
{
    private InputAction Key;
    public MainKeys KeyAction;
    private void OnEnable() {
        if (KeyAction == null){
            KeyAction = new MainKeys();
        }
        KeyAction.Playing.Enable();
    }
    public void OnDisable()
    {
        KeyAction.Playing.Disable();
    }
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
            if(noteScript.time*1000 <= BMSdataManager.Time.Elapsed.TotalMilliseconds+BMSdataManager.judgeTimings[0] && noteScript.time*1000 >= BMSdataManager.Time.Elapsed.TotalMilliseconds-BMSdataManager.judgeTimings[0]){
                sndPerKey[noteNum-1] = noteScript.snd;//PG
                Destroy(note);
                BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
                BMSdataManager.dManagerScript.playScore+=2;
                BMSdataManager.dManagerScript.playCombo++;
            }else if(noteScript.time*1000 <= BMSdataManager.Time.Elapsed.TotalMilliseconds+BMSdataManager.judgeTimings[1] && noteScript.time*1000 >= BMSdataManager.Time.Elapsed.TotalMilliseconds-BMSdataManager.judgeTimings[1]){
                sndPerKey[noteNum-1] = noteScript.snd;//GR
                Destroy(note);
                BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
                BMSdataManager.dManagerScript.playScore++;
                BMSdataManager.dManagerScript.playCombo++;
            }else if(noteScript.time*1000 <= BMSdataManager.Time.Elapsed.TotalMilliseconds+BMSdataManager.judgeTimings[2] && noteScript.time*1000 >= BMSdataManager.Time.Elapsed.TotalMilliseconds-BMSdataManager.judgeTimings[2]){
                sndPerKey[noteNum-1] = noteScript.snd;//GD
                Destroy(note);
                BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
                BMSdataManager.dManagerScript.playCombo++;
            }else if(noteScript.time*1000 <= BMSdataManager.Time.Elapsed.TotalMilliseconds+BMSdataManager.judgeTimings[3] && noteScript.time*1000 >= BMSdataManager.Time.Elapsed.TotalMilliseconds-BMSdataManager.judgeTimings[3]){
                sndPerKey[noteNum-1] = noteScript.snd;//BD
                Destroy(note);
                BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
                BMSdataManager.dManagerScript.playCombo=0;
            }
            if(sndPerKey[noteNum-1]!=0){
                BMSdataManager.channel[sndPerKey[noteNum-1]].stop();
                FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[sndPerKey[noteNum-1]], BMSdataManager.channelGroup, false, out BMSdataManager.channel[sndPerKey[noteNum-1]]);
            }
        }else{
            StartCoroutine(LNjudge(noteNum));
        }
    }
    IEnumerator LNjudge(int noteNum){
        int saveJudge = 0;//0:poor,BD,1:PG,2:GR,3:GD
        if((noteNum).Equals(1)){
            Key = KeyAction.Playing.Line1;
        }else if((noteNum).Equals(2)){
            Key = KeyAction.Playing.Line2;
        }else if((noteNum).Equals(3)){
            Key = KeyAction.Playing.Line3;
        }else if((noteNum).Equals(4)){
            Key = KeyAction.Playing.Line4;
        }else if((noteNum).Equals(5)){
            Key = KeyAction.Playing.Line5;
        }else if((noteNum).Equals(6)){
            Key = KeyAction.Playing.Line6;
        }else if((noteNum).Equals(8)){
            Key = KeyAction.Playing.Line8;
        }else if((noteNum).Equals(9)){
            Key = KeyAction.Playing.Line9;
        }
        GameObject note;
        Notescript noteScript;
        note = BMSdataManager.dManagerScript.Notes[noteNum-1].Peek();
        noteScript = note.GetComponent<Notescript>();
        if(noteScript.time*1000 <= BMSdataManager.Time.Elapsed.TotalMilliseconds+BMSdataManager.judgeTimings[3] && noteScript.time*1000 >= BMSdataManager.Time.Elapsed.TotalMilliseconds-BMSdataManager.judgeTimings[3]){
            if(noteScript.time*1000 <= BMSdataManager.Time.Elapsed.TotalMilliseconds+BMSdataManager.judgeTimings[0] && noteScript.time*1000 >= BMSdataManager.Time.Elapsed.TotalMilliseconds-BMSdataManager.judgeTimings[0]){
                sndPerKey[noteNum-1] = noteScript.snd;//PG
                Destroy(note);
                if(sndPerKey[noteNum-1]!=0){
                FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[sndPerKey[noteNum-1]], BMSdataManager.channelGroup, false, out BMSdataManager.channel[sndPerKey[noteNum-1]]);
                }
                BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
                note = BMSdataManager.dManagerScript.Notes[noteNum-1].Peek();
                noteScript = note.GetComponent<Notescript>();
                noteScript.EXtype = 4;
                BMSdataManager.dManagerScript.playCombo++;
                saveJudge = 1;
            }else if(noteScript.time*1000 <= BMSdataManager.Time.ElapsedMilliseconds+BMSdataManager.judgeTimings[1] && noteScript.time*1000 >= BMSdataManager.Time.ElapsedMilliseconds-BMSdataManager.judgeTimings[1]){
                sndPerKey[noteNum-1] = noteScript.snd;//GR
                Destroy(note);
                if(sndPerKey[noteNum-1]!=0){
                FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[sndPerKey[noteNum-1]], BMSdataManager.channelGroup, false, out BMSdataManager.channel[sndPerKey[noteNum-1]]);
                }
                BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
                note = BMSdataManager.dManagerScript.Notes[noteNum-1].Peek();
                noteScript = note.GetComponent<Notescript>();
                noteScript.EXtype = 4;
                BMSdataManager.dManagerScript.playCombo++;
                saveJudge = 2;
            }else if(noteScript.time*1000 <= BMSdataManager.Time.ElapsedMilliseconds+BMSdataManager.judgeTimings[2] && noteScript.time*1000 >= BMSdataManager.Time.ElapsedMilliseconds-BMSdataManager.judgeTimings[2]){
                sndPerKey[noteNum-1] = noteScript.snd;//GD
                Destroy(note);
                if(sndPerKey[noteNum-1]!=0){
                FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[sndPerKey[noteNum-1]], BMSdataManager.channelGroup, false, out BMSdataManager.channel[sndPerKey[noteNum-1]]);
                }
                BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
                note = BMSdataManager.dManagerScript.Notes[noteNum-1].Peek();
                noteScript = note.GetComponent<Notescript>();
                noteScript.EXtype = 4;
                BMSdataManager.dManagerScript.playCombo++;
                saveJudge = 3;
            }else if(noteScript.time*1000 <= BMSdataManager.Time.ElapsedMilliseconds+BMSdataManager.judgeTimings[3] && noteScript.time*1000 >= BMSdataManager.Time.ElapsedMilliseconds-BMSdataManager.judgeTimings[3]){
                sndPerKey[noteNum-1] = noteScript.snd;//BD
                Destroy(note);
                if(sndPerKey[noteNum-1]!=0){
                FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[sndPerKey[noteNum-1]], BMSdataManager.channelGroup, false, out BMSdataManager.channel[sndPerKey[noteNum-1]]);
                }
                BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
                note = BMSdataManager.dManagerScript.Notes[noteNum-1].Peek();
                BMSdataManager.dManagerScript.playCombo=0;
                saveJudge = 0;
            }
            if(!saveJudge.Equals(0)){
                while(noteScript.time*1000>BMSdataManager.Time.ElapsedMilliseconds){
                    yield return new WaitForSeconds(15/BMSdataManager.dManagerScript.BPM);
                    if(Key.ReadValue<float>() > 0.5f){
                        noteScript.EXtype = 4;
                    }else{
                        noteScript.EXtype = 2;
                        saveJudge = 0;
                        break;
                        //롱놑미스
                    }
                }
            }
            Destroy(note);
            BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
            if(saveJudge.Equals(1)){
                BMSdataManager.dManagerScript.playScore+=2;
            }else if(saveJudge.Equals(2)){
                BMSdataManager.dManagerScript.playScore++;
            }
        }
    }
}
