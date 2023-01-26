using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;


public class BMSdataManager : MonoBehaviour
{
    //---------노트(배경음, 변속, bga, stop 등등 이벤트들도 모두 "노트"로서 처리함)------
    //16진수, 36진수 디코더
    //로딩에 필요한 변수들
    private int noteType;
    private string noteData;
    private char[] seps;
    private int currentMeasure;
    //저장된 노트(아래 3개 리스트가 노트 하나를 이룸)
    public List<float> noteBeats = new List<float>();
    public List<int> noteTypes = new List<int>();
    public List<float> noteValues = new List<float>();//float로 하려 했으나 bpm때문에..
    //구역 크기는 데이터중 유일하게(아마도) 구역마다 처리되므로 따로처리
    public List<float> scalePerMeasure = new List<float>();
    //------------------------------------
    public static Dictionary<string, string> BGA = new Dictionary<string, string>();//#BMPxx a 저장
    public static Dictionary<string, float> BPMs = new Dictionary<string, float>();//#BPMxx a 저장
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
    IEnumerator BMSload(){
        yield return null;
        Debug.Log("로딩 시작");
        system.createChannelGroup (null, out channelGroup);
        channel.setChannelGroup(channelGroup);

        seps = new char[] {' ', ':'};//데이터 Split 기준 정함
        system = FMODUnity.RuntimeManager.CoreSystem;
        StreamReader reader = new StreamReader(new FileStream(fileLoc+"/amnehilasieSPN.bms", FileMode.Open), Encoding.GetEncoding("shift_jis"));
        //bms파일 판단하고 변수로 저장하는 과정
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
                    BPMs.Add(input.Substring(4,2), float.Parse(splitText[1]));
                }//-----------------------MAIN DATA FIELD----------------------
                if(input.Substring(0,1) == "#" && input.Substring(6,1) == ":"){
                    noteType = int.Parse(input.Split(':')[0].Substring(4));
                    noteData = input.Split(':')[1];
                    currentMeasure = int.Parse(input.Split(':')[0].Substring(1,3));
                    if(noteType.Equals(1)){//배경음
                        for(int i = 0; noteData.Length > i; i++){//2글자씩 읽음
                            if(noteData.Substring(i*2,2) != "00"){//값이 0이 아닐때
                                noteBeats.Add(currentMeasure+(i*noteData.Length/2));//저장
                                noteTypes.Add(1);
                                convertFromThirysix(noteData.Substring(i*2,2));
                                noteValues.Add(output);
                            }
                        }
                    }else if(noteType.Equals(2)){//마디별 크기
                        while(scalePerMeasure.Count < int.Parse(input.Split(':')[0].Substring(1,3))){
                            scalePerMeasure.Add(1);//빈마디는 1로 채워넣고
                        }
                        scalePerMeasure.Add(float.Parse(noteData));//현제 마디에 해당하는 값 넣기
                    }else if(noteType.Equals(3)){//BPM변속(구식)
                        for(int i = 0; noteData.Length > i; i++){
                            if(noteData.Substring(i*2,2) != "00"){
                                noteBeats.Add(currentMeasure+(i*noteData.Length/2));
                                noteTypes.Add(8);//BPM변속(신식)으로 모두 변환
                                convertFromSixteen(noteData.Substring(i*2,2));
                                noteValues.Add(output);//옛날 BPM변속은 16진수(16*16-1=255)
                            }
                        }
                    }else if(noteType.Equals(4)){
                        
                    }
                }
            }
        }
        reader.Close();
    }
}
