using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameState : MonoBehaviour {

    public cameraController currentCamera;
    public int phase = 3;
    public int pr;
    public int sr1;
    public int sr2;
    public int turnCount = 1;
    public int gen;
    public int max1 = 10;
    public int max2 = 5;
    public int pmax = 10;
    private GameObject canvas;
    private Button readyButton;
    private Button exitButton;
    private Text phaseText;
    private Text pirateText;
    private Text shipText;
    Server server;


    // Use this for initialization
    void Start ()
    {
        server = GameObject.FindObjectOfType<Server>();
        canvas = GameObject.Find("TextCanvas");
        readyButton = GameObject.Find("readyButton").GetComponent<Button>();
        phaseText = GameObject.Find("phaseText").GetComponent<Text>();
        
        pirateText = GameObject.Find("pirateText").GetComponent<Text>();
        shipText = GameObject.Find("shipText").GetComponent<Text>();

        switchCamera();
        phase = 1;

        readyButton.onClick.AddListener(() => { ready(true); });
        exitButton = GameObject.Find("exitButton").GetComponent<Button>();
        exitButton.onClick.AddListener(() => { exit(); });

    }

    public void exit()
    {
        Application.Quit();
    }

    public void ready(bool s)
    {
        
        if (!s || (phase % 2 == 1 && currentCamera.type == "Ship") || (phase % 2 == 0 && currentCamera.type == "Pirate"))
        {
            if (s)
                server.Ready();
            if (phase == 1)
            {
                phase++;
                pr = pmax;
            }
            else if (phase == 2)
            {
                rest();
                phase = 4;
                foreach (Room r in GameObject.Find("Board").GetComponentsInChildren(typeof(Room)))
                {
                    foreach (Piece p in r.piratePieces.Values)
                    {
                        if (p != null)
                            reveal(r);
                    }
                }
            }
            else if (phase == 4)
            {

                phase = 3;
                sr2 += gen;
                if (sr2 > max2)
                    sr2 = max2;
            }
            else if (phase == 3)
            {
                rest();
                if (turnCount < 3)
                {
                    turnCount++;
                    phase = 4;
                }
                else
                {
                    gen = 0;
                    phase = 1;
                    turnCount = 1;
                    sr1 += 3;
                    if (sr1 > max1)
                        sr1 = max1;
                }
            }
        }
    }

    public void switchCamera()
    {
        /*
        if (currentCamera == pC)
        {
            sC.enabled = true;
            currentCamera = sC;
            pC.enabled = false;
        }
        else
        {
            pC.enabled = true;
            currentCamera = pC;
            sC.enabled = false;
        }
        */
    }

    void rest()
    {
        foreach (Room r in GameObject.Find("Board").GetComponentsInChildren(typeof(Room)))
        {
            foreach (Piece p in r.piratePieces.Values)
            {
                if (p != null)
                    p.T = false;
            }
        }
    }

    public void reveal(Room r)
    {
        foreach (Piece p in r.shipPieces.Values)
        {
            if (p != null)
                p.T = true;
        }
    }

    public bool shipCost(int i)
    {
        int d = sr2 - i;
        if (d < 0)
        {
            if (sr1 + d >= 0)
            {
                sr1 += d;
                sr2 = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            sr2 -= i;
            return true;
        }
    }

    public bool pirateCost(int i)
    {
        int d = pr - i;
        if (d < 0)
        {
            return false;
        }
        else
        {
            pr = d;
            return true;
        }
    }

    


	// Update is called once per frame
	void Update () {

        if (server.status == 0)
        {
            phaseText.text = "Going online...";
        }
        else if (server.status == 1)
        {
            if (currentCamera.type == "Pirate")
            {
                phaseText.text = "Searching for ships to board...";
            }
            else
            {
                phaseText.text = "Waiting to be boarded by pirates...";
            }
        }
        if (server.status < 2)
        {
            pirateText.text = "Rations:\n" + pr;
            shipText.text = "Core: " + sr1 + "\nCapacitors: " + sr2;
            return;
        }
        else if (phase == 3 || phase == 4)
        {
            phaseText.text = "Wave ";
            phaseText.text += turnCount;
        }
        else
        {
            phaseText.text = "Setup";
        }

        if (phase == 1 || phase == 3)
        {
            phaseText.text += "\nShip's turn";
        }
        else
            phaseText.text += "\nPirate's turn";

        pirateText.text = "Rations:\n" + pr;
        shipText.text = "Core: " + sr1 + "\nCapacitors: " + sr2;

    }
}
