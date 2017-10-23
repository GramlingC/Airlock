using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.HttpClient;
using System;
using UnityEngine.SceneManagement;

public class Server : MonoBehaviour {

    public GameObject Board;
    public gameState gs;
    private List<Room> Rooms = new List<Room>();

    public bool pirate;
    public string player;
    public string read;
    public int status = 0;
    public string enemy;
    public int s = 7;
    public int p = 9;

    int frame = 0;

    // Use this for initialization
    void Start()
    {
        playerManager pM = GameObject.FindObjectOfType<playerManager>();
        pirate = pM.pirate;
        player = pM.player;

        foreach (Room r in Board.GetComponentsInChildren<Room>())
        {
            Rooms.Add(r);
        }

    }

    public void Exit()
    {
        HttpClient client = new HttpClient();

        string uri;

        if (pirate)
            uri = "http://boardserverconnection.azurewebsites.net/api/delete/1/" + player;
        else
            uri = "http://boardserverconnection.azurewebsites.net/api/delete/0/" + player;

        Debug.Log(uri);
        client.GetString(new System.Uri(uri), (r) =>
        {
            read = r.Data;
            read = read.Trim(new char[2] { '"', '\\' });
            Debug.Log(read);
        });
        playerManager pM = GameObject.FindObjectOfType<playerManager>();
        Destroy(pM.gameObject);
        Destroy(pM);
        SceneManager.LoadScene("Entry");
    }

    public void Update()
    {
        if (frame == 0)
            GoOnline();
        if (pirate && frame == 100)
            FindGame();
        if (frame == 200)
            ShowGames();
        if (pirate && frame == 300)
            Send(player);
        if (!pirate && frame == 400)
            Get();
        if (frame >400)
        {
            if (frame % 60 == 0)
                Get();
        }

        frame++;
    }

    public void Cost(int c)
    {
        Send("S_1_" + c);
    }

    public void Ready()
    {
        Send("R_1_1");
    }

    public void capture(int r)
    {
        Send("C_1_" + r.ToString());
    }

    public void attack(Piece x, Piece y)
    {
        Send("A_" + x.id.ToString() + "_" + y.id.ToString());
    }

    public void Sattack(int x, int y)
    {
        string a;
        string d;
        if (!pirate)
        {
            a = x.ToString() + "P";
            d = y.ToString() + "S";
        }
        else
        {
            a = x.ToString() + "S";
            d = y.ToString() + "P";
        }
        Piece attacker = GameObject.Find(a).GetComponent<Piece>();
        Piece defender = GameObject.Find(d).GetComponent<Piece>();
        
        if (!pirate)
        {
            defender.Endurance -= attacker.Endurance;
            defender.Security -= attacker.Security;
        }
        else
        {
            defender.Damage -= attacker.Damage;
        }
    }

    public void placePiece(Piece piece, int r)
    {
        Send("P_"+piece.id.ToString()+"_"+r.ToString());
    }

    public void SplaceShip(int id, int r)
    {
        
        Room room = Rooms[r-1];
        GameObject piece;
        GameObject clone;
        piece = GameObject.Find(id.ToString() + "S");

        if (id < 7)
        {
            clone = Instantiate(piece);
            
            clone.name = s + "S";
            clone.GetComponent<Piece>().id = s;
            s += 1;
            PieceLabel cloneL = clone.GetComponent<PieceLabel>();
            Destroy(cloneL);
        }
        else
            clone = piece;
        Piece cloneS = clone.GetComponent<Piece>();
        gs.shipCost(1);
        cloneS.picked = false;
        clone.transform.position = new Vector3(room.transform.position.x, piece.transform.position.y, room.transform.position.z);
        
        int pieces = 0;
        foreach (int k in room.shipPieces.Keys)
        {
            if (room.shipPieces[k] == null)
            {

                room.shipPieces[k] = cloneS;
                pieces = k;
                cloneS.home = room.gameObject;
                cloneS.placed = true;
                break;
            }
        }
        if (pieces == 1)
            clone.transform.position += (1.5f * Vector3.forward + 2 * Vector3.left);
        else if (pieces == 2)
            clone.transform.position += 1.5f * Vector3.forward + 2 * Vector3.right;
        else if (pieces == 3)
            clone.transform.position += 1.5f * Vector3.back + 2 * Vector3.left;
        else if (pieces == 4)
            clone.transform.position += 1.5f * Vector3.back + 2 * Vector3.right;
    }

