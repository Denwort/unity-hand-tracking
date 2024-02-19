using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GestureManager : NetworkBehaviour
{
    public string data = "";
    public XRDirectInteractor interactor;
    private GameObject[] resetear;
    private Vector3[] posiciones_resetear;
    private Quaternion[] rotaciones_resetear;
    // Start is called before the first frame update
    void Start()
    {
        interactor.allowSelect = false;
        if (IsLocalPlayer)
        {
            interactor.allowHover = true;
        }
        else
        {
            interactor.allowHover = false;
        }
        resetear = Reset.lista;
        posiciones_resetear = new Vector3[resetear.Length];
        rotaciones_resetear = new Quaternion[resetear.Length];
        for (int i = 0; i < resetear.Length; i++)
        {
            posiciones_resetear[i] = resetear[i].transform.position;
            rotaciones_resetear[i] = resetear[i].transform.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }

        // Manage left hand gestures
        if (data.Equals("Closed_Fist") && !interactor.allowSelect)
        {
            interactor.allowSelect = true;
        }
        else if (data.Equals("Open_Palm") && interactor.allowSelect)
        {
            interactor.allowSelect = false;
        }
        else if (data.Equals("Victory"))
        {
            // Reset grabable objects positions
            interactor.allowSelect = false;
            for (int i = 0; i < resetear.Length; i++)
            {
                // Reset velocity, rotation and position
                resetear[i].GetComponent<Rigidbody>().MovePosition(resetear[i].GetComponent<Rigidbody>().position + new Vector3());
                resetear[i].transform.rotation = rotaciones_resetear[i];
                resetear[i].transform.position = posiciones_resetear[i];
            }
        }
    }
}
