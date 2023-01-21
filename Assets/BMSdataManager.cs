using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;


public class BMSdataManager : MonoBehaviour
{
    public string fileLoc = "C:/Program Files (x86)/Steam/steamapps/common/Qwilight/SavesDir/Bundle/Amnehilesie1108/Amnehilesie";//파일 위치 지정
    public static float totalSCROLL;
    float scrollPerJoint;
    List<float> scalePerJoint = new List<float>();
    [SerializeField] GameObject nManager;
    public static float tT;
    int currentJoint;
    private char[] seps;
    List<int> noteJoint = new List<int>();
    List<int> noteData = new List<int>();
    List<string> noteBeat = new List<string>();
    public static FMOD.Sound[] snd;
    FMOD.System system;
    public static FMOD.Channel channel;
    public static FMOD.ChannelGroup channelGroup;
    //HEADER FILED 선언
    int PLAYER;//1:SP,2:Couple,3:DP
    string GENRE;
    string TITLE;
    string ARTIST;
    public static float BPM;
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
    public static List<string> WAV_num = new List<string>();
    public static List<string> WAV_file = new List<string>();
    public float loadDone;

    void Start(){
        StartCoroutine(BMSload());
    }
    void Update() {
        if(loadDone >= 1){
            tT += Time.deltaTime;
            totalSCROLL += Time.deltaTime;
        }
    }
    IEnumerator BMSload(){
        Debug.Log("로딩 시작");

        seps = new char[] {' ', ':'};//데이터 Splt 기준 정함
        system = FMODUnity.RuntimeManager.CoreSystem;
        StreamReader reader = new StreamReader(new FileStream(fileLoc+"/amnehilasieSPN.bms", FileMode.Open), Encoding.GetEncoding("shift_jis"));
        yield return null;
        //bms파일 판단하고 변수로 저장하는 과정
        while (!reader.EndOfStream){
            input = reader.ReadLine();
            Debug.Log(input);
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
                    WAV_file.Add(input.Substring(input.IndexOf(" ")+1));
                    WAV_num.Add(input.Substring(4,2));
                }//-----------------------MAIN DATA FIELD----------------------
                if(input.Substring(0,1) == "#" && input.Substring(6,1) == ":"){
                    noteJoint.Add(int.Parse(input.Split(':')[0].Substring(1,3)));
                    noteData.Add(int.Parse(input.Split(':')[0].Substring(4)));
                    noteBeat.Add(input.Split(':')[1]);
                }
            }
        }
        reader.Close();
        yield return null;
        //FMOD 소리 메모리에 넣는?과정
        snd = new FMOD.Sound[WAV_file.Count];
        for(int i = 0; WAV_file.Count > i;){
            system.createChannelGroup (null, out channelGroup);
            channel.setChannelGroup(channelGroup);
            system.createSound(fileLoc+"/"+WAV_file[i], FMOD.MODE.CREATESAMPLE | FMOD.MODE.ACCURATETIME, out snd[i]);
            i ++;
        }
        yield return null;
        //구역마다 크기를 미리 저장
        for(int i = 0; noteJoint.Count > i;){
            while(noteJoint.Count > i && currentJoint <= noteJoint[i]){
                if(noteData[i].Equals(2)){
                    while (scalePerJoint.Count < noteJoint[i]){
                        scalePerJoint.Add(1);
                    }
                    scalePerJoint.Add(float.Parse(noteBeat[i]));
                }
                i++;
            }
            i++;
            yield return null;
        }
        Debug.Log("완료");
        loadDone = 1;
        StartCoroutine(startPerJoint());

    }
    IEnumerator startPerJoint(){
        float SectionTime = 0f;
        currentJoint = 0;
        for(int i = 0; noteJoint.Count > i;){
            yield return null;
            if(currentJoint < noteJoint[i]){
                //이 사이에 bpm변경 들어갈 예정
                scrollPerJoint += (240/BPM)*scalePerJoint[currentJoint];
                SectionTime += scalePerJoint[currentJoint]*240/BPM;
                currentJoint++;
            }
            while(noteJoint.Count > i && currentJoint >= noteJoint[i]){
                if(noteData[i]>10||noteData[i].Equals(1)){
                    yield return null;
                    GameObject noteManager = Instantiate(nManager);
                    NoteManager nManagerCom = nManager.GetComponent<NoteManager>();
                    nManagerCom.noteBeat = noteBeat[i];
                    nManagerCom.secBPM = BPM;
                    nManagerCom.secScroll = scrollPerJoint;
                    nManagerCom.secScale = scalePerJoint[currentJoint];
                }
                i++;
            }
            i++;
        }
    }
}
