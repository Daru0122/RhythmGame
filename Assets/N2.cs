using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class N2 : MonoBehaviour
{
    public float scroll;
    public int snd;
    void Update()
    {
        gameObject.transform.position = new Vector3(92, 1080 - ((BMSdataManager.totalSCROLL-scroll)*1000) , 0);
        if((BMSdataManager.totalSCROLL-scroll)*1000 >= 723){
            FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.snd[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
            Destroy(gameObject);
        }
    }
}
