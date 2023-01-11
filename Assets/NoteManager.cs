using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public float bpm = 60f;
    public static int GreenNumber = 500;
    [SerializeField]  GameObject N_2;
    public static float SectionTime;
    public static float Tt = 0f;
    public static float NoteTime;
    Transform N2_pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Tt += Time.deltaTime;
        if(Tt - SectionTime >= 0){
            NoteTime = SectionTime;
            GameObject N2s = Instantiate(N_2);
            SectionTime += 60 / bpm;
            N2_pos = N_2.GetComponent<Transform>();
        }

    }
}
