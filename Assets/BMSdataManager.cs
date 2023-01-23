using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;


public class BMSdataManager : MonoBehaviour
{
    public static Dictionary<string, string> BGA;
    public static Dictionary<string, FMOD.Sound> WAV = new Dictionary<string, FMOD.Sound>();
    public bool[] LNactive = new bool[9];
    public float HI_SPEED;
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
    private FMOD.Sound snd;
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
    public float loadDone;

    void Start(){
        StartCoroutine(BMSload());
    }
    void Update() {
        if(loadDone >= 1){
            tT += Time.deltaTime;//1초에 1만큼
            totalSCROLL += Time.deltaTime*600*BPM/174545;
            //녹숫 = 174545/(BPM) (600=1000ms) bpm60일때 2909.08333....
            //속도 = 1000 / 녹숫ms (표시영역이 1일때 1초당) = 600/(174545/bpm)=600*bpm/174545
            //구역크기 = 1초에 1만큼 움직일때 = 240/bpm, 1초에 2만큼 = 480/bpm     =(240*600*bpm/174545)/bpm = 144000/174545
        }
    }
    IEnumerator BMSload(){
        yield return null;
        Debug.Log("로딩 시작");
        for(int i = 0; i <= 8;){
            LNactive[i] = false;
            i++;
        }
        system.createChannelGroup (null, out channelGroup);
        channel.setChannelGroup(channelGroup);

        seps = new char[] {' ', ':'};//데이터 Splt 기준 정함
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
                    WAV.Add(input.Substring(4,2),snd);
                }//----------------------------BGA---------------------
                else if(input.Substring(1,3) == "BMP"){

                }//------------------------BPM---------------
                else if(input.Substring(1,3) == "BPM"){

                }//-----------------------MAIN DATA FIELD----------------------
                if(input.Substring(0,1) == "#" && input.Substring(6,1) == ":"){
                    noteJoint.Add(int.Parse(input.Split(':')[0].Substring(1,3)));
                    noteData.Add(int.Parse(input.Split(':')[0].Substring(4)));
                    noteBeat.Add(input.Split(':')[1]);
                }
            }
        }
        reader.Close();
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
        }
        Debug.Log("완료");
        StartCoroutine(startPerJoint());

    }
    IEnumerator startPerJoint(){
        float SectionTime = 0f;
        currentJoint = 0;
        for(int i = 0; noteJoint.Count > i;){
            if(currentJoint < noteJoint[i]){
                //이 사이에 bpm변경 들어갈 예정
                SectionTime += (240/BPM)*scalePerJoint[currentJoint];
                scrollPerJoint += scalePerJoint[currentJoint]*144000/174545;
                currentJoint++;
            }
            while(noteJoint.Count > i && currentJoint >= noteJoint[i]){
                if(noteData[i]>10||noteData[i].Equals(1)){
                    GameObject noteManager = Instantiate(nManager);
                    NoteManager nManagerCom = noteManager.GetComponent<NoteManager>();
                    nManagerCom.SecTime = SectionTime;
                    nManagerCom.noteBeat = noteBeat[i];
                    nManagerCom.secBPM = BPM;
                    nManagerCom.secScroll = scrollPerJoint;
                    nManagerCom.secScale = scalePerJoint[currentJoint];
                    nManagerCom.secType = noteData[i];
                    nManagerCom.loadDone = false;
                    if(noteData[i]>50){
                        nManagerCom.LNactive = LNactive[noteData[i]-51];
                    }
                    if(!noteData[i].Equals(1)){
                        yield return new WaitUntil(()=> nManagerCom.loadDone);
                        Destroy(noteManager);
                    }
                    Debug.Log("노트생성 완");
                }
                i++;
            }
            i++;
        }
        loadDone = 1;
    }
}
