using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceLabel : MonoBehaviour
{

    public string message;
    private GameObject canvas;
    public GameObject text;
    public MonoBehaviour target;

    // Use this for initialization
    void Start()
    {
        canvas = GameObject.Find("TextCanvas");
        text = new GameObject();
        text.transform.parent = canvas.transform;
        text.AddComponent<RectTransform>();
        text.AddComponent<Text>();
        RectTransform childRectTransform = text.GetComponent<RectTransform>();
        //RectTransform r = GameObject.Find("TextCanvas/PickUp").GetComponent<RectTransform>();
        //childRectTransform.position = r;
        childRectTransform.sizeDelta = new Vector2(Screen.width / 10, Screen.height / 14);
        Text textComponent = text.GetComponent<Text>();
        textComponent.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
        textComponent.text = message;
        textComponent.color = Color.black;
        textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
        textComponent.verticalOverflow = VerticalWrapMode.Truncate;
        textComponent.alignment = TextAnchor.UpperCenter;
        textComponent.alignByGeometry = false;
        textComponent.resizeTextForBestFit = true;
        //textComponent.resizeTextMinSize = 15;
        //textComponent.resizeTextMaxSize = 50;


    }

    // Update is called once per frame
    void Update()
    {

        Text textComponent = text.GetComponent<Text>();
        textComponent.text = message;
        Vector3 pos = Camera.main.WorldToScreenPoint(target.transform.position);
        text.GetComponent<RectTransform>().position = new Vector3(pos.x, pos.y + (Screen.height / 18), pos.z);

    }

}