using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public string noteBeat;
    public float secBPM;
    public float secScroll;
    public float secScale;
    [SerializeField]  GameObject N_2;
    N2 N2com;
    void Start(){
        StartCoroutine(noteInstantiate());
    }
    IEnumerator noteInstantiate(){
        yield return null;
        for(int i = 0; i < noteBeat.Length/2;){
            yield return new WaitUntil(()=>BMSdataManager.totalSCROLL >=  secScroll+secScale*i*(240/secBPM/(noteBeat.Length/2)));
            if(noteBeat.Substring(i*2,2) != "00"){
                GameObject N2s = Instantiate(N_2);
                N2com = N2s.GetComponent<N2>();
                N2com.scroll = secScroll+secScale*i*(240/secBPM/(noteBeat.Length/2));
                N2com.snd = BMSdataManager.WAV_num.BinarySearch(noteBeat.Substring(i*2,2));
            }
            i++;
        }
    }
}
