using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PythonDataConverter : MonoBehaviour
{
    public string leftData;
    public string rightData;
    public bool allowLeft = true;
    public bool allowRight = true;
    public HandManager leftHandReciever;
    public HandManager rightHandReciever;

    // Start is called before the first frame update
    void Start()
    {
        leftData = "";
        rightData = "";
    }

    // Update is called once per frame
    void Update()
    {
        // Recieves data and sends to each hand manager
        string data = UDPReceiveLandmarks.mydata;
        if (data.Length == 0) return;

        string[] hands = data.Split('&');

        leftData = hands[0];
        rightData = hands[1];

        if(allowLeft)leftHandReciever.data = leftData;
        if(allowRight)rightHandReciever.data = rightData;
    }
}
