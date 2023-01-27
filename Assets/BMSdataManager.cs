using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;


public class BMSdataManager : MonoBehaviour
{
    public float loadDone;
    public GameObject noteObj;
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
    public static string fileLoc = "C:/Program Files (x86)/Steam/steamapps/common/Qwilight/SavesDir/Bundle/Amnehilesie1108/Amnehilesie";//파일 위치 지정
    public static System.Diagnostics.Stopwatch Time = new System.Diagnostics.Stopwatch();//초시계
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
    //--------------------------------------------
    string input;
    string[] splitText;
    float output;//함수 호출시 출력

    void Start(){
        StartCoroutine(BMSload());
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
    IEnumerator BMSload(){//bms파일을 파싱
        system = FMODUnity.RuntimeManager.CoreSystem;
        yield return null;
        Debug.Log("로딩 시작");
        system.createChannelGroup (null, out channelGroup);
        channel.setChannelGroup(channelGroup);

        seps = new char[] {' ', ':'};//데이터 Split 기준 정함
        StreamReader reader = new StreamReader(new FileStream(fileLoc+"/amnehilasieSPN.bms", FileMode.Open), Encoding.GetEncoding("shift_jis"));
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
                    LNTYPE = int.Parse(input.Substring(input.IndexOf(" ")+1));
                }//-----------------------------WAV----------------------------
                else if(input.Substring(1,3) == "WAV"){
                    system.createSound(fileLoc+"/"+input.Substring(input.IndexOf(" ")+1), FMOD.MODE.CREATESAMPLE | FMOD.MODE.ACCURATETIME, out snd);
                    convertFromThirysix(input.Substring(4,2));
                    WAV[(int)output] = snd;
                }//----------------------------BGA---------------------
                else if(input.Substring(1,3) == "BMP"){
                    BGA.Add(input.Substring(4,2), input.Substring(input.IndexOf(' ')+1));
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
                        while(scalePerMeasure.Count < int.Parse(input.Split(':')[0].Substring(1,3))){
                            scalePerMeasure.Add(1);//빈마디는 1로 채워넣고
                        }
                        scalePerMeasure.Add(float.Parse(noteData));//현제 마디에 해당하는 값 넣기
                    }else if(noteType.Equals(3)){//BPM변속(구식)-------------------------------------------------------------------
                        for(int i = 0; noteData.Length > i*2; i++){
                            if(noteData.Substring(i*2,2) != "00"){
                                convertFromSixteen(noteData.Substring(i*2,2));//옛날 BPM변속은 16진수(16*16-1=255)
                                notes.Add(new Note{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=8,noteValues=output.ToString()});//모두 신식 변속으로 변경
                            }
                        }
                    }else if(noteType.Equals(4)){//BGA------------------------------------------------------------
                        for(int i = 0; noteData.Length > i*2; i++){
                            if(noteData.Substring(i*2,2) != "00"){
                                convertFromThirysix(noteData.Substring(i*2,2));
                                notes.Add(new Note{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=4,noteValues=output.ToString()});
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
                                notes.Add(new Note{noteBeat=currentMeasure+(2*i/(float)noteData.Length), noteType=8,noteValues=BPMs[noteData.Substring(i*2,2)]});
                            }
                        }
                    }else if (noteType > 10){//노트들--------------------------------------------------------------------------------
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
        reader.Close();
        notes.Sort((x,y) => x.noteBeat.CompareTo(y.noteBeat));
        float noteBeat = 0;
        float preNoteBeat = 0;
        float noteTime = 0f;
        int currentbeat = 0;
        for(int i = 0; notes.Count > i;){
            while(notes.Count > i && currentbeat+1 > notes[i].noteBeat){
                noteType = notes[i].noteType;
                noteBeat = notes[i].noteBeat;
                noteTime += (noteBeat-preNoteBeat)*240/BPM*scalePerMeasure[currentbeat];
                if(noteType.Equals(1)||noteType>10){//배경음이나 노트라면
                    GameObject noteMade = Instantiate(noteObj);
                    Notescript nScript = noteMade.GetComponent<Notescript>();
                    nScript.time=noteTime;
                    nScript.snd=int.Parse(notes[i].noteValues);
                }else if(noteType.Equals(8)){
                    BPM = float.Parse(notes[i].noteValues);
                }
                preNoteBeat = noteBeat;
                i++;
            }
            if(notes.Count > i){
                currentbeat++;
                noteType = notes[i].noteType;
                noteBeat = notes[i].noteBeat;
                noteTime += (currentbeat-preNoteBeat)*240/BPM*scalePerMeasure[currentbeat-1];
                while(currentbeat < Mathf.FloorToInt(noteBeat)){
                    noteTime += 240/BPM*scalePerMeasure[currentbeat];
                    currentbeat++;
                }
                preNoteBeat = currentbeat;
            }
        }
        loadDone = 1;
        Time.Start();
    }
}
public struct Note{
    public float noteBeat;
    public string noteValues;
    public int noteType;
}
