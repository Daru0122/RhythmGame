using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class VideoManager : MonoBehaviour
{
    public float time;
    public string fileLoc;

    public IEnumerator playVideo(){
        yield return new WaitUntil(()=> BMSdataManager.Time.Elapsed.TotalMilliseconds >= time*1000);
        VideoPlayer videoPlayer = gameObject.GetComponent<VideoPlayer>();
        videoPlayer.url = fileLoc;
        videoPlayer.Play();
    }
}
