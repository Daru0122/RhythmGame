using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Runtime.InteropServices;
using System.Windows;
public class Player : MonoBehaviour
{
    
    public Thread player;
    public static Player playScript;
    public static float HISPEED = 2;
    public List<Queue<GameObject>> Notes = new List<Queue<GameObject>>();
    //플레이중 저장되는 데이터들
    public int playScore;
    public int playCombo;
    public int[] judgeCount = new int[5];
    //---------------------
    public FMOD.Sound[] WAV = new FMOD.Sound[1296];//#WAVxx 파일 저장
    private FMOD.ChannelGroup channelGroup;
    private FMOD.Channel[] channel = new FMOD.Channel[1296];
    public float BPM;
    public float BPMchangeTime;
    public float BPMchangescroll;
    public static float totalScroll;
    public bool STOPED;
    public FMOD.System system;
    public static System.Diagnostics.Stopwatch Time = new System.Diagnostics.Stopwatch();//초시계
    public Queue<BGnotes> BGMs = new Queue<BGnotes>();
    private int[] sndPerKey = new int[9];
    private bool[] KeyActived = new bool[9];
    private void Awake() {
        Notes.Add(new Queue<GameObject>());
        Notes.Add(new Queue<GameObject>());
        Notes.Add(new Queue<GameObject>());
        Notes.Add(new Queue<GameObject>());
        Notes.Add(new Queue<GameObject>());
        Notes.Add(new Queue<GameObject>());
        Notes.Add(new Queue<GameObject>());
        Notes.Add(new Queue<GameObject>());
        Notes.Add(new Queue<GameObject>());
        Notes.Add(new Queue<GameObject>());
        player = new Thread(playBGM);
        playScript = gameObject.GetComponent<Player>();
        system = FMODUnity.RuntimeManager.CoreSystem;
        if(!Application.isEditor){//에디터가 아닐때만 실행(CoreSystem을 닫는게 에디터에서 크래시를 일으키는듯)
            system.close();
        }
        system.setSoftwareChannels(1296);
        system.init(1296,FMOD.INITFLAGS.NORMAL, new System.IntPtr(0));
        system.getMasterChannelGroup(out channelGroup);
    }
    void Update(){
        if(STOPED){
            totalScroll=BPMchangescroll;
        }else{
            totalScroll = ((float)Time.Elapsed.TotalMilliseconds-BPMchangeTime*1000)*0.6f*BPM/174545+BPMchangescroll;
        }
        if(Input.GetKeyDown(KeyCode.F1)){
            HISPEED-=0.5f;
        }if(Input.GetKeyDown(KeyCode.F2)){
            HISPEED+=0.5f;
        }
    }
    public void GetFirstKeysnd(){
        sndPerKey[0] = Notes[0].Peek().GetComponent<Notescript>().snd;
        sndPerKey[1] = Notes[1].Peek().GetComponent<Notescript>().snd;
        sndPerKey[2] = Notes[2].Peek().GetComponent<Notescript>().snd;
        sndPerKey[3] = Notes[3].Peek().GetComponent<Notescript>().snd;
        sndPerKey[4] = Notes[4].Peek().GetComponent<Notescript>().snd;
        sndPerKey[5] = Notes[5].Peek().GetComponent<Notescript>().snd;
        sndPerKey[7] = Notes[7].Peek().GetComponent<Notescript>().snd;
        sndPerKey[8] = Notes[8].Peek().GetComponent<Notescript>().snd;
    }
    public void playBGM(){
        UnityRawInput.RawInput.Start();
        Time.Start();
        BGnotes currentBGM = BGMs.Peek();
        while (true){
            Thread.Sleep(1);
            if(BGMs.Count > 0){
                while(currentBGM.noteTime*1000<=Time.Elapsed.TotalMilliseconds){
                    currentBGM = BGMs.Dequeue();
                    channel[currentBGM.noteSnd].stop();
                    if(UnityRawInput.RawInput.AnyKeyDown){
                        system.playSound(WAV[currentBGM.noteSnd], channelGroup, false, out channel[currentBGM.noteSnd]);
                    }
                    if(BGMs.Count <= 0){
                        break;
                    }
                    currentBGM = BGMs.Peek();
                }
            }
            //--------------------
            if(true){
                if(KeyActived[0]){
                    system.playSound(WAV[22], channelGroup, false, out channel[22]);
                    KeyActived[0]=false;
                }
            }else{
                KeyActived[0]=true;
            }
            if(true){
                if(KeyActived[1]){
                    system.playSound(WAV[2], channelGroup, false, out channel[2]);
                    KeyActived[1]=false;
                }
            }else{
                KeyActived[1]=true;
            }
            if(true){
                if(KeyActived[2]){
                    system.playSound(WAV[2], channelGroup, false, out channel[2]);
                    KeyActived[2]=false;
                }
            }else{
                KeyActived[2]=true;
            }
            if(true){
                if(KeyActived[3]){
                    system.playSound(WAV[2], channelGroup, false, out channel[2]);
                    KeyActived[3]=false;
                }
            }else{
                KeyActived[3]=true;
            }
            if(true){
                if(KeyActived[4]){
                    system.playSound(WAV[2], channelGroup, false, out channel[2]);
                    KeyActived[4]=false;
                }
            }else{
                KeyActived[4]=true;
            }
            if(true){
                if(KeyActived[5]){
                    system.playSound(WAV[2], channelGroup, false, out channel[2]);
                    KeyActived[5]=false;
                }
            }else{
                KeyActived[5]=true;
            }
            if(true){
                if(KeyActived[7]){
                    system.playSound(WAV[2], channelGroup, false, out channel[2]);
                    KeyActived[7]=false;
                }
            }else{
                KeyActived[7]=true;
            }
            if(true){
                if(KeyActived[8]){
                    system.playSound(WAV[2], channelGroup, false, out channel[2]);
                    KeyActived[8]=false;
                }
            }else{
                KeyActived[8]=true;
            }
        }
    }
    void Judge(int noteNum){
        GameObject note = Notes[noteNum-1].Peek();
        Notescript noteScript = note.GetComponent<Notescript>();
        if(noteScript.noteType<50){
            if(noteScript.time*1000 <= Time.Elapsed.TotalMilliseconds+dataManager.judgeTimings[0] && noteScript.time*1000 >= Time.Elapsed.TotalMilliseconds-dataManager.judgeTimings[0]){
                sndPerKey[noteNum-1] = noteScript.snd;//PG
                Destroy(note);
                Notes[noteNum-1].Dequeue();
                playScore+=2;
                playCombo++;
            }else if(noteScript.time*1000 <= Time.Elapsed.TotalMilliseconds+dataManager.judgeTimings[1] && noteScript.time*1000 >= Time.Elapsed.TotalMilliseconds-dataManager.judgeTimings[1]){
                sndPerKey[noteNum-1] = noteScript.snd;//GR
                Destroy(note);
                Notes[noteNum-1].Dequeue();
                playScore++;
                playCombo++;
            }else if(noteScript.time*1000 <= Time.Elapsed.TotalMilliseconds+dataManager.judgeTimings[2] && noteScript.time*1000 >= Time.Elapsed.TotalMilliseconds-dataManager.judgeTimings[2]){
                sndPerKey[noteNum-1] = noteScript.snd;//GD
                Destroy(note);
                Notes[noteNum-1].Dequeue();
                playCombo++;
            }else if(noteScript.time*1000 <= Time.Elapsed.TotalMilliseconds+dataManager.judgeTimings[3] && noteScript.time*1000 >= Time.Elapsed.TotalMilliseconds-dataManager.judgeTimings[3]){
                sndPerKey[noteNum-1] = noteScript.snd;//BD
                Destroy(note);
                Notes[noteNum-1].Dequeue();
                playCombo=0;
            }
            if(sndPerKey[noteNum-1]!=0){
                channel[sndPerKey[noteNum-1]].stop();
                FMODUnity.RuntimeManager.CoreSystem.playSound(WAV[sndPerKey[noteNum-1]], channelGroup, false, out channel[sndPerKey[noteNum-1]]);
            }
        }else{
            StartCoroutine(LNjudge(noteNum));
        }
    }
    IEnumerator LNjudge(int noteNum){
        int saveJudge = 0;//0:poor,BD,1:PG,2:GR,3:GD
        GameObject note;
        Notescript noteScript;
        GameObject Endnote;
        Notescript EndnoteScript;
        note = Notes[noteNum-1].Peek();
        noteScript = note.GetComponent<Notescript>();
        if(noteScript.time*1000 <= Time.Elapsed.TotalMilliseconds+dataManager.judgeTimings[3] && noteScript.time*1000 >= Time.Elapsed.TotalMilliseconds-dataManager.judgeTimings[3]){
            if(noteScript.time*1000 <= Time.Elapsed.TotalMilliseconds+dataManager.judgeTimings[0] && noteScript.time*1000 >= Time.Elapsed.TotalMilliseconds-dataManager.judgeTimings[0]){
                sndPerKey[noteNum-1] = noteScript.snd;//PG
                Destroy(note);
                if(sndPerKey[noteNum-1]!=0){
                FMODUnity.RuntimeManager.CoreSystem.playSound(WAV[sndPerKey[noteNum-1]], channelGroup, false, out channel[sndPerKey[noteNum-1]]);
                }
                Notes[noteNum-1].Dequeue();
                Endnote = Notes[noteNum-1].Peek();
                EndnoteScript = Endnote.GetComponent<Notescript>();
                EndnoteScript.EXtype = 4;
                playCombo++;
                saveJudge = 1;
            }else if(noteScript.time*1000 <= Time.ElapsedMilliseconds+dataManager.judgeTimings[1] && noteScript.time*1000 >= Time.ElapsedMilliseconds-dataManager.judgeTimings[1]){
                sndPerKey[noteNum-1] = noteScript.snd;//GR
                Destroy(note);
                if(sndPerKey[noteNum-1]!=0){
                FMODUnity.RuntimeManager.CoreSystem.playSound(WAV[sndPerKey[noteNum-1]], channelGroup, false, out channel[sndPerKey[noteNum-1]]);
                }
                Notes[noteNum-1].Dequeue();
                Endnote = Notes[noteNum-1].Peek();
                EndnoteScript = Endnote.GetComponent<Notescript>();
                EndnoteScript.EXtype = 4;
                playCombo++;
                saveJudge = 2;
            }else if(noteScript.time*1000 <= Time.ElapsedMilliseconds+dataManager.judgeTimings[2] && noteScript.time*1000 >= Time.ElapsedMilliseconds-dataManager.judgeTimings[2]){
                sndPerKey[noteNum-1] = noteScript.snd;//GD
                Destroy(note);
                if(sndPerKey[noteNum-1]!=0){
                FMODUnity.RuntimeManager.CoreSystem.playSound(WAV[sndPerKey[noteNum-1]], channelGroup, false, out channel[sndPerKey[noteNum-1]]);
                }
                Notes[noteNum-1].Dequeue();
                Endnote = Notes[noteNum-1].Peek();
                EndnoteScript = Endnote.GetComponent<Notescript>();
                EndnoteScript.EXtype = 4;
                playCombo++;
                saveJudge = 3;
            }else{
                sndPerKey[noteNum-1] = noteScript.snd;//BD
                Destroy(note);
                if(sndPerKey[noteNum-1]!=0){
                FMODUnity.RuntimeManager.CoreSystem.playSound(WAV[sndPerKey[noteNum-1]], channelGroup, false, out channel[sndPerKey[noteNum-1]]);
                }
                Notes[noteNum-1].Dequeue();
                Endnote = Notes[noteNum-1].Peek();
                EndnoteScript = Endnote.GetComponent<Notescript>();
                playCombo=0;
                saveJudge = 0;
            }
            if(!saveJudge.Equals(0)){
                float tickTime=15/BPM;
                while(EndnoteScript.time*1000>Time.ElapsedMilliseconds-(15/BPM)){
                    yield return new WaitUntil(()=>Time.ElapsedMilliseconds>=noteScript.time+tickTime);
                    if(true){
                        noteScript.EXtype = 4;
                        tickTime+=15/BPM;
                    }else{
                        noteScript.EXtype = 2;
                        saveJudge = 0;
                        break;
                        //롱놑미스
                    }
                }
            }
            yield return new WaitUntil(()=>EndnoteScript.time*1000<=Time.ElapsedMilliseconds);
            Destroy(Endnote);
            Notes[noteNum-1].Dequeue();
            if(saveJudge.Equals(1)){
                playScore+=2;
            }else if(saveJudge.Equals(2)){
                playScore++;
            }
        }
    }
}
