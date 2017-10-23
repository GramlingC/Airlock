using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour {

    private gameState g;
    public int roomNumber;
    public Dictionary<int, Piece> shipPieces;
    public Dictionary<int, Piece> piratePieces;
    public List<GameObject> neighbors;
    public bool Breach = false;
    private CaptureButton cb;
    public bool edge;
    Server server;
	// Use this for initialization
	void Start () {
        g = FindObjectOfType<gameState>();
        server = GameObject.FindObjectOfType<Server>();

        cb = gameObject.AddComponent<CaptureButton>() as CaptureButton;
        cb.target = this;
        cb.enabled = false;

        cb.button = new GameObject();
        cb.button.AddComponent<Button>();
        cb.button.GetComponent<Button>().onClick.AddListener(() => { capture(true); });


        shipPieces = new Dictionary<int, Piece>();
        piratePieces = new Dictionary<int, Piece>();
        for(int i = 1; i<5; i++)
        {
            shipPieces.Add(i, null);
            piratePieces.Add(i, null);
        }
        RaycastHit hit;

        Ray ray = new Ray(transform.position, Vector3.right);

        if (Physics.Raycast(ray, out hit, 5f))
            neighbors.Add(hit.collider.gameObject);

        ray = new Ray(transform.position, Vector3.left);

        if (Physics.Raycast(ray, out hit, 5f))
            neighbors.Add(hit.collider.gameObject);

        ray = new Ray(transform.position, Vector3.forward);

        if (Physics.Raycast(ray, out hit, 5f))
            neighbors.Add(hit.collider.gameObject);

        ray = new Ray(transform.position, Vector3.back);

        if (Physics.Raycast(ray, out hit, 5f))
            neighbors.Add(hit.collider.gameObject);
    }
	
    public void capture(bool s)
    {
        if (s)
        {
            server.capture(roomNumber);
            if (Breach || !edge || g.currentCamera.GetComponent<cameraController>().type == "Ship" || g.phase == 1 || g.phase == 3)
            {
                cb.enabled = false;
                return;
            }
        }
        if (g.pirateCost(2))
        {
            if (roomNumber == 14)
            {
                g.bridgeCaptured();
            }
            Breach = true;
            Destroy(cb.button);
            cb.enabled = false;
            gameObject.GetComponent<MeshRenderer>().material = Resources.Load("breach", typeof(Material)) as Material;
            g.pmax++;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (Breach)
            return;
        if (!edge || g.currentCamera.GetComponent<cameraController>().type == "Ship" || g.phase == 1 || g.phase == 3)
        {
            cb.enabled = false;
            cb.button.SetActive(false);
            return;
        }
        if (piratePieces[1] != null || piratePieces[2] != null || piratePieces[3] != null || piratePieces[4] != null)
        {
            if (shipPieces[1] == null && shipPieces[2] == null && shipPieces[3] == null && shipPieces[4] == null)
            {
                cb.enabled = true;
                cb.button.SetActive(true);
                return;
            }
        }
        cb.enabled = false;
	}
}
