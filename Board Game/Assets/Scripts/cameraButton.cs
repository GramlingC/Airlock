using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cameraButton : MonoBehaviour {

    private bool p;
    private Text t;
    private gameState g;
    private Button B;

	// Use this for initialization
	void Start ()
    {
        B = gameObject.GetComponent<Button>();
        B.onClick.AddListener(OnClick);

        g = FindObjectOfType<gameState>();
        t = GetComponentInChildren<Text>();
        p = false;
        updateText();
	}

    void updateText()
    { 
        if (p)
        {
            t.text = "Pirate Camera";
        }
        else
        {
            t.text = "Ship Camera";
        }
    }

    void OnClick()
    {
        //Debug.Log("Click");
        p = !p;
        updateText();
        g.switchCamera();
    }
	
	// Update is called once per frame
	void Update ()
    {   	
	}
}
