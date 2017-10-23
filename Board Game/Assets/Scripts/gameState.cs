using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameState : MonoBehaviour {

    public cameraController currentCamera;
    public Camera cam1;
    public Camera cam2;
    public int phase = 3;
    public int pr;
    public int sr1;
    public int sr2;
    public int turnCount = 1;
    public int gen;
    public int max1 = 10;
    public int max2 = 5;
    public int pmax = 10;
    public int round = 10;
    private GameObject canvas;
    private Button readyButton;
    private Button exitButton;
    private Button cameraButton;
    private Text phaseText;
    private Text pirateText;
    private Text shipText;
    Server server;


    // Use this for initialization
    void Start ()
    {
        cam1 = GameObject.Find("Camera1").GetComponent<Camera>();
        cam2 = GameObject.Find("Camera2").GetComponent<Camera>();
        currentCamera = cam1.GetComponent<cameraController>();
        cam2.enabled = false;
        server = GameObject.FindObjectOfType<Server>();
        canvas = GameObject.Find("TextCanvas");
        readyButton = GameObject.Find("readyButton").GetComponent<Button>();
        phaseText = GameObject.Find("phaseText").GetComponent<Text>();
        
        pirateText = GameObject.Find("pirateText").GetComponent<Text>();
        shipText = GameObject.Find("shipText").GetComponent<Text>();

        //switchCamera();
        phase = 1;

        readyButton.onClick.AddListener(() => { ready(true); });
        exitButton = GameObject.Find("exitButton").GetComponent<Button>();
        exitButton.onClick.AddListener(() => { exit(); });
        cameraButton = GameObject.Find("cameraButton").GetComponent<Button>();
        cameraButton.onClick.AddListener(() => { switchCamera(); });

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
                    round--;
                    if (round == 0)
                        end();
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
        
        if (currentCamera == cam1.GetComponent<cameraController>())
        {
            cam2.enabled = true;
            currentCamera = cam2.GetComponent<cameraController>();
            cam1.enabled = false;
        }
        else
        {
            cam1.enabled = true;
            currentCamera = cam1.GetComponent<cameraController>();
            cam2.enabled = false;
        }
        
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

    public void bridgeCaptured()
    {
        if (currentCamera.type == "Pirate")
        {
            SceneManager.LoadScene("Victory");
        }
        else
            SceneManager.LoadScene("Defeat");
    }

    void end()
    {
        if (currentCamera.type == "Ship")
        {
            SceneManager.LoadScene("Victory");
        }
        else
            SceneManager.LoadScene("Defeat");
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
            phaseText.text = "Round " + round;
            phaseText.text += ": Wave ";
            phaseText.text += turnCount;
        }
        else
        {
            phaseText.text = "Round " + round;
            phaseText.text += ": Setup";
        }

        if (phase == 1 || phase == 3)
        {
            if (currentCamera.type == "Pirate")
            {
                phaseText.text += "\n"+ server.enemy+"'s turn";
            }
            else
                phaseText.text += "\n" + server.player + "'s turn";
        }
        else
        {
            if (currentCamera.type == "Ship")
            {
                phaseText.text += "\n" + server.enemy + "'s turn";
            }
            else
                phaseText.text += "\n" + server.player + "'s turn";
        }

        pirateText.text = "Rations:\n" + pr;
        shipText.text = "Core: " + sr1 + "\nCapacitors: " + sr2;

    }
}
