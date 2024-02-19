using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandManager : NetworkBehaviour
{
    public string data;
    private Vector3[] input;

    public GameObject[] miniHand;
    public string handedness;

    public GameObject[] handPoints;
    public GameObject handVisualModel;

    private Vector3 offset;
    private OneEuroFilter<Vector3>[] vector3Filter;

    // Start is called before the first frame update
    void Start()
    {
        
        input = new Vector3[21];
        vector3Filter = new OneEuroFilter<Vector3>[27]; 
        for (int i = 0; i < vector3Filter.Length; i++) vector3Filter[i] = new OneEuroFilter<Vector3>(60.0f); // freq, mincutoff, beta, dcutoff
        offset = new Vector3(0.8f, 2.0f, 1.6f);

    }

    // Update is called once per frame
    void Update()
    {

        if (!IsOwner) { return; }

        // Transform strings to Vector3
        if (data.Length <= 2) return; // "" or "[]"
        data = data.Remove(0, 1);
        data = data.Remove(data.Length - 1, 1);
        string[] points = data.Split(",");
        if (points.Length != 63) return; // Hand points data incomplete
        for (int i = 0; i < 21; i++)
            input[i] = new Vector3(float.Parse(points[i * 3]) * -1.0f, float.Parse(points[i * 3 + 1]) * -1.0f, float.Parse(points[i * 3 + 2]) * -1.0f);

        // Set wrist coordinates
        miniHand[0].transform.position = input[0] + offset;

        // Get other handpoints coordinates
        // Thumb
        adjustPosition(1, 0, 0.04859972f);
        adjustPosition(2, 1, 0.03251329f);
        adjustPosition(3, 2, 0.03379273f);
        adjustPosition(4, 3, 0.02462161f);
        // Index
        adjustPosition(5, 0, 0.10226781f);
        adjustPosition(6, 5, 0.03792713f);
        adjustPosition(7, 6, 0.02430382f);
        adjustPosition(8, 7, 0.02238879f);
        // Middle
        adjustPosition(9, 0, 0.09691545f); 
        adjustPosition(10, 9, 0.04292690f);
        adjustPosition(11, 10, 0.02754972f);
        adjustPosition(12, 11, 0.02499187f); 
        // Ring
        adjustPosition(13, 0, 0.09232190f); 
        adjustPosition(14, 13, 0.03899639f);
        adjustPosition(15, 14, 0.02657295f);
        adjustPosition(16, 15, 0.02438015f); 
        // Pinky
        adjustPosition(17, 0, 0.08782507f); // 0->6 + 0->7
        adjustPosition(18, 17, 0.03072081f); // 7->8
        adjustPosition(19, 18, 0.02031080f); // 8->9
        adjustPosition(20, 19, 0.02195729f); // 9->10

        // Extrapolate to create extra handpoints for XR Hands
        // Metacarpals: index, middle, ring, little
        miniHand[21].transform.position = Vector3.Lerp(miniHand[0].transform.position, miniHand[5].transform.position, 0.04256283f/0.10226781f); // 0->1 / ( 0->1 + 1->2 )
        miniHand[22].transform.position = Vector3.Lerp(miniHand[0].transform.position, miniHand[9].transform.position, 0.03531365f/0.09691545f); // 0->11 / ( 0->11 + 11->12 )
        miniHand[23].transform.position = Vector3.Lerp(miniHand[0].transform.position, miniHand[13].transform.position, 0.03834479f/0.09232190f); // 0->17 / ( 0->17 + 17->18 )
        miniHand[24].transform.position = Vector3.Lerp(miniHand[0].transform.position, miniHand[17].transform.position, 0.04217497f/0.08782507f); // 0->6 / ( 0->6 + 6->7 )
        // Palm
        miniHand[25].transform.position = Vector3.Lerp(miniHand[0].transform.position, miniHand[9].transform.position, 0.04784770f/0.09691545f); // 0->16 / ( 0->11 + 0->12 )
        // Model
        miniHand[26].transform.position = miniHand[9].transform.position; // Center of hand collider is in middle proximal


        // Send calculated points to the hand model
        sendToHandModel(26, 0);
        sendToHandModel(0, 0);
        // Index
        sendToHandModel(1, 21);
        sendToHandModel(2, 5);
        sendToHandModel(3, 6);
        sendToHandModel(4, 7);
        sendToHandModel(5, 8);
        
        // Little
        sendToHandModel(6, 24);
        sendToHandModel(7, 17);
        sendToHandModel(8, 18);
        sendToHandModel(9, 19);
        sendToHandModel(10, 20);
        // Middle
        sendToHandModel(11, 22);
        sendToHandModel(12, 9);
        sendToHandModel(13, 10);
        sendToHandModel(14, 11);
        sendToHandModel(15, 12);
        // Palm
        sendToHandModel(16, 25);
        // Ring
        sendToHandModel(17, 23);
        sendToHandModel(18, 13);
        sendToHandModel(19, 14);
        sendToHandModel(20, 15);
        sendToHandModel(21, 16);
        // Thumb
        sendToHandModel(22, 1);
        sendToHandModel(23, 2);
        sendToHandModel(24, 3);
        sendToHandModel(25, 4);
        // Adjust wrist rotation
        adjustWrist(handPoints[0], handPoints[11], handPoints[7], handedness);

        // Actualizar la forma del collider
        updateCollider();

    }

    void adjustPosition(int handpoint, int handpoint_padre, float factor)
    {
        //Vector3 point = input_coordenadas[handpoint]/1000;
        //Vector3 point = input_coordenadas[handpoint];
        Vector3 point = miniHand[handpoint_padre].transform.position + (Vector3.Normalize(input[handpoint] - input[handpoint_padre]) * factor);
        miniHand[handpoint].transform.position = point;
    }

    void sendToHandModel(int a, int b)
    {
        /*
        XRHandJoint x = hand.GetJoint(XRHandJointID.IndexTip);
        Pose newpose = new Pose();
        x.SetPose(newpose);
        */
        handPoints[a].transform.position = vector3Filter[b].Filter(miniHand[b].transform.position); // vector3Filter[b].Filter(miniHand[b].transform.position);
    }

    void adjustWrist(GameObject wrist, GameObject forward, GameObject right, string handedness)
    {
        Vector3 forwardDirection = forward.transform.position - wrist.transform.position;
        Vector3 rightDirection = new Vector3();
        if (handedness.Equals("Left"))
            rightDirection = wrist.transform.position - right.transform.position;
        if (handedness.Equals("Right"))
            rightDirection = right.transform.position - wrist.transform.position;
        Vector3 upDirection = Vector3.Cross(forwardDirection, rightDirection);

        Quaternion orientation = Quaternion.LookRotation(forwardDirection, upDirection);

        wrist.transform.rotation = orientation;
    }

    void updateCollider()
    {
        Mesh mesh = new Mesh();
        handVisualModel.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
        handVisualModel.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

}
