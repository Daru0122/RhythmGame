using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    Dictionary<int, int> sprite = new Dictionary<int, int>();
    Dictionary<int, float> Location= new Dictionary<int, float>();
    public int EX;
    private float HI_SPEED;
    private float xLoc;
    public Sprite[] Notes;
    SpriteRenderer spriteRenderer;
    BMSdataManager bmsDM;
    public float time;
    public int type;
    public float scroll;
    public string snd;
    void Start(){
//---------이미지 지정-------
        //일반노트 스프라이트
        sprite.Add(1, 1);
        sprite.Add(2, 2);
        sprite.Add(3, 1);
        sprite.Add(4, 2);
        sprite.Add(5, 1);
        sprite.Add(6, 0);
        sprite.Add(8, 2);
        sprite.Add(9, 1);
        //롱노트 시작점 스프라이트
        sprite.Add(41, 4);
        sprite.Add(42, 5);
        sprite.Add(43, 4);
        sprite.Add(44, 5);
        sprite.Add(45, 4);
        sprite.Add(46, 3);
        sprite.Add(48, 5);
        sprite.Add(49, 4);
        //롱노트 종점 스프라이트
        sprite.Add(141, 7);
        sprite.Add(142, 8);
        sprite.Add(143, 7);
        sprite.Add(144, 8);
        sprite.Add(145, 7);
        sprite.Add(146, 6);
        sprite.Add(148, 8);
        sprite.Add(149, 7);
//-----------위치지정------
        Location.Add(1,92);
        Location.Add(2,146);
        Location.Add(3,188);
        Location.Add(4,242);
        Location.Add(5,284);
        Location.Add(6,0);
        Location.Add(8,338);
        Location.Add(9,380);
        
        



        bmsDM = GameObject.Find("DataManager").GetComponent<BMSdataManager>();
        HI_SPEED = bmsDM.HI_SPEED;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (type > 10 && type < 20){
            spriteRenderer.sprite = Notes[sprite[type-10]];
            xLoc = Location[type-10];
        }else if (type>50){
            if (EX.Equals(1)){
                spriteRenderer.sprite = Notes[sprite[type-10]];
            }else if (EX.Equals(2)){
                spriteRenderer.sprite = Notes[sprite[type-10+100]];
            }
            xLoc = Location[type-50];
        }
    }
    void Update()
    {
        gameObject.transform.position = new Vector3(xLoc, 1080 - ((BMSdataManager.totalSCROLL-scroll+(1/HI_SPEED))*723*HI_SPEED) , 0);
        if(BMSdataManager.tT >= time){
            FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[snd], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
            if(EX.Equals(1)){
                Debug.Log("롱놑 시작");
            }else if(EX.Equals(2)){
                Debug.Log("롱놑 끝");
            }
            Destroy(gameObject);
        }
    }
}
