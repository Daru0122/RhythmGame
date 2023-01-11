using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class N2 : MonoBehaviour
{
    float Notetime;
    // Start is called before the first frame update
    void Start()
    {
        Notetime = NoteManager.NoteTime;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(92, 1080 - ((NoteManager.Tt - Notetime) * 723 * 1000 / NoteManager.GreenNumber) , 0);
        if((NoteManager.Tt - Notetime) * 1000 >= NoteManager.GreenNumber){
            Destroy(gameObject);
            Debug.Log("부셔");
        }
    }
}
