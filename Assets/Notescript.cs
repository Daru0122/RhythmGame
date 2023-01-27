using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notescript : MonoBehaviour
{
    BMSdataManager dManagerScript;
    public float time;
    public int snd;
    void Start(){
        dManagerScript = GameObject.Find("DataManager").GetComponent<BMSdataManager>();
        StartCoroutine(bgm());
    }
    IEnumerator bgm(){
        yield return new WaitUntil(()=> dManagerScript.loadDone >= 1);
        yield return new WaitUntil(()=> BMSdataManager.Time.ElapsedMilliseconds >= time*1000);
        FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
    }
}