    public void SplacePirate(int id, int r)
    {
        Room room = Rooms[r - 1];
        GameObject piece;
        GameObject clone;
        piece = GameObject.Find(id.ToString() + "P");
        if (id < 9)
        {
            clone = Instantiate(piece);
            clone.name = p + "P";
            clone.GetComponent<Piece>().id = p;
            p += 1;
            PieceLabel cloneL = clone.GetComponent<PieceLabel>();
            Destroy(cloneL);
        }
        else
        {
            clone = Instantiate(piece);
            PieceLabel cloneL = clone.GetComponent<PieceLabel>();
            Destroy(cloneL);
            Destroy(piece.GetComponent<PieceLabel>().text);
            Destroy(piece.GetComponent<PieceLabel>());
            Destroy(piece);
            clone.name = clone.GetComponent<Piece>().id.ToString() + "P";

        }
        Piece cloneS = clone.GetComponent<Piece>();
        cloneS.picked = false;
        clone.transform.position = new Vector3(room.transform.position.x, piece.transform.position.y, room.transform.position.z);

        int pieces = 0;
        foreach (int k in room.piratePieces.Keys)
        {
            if (room.piratePieces[k] == null)
            {
                room.piratePieces[k] = cloneS;
                pieces = k;
                cloneS.home = room.gameObject;
                cloneS.placed = true;
                break;
            }
        }

        if (pieces == 1)
            clone.transform.position += 1.5f * Vector3.forward;
        else if (pieces == 2)
            clone.transform.position += 1.5f * Vector3.left;
        else if (pieces == 3)
            clone.transform.position += 1.5f * Vector3.right;
        else if (pieces == 4)
            clone.transform.position += 1.5f * Vector3.back;
        gs.reveal(room);
        cloneS.T = true;
        if (id < 9)
            gs.pirateCost(cloneS.Cost);
    }

    public void genAdd()
    {
        Send("G_1_1");
    }

    public void GoOnline()
    {
        try
        {
            HttpClient client = new HttpClient();

            string uri;

            if (pirate)
                uri = "http://boardserverconnection.azurewebsites.net/api/playeradd/1/" + player;
            else
                uri = "http://boardserverconnection.azurewebsites.net/api/playeradd/0/" + player;

            client.GetString(new System.Uri(uri), (r) =>
            {
                read = r.Data;
                read = read.Trim(new char[2] { '"', '\\' });
                status++;
                Debug.Log(read);
                if (read != "Created")
                    Exit();
            });
        }
        catch (Exception E)
        {
            Exit();
        }
    }

    public void JoinGame(string target)
    {
        HttpClient client = new HttpClient();

        string uri;

        if (pirate)
            uri = "http://boardserverconnection.azurewebsites.net/api/joingame/1/" + player + "/" + target;
        else
            uri = "http://boardserverconnection.azurewebsites.net/api/joingame/0/" + player + "/" + target;
        Debug.Log(uri);
        client.GetString(new System.Uri(uri), (r) =>
        {
            enemy = target;
            read = r.Data;
            read = read.Trim(new char[2] { '"', '\\' });
            Debug.Log(read);
            status++;
            if (read != "Created")
                Exit();
        });

    }

    public void Send(string message)
    {
        HttpClient client = new HttpClient();

        string uri;

        if (pirate)
            uri = "http://boardserverconnection.azurewebsites.net/api/sendmessage/1/" + player + "/" + message;
        else
            uri = "http://boardserverconnection.azurewebsites.net/api/sendmessage/0/" + player + "/" + message;

        client.GetString(new System.Uri(uri), (r) =>
        {
            read = r.Data;
            read = read.Trim(new char[2] { '"', '\\' });
            Debug.Log(message + " sent");
        });

    }

