using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Setting : MonoBehaviour
{
    [SerializeField] public Dropdown resolutionMenu;
    [SerializeField] public TMP_InputField maxFrameRateMenu;
    [SerializeField] public Dropdown SoundDeviceMenu;
    private List<Resolution> res = new List<Resolution>();
    void Awake(){
        res.AddRange(Screen.resolutions);
        resolutionMenu.options.Clear();
        for(int i = 0; i<res.Count;i++){
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = res[i].width+"x"+res[i].height+"("+res[i].refreshRate+")";
            resolutionMenu.options.Add(option);
        }
        resolutionMenu.RefreshShownValue();
    }
    public void OnApplyButton()
    {
        Screen.SetResolution(res[resolutionMenu.value].width,res[resolutionMenu.value].width,true,res[resolutionMenu.value].refreshRate);
        Application.targetFrameRate = int.Parse(maxFrameRateMenu.text);
        SceneManager.LoadScene("Menu");
    }
}
