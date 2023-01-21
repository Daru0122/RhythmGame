using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartClick : MonoBehaviour
{
    public void Onclick(){
        SceneManager.LoadScene("Loading");
    }
}
