using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string facebookUri;
    public string instagramUri;
    public string itchioUri;

    // Start is called before the first frame update
    public void Click(string action)
    {
        switch (action)
        {
            case "play":
                Play();
                break;
            case "facebook":
                OpenLink(this.facebookUri);
                break;
            case "instagram":
                OpenLink(this.instagramUri);
                break;
            case "itch":
                OpenLink(this.itchioUri);
                break;
        }
        
    }

    private void Play()
    {
        SceneManager.LoadScene(1);
        print("going store");
    }

    private void OpenLink(string uri)
    {
        print(uri);
        Application.OpenURL(uri);
    }
    
}
