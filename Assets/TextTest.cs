using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextTest : MonoBehaviour
{
    string input;
    public string File_Loc;
    public static List<string> WAV_num = new List<string>();
    public static List<string> WAV_file = new List<string>();
    void Start()
    {
        File_Loc = "C:/Program Files (x86)/Steam/steamapps/common/Qwilight/SavesDir/Bundle/BOFXVI_EmoCosine_Cutter_wav/BOFXVI_EmoCosine_Cutter/_cutter_12_SPN.bms";

        StreamReader testStR = new StreamReader(new FileStream(File_Loc, FileMode.Open));
        while (true != testStR.EndOfStream)
        {
            input = testStR.ReadLine();
            if(input.Length >= 4)
            {
                Debug.Log("if진입");
                if(input.Substring(0, 4) == "#WAV")
                {
                    Debug.Log("wav읽힘");
                    WAV_num.Add(input.Substring(4,2));
                    WAV_file.Add(input.Substring(input.IndexOf(' ')+1));
                }
            }
        }
        Debug.Log("끝");
        testStR.Close();
    }
}
