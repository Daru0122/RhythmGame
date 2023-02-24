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
    [SerializeField] public TMP_InputField dspSizeMunu;
    private List<Resolution> res = new List<Resolution>();
    FMOD.System SndSystem;
    void Awake(){
        SndSystem = FMODUnity.RuntimeManager.CoreSystem;
        refreshResolutionMenu();
        refreshSndDrivers();
    }
    private void refreshResolutionMenu(){
        res.AddRange(Screen.resolutions);
        resolutionMenu.options.Clear();
        for(int i = 0; i<res.Count;i++){
            Dropdown.OptionData resOption = new Dropdown.OptionData();
            resOption.text = res[i].width+"x"+res[i].height+"("+res[i].refreshRate+")";
            resolutionMenu.options.Add(resOption);
        }
        resolutionMenu.RefreshShownValue();
        resolutionMenu.value = res.IndexOf(Screen.currentResolution);
    }
    private void refreshSndDrivers(){
        SndSystem.setOutput(FMOD.OUTPUTTYPE.WASAPI);
        int driverNum;
        SndSystem.getNumDrivers(out driverNum);
        string deviceName;
        System.Guid guid;
        FMOD.SPEAKERMODE speakerMode;
        int emptyint;
        SoundDeviceMenu.options.Clear();
        for(int i = 0; i<driverNum; i++){
            Dropdown.OptionData deviceOption = new Dropdown.OptionData();
            SndSystem.getDriverInfo(i,out deviceName,30,out guid,out emptyint,out speakerMode,out emptyint);
            deviceOption.text = deviceName;
            SoundDeviceMenu.options.Add(deviceOption);
        }
        SndSystem.getDriver(out int a);
        SoundDeviceMenu.value = a;
    }
    public void OnApplyButton()
    {
        Screen.SetResolution(res[resolutionMenu.value].width,res[resolutionMenu.value].height,true,res[resolutionMenu.value].refreshRate);
        Application.targetFrameRate = int.Parse(maxFrameRateMenu.text);
        SceneManager.LoadScene("Menu");
        SndSystem.setDSPBufferSize(uint.Parse(dspSizeMunu.text),1);
        SndSystem.setDriver(SoundDeviceMenu.value);
    }
}
