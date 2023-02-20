using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Judgement : MonoBehaviour
{
    private int[] sndPerKey = new int[9];
    private InputAction Key;
    public MainKeys KeyAction;
    private bool[] KeyActived = new bool[9];
    private void OnEnable() {
        if (KeyAction == null){
            KeyAction = new MainKeys();
        }
        KeyAction.Playing.Enable();
    }
    private void Update() {//functionKeys
        if(Input.GetKeyDown(KeyCode.F1)){
            BMSdataManager.dManagerScript.HISPEED-=0.5f;
        }if(Input.GetKeyDown(KeyCode.F2)){
            BMSdataManager.dManagerScript.HISPEED+=0.5f;
        }
    }
    public IEnumerator JudgeUpdate() {
        while(true){
            yield return new WaitForFixedUpdate();
            InputSystem.Update();
            if(KeyAction.Playing.Line1.ReadValue<float>()>0){
                if(KeyActived[0]){
                    Judge(1);
                    KeyActived[0]=false;
                }
            }else{
                KeyActived[0]=true;
            }
            if(KeyAction.Playing.Line2.ReadValue<float>()>0){
                if(KeyActived[1]){
                    Judge(2);
                    KeyActived[1]=false;
                }
            }else{
                KeyActived[1]=true;
            }
            if(KeyAction.Playing.Line3.ReadValue<float>()>0){
                if(KeyActived[2]){
                    Judge(3);
                    KeyActived[2]=false;
                }
            }else{
                KeyActived[2]=true;
            }
            if(KeyAction.Playing.Line4.ReadValue<float>()>0){
                if(KeyActived[3]){
                    Judge(4);
                    KeyActived[3]=false;
                }
            }else{
                KeyActived[3]=true;
            }
            if(KeyAction.Playing.Line5.ReadValue<float>()>0){
                if(KeyActived[4]){
                    Judge(5);
                    KeyActived[4]=false;
                }
            }else{
                KeyActived[4]=true;
            }
            if(KeyAction.Playing.Line6.ReadValue<float>()>0){
                if(KeyActived[5]){
                    Judge(6);
                    KeyActived[5]=false;
                }
            }else{
                KeyActived[5]=true;
            }
            if(KeyAction.Playing.Line8.ReadValue<float>()>0){
                if(KeyActived[7]){
                    Judge(8);
                    KeyActived[7]=false;
                }
            }else{
                KeyActived[7]=true;
            }
            if(KeyAction.Playing.Line9.ReadValue<float>()>0){
                if(KeyActived[8]){
                    Judge(9);
                    KeyActived[8]=false;
                }
            }else{
                KeyActived[8]=true;
            }
        }
    }
    public void OnDisable()
    {
        KeyAction.Playing.Disable();
    }
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
        GameObject Endnote;
        Notescript EndnoteScript;
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
                Endnote = BMSdataManager.dManagerScript.Notes[noteNum-1].Peek();
                EndnoteScript = Endnote.GetComponent<Notescript>();
                EndnoteScript.EXtype = 4;
                BMSdataManager.dManagerScript.playCombo++;
                saveJudge = 1;
            }else if(noteScript.time*1000 <= BMSdataManager.Time.ElapsedMilliseconds+BMSdataManager.judgeTimings[1] && noteScript.time*1000 >= BMSdataManager.Time.ElapsedMilliseconds-BMSdataManager.judgeTimings[1]){
                sndPerKey[noteNum-1] = noteScript.snd;//GR
                Destroy(note);
                if(sndPerKey[noteNum-1]!=0){
                FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[sndPerKey[noteNum-1]], BMSdataManager.channelGroup, false, out BMSdataManager.channel[sndPerKey[noteNum-1]]);
                }
                BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
                Endnote = BMSdataManager.dManagerScript.Notes[noteNum-1].Peek();
                EndnoteScript = Endnote.GetComponent<Notescript>();
                EndnoteScript.EXtype = 4;
                BMSdataManager.dManagerScript.playCombo++;
                saveJudge = 2;
            }else if(noteScript.time*1000 <= BMSdataManager.Time.ElapsedMilliseconds+BMSdataManager.judgeTimings[2] && noteScript.time*1000 >= BMSdataManager.Time.ElapsedMilliseconds-BMSdataManager.judgeTimings[2]){
                sndPerKey[noteNum-1] = noteScript.snd;//GD
                Destroy(note);
                if(sndPerKey[noteNum-1]!=0){
                FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[sndPerKey[noteNum-1]], BMSdataManager.channelGroup, false, out BMSdataManager.channel[sndPerKey[noteNum-1]]);
                }
                BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
                Endnote = BMSdataManager.dManagerScript.Notes[noteNum-1].Peek();
                EndnoteScript = Endnote.GetComponent<Notescript>();
                EndnoteScript.EXtype = 4;
                BMSdataManager.dManagerScript.playCombo++;
                saveJudge = 3;
            }else{
                sndPerKey[noteNum-1] = noteScript.snd;//BD
                Destroy(note);
                if(sndPerKey[noteNum-1]!=0){
                FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[sndPerKey[noteNum-1]], BMSdataManager.channelGroup, false, out BMSdataManager.channel[sndPerKey[noteNum-1]]);
                }
                BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
                Endnote = BMSdataManager.dManagerScript.Notes[noteNum-1].Peek();
                EndnoteScript = Endnote.GetComponent<Notescript>();
                BMSdataManager.dManagerScript.playCombo=0;
                saveJudge = 0;
            }
            if(!saveJudge.Equals(0)){
                float tickTime=15/BMSdataManager.dManagerScript.BPM;
                while(EndnoteScript.time*1000>BMSdataManager.Time.ElapsedMilliseconds-(15/BMSdataManager.dManagerScript.BPM)){
                    yield return new WaitUntil(()=>BMSdataManager.Time.ElapsedMilliseconds>=noteScript.time+tickTime);
                    if(Key.ReadValue<float>() > 0){
                        noteScript.EXtype = 4;
                        tickTime+=15/BMSdataManager.dManagerScript.BPM;
                    }else{
                        noteScript.EXtype = 2;
                        saveJudge = 0;
                        break;
                        //롱놑미스
                    }
                }
            }
            yield return new WaitUntil(()=>EndnoteScript.time*1000<=BMSdataManager.Time.ElapsedMilliseconds);
            Destroy(Endnote);
            BMSdataManager.dManagerScript.Notes[noteNum-1].Dequeue();
            if(saveJudge.Equals(1)){
                BMSdataManager.dManagerScript.playScore+=2;
            }else if(saveJudge.Equals(2)){
                BMSdataManager.dManagerScript.playScore++;
            }
        }
    }
}
