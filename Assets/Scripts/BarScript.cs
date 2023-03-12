using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarScript : MonoBehaviour
{
    private Transform tr;
    public float scroll;
    public float time;
    public IEnumerator barGO(){
        tr = gameObject.GetComponent<Transform>();
        while(Player.Time.Elapsed.TotalMilliseconds<time*1000){
            yield return null;
            tr.position = new Vector3(215.5f+dataManager.playAreaX,((scroll-Player.totalScroll)*723*Player.HISPEED)+358.5f,0);
        }
        Destroy(gameObject);
    }
}
