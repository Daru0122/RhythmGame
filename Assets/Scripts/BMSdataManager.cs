using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;


public class BMSdataManager : MonoBehaviour
{
    public static System.Diagnostics.Stopwatch Time = new System.Diagnostics.Stopwatch();//초시계
    public static float playAreaX = 70;
    //플레이중 저장되는 데이터들
    public int playScore;
    public int playCombo;
    public int[] judgeCount = new int[5];
    //------------
    private BGnotes currentBGM;
    private Queue<BGnotes> BGMs = new Queue<BGnotes>();
    public bool STOPED;
    public float BPMchangeTime;
    public float BPMchangescroll;
    public static float[] judgeTimings = new float[5]{
        40,
        80,
        120,
        140,
        150
    };
    public static KeyCode[] keyBinds = new KeyCode[9];//키바인딩
    public List<Queue<GameObject>> Notes = new List<Queue<GameObject>>();
    public bool[] LNactive = new bool[20];
    public float[] LNtime = new float[20];
    //생성된 노트에서 이미지와 위치를 불러오는 용도
    public static Dictionary<int, int> noteSprite = new Dictionary<int, int>();
    public static Dictionary<int, float> noteLoc = new Dictionary<int, float>();
    //----------------------------------------------------------------------------------
    public static BMSdataManager dManagerScript;
    public float loadDone;
    public GameObject barObj;
    public GameObject noteObj;
    public GameObject videoObj;
    //---------노트(배경음, 변속, bga, stop 등등 이벤트들도 모두 "노트"로서 처리함)------
    //16진수, 36진수 디코더
    //로딩에 필요한 변수들
    private int noteType;
    private string noteData;
    private char[] seps;
    private int currentMeasure;
    //저장된 노트
    public List<Note> notes = new List<Note>();
    //구역 크기는 데이터중 유일하게(아마도) 구역마다 처리되므로 따로처리
    public List<float> scalePerMeasure = new List<float>();
    //------------------------------------
    public static Dictionary<string, string> BGA = new Dictionary<string, string>();//#BMPxx a 저장
    public static Dictionary<string, string> BPMs = new Dictionary<string, string>();//#BPMxx a 저장
    public static FMOD.Sound[] WAV = new FMOD.Sound[1296];//#WAVxx 파일 저장
    public static string[] Stops = new string[1296];
    public string fileLoc;//파일 위치 지정
    public static float totalScroll;
    //-------FMOD설정---------
    private FMOD.Sound snd;
    FMOD.System system;
    public static FMOD.Channel channel;
    public static FMOD.ChannelGroup channelGroup;
    //------------------------
    //HEADER FILED 선언
    int PLAYER;//1:SP,2:Couple,3:DP
    string GENRE;
    string TITLE;
    string ARTIST;
    public float firstBPM;
    public float BPM;
    int PLAYLEVEL;
    int RANK;//판정 난이도

    string SUBTITLE;
    string SUBARTIST;
    string STAGEFILE;
    string BANNER;

    int DIFFICULTY;
    float TOTAL;
    int LNTYPE;
    string LNOBJ;
    //--------------------------------------------
    string input;
    string[] splitText;
    int output;//함수 호출시 출력

