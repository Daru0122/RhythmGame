using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;


public class BMSdataManager : MonoBehaviour
{
    public static KeyCode[] keyBinds = new KeyCode[9];
    public bool[] inputActive = new bool[9];
    public int combos;
    public int score;
    public static float[] judgeTimings = new float[5];
    public List<Queue<GameObject>> notes = new List<Queue<GameObject>>();
    public static Dictionary<string, string> BGA = new Dictionary<string, string>();
    public static Dictionary<string, float> BPMs = new Dictionary<string, float>();
    public static Dictionary<string, FMOD.Sound> WAV = new Dictionary<string, FMOD.Sound>();
    public bool[] LNactive = new bool[9];
    public float HI_SPEED;
    public static string fileLoc = "C:/Program Files (x86)/Steam/steamapps/common/Qwilight/SavesDir/Bundle/Amnehilesie1108/Amnehilesie";//파일 위치 지정
    public static float totalSCROLL;
    float scrollPerJoint;
    List<float> scalePerJoint = new List<float>();
    [SerializeField] GameObject nManager;
    [SerializeField] GameObject VideoPlayer;
    public static System.Diagnostics.Stopwatch tT = new System.Diagnostics.Stopwatch();
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
        judgeTimings[0] = 40;
        judgeTimings[1] = 80;
        judgeTimings[2] = 120;
        judgeTimings[3] = 15;
        judgeTimings[4] = 150;
        keyBinds[0] = KeyCode.S;
        keyBinds[1] = KeyCode.D;
        keyBinds[2] = KeyCode.F;
        keyBinds[3] = KeyCode.Space;
        keyBinds[4] = KeyCode.J;
        keyBinds[5] = KeyCode.LeftShift;
        keyBinds[7] = KeyCode.K;
        keyBinds[8] = KeyCode.L;
        notes.Add(new Queue<GameObject>());
        notes.Add(new Queue<GameObject>());
        notes.Add(new Queue<GameObject>());
        notes.Add(new Queue<GameObject>());
        notes.Add(new Queue<GameObject>());
        notes.Add(new Queue<GameObject>());
        notes.Add(new Queue<GameObject>());
        notes.Add(new Queue<GameObject>());
        notes.Add(new Queue<GameObject>());
        StartCoroutine(BMSload());
    }
    void Update() {
        totalSCROLL = tT.ElapsedMilliseconds*0.6f*BPM/174545;
    }
    IEnumerator play(){
        yield return new WaitUntil(()=> loadDone >= 1);
        tT.Start();
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
                    BGA.Add(input.Substring(4,2), input.Substring(input.IndexOf(' ')+1));
                }//------------------------BPM---------------
                else if(input.Substring(1,3) == "BPM"){
                    BPMs.Add(input.Substring(4,2), float.Parse(input.Substring(input.IndexOf(' '))));
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
                }else if(noteData[i].Equals(4)){
                    playVideo playVideo = VideoPlayer.GetComponent<playVideo>();
                    playVideo.data = noteBeat[i];
                    playVideo.totalSecTime = SectionTime;
                    playVideo.secTime = 240*scalePerJoint[i]/BPM;
                    StartCoroutine(playVideo.play());
                }
                i++;
            }
            i++;
        }
        loadDone = 1;
        StartCoroutine(play());
    }
}
