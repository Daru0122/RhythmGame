using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Runtime.InteropServices;
using System.Windows;
public class Player : MonoBehaviour
{
    public UnityRawInput.RawKey[] keybind = new UnityRawInput.RawKey[9];
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
    public static long elpasedTick;//초시계
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
        if(STOPED){
            totalScroll=BPMchangescroll;
        }else{
            totalScroll = (elpasedTick-BPMchangeTime*1000*10000)*0.00006f*BPM/174545+BPMchangescroll;
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
        keybind[0]=UnityRawInput.RawKey.S;
        keybind[1]=UnityRawInput.RawKey.D;
        keybind[2]=UnityRawInput.RawKey.F;
        keybind[3]=UnityRawInput.RawKey.Space;
        keybind[4]=UnityRawInput.RawKey.J;
        keybind[5]=UnityRawInput.RawKey.LeftShift;
        keybind[7]=UnityRawInput.RawKey.K;
        keybind[8]=UnityRawInput.RawKey.L;
    }
    private void getSnd(int num){
        sndPerKey[num]=Notes[num][judgeProgress[num]].snd;
    }
    public void playBGM(){
        DateTime StartTime = DateTime.Now;
        BGnotes currentBGM = BGMs.Peek();
        long time = 0;
        while (IsGameGoing()){
            elpasedTick = DateTime.Now.Ticks - StartTime.Ticks;
            time += 1250;
            while (time>elpasedTick){
                elpasedTick = DateTime.Now.Ticks - StartTime.Ticks;//0.125ms 대기
            }
            if(BGMs.Count > 0){
                while(currentBGM.noteTime*1000*10000<=time){
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
            CheckMissedNote(0,time);
            CheckMissedNote(1,time);
            CheckMissedNote(2,time);
            CheckMissedNote(3,time);
            CheckMissedNote(4,time);
            CheckMissedNote(5,time);
            CheckMissedNote(7,time);
            CheckMissedNote(8,time);
            //--------------------------
            Judge(1,time);
            Judge(2,time);
            Judge(3,time);
            Judge(4,time);
            Judge(5,time);
            Judge(6,time);
            Judge(8,time);
            Judge(9,time);
            if(scrolls.Count>0){
                if(scrolls.Peek().time*1000*10000<=elpasedTick){
                    if(scrolls.Peek().type.Equals(8)){//변속
                        BPM=scrolls.Peek().value;
                        BPMchangeTime=scrolls.Peek().time;
                        BPMchangescroll=scrolls.Peek().scroll;
                        scrolls.Dequeue();
                    }else if(scrolls.Peek().type.Equals(9)){//정지
                        STOPED=true;
                        BPMchangeTime=scrolls.Peek().time;
                        BPMchangescroll=scrolls.Peek().scroll;
                        if((scrolls.Peek().time+scrolls.Peek().value)*1000*10000<=elpasedTick){
                            STOPED=false;
                            scrolls.Dequeue();
                        }
                    }
                }
            }
        }
        Debug.Log("끝");
    }
    void CheckMissedNote(int noteNum,long time){
        if(Notes[noteNum].Count>judgeProgress[noteNum]){
            if((Notes[noteNum][judgeProgress[noteNum]].Time*1000+dataManager.judgeTimings[4])*10000<time){
                Note note = Notes[noteNum][judgeProgress[noteNum]];
                if(note.Type==1){
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
    bool IsGameGoing(){
        if(Notes[0].Count<=0 & Notes[1].Count<=0 & Notes[2].Count<=0 & Notes[3].Count<=0 & Notes[4].Count<=0 & Notes[5].Count<=0 & Notes[6].Count<=0 & Notes[8].Count<=0 & Notes[9].Count<=0 & BGMs.Count<=0){
            return false;
        }else{
            return true;
        }
    }
    void Judge(int noteNum,long time){
        if(UnityRawInput.RawInput.IsKeyDown(keybind[noteNum-1])){
            if(Notes[noteNum-1].Count>judgeProgress[noteNum-1]){
                if(Notes[noteNum-1][judgeProgress[noteNum-1]].Type>=2){
                    if(Notes[noteNum-1].Count>judgeProgress[noteNum-1]){
                        Note note = Notes[noteNum-1][judgeProgress[noteNum-1]];
                        if((note.Time*1000)*10000 >= time){
                            note.Type=3;
                            Notes[noteNum-1][judgeProgress[noteNum-1]] = note;
                        }else{
                            sndPerKey[noteNum-1] = note.snd;//PG
                            note.proced = true;
                            Notes[noteNum-1][judgeProgress[noteNum-1]] = note;
                            judgeProgress[noteNum-1]+=1;
                        }
                    }
                }else{
                    if(KeyActived[noteNum-1]){
                        Note note = Notes[noteNum-1][judgeProgress[noteNum-1]];
                        if((note.Time*1000+dataManager.judgeTimings[4])*10000 >= time && (note.Time*1000-dataManager.judgeTimings[4])*10000 <= time){
                            sndPerKey[noteNum-1] = note.snd;//PG
                            note.proced = true;
                            Notes[noteNum-1][judgeProgress[noteNum-1]] = note;
                            judgeProgress[noteNum-1]+=1;
                        }
                        system.playSound(WAV[sndPerKey[noteNum-1]], channelGroup, false, out channel[sndPerKey[noteNum-1]]);
                    }
                }
            }
            KeyActived[noteNum-1]=false;
        }else{
            KeyActived[noteNum-1]=true;
        }
    }
}