    void Start(){
        fileLoc = FileNameInput.value;
        //키바인딩-----------------------
        keyBinds[0] = KeyCode.S;
        keyBinds[1] = KeyCode.D;
        keyBinds[2] = KeyCode.F;
        keyBinds[3] = KeyCode.Space;
        keyBinds[4] = KeyCode.J;
        keyBinds[5] = KeyCode.LeftShift;
        keyBinds[7] = KeyCode.K;
        keyBinds[8] = KeyCode.L;
        //---------------------------------------------------
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

        dManagerScript = gameObject.GetComponent<BMSdataManager>();
        //일반노트 스프라이트
        noteSprite.Add(1, 1);
        noteSprite.Add(2, 2);
        noteSprite.Add(3, 1);
        noteSprite.Add(4, 2);
        noteSprite.Add(5, 1);
        noteSprite.Add(6, 0);
        noteSprite.Add(8, 2);
        noteSprite.Add(9, 1);
        //롱노트 시작점 스프라이트
        noteSprite.Add(41, 4);
        noteSprite.Add(42, 5);
        noteSprite.Add(43, 4);
        noteSprite.Add(44, 5);
        noteSprite.Add(45, 4);
        noteSprite.Add(46, 3);
        noteSprite.Add(48, 5);
        noteSprite.Add(49, 4);
        //롱노트 종점 스프라이트
        noteSprite.Add(141, 7);
        noteSprite.Add(142, 8);
        noteSprite.Add(143, 7);
        noteSprite.Add(144, 8);
        noteSprite.Add(145, 7);
        noteSprite.Add(146, 6);
        noteSprite.Add(148, 8);
        noteSprite.Add(149, 7);
        //롱노트 중앙 스프라이트
        noteSprite.Add(241, 10);
        noteSprite.Add(242, 11);
        noteSprite.Add(243, 10);
        noteSprite.Add(244, 11);
        noteSprite.Add(245, 10);
        noteSprite.Add(246, 9);
        noteSprite.Add(248, 11);
        noteSprite.Add(249, 10);
//-----------위치지정------
        noteLoc.Add(1,92);
        noteLoc.Add(2,146);
        noteLoc.Add(3,188);
        noteLoc.Add(4,242);
        noteLoc.Add(5,284);
        noteLoc.Add(6,0);
        noteLoc.Add(8,338);
        noteLoc.Add(9,380);



//로딩 시작---------------------------------------------------
        BMSload(fileLoc);
        //플레이
        Judgement judge = GameObject.Find("InputManager").GetComponent<Judgement>();
        StartCoroutine(judge.getFirstKeysound());
        StartCoroutine(playBGM());
        BPM = firstBPM;
        Time.Start();
    }
    void Update(){
        if(STOPED){
            totalScroll=BPMchangescroll;
        }else{
            totalScroll = ((float)Time.Elapsed.TotalMilliseconds-BPMchangeTime*1000)*0.6f*BPM/174545+BPMchangescroll;
        }
    }
    float convertFromThirysix(string value){//두자리수만 가능
        string[] convertToThirysix = new string[36]{
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
        };
        output = (36*(Array.IndexOf(convertToThirysix, value.Substring(0,1))) + (Array.IndexOf(convertToThirysix, value.Substring(1,1))));
        return output;
    }
    float convertFromSixteen(string value){//두자리수만 가능
        string[] convertToThirysix = new string[16]{
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
        };
        output = (16*(Array.IndexOf(convertToThirysix, value.Substring(0,1))) + (Array.IndexOf(convertToThirysix, value.Substring(1,1))));
        return output;
    }
    private void BMSload(string fileLocation){//bms파일을 파싱
        system = FMODUnity.RuntimeManager.CoreSystem;
        Debug.Log("로딩 시작");
        system.createChannelGroup (null, out channelGroup);
        channel.setChannelGroup(channelGroup);

        seps = new char[] {' ', ':'};//데이터 Split 기준 정함
        StreamReader reader = new StreamReader(new FileStream(fileLocation, FileMode.Open), Encoding.GetEncoding("shift_jis"));
        while (!reader.EndOfStream){
            input = reader.ReadLine();
            if(input.Length >= 4 && !input.StartsWith("*")){
                splitText = input.Split(seps);
                //HEADER FIELD
                if(splitText[0].Equals("#PLAYER")){
                    PLAYER = int.Parse(input.Substring(input.IndexOf(" ")+1));
                }else if(splitText[0].Equals("#GENRE")){
                    GENRE = input.Substring(input.IndexOf(" ")+1);
                }else if(splitText[0].Equals("#TITLE")){
                    TITLE = input.Substring(input.IndexOf(" ")+1);
                }else if(splitText[0].Equals("#ARTIST")){
                    ARTIST = input.Substring(input.IndexOf(" ")+1);
                }else if(splitText[0].Equals("#BPM")){
                    BPM = float.Parse(input.Substring(input.IndexOf(" ")+1));
                    firstBPM = BPM;
                }else if(splitText[0].Equals("#PLAYLEVEL")){
                    PLAYLEVEL = int.Parse(input.Substring(input.IndexOf(" ")+1));
                }else if(splitText[0].Equals("#RANK")){
                    RANK = int.Parse(input.Substring(input.IndexOf(" ")+1));
                }//expand
                else if(splitText[0].Equals("#SUBTITLE")){
                    SUBTITLE = input.Substring(input.IndexOf(" ")+1);
                }else if(splitText[0].Equals("#SUBARTIST")){
                    SUBARTIST = input.Substring(input.IndexOf(" ")+1);
                }else if(splitText[0].Equals("#STAGEFILE")){
                    STAGEFILE = input.Substring(input.IndexOf(" ")+1);
                }else if(splitText[0].Equals("#BANNER")){
                    BANNER = input.Substring(input.IndexOf(" ")+1);
                }else if(splitText[0].Equals("#DIFFICULTY")){
                    DIFFICULTY = int.Parse(input.Substring(input.IndexOf(" ")+1));
                }else if(splitText[0].Equals("#TOTAL")){
                    TOTAL = float.Parse(input.Substring(input.IndexOf(" ")+1));
                }else if(splitText[0].Equals("#PLAYER")){
                    PLAYER = int.Parse(input.Substring(input.IndexOf(" ")+1));
                }else if(splitText[0].Equals("#LNTYPE")){//https://nvyu.net/rdm/rby_ex.php
                    LNTYPE = int.Parse(input.Substring(input.IndexOf(" ")+1));
                }else if(splitText[0].Equals("#LNOBJ")){//LNOBG형 롱노트 발견
                    LNOBJ = input.Substring(input.IndexOf(" ")+1);
                    LNTYPE = 3;//LNTYPE과 LNOBJ는 함꼐 쓰일 수 없음
                }else if(input.Substring(1,4) == "STOP"){//STOP 발견
                    convertFromThirysix(input.Substring(5,2));
                    Stops[output] = splitText[1];
                }//-----------------------------WAV----------------------------
                else if(input.Substring(1,3) == "WAV"){
                    if(File.Exists(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".ogg")){//ogg파일 존재여부
                        system.createSound(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".ogg", FMOD.MODE.CREATESAMPLE | FMOD.MODE.ACCURATETIME, out snd);
                    }else if(File.Exists(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".wav")){//wav파일 존재여부
                        system.createSound(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".wav", FMOD.MODE.CREATESAMPLE | FMOD.MODE.ACCURATETIME, out snd);
                    }else if(File.Exists(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".mp3")){//mp3파일 존재여부
                        system.createSound(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".mp3", FMOD.MODE.CREATESAMPLE | FMOD.MODE.ACCURATETIME, out snd);
                    }else{
                        Debug.Log("오디오파일 발견 불가"+input.Substring(7));
                    }
                    convertFromThirysix(input.Substring(4,2));
                    WAV[output] = snd;
                }//----------------------------BGA---------------------
                else if(input.Substring(1,3) == "BMP"){
                    if(File.Exists(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".mp4")){//mp4파일 존재여부
                        BGA.Add(input.Substring(4,2), Path.GetFileNameWithoutExtension(input.Substring(7))+".mp4");
                    }else if(File.Exists(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".wmv")){//wmv파일 존재여부
                        BGA.Add(input.Substring(4,2), Path.GetFileNameWithoutExtension(input.Substring(7))+".wmv");
                    }else if(File.Exists(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".mpeg")){//mpeg파일 존재여부
                        BGA.Add(input.Substring(4,2), Path.GetFileNameWithoutExtension(input.Substring(7))+".mpeg");
                    }else{
                        Debug.Log("bga파일 발견 불가"+input.Substring(7));
                    }
                }//------------------------BPM---------------
                else if(input.Substring(1,3) == "BPM"){
                    BPMs.Add(input.Substring(4,2), splitText[1]);
                }//-----------------------MAIN DATA FIELD----------------------
                if(input.Substring(0,1) == "#" && input.Substring(6,1) == ":"){
                    noteType = int.Parse(input.Split(':')[0].Substring(4));
                    noteData = input.Split(':')[1];
                    currentMeasure = int.Parse(input.Split(':')[0].Substring(1,3));
                    if(noteType.Equals(1)){//배경음-------------------------------------------------------------------------------
                        for(int i = 0; noteData.Length > i*2; i++){//2글자씩 읽음
                            if(noteData.Substring(i*2,2) != "00"){//값이 0이 아닐때
                                convertFromThirysix(noteData.Substring(i*2,2));
                                notes.Add(new Note{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=1,noteValues=output.ToString()});
                            }
                        }
                    }else if(noteType.Equals(2)){//마디별 크기---------------------------------------------------------------------
                        while(scalePerMeasure.Count < int.Parse(input.Substring(1,3))){
                            scalePerMeasure.Add(1);//빈마디는 1로 채워넣고
                        }
                        scalePerMeasure.Add(float.Parse(noteData));//현제 마디에 해당하는 값 넣기
                    }else if(noteType.Equals(3)){//BPM변속(구식)-------------------------------------------------------------------
                        for(int i = 0; noteData.Length > i*2; i++){
                            if(noteData.Substring(i*2,2) != "00"){
                                convertFromSixteen(noteData.Substring(i*2,2));//옛날 BPM변속은 16진수(16*16-1=255)
                                notes.Add(new Note{noteBeat=(currentMeasure+(2*i/(float)noteData.Length)), noteType=8,noteValues=output.ToString()});//모두 신식 변속으로 변경
                            }
                        }
                    }else if(noteType.Equals(4)){//BGA------------------------------------------------------------
                        for(int i = 0; noteData.Length > i*2; i++){
                            if(noteData.Substring(i*2,2) != "00"){
                                notes.Add(new Note{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=4,noteValues = BGA[noteData.Substring(i*2,2)]});
                            }
                        }
                    }else if(noteType.Equals(5)){//POOR BGA------------------------------------------------------------
                        for(int i = 0; noteData.Length > i*2; i++){
                            if(noteData.Substring(i*2,2) != "00"){
                                convertFromThirysix(noteData.Substring(i*2,2));
                                notes.Add(new Note{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=5,noteValues=output.ToString()});
                            }
                        }//-------ExtraObject(06)과 BGA-Layer(07)은 지원하지 않습니다-------
                    }else if(noteType.Equals(8)){//BPM변속(신식)------------------------------------------------------------------
                        for(int i = 0; noteData.Length > i*2; i++){
                            if(noteData.Substring(i*2,2) != "00"){
                                notes.Add(new Note{noteBeat=(currentMeasure+(2*i/(float)noteData.Length)), noteType=8,noteValues=BPMs[noteData.Substring(i*2,2)]});
                            }
                        }
                    }else if(noteType.Equals(9)){//STOP------------------------------------------------------------------
                        for(int i = 0; noteData.Length > i*2; i++){
                            if(noteData.Substring(i*2,2) != "00"){
                                convertFromThirysix(noteData.Substring(i*2,2));
                                notes.Add(new Note{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=9,noteValues=Stops[output]});
                            }
                        }
                    }else if (noteType > 10){//노트들--------------------------------------------------------------------------------
                        if(LNTYPE.Equals(2)){
                            string currentValue = "00";
                            for(int i = 0; noteData.Length > i*2;){
                                if(noteData.Substring(i*2,2) != "00"){
                                    currentValue = noteData.Substring(i*2,2);
                                    convertFromThirysix(noteData.Substring(i*2,2));
                                    notes.Add(new Note{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=noteType,noteValues=output.ToString()});
                                    i++;
                                    if(currentValue.Equals(noteData.Substring(i*2,2))){
                                        notes.RemoveAt(notes.Count-1);
                                        notes.Add(new Note{noteBeat=currentMeasure+(2*(i-1)/(float)noteData.Length), noteType=noteType+40,noteValues=output.ToString()});
                                        i++;
                                        while(currentValue.Equals(noteData.Substring(i*2,2))){
                                            i++;
                                        }
                                        notes.Add(new Note{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=noteType+40,noteValues=output.ToString()});
                                        i++;
                                    }
                                }
                            }
                        }else if(LNTYPE.Equals(3)){
                            int preMeasure = 0;
                            for(int i = 0; noteData.Length > i*2; i++){
                                if(noteData.Substring(i*2,2) != "00"){
                                    if(noteData.Substring(i*2,2).Equals(LNOBJ)){
                                        int loc= notes.FindLastIndex(x => x.noteType==noteType);
                                        notes[loc] = new Note{noteBeat=notes[loc].noteBeat, noteType=notes[loc].noteType+40, noteValues=notes[loc].noteValues};
                                        notes.Add(new Note{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=noteType+40,noteValues=output.ToString()});
                                    }else{
                                        preMeasure = i;
                                        convertFromThirysix(noteData.Substring(i*2,2));
                                        notes.Add(new Note{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=noteType,noteValues=output.ToString()});
                                    }
                                }
                            }
                        }else{
                            for(int i = 0; noteData.Length > i*2; i++){
                                if(noteData.Substring(i*2,2) != "00"){
                                    convertFromThirysix(noteData.Substring(i*2,2));
                                    notes.Add(new Note{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=noteType,noteValues=output.ToString()});
                                }
                            }
                        }
                    }
                }
            }
        }
        reader.Close();
        notes.Sort((x,y) => x.noteBeat.CompareTo(y.noteBeat));
        while(scalePerMeasure.Count < notes[notes.Count-1].noteBeat+1){
            scalePerMeasure.Add(1);
        }
        float noteBeat = 0;
        float preNoteBeat = 0;
        float noteTime = 0f;
        float noteScroll = 0f;
        int currentbeat = 0;
        for(int i = 0; notes.Count > i;){
            while(notes.Count > i && currentbeat+1 > notes[i].noteBeat){
                noteType = notes[i].noteType;
                noteBeat = notes[i].noteBeat;
                noteTime += (noteBeat-preNoteBeat)*240/BPM*scalePerMeasure[currentbeat];
                noteScroll += (noteBeat-preNoteBeat)*144000/174545*scalePerMeasure[currentbeat];
                if(noteType.Equals(1)){
                    BGMs.Enqueue(new BGnotes{noteTime=noteTime, noteSnd=WAV[int.Parse(notes[i].noteValues)]});
                }else if(noteType>10){//노트라면
                    GameObject noteMade = Instantiate(noteObj);
                    Notescript nScript = noteMade.GetComponent<Notescript>();
                    nScript.time=noteTime;
                    nScript.snd=int.Parse(notes[i].noteValues);
                    nScript.scroll = noteScroll;
                    nScript.noteType = noteType;
                    if(noteType>50){
                        Notes[noteType-51].Enqueue(noteMade);
                        if(LNactive[noteType-51]){//롱놑 끝부분이면
                            LNactive[noteType-51] = false;
                            nScript.EXtype = 2;
                            nScript.snd=0;
                            GameObject LNmade = Instantiate(noteObj);
                            Notescript LNscript = LNmade.GetComponent<Notescript>();
                            LNscript.EXtype = 3;
                            LNscript.scroll = LNtime[noteType-51];
                            LNscript.snd=0;
                            LNscript.noteType = noteType;
                            LNscript.LNtime = noteScroll-LNtime[noteType-51];//계산한 롱노트 시간 보냄
                            LNscript.lnEndnote = nScript.GetComponent<Notescript>();
                        }else{//아니면
                            LNactive[noteType-51] = true;
                            nScript.EXtype = 1;
                            LNtime[noteType-51] = noteScroll;//롱노트 시작부분 체크(거리계산 위해)
                        }
                    }else{
                        nScript.EXtype = 0;
                        Notes[noteType-11].Enqueue(noteMade);
                    }
                }else if(noteType.Equals(8)){//변속이라면
                    BPM = float.Parse(notes[i].noteValues);
                    GameObject notemade = Instantiate(noteObj);
                    Notescript nscript = notemade.GetComponent<Notescript>();
                    nscript.noteType = 8;
                    nscript.time = noteTime;
                    nscript.scroll = noteScroll;
                    nscript.LNtime = BPM;
                }else if(noteType.Equals(4)){//BGA라면
                    VideoManager vManagerScript = videoObj.GetComponent<VideoManager>();
                    vManagerScript.time = noteTime;
                    vManagerScript.fileLoc = Path.GetDirectoryName(fileLocation)+'/'+notes[i].noteValues;
                    StartCoroutine(vManagerScript.playVideo());
                }else if(noteType.Equals(9)){//STOP이라면
                    GameObject notemade = Instantiate(noteObj);
                    Notescript nscript = notemade.GetComponent<Notescript>();
                    nscript.noteType = 9;
                    nscript.time = noteTime;
                    nscript.scroll = noteScroll;
                    nscript.LNtime = (float.Parse(notes[i].noteValues))/192*240/BPM*scalePerMeasure[currentbeat];
                    noteTime += (float.Parse(notes[i].noteValues))/192*240/BPM*scalePerMeasure[currentbeat];
                }
                preNoteBeat = noteBeat;
                i++;
            }
            if(notes.Count > i){
                currentbeat++;
                noteType = notes[i].noteType;
                noteBeat = notes[i].noteBeat;
                noteTime += (currentbeat-preNoteBeat)*240/BPM*scalePerMeasure[currentbeat-1];
                noteScroll += (currentbeat-preNoteBeat)*144000*scalePerMeasure[currentbeat-1]/174545;
                GameObject bar = Instantiate(barObj);
                BarScript barscript = bar.GetComponent<BarScript>();
                barscript.scroll = noteScroll;
                StartCoroutine(barscript.barGO());
                while(currentbeat < Mathf.FloorToInt(noteBeat)){
                    noteTime += 240/BPM*scalePerMeasure[currentbeat];
                    noteScroll += 144000*scalePerMeasure[currentbeat]/174545;
                    currentbeat++;
                    bar = Instantiate(barObj);
                    barscript = bar.GetComponent<BarScript>();
                    barscript.scroll = noteScroll;
                    StartCoroutine(barscript.barGO());
                }
                preNoteBeat = currentbeat;
            }
        }
        loadDone = 1;
    }
    IEnumerator playBGM(){
        currentBGM = BGMs.Peek();
        while (BGMs.Count > 0){
            yield return new WaitForFixedUpdate();
            if(currentBGM.noteTime*1000<=Time.Elapsed.TotalMilliseconds){
                currentBGM = BGMs.Dequeue();
                FMODUnity.RuntimeManager.CoreSystem.playSound(currentBGM.noteSnd, BMSdataManager.channelGroup, false, out BMSdataManager.channel);
                if(BGMs.Count <= 0){
                    break;
                }
                currentBGM = BGMs.Peek();
            }
        }
    }
}
public struct Note{
    public float noteBeat;
    public string noteValues;
    public int noteType;
}
public struct BGnotes{
    public float noteTime;
    public FMOD.Sound noteSnd;
}
