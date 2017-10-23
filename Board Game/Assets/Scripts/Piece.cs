using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
    public new Camera camera;
    public int id;
    public string type;
    public string Name;
    public bool picked = false;
    public int Endurance = 10;
    public int Security = 10;
    public int Damage = 10;
    public int Cost = 10;
    public bool T =  false;
    public GameObject home;
    public bool placed = false;
    private PieceLabel label;
    private string tLabel;
    private gameState g;
    private cameraController cameraS;
    public bool E;
    public Server server;


    // Use this for initialization
    void Start ()
    {
        camera = GameObject.Find("Camera1").GetComponent<Camera>();
        cameraS = camera.GetComponent<cameraController>();
        server = GameObject.FindObjectOfType<Server>();
        home = null;
        g = FindObjectOfType<gameState>();

        label = gameObject.AddComponent<PieceLabel>() as PieceLabel;
        label.target = this;
        /*
        if (transform.parent.parent.gameObject.name == "pirateBoard")
            server = GameObject.Find("PirateServer").GetComponent<Server>();
        else
            server = GameObject.Find("ShipServer").GetComponent<Server>();
            */
    }
	
	// Update is called once per frame
	void Update () {
        camera = g.currentCamera.gameObject.GetComponent<Camera>();



        tLabel = Name;
        if (Name == "Generators")
        {
            tLabel += ":\n" + g.gen;
            label.message = tLabel;
            return;
        }
        tLabel += "-";
        tLabel += Cost;
        if (E)
        {
            tLabel += "\nE:";
            tLabel += Endurance;
        }
        else
        {
            tLabel += "\nS:";
            tLabel += Security;
        }
        if (type == "Ship")
            tLabel += " D:";
        else
            tLabel += " H:";
        tLabel += Damage;

        
        if (type == "Ship" && cameraS.type == "Pirate" && !T)
        {
            tLabel = "";
        }

        label.message = tLabel;

        if (picked)
        {
            Vector2 distance = Input.mousePosition - camera.WorldToScreenPoint(transform.position);
            if (cameraS.inv)
                transform.position -= new Vector3(distance.x, 0, distance.y) / 100;
            else
                transform.position += new Vector3(distance.x, 0, distance.y) / 100;
            
        }
        else
        {
            if (type == "Pirate")
            {
                if (Damage <= 0)
                { 
                    Destroy(label.text);
                    Destroy(label.gameObject);
                    Destroy(this.gameObject);
                }
            }
            else
            {
                if (E)
                {
                    if (Endurance <= 0)
                    {
                        Destroy(label.text);
                        Destroy(label.gameObject);
                        Destroy(this.gameObject);
                    }
                }
                else
                {
                    if (Security <= 0)
                    {
                        Destroy(label.text);
                        Destroy(label.gameObject);
                        Destroy(this.gameObject);
                    }
                }
            }
        }

		
	}

    void Reveal(Room r)
    {
        foreach (Piece sP in r.shipPieces.Values)
        {
            sP.T = true;
        }
    }

    void Return()
    {
        picked = false;
        int pieces = 0;
        transform.position = new Vector3(home.transform.position.x, transform.position.y -1, home.transform.position.z);
        Room room = (Room)home.GetComponent(typeof(Room));
        if (type == "Ship")
        {
            foreach (int k in room.shipPieces.Keys)
            {
                if (room.shipPieces[k] == this)
                {
                    pieces = k;
                    break;
                }
            }
            if (pieces == 1)
                transform.position += (1.5f*Vector3.forward + 2*Vector3.left);
            else if (pieces == 2)
                transform.position += 1.5f*Vector3.forward + 2*Vector3.right;
            else if (pieces == 3)
                transform.position += 1.5f*Vector3.back + 2*Vector3.left;
            else if (pieces == 4)
                transform.position += 1.5f*Vector3.back + 2*Vector3.right;
        }
        else
        {
            foreach (int k in room.piratePieces.Keys)
            {
                if (room.piratePieces[k] == null)
                {
                    room.piratePieces[k] = this;
                    pieces = k;
                    break;
                }
            }
            if (pieces == 1)
                transform.position += 1.5f*Vector3.forward;
            else if (pieces == 2)
                transform.position += 1.5f*Vector3.left;
            else if (pieces == 3)
                transform.position += 1.5f*Vector3.right;
            else if (pieces == 4)
                transform.position += 1.5f*Vector3.back;
        }
    }
    

    void attack(Piece target)
    {
        if (target == null)
            return;
        if (type == "Pirate")
        {
            if (!home.gameObject.GetComponent<Room>().shipPieces.ContainsValue(target))
            {
                return;
            }
            target.Endurance -= Endurance;
            target.Security -= Security;
            T = true;
            Debug.Log(target.Endurance);
            Return();
            server.attack(this, target);
        }
        else
        {
            if (!home.gameObject.GetComponent<Room>().piratePieces.ContainsValue(target))
            {
                return;
            }


            if (E == true)
            {
                Return();
                if (!g.shipCost(Cost))
                    return;
                else
                    server.Cost(Cost);
            }
            target.Damage -= Damage;
            server.attack(this, target);

        }
    }

    void OnMouseDown()
    {
        if ((g.phase % 2 == 1 || cameraS.type == "Ship") && type == "Pirate")
            return;

        if ((g.phase % 2 == 0 || cameraS.type == "Pirate") && type == "Ship")
            return;

        if ((g.phase > 2 && !placed && type == "Ship") || (g.phase < 3 && placed))
            return;

        if (type == "Pirate" && T == true && placed)
            return;

        if (Name == "Generators")
        {
            if (g.shipCost(1))
            {
                g.gen++;
                server.genAdd();
            }
            return;
        }

        RaycastHit hit;

        Physics.Raycast(transform.position, Vector3.down, out hit);
        
        if (!picked)
        {
            if (!placed)
            {
                
                GameObject clone = Instantiate(this.gameObject);

                Piece cloneS = clone.GetComponent<Piece>();
                cloneS.picked = true;

                clone.transform.position += Vector3.up;
                PieceLabel cloneL = clone.GetComponent<PieceLabel>();
                Destroy(cloneL);
                return;
            }
            picked = true;
            transform.position += Vector3.up;
            if (hit.transform.gameObject.tag == "Room")
            {
                Room room = (Room)hit.transform.gameObject.GetComponent(typeof(Room));
                if (type == "Ship")
                {
                    if (E == false)
                    {
                        picked = false;
                        transform.position -= Vector3.up;
                        if (!g.shipCost(Cost))
                            return;
                        server.Cost(Cost);
                        foreach (Piece p in room.piratePieces.Values)
                        {
                            attack(p);
                        }
                    }
                           
                }
                else
                {
                    foreach (int k in room.piratePieces.Keys)
                    {
                        if (room.piratePieces[k] == this)
                        {
                            room.piratePieces[k] = null;
                            break;
                        }
                    }
                }
            }
        }
        else 
        {
            if (!Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                if (!placed)
                {
                    Destroy(label.text);
                    Destroy(label.gameObject);
                    Destroy(gameObject);
                }
                else
                    Return();
                return;
            }
            if (hit.transform.gameObject.tag == "Room")
            {

                if (type == "Ship" && placed)
                {
                    Return();
                    return;
                }
                picked = false;
                transform.position -= Vector3.up;
                transform.position = new Vector3(hit.transform.position.x,transform.position.y,hit.transform.position.z);
                Room room = (Room)hit.transform.gameObject.GetComponent(typeof(Room));
                int pieces = 0;


                if (type == "Ship")
                {
                    if (room.Breach)
                    {
                        transform.position += Vector3.up;
                        picked = true;
                        return;
                    }
                        
                    foreach (int k in room.shipPieces.Keys)
                    {
                        if (room.shipPieces[k] == null)
                        {
                            if (!placed && !g.shipCost(1))
                            {
                                Destroy(label.text);
                                Destroy(label.gameObject);
                                Destroy(gameObject);
                                return;
                            }
                            room.shipPieces[k] = this;
                            pieces = k;
                            home = hit.transform.gameObject;
                            placed = true;
                            break;
                        }
                    }
                    if (pieces == 1)
                        transform.position += (1.5f*Vector3.forward + 2*Vector3.left);
                    else if (pieces == 2)
                        transform.position += 1.5f*Vector3.forward + 2*Vector3.right;
                    else if (pieces == 3)
                        transform.position += 1.5f*Vector3.back + 2*Vector3.left;
                    else if (pieces == 4)
                        transform.position += 1.5f*Vector3.back + 2*Vector3.right;
                    else
                    {
                        picked = true;
                        transform.position += Vector3.up;
                    }
                    server.placePiece(this,room.roomNumber);
                    
                    id = server.s;
                    name = id + "S";
                    server.s++;
                }


                else if (type == "Pirate")
                {
                    foreach (int k in room.piratePieces.Keys)
                    {
                        if (room.piratePieces[k] == null)
                        {
                            
                            if (!placed)
                            {
                                if (!placed && !g.pirateCost(Cost))
                                {
                                    Destroy(label.text);
                                    Destroy(label.gameObject);
                                    Destroy(gameObject);
                                    return;
                                }
                                if (!room.Breach)
                                {
                                    picked = true;
                                    transform.position += Vector3.up;
                                    return;
                                }

                            }
                            else if (home != hit.transform.gameObject)
                            {
                                if (!room.neighbors.Contains(home.gameObject))
                                {
                                    picked = true;
                                    transform.position += Vector3.up;
                                    return;
                                }
                                T = true;
                                g.reveal(room);
                                
                            }
                            room.piratePieces[k] = this;
                            home = hit.transform.gameObject;
                            pieces = k;
                            break;
                        }
                    }
                    if (pieces == 1)
                        transform.position += 1.5f*Vector3.forward ;
                    else if (pieces == 2)
                        transform.position += 1.5f*Vector3.left ;
                    else if (pieces == 3)
                        transform.position += 1.5f*Vector3.right ;
                    else if (pieces == 4)
                        transform.position += 1.5f*Vector3.back ;
                    else
                    {
                        picked = true;
                        transform.position += Vector3.up;
                    }
                    server.placePiece(this, room.roomNumber);
                    if (!placed)
                    {
                        placed = true;
                        id = server.p;
                        name = server.p + "P";
                        server.p++;
                    }
                    
                }

            }
            else if (hit.transform.gameObject.tag == "Piece")
            {
                Piece target = (Piece)hit.transform.GetComponent(typeof(Piece));
                if (target.type != type)
                {

                    attack(target);
                }

            }
        }
    }

}
