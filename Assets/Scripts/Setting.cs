using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Setting : MonoBehaviour
{
    private struct SndDevice{
        public string name;
        public FMOD.OUTPUTTYPE mode;
        public int val;

    }
    [SerializeField] public Dropdown resolutionMenu;
    [SerializeField] public TMP_InputField maxFrameRateMenu;
    [SerializeField] public Dropdown SoundDeviceMenu;
    [SerializeField] public TMP_InputField dspSizeMunu;
    private List<Resolution> res = new List<Resolution>();
    private List<SndDevice> Audio = new List<SndDevice>();
    FMOD.System SndSystem;
    FMOD.System AltSndSystem;
    void Awake(){
        SndSystem = FMODUnity.RuntimeManager.CoreSystem;
        FMOD.Factory.System_Create(out AltSndSystem);
        refreshResolutionMenu();
        refreshSndDrivers();
    }
    private void refreshResolutionMenu(){
        res.AddRange(Screen.resolutions);
        res.Reverse();
        resolutionMenu.options.Clear();
        for(int i = 0; i<res.Count;i++){
            Dropdown.OptionData resOption = new Dropdown.OptionData();
            resOption.text = res[i].width+"x"+res[i].height+"("+res[i].refreshRate+")";
            resolutionMenu.options.Add(resOption);
        }
        resolutionMenu.RefreshShownValue();
        resolutionMenu.value = res.IndexOf(Screen.currentResolution);
        maxFrameRateMenu.text = res[res.IndexOf(Screen.currentResolution)].refreshRate.ToString();
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
            SndSystem.getDriverInfo(i,out deviceName,50,out guid,out emptyint,out speakerMode,out emptyint);
            deviceOption.text = deviceName;
            SoundDeviceMenu.options.Add(deviceOption);
            Audio.Add(new SndDevice{name=deviceName, mode=FMOD.OUTPUTTYPE.WASAPI, val=i});
        }
        AltSndSystem.setOutput(FMOD.OUTPUTTYPE.ASIO);
        AltSndSystem.getNumDrivers(out driverNum);
        for(int i = 0; i<driverNum; i++){
            Dropdown.OptionData deviceOption = new Dropdown.OptionData();
            AltSndSystem.getDriverInfo(i,out deviceName,50,out guid,out emptyint,out speakerMode,out emptyint);
            deviceOption.text = deviceName;
            SoundDeviceMenu.options.Add(deviceOption);
            Audio.Add(new SndDevice{name=deviceName, mode=FMOD.OUTPUTTYPE.ASIO, val=i});
        }
        AltSndSystem.release();
        SndSystem.getDriver(out int a);
        SoundDeviceMenu.value = a;
        SndSystem.getDSPBufferSize(out uint b, out int c);
        SndSystem.setDSPBufferSize(b,1);
        dspSizeMunu.text = b.ToString();
    }
    public void OnApplyButton()
    {
        Screen.SetResolution(res[resolutionMenu.value].width,res[resolutionMenu.value].height,true,res[resolutionMenu.value].refreshRate);
        Application.targetFrameRate = int.Parse(maxFrameRateMenu.text);
        SceneManager.LoadScene("Menu");
        SndSystem.setDSPBufferSize(uint.Parse(dspSizeMunu.text),1);
        if(Audio[SoundDeviceMenu.value].mode.Equals(FMOD.OUTPUTTYPE.WASAPI)){
            SndSystem.setOutput(FMOD.OUTPUTTYPE.WASAPI);
            SndSystem.setDriver(Audio[SoundDeviceMenu.value].val);
        }
        if(Audio[SoundDeviceMenu.value].mode.Equals(FMOD.OUTPUTTYPE.ASIO)){
            SndSystem.setOutput(FMOD.OUTPUTTYPE.ASIO);
            SndSystem.setDriver(Audio[SoundDeviceMenu.value].val);
        }
    }
}