    public void FindGame()
    {

        HttpClient client = new HttpClient();

        string uri;

        if (pirate)
            uri = "http://boardserverconnection.azurewebsites.net/api/players/1/";
        else
            uri = "http://boardserverconnection.azurewebsites.net/api/players/0/";

        client.GetString(new System.Uri(uri), (r) =>
        {
            read = r.Data;
            read = read.Trim(new char[2] { '"', '\\' });
            string[] options = read.Split('-');
            try
            {
                read = options[1];
            }
            catch
            {
                frame = 1;
                return;
            }
            Debug.Log(read);
            JoinGame(read);
        });

    }

    public void ShowGames()
    {
        HttpClient client = new HttpClient();

        string uri;

        uri = "http://boardserverconnection.azurewebsites.net/api/games/";

        client.GetString(new System.Uri(uri), (r) =>
        {
            read = r.Data;
            read = read.Trim(new char[2] { '"', '\\' });
            string[] options = read.Split('-');
            foreach(string x in options)
            {
                Debug.Log(x);
            }
        });

    }

    public void Get()
    {
        HttpClient client = new HttpClient();

        string uri;

        if (pirate)
            uri = "http://boardserverconnection.azurewebsites.net/api/players/1/" + player;
        else
            uri = "http://boardserverconnection.azurewebsites.net/api/players/0/" + player;

        client.GetString(new System.Uri(uri), (r) =>
        {
            read = r.Data;
            read = read.Trim(new char[2] { '"', '\\' });
            string[] options = read.Split('-');
            if (status < 2 && !pirate)
            {
                try
                {
                    SetEnemy(options[1]);
                    status = 2;
                }
                catch
                {
                    frame = 300;
                }
                return;
            }
            foreach (string x in options)
            {
                Debug.Log(x);
                Use(x);
            }
        });

    }

    public void Use(string x)
    {
        if (x == "ready")
        {
            return;
        }
        if (x == "end")
        {
            EndGame();
            SceneManager.LoadScene("Disconnected");
        }
        string[] xs = x.Split('_');
        if (xs[0] == "R")
        {
            gs.ready(false);
            return;
        }
        if (xs[0] == "P")
        {
            if (pirate)
            {
                SplaceShip(Convert.ToInt32(xs[1]), Convert.ToInt32(xs[2]));
                return;
            }
            else
            {
                SplacePirate(Convert.ToInt32(xs[1]), Convert.ToInt32(xs[2]));
                return;
            }
        }
        if (xs[0] == "A")
        {
            Sattack(Convert.ToInt32(xs[1]), Convert.ToInt32(xs[2]));
            return;
        }
        if (xs[0] == "G")
        {
            gs.shipCost(1);
            gs.gen++;
            return;
        }
        if (xs[0] == "S")
        {
            gs.shipCost(Convert.ToInt32(xs[2]));
            return;
        }
        if (xs[0] == "C")
        {
            Room room = Rooms[Convert.ToInt32(xs[2]) - 1];
            room.capture(false);

            return;
        }
    }


    public void SetEnemy(string target)
    {
        enemy = target;
    }

    public void EndGame()
    {
        HttpClient client = new HttpClient();

        string uri;

        uri = "http://boardserverconnection.azurewebsites.net/api/endgame/" + player;

        Debug.Log(uri);
        client.GetString(new System.Uri(uri), (r) =>
        {
            read = r.Data;
            read = read.Trim(new char[2] { '"', '\\' });
            Debug.Log(read);
        });
    }

    public void OnApplicationQuit()
    {

        HttpClient client = new HttpClient();

        string uri;

        if (pirate)
            uri = "http://boardserverconnection.azurewebsites.net/api/sendmessage/1/" + player + "/end";
        else
            uri = "http://boardserverconnection.azurewebsites.net/api/sendmessage/0/" + player + "/end";

        client.GetString(new System.Uri(uri), (r) =>
        {
            read = r.Data;
            read = read.Trim(new char[2] { '"', '\\' });
        });

        Exit();
    }
}
