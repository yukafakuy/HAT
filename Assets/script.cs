using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class script : MonoBehaviour
{

    public Button StartButton;
    public TMP_Text HelloText;

    // Start is called before the first frame update
    void Start()
    { 
        StartButton.onClick.AddListener(StartButtonOnClick);
    }

    void StartButtonOnClick()
    {
        HelloText.text = "Unity Baptism Completed!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
