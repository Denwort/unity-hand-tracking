using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class HandManagerRAW : MonoBehaviour
{
    public GameObject[] handPoints;
    public string data;
    private Vector3[] input_coordenadas;
    public GameObject visual;

    public string handedness;
    public GameObject[] destination;
    private Vector3 ajuste;

    private OneEuroFilter<Vector3>[] vector3Filter;

    // Start is called before the first frame update
    void Start()
    {
        input_coordenadas = new Vector3[21];
        vector3Filter = new OneEuroFilter<Vector3>[27]; 
        for (int i = 0; i < vector3Filter.Length; i++) vector3Filter[i] = new OneEuroFilter<Vector3>(60.0f); // freq, mincutoff, beta, dcutoff
        ajuste = new Vector3(0.5f,2,0);
        getDistance(0, 1);
        getDistance(1, 2);
        getDistance(2, 3);
        getDistance(3, 4);
        getDistance(4, 5);

        getDistance(0, 6);
        getDistance(6, 7);
        getDistance(7, 8);
        getDistance(8, 9);
        getDistance(9, 10);

        getDistance(0, 11);
        getDistance(11, 12);
        getDistance(12, 13);
        getDistance(13, 14);
        getDistance(14, 15);

        getDistance(0, 16);

        getDistance(0, 17);
        getDistance(17, 18);
        getDistance(18, 19);
        getDistance(19, 20);
        getDistance(20, 21);

        getDistance(0, 22);
        getDistance(22, 23);
        getDistance(23, 24);
        getDistance(24, 25);

    }

    // Update is called once per frame
    void Update()
    {

        // Transform strings to Vector3
        if (data.Length <= 2) return; // "" or "[]"
        data = data.Remove(0, 1);
        data = data.Remove(data.Length - 1, 1);
        string[] points = data.Split(",");
        if (points.Length != 63) return; // Hand points data incomplete
        for (int i = 0; i < 21; i++)
            //input_coordenadas[i] = new Vector3(float.Parse(points[i * 3]) *-1.0f, float.Parse(points[i * 3 + 1]), float.Parse(points[i * 3 + 2]) * -1.0f );
            input_coordenadas[i] = new Vector3(float.Parse(points[i * 3]) * -1.0f, float.Parse(points[i * 3 + 1]) * -1.0f, float.Parse(points[i * 3 + 2]) * -1.0f);

        handPoints[9].transform.position = input_coordenadas[9] + ajuste;
        // Get wrist coordinates
        adjustPosition(0, 9, 0.0f);

        // Get other handpoints coordinates
        // Thumb
        adjustPosition(1, 0, 0.04859972f);
        adjustPosition(2, 1, 0.03251329f);
        adjustPosition(3, 2, 0.03379273f);
        adjustPosition(4, 3, 0.02462161f);
        // Index
        adjustPosition(5, 0, 0.10226781f);
        adjustPosition(6, 5, 104.40f); //faltaaa
        adjustPosition(7, 6, 92.03f);
        adjustPosition(8, 7, 89.19f);
        // Middle
        adjustPosition(9, 0, 390.25f);
        adjustPosition(10, 9, 111.04f);
        adjustPosition(11, 10, 107.04f);
        adjustPosition(12, 11, 94.26f);
        // Ring
        adjustPosition(13, 0, 369.94f);
        adjustPosition(14, 13, 103.32f);
        adjustPosition(15, 14, 96.88f);
        adjustPosition(16, 15, 96.33f);
        // Pinky
        adjustPosition(17, 0, 341.72f);
        adjustPosition(18, 17, 85.59f);
        adjustPosition(19, 18, 62.37f);
        adjustPosition(20, 19, 90.14f);

        // Extrapolate to create extra handpoints for XR Hands
        handPoints[21].transform.position = Vector3.Lerp(handPoints[0].transform.position, handPoints[5].transform.position, 0.45f);
        handPoints[22].transform.position = Vector3.Lerp(handPoints[0].transform.position, handPoints[9].transform.position, 0.4f);
        handPoints[23].transform.position = Vector3.Lerp(handPoints[0].transform.position, handPoints[13].transform.position, 0.4f);
        handPoints[24].transform.position = Vector3.Lerp(handPoints[0].transform.position, handPoints[17].transform.position, 0.45f);

        handPoints[25].transform.position = Vector3.Lerp(handPoints[0].transform.position, handPoints[9].transform.position, 0.55f);

        handPoints[26].transform.position = handPoints[9].transform.position;



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
        adjustWrist(destination[0], destination[11], destination[7], handedness);

        aimTo(1, 2);
        aimTo(2, 3);
        aimTo(3, 4);
        aimTo(4, 5);
        aimBack(5, 4);

        aimTo(6, 7);
        aimTo(7, 8);
        aimTo(8, 9);
        aimTo(9, 10);
        aimBack(10, 9);

        aimTo(11, 12);
        aimTo(12, 13);
        aimTo(13, 14);
        aimTo(14, 15);
        aimBack(15, 14);

        aimBack(16, 0);

        aimTo(17, 18);
        aimTo(18, 19);
        aimTo(19, 20);
        aimTo(20, 21);
        aimBack(21, 20);

        // aimTo2(22, 23);
        aimTo2(23, 24);
        aimTo2(24, 25);
        aimBack(25, 24);

        // Actualizar la forma del collider
        updateCollider();

    }

    void adjustPosition(int handpoint, int handpoint_padre, float factor)
    {
        //Vector3 point = input_coordenadas[handpoint]/1000;
        Vector3 point = input_coordenadas[handpoint];
        //Vector3 point = handPoints[handpoint_padre].transform.position + (Vector3.Normalize(input_coordenadas[handpoint] - input_coordenadas[handpoint_padre]) * factor * 0.00025f);
        handPoints[handpoint].transform.position = point;
    }

    void sendToHandModel(int a, int b)
    {
        destination[a].transform.position = vector3Filter[b].Filter(handPoints[b].transform.position) + new Vector3(+0.4f, +1.7f, +1.5f);
    }

    void aimTo(int a, int b)
    {
        Vector3 forwardDirection = destination[b].transform.position - destination[a].transform.position;
        Quaternion orientation = Quaternion.LookRotation(forwardDirection, destination[0].transform.up);
        destination[a].transform.rotation = orientation;
    }
    void aimBack(int a, int b)
    {
        destination[b].transform.rotation = destination[a].transform.rotation;
    }
    void aimTo2(int a, int b)
    {
        Vector3 forwardDirection = destination[b].transform.position - destination[a].transform.position;
        Quaternion orientation = Quaternion.LookRotation(forwardDirection, destination[0].transform.right);
        destination[a].transform.rotation = orientation;
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
        visual.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
        visual.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void getDistance(int a, int b)
    {
        print(a+ "->"+ b+ " : "+ Vector3.Distance(destination[a].transform.position, destination[b].transform.position));
    }
}
