using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureButton : MonoBehaviour
{

    private GameObject canvas;
    public GameObject button;
    public GameObject text;
    public MonoBehaviour target;

    // Use this for initialization
    void Start()
    {
        canvas = GameObject.Find("TextCanvas");
        //button = new GameObject();
        button.transform.parent = canvas.transform;
        button.AddComponent<RectTransform>();
        button.AddComponent<Image>();
        
        RectTransform childRectTransform = button.GetComponent<RectTransform>();

        childRectTransform.sizeDelta = new Vector2(Screen.width / 12, Screen.height / 16);

        Image image = button.GetComponent<Image>();

        image.sprite = GameObject.Find("readyButton").GetComponent<Image>().sprite;

        text = new GameObject();

        text.transform.parent = button.transform;

        text.AddComponent<Text>();
        //text.AddComponent<RectTransform>();
        RectTransform textTransform = text.GetComponent<RectTransform>();
        textTransform.sizeDelta = new Vector2(Screen.width / 16, Screen.height / 20);

        Text textComponent = text.GetComponent<Text>();
        textComponent.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
        textComponent.text = "Capture";
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
        /*
        Text textComponent = text.GetComponent<Text>();
        textComponent.text = message;
        */
        Vector3 pos = Camera.main.WorldToScreenPoint(target.transform.position);
        button.GetComponent<RectTransform>().position = new Vector3(pos.x, pos.y, pos.z);
        text.GetComponent<RectTransform>().position = new Vector3(pos.x, pos.y, pos.z);

    }

}