using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerManager : MonoBehaviour {

    public string player;
    public bool pirate;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(this);
    }
}
