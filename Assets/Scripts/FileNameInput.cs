using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class FileNameInput : MonoBehaviour
{
    public TMP_InputField field;
    public static string value;
    void Awake(){
        value = field.text;
    }
    public void Onbutton()
    {
        value = field.text.Replace("\\","/");
        SceneManager.LoadScene("Play");
    }
}
