using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO;
public class playVideo : MonoBehaviour
{
    public float secTime;
    public float totalSecTime;
    public string data;
    VideoPlayer videoPlayer;
    void Start(){
        videoPlayer = gameObject.GetComponent<VideoPlayer>();
    }
    public IEnumerator play(){
        for (int i = 0; i < data.Length/2;){
            Debug.Log("for");
            if(data.Substring(i*2,2) != "00"){
                Debug.Log("재생준비");
                videoPlayer.url = BMSdataManager.fileLoc+"/"+System.IO.Path.GetFileNameWithoutExtension(BMSdataManager.BGA[data.Substring(i*2,2)])+".mp4";
                yield return new WaitUntil(()=> BMSdataManager.tT.ElapsedMilliseconds > 1000*totalSecTime+secTime*i/(data.Length/2));
                Debug.Log("재생");
                videoPlayer.Play();
            }
            i++;
            yield return null;
        }
    }
}
