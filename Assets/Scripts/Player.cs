using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Runtime.InteropServices;
using System.Windows;
public class Player : MonoBehaviour
{
    public int[] judgeProgress = new int[9];
    private void OnEnable ()
    {
        UnityRawInput.RawInput.WorkInBackground = true;
        UnityRawInput.RawInput.InterceptMessages = false;

        UnityRawInput.RawInput.Start();
    }

    private void OnDisable ()
    {
        UnityRawInput.RawInput.Stop();
    }

    private void OnValidate ()
    {
        // Apply options when toggles are clicked in editor.
        // OnValidate is invoked only in the editor (won't affect build).
        UnityRawInput.RawInput.InterceptMessages = false;
        UnityRawInput.RawInput.WorkInBackground = true;
    }
    //여기까지 rawinput기본 설정

    //----------

    public Thread player;
    public static Player playScript;
    public static float HISPEED = 2;
    public List<List<Note>> Notes = new List<List<Note>>();

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
    public Queue<ScrollNote> scrolls = new Queue<ScrollNote>();
    private int[] sndPerKey = new int[9];
    private bool[] KeyActived = new bool[9];
    private void Awake() {
        Notes.Add(new List<Note>());
        Notes.Add(new List<Note>());
        Notes.Add(new List<Note>());
        Notes.Add(new List<Note>());
        Notes.Add(new List<Note>());
        Notes.Add(new List<Note>());
        Notes.Add(new List<Note>());
        Notes.Add(new List<Note>());
        Notes.Add(new List<Note>());
        Notes.Add(new List<Note>());
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
        if(scrolls.Count>0){
            if(scrolls.Peek().time*1000<=Time.Elapsed.TotalMilliseconds){
                if(scrolls.Peek().type.Equals(8)){//변속
                    BPM=scrolls.Peek().value;
                    BPMchangeTime=scrolls.Peek().time;
                    BPMchangescroll=scrolls.Peek().scroll;
                    scrolls.Dequeue();
                }else if(scrolls.Peek().type.Equals(9)){//정지
                    STOPED=true;
                    BPMchangeTime=scrolls.Peek().time;
                    BPMchangescroll=scrolls.Peek().scroll;
                    if((scrolls.Peek().time+scrolls.Peek().value)*1000<=Time.Elapsed.TotalMilliseconds){
                        STOPED=false;
                        scrolls.Dequeue();
                    }
                }
            }
        }if(STOPED){
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
        getSnd(0);
        getSnd(1);
        getSnd(2);
        getSnd(3);
        getSnd(4);
        getSnd(5);
        getSnd(7);
        getSnd(8);
    }
    private void getSnd(int num){
        sndPerKey[num]=Notes[num][judgeProgress[num]].snd;
    }
    public void playBGM(){
        Time.Start();
        BGnotes currentBGM = BGMs.Peek();
        float TimeCheck = 0;
        int tickCheck = 0;
        int Tick=0;
        while (true){
            if(Tick-(int)Time.Elapsed.TotalMilliseconds>0){
                Thread.Sleep(1);
            }
            Tick++;
            tickCheck++;
            if(TimeCheck<(float)Time.Elapsed.TotalMilliseconds){
                TimeCheck+=1000;
                Debug.Log(tickCheck);
                tickCheck=0;
            }
            if(BGMs.Count > 0){
                while(currentBGM.noteTime*1000<=Time.Elapsed.TotalMilliseconds){
                    currentBGM = BGMs.Dequeue();
                    channel[currentBGM.noteSnd].stop();
                    system.playSound(WAV[currentBGM.noteSnd], channelGroup, false, out channel[currentBGM.noteSnd]);
                    if(BGMs.Count <= 0){
                        break;
                    }
                    currentBGM = BGMs.Peek();
                }
            }
            //--------------------
            CheckMissedNote(0);
            CheckMissedNote(1);
            CheckMissedNote(2);
            CheckMissedNote(3);
            CheckMissedNote(4);
            CheckMissedNote(5);
            CheckMissedNote(7);
            CheckMissedNote(8);
            if(UnityRawInput.RawInput.IsKeyDown(UnityRawInput.RawKey.S)){
                if(KeyActived[0]){
                    Judge(1);
                    KeyActived[0]=false;
                }
            }else{
                KeyActived[0]=true;
            }
            if(UnityRawInput.RawInput.IsKeyDown(UnityRawInput.RawKey.D)){
                if(KeyActived[1]){
                    Judge(2);
                    KeyActived[1]=false;
                }
            }else{
                KeyActived[1]=true;
            }
            if(UnityRawInput.RawInput.IsKeyDown(UnityRawInput.RawKey.F)){
                if(KeyActived[2]){
                    Judge(3);
                    KeyActived[2]=false;
                }
            }else{
                KeyActived[2]=true;
            }
            if(UnityRawInput.RawInput.IsKeyDown(UnityRawInput.RawKey.Space)){
                if(KeyActived[3]){
                    Judge(4);
                    KeyActived[3]=false;
                }
            }else{
                KeyActived[3]=true;
            }
            if(UnityRawInput.RawInput.IsKeyDown(UnityRawInput.RawKey.J)){
                if(KeyActived[4]){
                    Judge(5);
                    KeyActived[4]=false;
                }
            }else{
                KeyActived[4]=true;
            }
            if(UnityRawInput.RawInput.IsKeyDown(UnityRawInput.RawKey.LeftShift)){
                if(KeyActived[5]){
                    Judge(6);
                    KeyActived[5]=false;
                }
            }else{
                KeyActived[5]=true;
            }
            if(UnityRawInput.RawInput.IsKeyDown(UnityRawInput.RawKey.K)){
                if(KeyActived[7]){
                    Judge(8);
                    KeyActived[7]=false;
                }
            }else{
                KeyActived[7]=true;
            }
            if(UnityRawInput.RawInput.IsKeyDown(UnityRawInput.RawKey.L)){
                if(KeyActived[8]){
                    Judge(9);
                    KeyActived[8]=false;
                }
            }else{
                KeyActived[8]=true;
            }
        }
    }
    void CheckMissedNote(int noteNum){
        if(Notes[noteNum].Count>judgeProgress[noteNum]){
            if(Notes[noteNum][judgeProgress[noteNum]].Time*1000+dataManager.judgeTimings[4]<Time.Elapsed.TotalMilliseconds){
                Note note = Notes[noteNum][judgeProgress[noteNum]];
                if(note.Type2==1){
                    note.proced = true;
                    Notes[noteNum][judgeProgress[noteNum]] = note;
                    judgeProgress[noteNum]+=1;
                    note = Notes[noteNum][judgeProgress[noteNum]];
                    note.proced = true;
                    Notes[noteNum][judgeProgress[noteNum]] = note;
                    judgeProgress[noteNum]+=1;
                }else{
                    note.proced = true;
                    Notes[noteNum][judgeProgress[noteNum]] = note;
                    judgeProgress[noteNum]+=1;
                }
                getSnd(noteNum);
            }
        }
    }
    void Judge(int noteNum){
        if(Notes[noteNum-1].Count>judgeProgress[noteNum-1]){
            Note note = Notes[noteNum-1][judgeProgress[noteNum-1]];
            if(note.Time*1000+dataManager.judgeTimings[4] >= Time.Elapsed.TotalMilliseconds && note.Time*1000-dataManager.judgeTimings[4] <= Time.Elapsed.TotalMilliseconds){
                sndPerKey[noteNum-1] = note.snd;//PG
                note.proced = true;
                Notes[noteNum-1][judgeProgress[noteNum-1]] = note;
                judgeProgress[noteNum-1]+=1;
                if(note.Type2==1){
                    note = Notes[noteNum-1][judgeProgress[noteNum-1]];
                    sndPerKey[noteNum-1] = note.snd;//PG
                    note.proced = true;
                    Notes[noteNum-1][judgeProgress[noteNum-1]] = note;
                    judgeProgress[noteNum-1]+=1;
                }
                playScore+=2;
                playCombo++;
            }
            system.playSound(WAV[sndPerKey[noteNum-1]], channelGroup, false, out channel[sndPerKey[noteNum-1]]);
        }
    }
}
