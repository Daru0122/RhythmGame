using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class dataManager : MonoBehaviour
{
    public List<preNote> notes = new List<preNote>();
    private StreamReader reader;
    FMOD.RESULT result;
    public static float playAreaX = 70;
    //------------
    private BGnotes currentBGM;
    public static float[] judgeTimings = new float[5]{
        40,
        80,
        120,
        140,
        150
    };
    public static KeyCode[] keyBinds = new KeyCode[9];//키바인딩
    public bool[] LNactive = new bool[20];
    public float[] LNtime = new float[20];
    //생성된 노트에서 이미지와 위치를 불러오는 용도
    public static Dictionary<int, int> noteSprite = new Dictionary<int, int>();
    public static Dictionary<int, float> noteLoc = new Dictionary<int, float>();
    //----------------------------------------------------------------------------------
    public static float loadDone;
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

    //구역 크기는 데이터중 유일하게(아마도) 구역마다 처리되므로 따로처리
    private Dictionary<int, float> measureScale = new Dictionary<int, float>();
    public List<float> scalePerMeasure = new List<float>();
    //------------------------------------
    public static Dictionary<string, string> BGA = new Dictionary<string, string>();//#BMPxx a 저장
    public static Dictionary<string, string> BPMs = new Dictionary<string, string>();//#BPMxx a 저장
    public static string[] Stops = new string[1296];
    public string fileLoc;//파일 위치 지정
    //-------FMOD설정---------
    private FMOD.Sound snd;
    //------------------------
    //HEADER FILED 선언
    int PLAYER;//1:SP,2:Couple,3:DP
    string GENRE;
    string TITLE;
    string ARTIST;
    public float firstBPM;
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
        fileLoc = Menu.value;
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

        //일반노트 스프라이트
        noteSprite.Add(0, 1);
        noteSprite.Add(1, 2);
        noteSprite.Add(2, 1);
        noteSprite.Add(3, 2);
        noteSprite.Add(4, 1);
        noteSprite.Add(5, 0);
        noteSprite.Add(7, 2);
        noteSprite.Add(8, 1);
        //롱노트 시작점 스프라이트
        noteSprite.Add(40, 4);
        noteSprite.Add(41, 5);
        noteSprite.Add(42, 4);
        noteSprite.Add(43, 5);
        noteSprite.Add(44, 4);
        noteSprite.Add(45, 3);
        noteSprite.Add(47, 5);
        noteSprite.Add(48, 4);
        //롱노트 종점 스프라이트
        noteSprite.Add(140, 7);
        noteSprite.Add(141, 8);
        noteSprite.Add(142, 7);
        noteSprite.Add(143, 8);
        noteSprite.Add(144, 7);
        noteSprite.Add(145, 6);
        noteSprite.Add(147, 8);
        noteSprite.Add(148, 7);
        //롱노트 중앙 스프라이트
        noteSprite.Add(240, 10);
        noteSprite.Add(241, 11);
        noteSprite.Add(242, 10);
        noteSprite.Add(243, 11);
        noteSprite.Add(244, 10);
        noteSprite.Add(245, 9);
        noteSprite.Add(247, 11);
        noteSprite.Add(248, 10);
//-----------위치지정------
        noteLoc.Add(0,92);
        noteLoc.Add(1,146);
        noteLoc.Add(2,188);
        noteLoc.Add(3,242);
        noteLoc.Add(4,284);
        noteLoc.Add(5,0);
        noteLoc.Add(7,338);
        noteLoc.Add(8,380);



//로딩 시작---------------------------------------------------
        BMSload(fileLoc);
        //플레이
        Player.playScript.BPM = firstBPM;
        Player.playScript.GetFirstKeysnd();
        Player.playScript.BPMchangeTime=0;
        Player.playScript.BPMchangescroll=0;
        Player.playScript.player.Start();
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
    private void IF(int Random,string fileLocation,int value){
        if(Random == value){
            while(!reader.EndOfStream){
                ReadNextLine();
                if(input.Equals("#ENDIF")||splitText[0].Equals("#ELSEIF")||input.Equals("#ELSE")){ 
                    break;
                }
                FindLine(fileLocation);
            }
        }else{
            while(!reader.EndOfStream){
                ReadNextLine();
                if(input.Equals("#ENDIF")||splitText[0].Equals("#ELSEIF")||input.Equals("#ELSE")){ 
                    break;
                }
            }
        }
    }
    private void ReadNextLine(){//유효한 다음줄을 input에 불러냄
        while((!reader.EndOfStream)){
            input = reader.ReadLine();
            if(input!=null){
                if(input.Contains("#")){
                    input = input.Substring(input.IndexOf("#"));
                    splitText = input.Split(seps);
                    break;
                }
            }
        }
    }
    private void FindLine(string fileLocation){
        if(input!=null&&input.Length>=4&&input.StartsWith("#")){
            if(splitText[0].Equals("#RANDOM")){
                int Random = UnityEngine.Random.Range(1,int.Parse(splitText[1])+1);
                List<int> exepted = new List<int>();
                ReadNextLine();
                while(!input.Equals("#ENDRANDOM")){
                    if(reader.EndOfStream){
                        break;
                    }
                    if(splitText[0].Equals("#IF")){
                        exepted.Clear();
                        exepted.Add(int.Parse(splitText[1]));
                        IF(Random,fileLocation,int.Parse(splitText[1]));
                    }else if(splitText[0].Equals("#ELSEIF")){
                        if(!exepted.Contains(Random)){
                            IF(Random,fileLocation,int.Parse(splitText[1]));
                        }
                        exepted.Add(int.Parse(splitText[1]));
                    }else if(splitText[0].Equals("#ELSE")){
                        if(!exepted.Contains(Random)){
                            ReadNextLine();
                            while(!input.Equals("#ENDIF")&&!splitText[0].Equals("#ELSEIF")&&!input.Equals("#ELSE")){
                                ReadNextLine();
                                if(!input.Equals("#ENDIF")&&!splitText[0].Equals("#ELSEIF")&&!input.Equals("#ELSE")){
                                    FindLine(fileLocation);
                                }
                            }
                        }else{
                            while(!input.Equals("#ENDIF")&&!splitText[0].Equals("#ELSEIF")&&!input.Equals("#ELSE")){
                                ReadNextLine();
                            }
                        }
                    }else if(input.Equals("#ENDIF")){
                    }else if(splitText[0].Equals("#SETRANDOM")){
                        Random=int.Parse(splitText[1]);
                    }else if(input.Equals("#ENDRANDOM")){
                    }else{
                        FindLine(fileLocation);
                    }
                    ReadNextLine();
                }
            }
            //HEADER FIELD
            else if(splitText[0].Equals("#PLAYER")){
                PLAYER = int.Parse(input.Substring(input.IndexOf(" ")+1));
            }else if(splitText[0].Equals("#GENRE")){
                GENRE = input.Substring(input.IndexOf(" ")+1);
            }else if(splitText[0].Equals("#TITLE")){
                TITLE = input.Substring(input.IndexOf(" ")+1);
            }else if(splitText[0].Equals("#ARTIST")){
                ARTIST = input.Substring(input.IndexOf(" ")+1);
            }else if(splitText[0].Equals("#BPM")){
            Player.playScript.BPM = float.Parse(input.Substring(input.IndexOf(" ")+1));
            firstBPM = Player.playScript.BPM;
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
                    Player.playScript.system.createSound(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".ogg", FMOD.MODE.CREATESAMPLE | FMOD.MODE.ACCURATETIME, out snd);
                }else if(File.Exists(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".wav")){//wav파일 존재여부
                    Player.playScript.system.createSound(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".wav", FMOD.MODE.CREATESAMPLE | FMOD.MODE.ACCURATETIME, out snd);
                }else if(File.Exists(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".mp3")){//mp3파일 존재여부
                    Player.playScript.system.createSound(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".mp3", FMOD.MODE.CREATESAMPLE | FMOD.MODE.ACCURATETIME, out snd);
                }
                convertFromThirysix(input.Substring(4,2));
                Player.playScript.WAV[output] = snd;
            }//----------------------------BGA---------------------
            else if(input.Substring(1,3) == "BMP"){
                if(File.Exists(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".mp4")){//mp4파일 존재여부
                    BGA.Add(input.Substring(4,2), Path.GetFileNameWithoutExtension(input.Substring(7))+".mp4");
                }else if(File.Exists(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".wmv")){//wmv파일 존재여부
                    BGA.Add(input.Substring(4,2), Path.GetFileNameWithoutExtension(input.Substring(7))+".wmv");
                }else if(File.Exists(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".mpeg")){//mpeg파일 존재여부
                    BGA.Add(input.Substring(4,2), Path.GetFileNameWithoutExtension(input.Substring(7))+".mpeg");
                }else if(File.Exists(Path.GetDirectoryName(fileLocation)+'/'+Path.GetFileNameWithoutExtension(input.Substring(7))+".mpg")){//mpg파일 존재여부
                    BGA.Add(input.Substring(4,2), Path.GetFileNameWithoutExtension(input.Substring(7))+".mpg");
                }
            }//------------------------BPM---------------
            else if(input.Substring(1,3) == "BPM"){
                BPMs.Add(input.Substring(4,2), splitText[1]);
            }//-----------------------MAIN DATA FIELD----------------------
            else if(input.Substring(0,1) == "#" && input.Substring(6,1) == ":"){
                noteType = int.Parse(input.Split(':')[0].Substring(4));
                noteData = input.Split(':')[1];
                currentMeasure = int.Parse(input.Split(':')[0].Substring(1,3));
                if(noteType.Equals(1)){//배경음-------------------------------------------------------------------------------
                    for(int i = 0; noteData.Length > i*2; i++){//2글자씩 읽음
                        if(noteData.Substring(i*2,2) != "00"){//값이 0이 아닐때
                            convertFromThirysix(noteData.Substring(i*2,2));
                            notes.Add(new preNote{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=1,noteValues=output.ToString()});
                        }
                    }
                }else if(noteType.Equals(2)){//마디별 크기---------------------------------------------------------------------
                    measureScale.Add(currentMeasure,float.Parse(noteData));
                }else if(noteType.Equals(3)){//BPM변속(구식)-------------------------------------------------------------------
                    for(int i = 0; noteData.Length > i*2; i++){
                        if(noteData.Substring(i*2,2) != "00"){
                            convertFromSixteen(noteData.Substring(i*2,2));//옛날 BPM변속은 16진수(16*16-1=255)
                            notes.Add(new preNote{noteBeat=(currentMeasure+(2*i/(float)noteData.Length)), noteType=8,noteValues=output.ToString()});//모두 신식 변속으로 변경
                        }
                    }
                }else if(noteType.Equals(4)){//BGA------------------------------------------------------------
                    for(int i = 0; noteData.Length > i*2; i++){
                        if(noteData.Substring(i*2,2) != "00"){
                            notes.Add(new preNote{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=4,noteValues = BGA[noteData.Substring(i*2,2)]});
                        }
                    }
                }else if(noteType.Equals(5)){//POOR BGA------------------------------------------------------------
                    for(int i = 0; noteData.Length > i*2; i++){
                        if(noteData.Substring(i*2,2) != "00"){
                            convertFromThirysix(noteData.Substring(i*2,2));
                            notes.Add(new preNote{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=5,noteValues=output.ToString()});
                        }
                    }//-------ExtraObject(06)과 BGA-Layer(07)은 지원하지 않습니다-------
                }else if(noteType.Equals(8)){//BPM변속(신식)------------------------------------------------------------------
                    for(int i = 0; noteData.Length > i*2; i++){
                        if(noteData.Substring(i*2,2) != "00"){
                            notes.Add(new preNote{noteBeat=(currentMeasure+(2*i/(float)noteData.Length)), noteType=8,noteValues=BPMs[noteData.Substring(i*2,2)]});
                        }
                    }
                }else if(noteType.Equals(9)){//STOP------------------------------------------------------------------
                    for(int i = 0; noteData.Length > i*2; i++){
                        if(noteData.Substring(i*2,2) != "00"){
                            convertFromThirysix(noteData.Substring(i*2,2));
                            notes.Add(new preNote{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=9,noteValues=Stops[output]});
                        }
                    }
                }else if (noteType > 10){//노트들--------------------------------------------------------------------------------
                    if(LNTYPE.Equals(2)){
                        string currentValue = "00";
                        for(int i = 0; noteData.Length > i*2;){
                            if(noteData.Substring(i*2,2) != "00"){
                                currentValue = noteData.Substring(i*2,2);
                                convertFromThirysix(noteData.Substring(i*2,2));
                                notes.Add(new preNote{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=noteType,noteValues=output.ToString()});
                                i++;
                                if(currentValue.Equals(noteData.Substring(i*2,2))){
                                    notes.RemoveAt(notes.Count-1);
                                    notes.Add(new preNote{noteBeat=currentMeasure+(2*(i-1)/(float)noteData.Length), noteType=noteType+40,noteValues=output.ToString()});
                                    i++;
                                    while(currentValue.Equals(noteData.Substring(i*2,2))){
                                        i++;
                                    }
                                    notes.Add(new preNote{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=noteType+40,noteValues=output.ToString()});
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
                                    notes[loc] = new preNote{noteBeat=notes[loc].noteBeat, noteType=notes[loc].noteType+40, noteValues=notes[loc].noteValues};
                                    notes.Add(new preNote{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=noteType+40,noteValues=output.ToString()});
                                }else{
                                    preMeasure = i;
                                    convertFromThirysix(noteData.Substring(i*2,2));
                                    notes.Add(new preNote{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=noteType,noteValues=output.ToString()});
                                }
                            }
                        }
                    }else{
                        for(int i = 0; noteData.Length > i*2; i++){
                            if(noteData.Substring(i*2,2) != "00"){
                                convertFromThirysix(noteData.Substring(i*2,2));
                                notes.Add(new preNote{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=noteType,noteValues=output.ToString()});
                            }
                        }
                    }
                }
            }
        }
    }
    private void BMSload(string fileLocation){//bms파일을 파싱
        seps = new char[] {' ', ':'};//데이터 Split 기준 정함
        reader = new StreamReader(new FileStream(fileLocation, FileMode.Open), Encoding.GetEncoding(932));
        while (!reader.EndOfStream){
            ReadNextLine();
            FindLine(fileLocation);
        }
        reader.Close();
        notes.Sort((x,y) => {
            int ret = x.noteBeat.CompareTo(y.noteBeat);
            return ret != 0 ? ret : x.noteType.CompareTo(y.noteType);});
        for(int i=0;i < notes[notes.Count-1].noteBeat+1;i++){
            if(measureScale.ContainsKey(i)){
                scalePerMeasure.Add(measureScale[i]);
            }else{
                scalePerMeasure.Add(1);
            }
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
                noteTime += (noteBeat-preNoteBeat)*240/Player.playScript.BPM*scalePerMeasure[currentbeat];
                noteScroll += (noteBeat-preNoteBeat)*144000/174545*scalePerMeasure[currentbeat];
                if(noteType.Equals(1)){//배경음
                    Player.playScript.BGMs.Enqueue(new BGnotes{noteTime=noteTime,noteSnd=int.Parse(notes[i].noteValues)});
                }else if(noteType>10&&noteType<50){//일반노트
                    GameObject noteMade = Instantiate(noteObj);
                    noteMade.transform.SetParent(GameObject.Find("PlayArea").transform);
                    Notescript noteSc=noteMade.GetComponent<Notescript>();
                    noteSc.noteLine = noteType-11;
                    noteSc.noteNumber = Player.playScript.Notes[noteType-11].Count;
                    Player.playScript.Notes[noteType-11].Add(new Note{note=noteMade,snd=int.Parse(notes[i].noteValues),Time=noteTime,scroll=noteScroll,Type=0,proced=false});
                }else if(noteType>50){//롱노트
                    if(LNactive[noteType-51]){//롱노트 끝부분
                        GameObject noteMade = Instantiate(noteObj);
                        noteMade.transform.SetParent(GameObject.Find("PlayArea").transform);
                        Notescript noteSc=noteMade.GetComponent<Notescript>();
                        noteSc.noteLine = noteType-51;
                        noteSc.noteNumber = Player.playScript.Notes[noteType-51].Count;
                        Player.playScript.Notes[noteType-51].Add(new Note{note=noteMade,snd=int.Parse(notes[i].noteValues),Time=noteTime,scroll=noteScroll,Type=2,proced=false});
                        noteMade = Instantiate(noteObj);
                        noteMade.transform.SetParent(GameObject.Find("PlayArea").transform);
                        noteSc=noteMade.GetComponent<Notescript>();
                        noteSc.noteLine = noteType-51;
                        noteSc.noteNumber = Player.playScript.Notes[noteType-51].Count-1;
                        noteSc.noteType=3;
                        LNactive[noteType-51]=false;
                    }else{//롱노트 시작부분
                        GameObject noteMade = Instantiate(noteObj);
                        noteMade.transform.SetParent(GameObject.Find("PlayArea").transform);
                        Notescript noteSc=noteMade.GetComponent<Notescript>();
                        noteSc.noteLine = noteType-51;
                        noteSc.noteNumber = Player.playScript.Notes[noteType-51].Count;
                        Player.playScript.Notes[noteType-51].Add(new Note{note=noteMade,snd=int.Parse(notes[i].noteValues),Time=noteTime,scroll=noteScroll,Type=1,proced=false});
                        LNactive[noteType-51]=true;
                    }
                }else if(noteType.Equals(8)){//변속이라면
                    Player.playScript.BPM = float.Parse(notes[i].noteValues);
                    Player.playScript.scrolls.Enqueue(new ScrollNote{type=8,time=noteTime,scroll=noteScroll,value=float.Parse(notes[i].noteValues)});
                }else if(noteType.Equals(4)){//BGA라면
                    VideoManager vManagerScript = videoObj.GetComponent<VideoManager>();
                    vManagerScript.time = noteTime;
                    vManagerScript.fileLoc = Path.GetDirectoryName(fileLocation)+'/'+notes[i].noteValues;
                    StartCoroutine(vManagerScript.playVideo());
                }else if(noteType.Equals(9)){//STOP이라면
                    Player.playScript.scrolls.Enqueue(new ScrollNote{type=9,time=noteTime,scroll=noteScroll,value=(float.Parse(notes[i].noteValues))/192*240/Player.playScript.BPM});
                    noteTime += (float.Parse(notes[i].noteValues))/192*240/Player.playScript.BPM;
                }
                preNoteBeat = noteBeat;
                i++;
            }
            if(notes.Count > i){
                currentbeat++;
                noteType = notes[i].noteType;
                noteBeat = notes[i].noteBeat;
                noteTime += (currentbeat-preNoteBeat)*240/Player.playScript.BPM*scalePerMeasure[currentbeat-1];
                noteScroll += (currentbeat-preNoteBeat)*144000*scalePerMeasure[currentbeat-1]/174545;
                GameObject bar = Instantiate(barObj);
                bar.transform.SetParent(GameObject.Find("PlayArea").transform);
                BarScript barscript = bar.GetComponent<BarScript>();
                barscript.scroll = noteScroll;
                barscript.time = noteTime;
                StartCoroutine(barscript.barGO());
                while(currentbeat < Mathf.FloorToInt(noteBeat)){
                    noteTime += 240/Player.playScript.BPM*scalePerMeasure[currentbeat];
                    noteScroll += 144000*scalePerMeasure[currentbeat]/174545;
                    currentbeat++;
                    bar = Instantiate(barObj);
                    bar.transform.SetParent(GameObject.Find("PlayArea").transform);
                    barscript = bar.GetComponent<BarScript>();
                    barscript.scroll = noteScroll;
                    barscript.time = noteTime;
                    StartCoroutine(barscript.barGO());
                }
                preNoteBeat = currentbeat;
            }
        }
        notes.Clear();
        loadDone = 1;
    }
}
public struct preNote{
    public float noteBeat;
    public string noteValues;
    public int noteType;
}
public struct Note{
    public int snd;
    public float Time;
    public int Type;
    public float scroll;
    public GameObject note;
    public bool proced;
}
public struct BGnotes{
    public float noteTime;
    public int noteSnd;
}
public struct ScrollNote{
    public uint type;
    public float time;
    public float scroll;
    public float value;
}
