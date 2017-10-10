using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour {

    public float Speed = .01f;
    public bool inv;
    private Camera c;
    public string type;

	// Use this for initialization
	void Start () {
        c = gameObject.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (c.enabled)
        {
            float mov_V = Input.GetAxis("Vertical") * Speed;
            float mov_H = Input.GetAxis("Horizontal") * Speed;
            if (inv)
            {
                transform.position -= new Vector3(mov_H, 0, mov_V);
            }
            else
            {
                transform.position += new Vector3(mov_H, 0, mov_V);
            }
            
        }
    }
}
