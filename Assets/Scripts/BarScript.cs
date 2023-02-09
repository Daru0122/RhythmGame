using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarScript : MonoBehaviour
{
    private Transform tr;
    public float scroll;
    public IEnumerator barGO(){
        tr = gameObject.GetComponent<Transform>();
        while(BMSdataManager.totalScroll<scroll){
            yield return null;
            tr.position = new Vector3(215.5f+BMSdataManager.playAreaX,((scroll-BMSdataManager.totalScroll)*723)+358.5f,0);
        }
        Destroy(gameObject);
    }
}
