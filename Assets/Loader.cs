using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public Slider progressbar;
    private void Start(){
        StartCoroutine(Load());
    }
    IEnumerator Load(){
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync("Play");
        op.allowSceneActivation = false;//false일땐 0.9까지만 로딩

        while(!op.isDone){
            yield return null;
            progressbar.value = op.progress;
            if(Input.anyKeyDown){
                op.allowSceneActivation = true;
            }
        }

    }
}
