using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        T[] result = new T[length];
        Array.Copy(array, offset, result, 0, length);
        return result;
    }
}

public class PythonDataConverter : MonoBehaviour
{
    public UDPReceive udpReceive;
    public string leftData;
    public string rightData;
    public HandManagerNEW leftHandReciever;
    public HandManagerNEW rightHandReciever;

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
        string data = udpReceive.data;
        if (data.Length == 0) return;

        string[] hands = data.Split('&');

        leftData = hands[0];
        rightData = hands[1];

        leftHandReciever.data = leftData;
        rightHandReciever.data = rightData;
    }
}
