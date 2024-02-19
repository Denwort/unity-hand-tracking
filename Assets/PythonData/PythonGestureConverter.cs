using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class PythonGestureConverter : NetworkBehaviour
{
    public string leftData;
    public string rightData;

    public bool allowLeft = true;
    public bool allowRight = true;

    public GestureManager leftHandReciever;
    public GestureManager rightHandReciever;


    // Start is called before the first frame update
    void Start()
    {
        leftData = "";
        rightData = "";
    }

    // Update is called once per frame
    void Update()
    {
        string data = UDPRecieveGestures.mydata;
        if (data.Length <= 2) return; // "" or "[]"
        data = data.Remove(0, 1);
        data = data.Remove(data.Length - 1, 1);

        string[] hands = data.Split(',');

        // Remove '' sent by python
        hands[0] = hands[0].Remove(0, 1);
        hands[0] = hands[0].Remove(hands[0].Length - 1, 1);
        hands[1] = hands[1].Remove(0, 2);
        hands[1] = hands[1].Remove(hands[1].Length - 1, 1);

        leftData = hands[0];
        rightData = hands[1];

        if (allowLeft) leftHandReciever.data = leftData;
        if (allowRight) rightHandReciever.data = rightData;


    }
}
