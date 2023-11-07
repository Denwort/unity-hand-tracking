using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class PythonGestureConverter : MonoBehaviour
{
    public UDPReceive udpReceive;
    public string leftData;
    public string rightData;
    public XRDirectInteractor leftInteractor;
    public XRDirectInteractor rightInteractor;
    public GameObject[] resetear;
    private Vector3[] posiciones_resetear;
    private bool reseteado = false;


    // Start is called before the first frame update
    void Start()
    {
        leftData = "";
        rightData = "";
        leftInteractor.allowSelect = false;
        rightInteractor.allowSelect = false;
        posiciones_resetear = new Vector3[resetear.Length];
        for(int i = 0; i < resetear.Length; i++)
        {
            posiciones_resetear[i] = resetear[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        string data = udpReceive.data;
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

        // Manage left hand gestures
        if (leftData.Equals("Closed_Fist") && leftInteractor.allowSelect!= true){
            leftInteractor.allowSelect = true;
        } else if(leftData.Equals("Open_Palm") && leftInteractor.allowSelect != false){
            leftInteractor.allowSelect = false;
        }

        // Manage right hand gestures
        if (rightData.Equals("Closed_Fist") && rightInteractor.allowSelect!= true){
            reseteado = false;
            rightInteractor.allowSelect = true;
        } else if (rightData.Equals("Open_Palm") && rightInteractor.allowSelect!= false){
            rightInteractor.allowSelect = false;
        } else if(rightData.Equals("Victory") && reseteado == false)
        {
            // Reset grabable objects positions
            reseteado = true;
            rightInteractor.allowSelect = false;
            leftInteractor.allowSelect = false;
            for(int i = 0; i < resetear.Length; i++)
            {
                resetear[i].transform.position = posiciones_resetear[i];
                resetear[i].GetComponent<Rigidbody>().velocity = new Vector3();

            }
        }
        
    }
}
