using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class PlayButton : MonoBehaviour
{   
    public Action PlayButtonPressed;

    public void OnPlayButtonClicked()
    {
        PlayButtonPressed?.Invoke();
    }
}
