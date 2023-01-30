using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가나다라
/// </summary>
public class Judgement : MonoBehaviour
{
    /// <summary>
    /// �Ҹ���ȣ�� ����
    /// </summary>
    int[] sndPerKey = new int[9];

    /// <summary>
    /// Ű�����ȴ��� �Ǵ�.
    /// </summary>
    /// <param name="noteNum">��Ʈ��ȣ</param>
    /// <param name="noteScript"></param>
    /// <param name="note"></param>
    void Judge(int noteNum, Notescript noteScript, GameObject note){
        //1. ...�� �Ǵ��ϰ� ..�� �Ѵ�.
        if(noteScript.time*1000 <= BMSdataManager.Time.ElapsedMilliseconds+BMSdataManager.judgeTimings[1] && noteScript.time*1000 >= BMSdataManager.Time.ElapsedMilliseconds-BMSdataManager.judgeTimings[1]){
            sndPerKey[noteNum] = noteScript.snd;
            Destroy(note);
            BMSdataManager.dManagerScript.Notes[noteNum].Dequeue();
        }
        //2. .... �� �Ѵ�.
        if(noteScript.snd!=0){
            FMODUnity.RuntimeManager.CoreSystem.playSound(BMSdataManager.WAV[sndPerKey[noteNum]], BMSdataManager.channelGroup, false, out BMSdataManager.channel);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator Judgement0(){
        while(BMSdataManager.dManagerScript.Notes[0].Count > 0){
            yield return new WaitUntil(()=> Input.GetKey(BMSdataManager.keyBinds[0]));
            GameObject note = BMSdataManager.dManagerScript.Notes[0].Peek();
            Notescript noteScript = note.GetComponent<Notescript>();
            Judge(0, noteScript, note);
            yield return new WaitUntil(()=> Input.GetKeyUp(BMSdataManager.keyBinds[0]));
        }
    }
    public IEnumerator Judgement1(){
        while(BMSdataManager.dManagerScript.Notes[1].Count > 0){
            yield return new WaitUntil(()=> Input.GetKey(BMSdataManager.keyBinds[1]));
            GameObject note = BMSdataManager.dManagerScript.Notes[1].Peek();
            Notescript noteScript = note.GetComponent<Notescript>();
            Judge(1, noteScript, note);
            yield return new WaitUntil(()=> Input.GetKeyUp(BMSdataManager.keyBinds[1]));
        }
    }
    public IEnumerator Judgement2(){
        while(BMSdataManager.dManagerScript.Notes[2].Count > 0){
            yield return new WaitUntil(()=> Input.GetKey(BMSdataManager.keyBinds[2]));
            GameObject note = BMSdataManager.dManagerScript.Notes[2].Peek();
            Notescript noteScript = note.GetComponent<Notescript>();
            Judge(2, noteScript, note);
            yield return new WaitUntil(()=> Input.GetKeyUp(BMSdataManager.keyBinds[2]));
        }
    }
    public IEnumerator Judgement3(){
        while(BMSdataManager.dManagerScript.Notes[3].Count > 0){
            yield return new WaitUntil(()=> Input.GetKey(BMSdataManager.keyBinds[3]));
            GameObject note = BMSdataManager.dManagerScript.Notes[3].Peek();
            Notescript noteScript = note.GetComponent<Notescript>();
            Judge(3, noteScript, note);
            yield return new WaitUntil(()=> Input.GetKeyUp(BMSdataManager.keyBinds[3]));
        }
    }
    public IEnumerator Judgement4(){
        while(BMSdataManager.dManagerScript.Notes[4].Count > 0){
            yield return new WaitUntil(()=> Input.GetKey(BMSdataManager.keyBinds[4]));
            GameObject note = BMSdataManager.dManagerScript.Notes[4].Peek();
            Notescript noteScript = note.GetComponent<Notescript>();
            Judge(4, noteScript, note);
            yield return new WaitUntil(()=> Input.GetKeyUp(BMSdataManager.keyBinds[4]));
        }
    }
    public IEnumerator Judgement5(){
        while(BMSdataManager.dManagerScript.Notes[5].Count > 0){
            yield return new WaitUntil(()=> Input.GetKey(BMSdataManager.keyBinds[5]));
            GameObject note = BMSdataManager.dManagerScript.Notes[5].Peek();
            Notescript noteScript = note.GetComponent<Notescript>();
            Judge(5, noteScript, note);
            yield return new WaitUntil(()=> Input.GetKeyUp(BMSdataManager.keyBinds[6]));
        }
    }
    public IEnumerator Judgement6(){
        while(BMSdataManager.dManagerScript.Notes[6].Count > 0){
            yield return new WaitUntil(()=> Input.GetKey(BMSdataManager.keyBinds[6]));
            GameObject note = BMSdataManager.dManagerScript.Notes[6].Peek();
            Notescript noteScript = note.GetComponent<Notescript>();
            Judge(6, noteScript, note);
            yield return new WaitUntil(()=> Input.GetKeyUp(BMSdataManager.keyBinds[6]));
        }
    }
    public IEnumerator Judgement8(){
        while(BMSdataManager.dManagerScript.Notes[8].Count > 0){
            yield return new WaitUntil(()=> Input.GetKey(BMSdataManager.keyBinds[8]));
            GameObject note = BMSdataManager.dManagerScript.Notes[8].Peek();
            Notescript noteScript = note.GetComponent<Notescript>();
            Judge(8, noteScript, note);
            yield return new WaitUntil(()=> Input.GetKeyUp(BMSdataManager.keyBinds[8]));
        }
    }
    public IEnumerator Judgement9(){
        while(BMSdataManager.dManagerScript.Notes[9].Count > 0){
            yield return new WaitUntil(()=> Input.GetKey(BMSdataManager.keyBinds[9]));
            GameObject note = BMSdataManager.dManagerScript.Notes[9].Peek();
            Notescript noteScript = note.GetComponent<Notescript>();
            Judge(9, noteScript, note);
            yield return new WaitUntil(()=> Input.GetKeyUp(BMSdataManager.keyBinds[9]));
        }
    }
}
