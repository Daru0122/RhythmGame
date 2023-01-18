using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;


public class BMSdataManager : MonoBehaviour
{
    List<int> noteData;
    List<float> noteBeat;
    FMOD.Sound[] snd;
    FMOD.System system;
    FMOD.Channel channel;
    FMOD.ChannelGroup channelGroup;
    //HEADER FILED 선언
    int PLAYER;//1:SP,2:Couple,3:DP
    string GENRE;
    string TITLE;
    string ARTIST;
    float BPM;
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
    public string fileLoc;
    public static List<string> WAV_num = new List<string>();
    public static List<string> WAV_file = new List<string>();
    void Start(){
        system = FMODUnity.RuntimeManager.CoreSystem;
        fileLoc = "C:/Program Files (x86)/Steam/steamapps/common/Qwilight/SavesDir/Bundle/Amnehilesie1108/Amnehilesie";
        StreamReader testStR = new StreamReader(new FileStream(fileLoc+"/amnehilasieSPA.bms", FileMode.Open));
        while (true != testStR.EndOfStream){
            input = testStR.ReadLine();
            if(input.Length >= 1 && input.Substring(0,1) == "#"){
                if(input.IndexOf(' ') > 0){
                //HEADER FIELD
                    if(input.Substring(1,input.IndexOf(" ")-1) == "PLAYER"){
                        PLAYER = int.Parse(input.Substring(input.IndexOf(" ")+1));
                    }else if(input.Substring(1,input.IndexOf(" ")-1) == "GENRE"){
                        GENRE = input.Substring(input.IndexOf(" ")+1);
                    }else if(input.Substring(1,input.IndexOf(" ")-1) == "TITLE"){
                        TITLE = input.Substring(input.IndexOf(" ")+1);
                    }else if(input.Substring(1,input.IndexOf(" ")-1) == "ARTIST"){
                        ARTIST = input.Substring(input.IndexOf(" ")+1);
                    }else if(input.Substring(1,input.IndexOf(" ")-1) == "BPM"){
                        BPM = float.Parse(input.Substring(input.IndexOf(" ")+1));
                    }else if(input.Substring(1,input.IndexOf(" ")-1) == "PLAYLEVEL"){
                        PLAYLEVEL = int.Parse(input.Substring(input.IndexOf(" ")+1));
                    }else if(input.Substring(1,input.IndexOf(" ")-1) == "RANK"){
                        RANK = int.Parse(input.Substring(input.IndexOf(" ")+1));
                    }//expand
                    else if(input.Substring(1,input.IndexOf(" ")-1) == "SUBTITLE"){
                        SUBTITLE = input.Substring(input.IndexOf(" ")+1);
                    }else if(input.Substring(1,input.IndexOf(" ")-1) == "SUBARTIST"){
                        SUBARTIST = input.Substring(input.IndexOf(" ")+1);
                    }else if(input.Substring(1,input.IndexOf(" ")-1) == "STAGEFILE"){
                        STAGEFILE = input.Substring(input.IndexOf(" ")+1);
                    }else if(input.Substring(1,input.IndexOf(" ")-1) == "BANNER"){
                        BANNER = input.Substring(input.IndexOf(" ")+1);
                    }else if(input.Substring(1,input.IndexOf(" ")-1) == "DIFFICULTY"){
                        DIFFICULTY = int.Parse(input.Substring(input.IndexOf(" ")+1));
                    }else if(input.Substring(1,input.IndexOf(" ")-1) == "TOTAL"){
                        TOTAL = float.Parse(input.Substring(input.IndexOf(" ")+1));
                    }else if(input.Substring(1,input.IndexOf(" ")-1) == "LNTYPE"){
                        LNTYPE = int.Parse(input.Substring(input.IndexOf(" ")+1));
                    }//-----------------------------WAV----------------------------
                    else if(input.Substring(1,3) == "WAV"){
                        WAV_file.Add(input.Substring(input.IndexOf(" ")+1));
                        WAV_num.Add(input.Substring(4,2));
                    }
                }if(input.Substring(0,1) == "#" && input.Substring(6,1) == ":"){
                    Debug.Log(input);
                    noteData.Add(int.Parse(input.Substring(1,5)));
                    noteBeat.Add(float.Parse(input.Substring(1,5)));
                }
            }
        }
        testStR.Close();
        snd = new FMOD.Sound[WAV_file.Count];
        for(int i = 0; WAV_file.Count > i;){
            system.createChannelGroup (null, out channelGroup);
            channel.setChannelGroup(channelGroup);
            system.createSound(fileLoc+"/"+WAV_file[i], FMOD.MODE.CREATESAMPLE | FMOD.MODE.ACCURATETIME, out snd[i]);
            i ++;
        }
        system.playSound(snd[1], channelGroup, false,out channel);
    }
    void Update() {
        
    }
}
