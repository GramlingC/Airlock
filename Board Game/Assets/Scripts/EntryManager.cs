using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EntryManager : MonoBehaviour {

    public string player;
    private bool ready = false;
    private Button button;
    private Toggle toggle;
    private InputField input;
    private Button exitButton;

	// Use this for initialization
	void Start () {
        button = GameObject.Find("Button").GetComponent<Button>();
        toggle = GameObject.FindObjectOfType<Toggle>();
        input = GameObject.FindObjectOfType<InputField>();

        button.onClick.AddListener(begin);
        input.onEndEdit.AddListener(readyChange);

        exitButton = GameObject.Find("exitButton").GetComponent<Button>();
        exitButton.onClick.AddListener(() => { exit(); });

    }

    public void exit()
    {
        Application.Quit();
    }

    void readyChange(string u)
    {
        ready = true;
        player = u;
    }

    void begin()
    {
        if (!ready)
        {
            return;
        }
        playerManager pM = GameObject.FindObjectOfType<playerManager>();
        pM.player = player;
        pM.pirate = toggle.isOn;
        if (toggle.isOn)
        {
            SceneManager.LoadScene("PirateBoard");
        }
        else
        {
            SceneManager.LoadScene("ShipBoard");
        }
    }

	
    
}
