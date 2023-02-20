using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    public TMP_InputField field;
    public static string value;
    void Awake(){
        value = field.text;
    }
    private void FixedUpdate() {
        InputSystem.Update();
    }
    public void OnStartbutton()
    {
        value = field.text.Replace("\\","/");
        SceneManager.LoadScene("Play");
    }
    public void OnSettingbutton()
    {
        SceneManager.LoadScene("Setting");
    }
}
