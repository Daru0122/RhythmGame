using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
/// <summary>
/// 한글테스트
/// </summary>
public class VideoManager : MonoBehaviour
{
    public float time;
    public string fileLoc;

    public IEnumerator playVideo(){
        yield return new WaitUntil(()=> BMSdataManager.Time.ElapsedMilliseconds >= time*1000);
        VideoPlayer videoPlayer = gameObject.GetComponent<VideoPlayer>();
        videoPlayer.url = fileLoc;
        videoPlayer.Play();
    }
}
