using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class HandManagerOLD : MonoBehaviour
{
    public GameObject[] handPoints;
    public string data;
    private SphereCollider esfera;
    private float radio;
    private Vector3[] input_coordenadas;
    private Vector3[] input_coordenadas_correctas;
    private float distanciaPrevenidaCentro = 0;
    private float distanciaPrevenidaArticulaciones = 5;
    private Vector3 centro;
    private float rangoMinimoDeteccion = -450;
    private float tamanhoMano = 0.25f;
    private float[] tiempos_iniciales;

    public string handedness;
    public GameObject[] destination;
    private Vector3 ajuste;

    public float speed = 1.0F;
    private float startTime;
    private float journeyLength;

    private OneEuroFilter<Vector3>[] vector3Filter;

    // Start is called before the first frame update
    void Start()
    {
        
        esfera = GetComponent<SphereCollider>();
        radio = esfera.radius;
        input_coordenadas = new Vector3[21];
        input_coordenadas_correctas = new Vector3[21];
        centro = new Vector3(0, 0, 0);

        tiempos_iniciales = new float[27];

        ajuste = new Vector3(-0.55f, +1.10f, +1.70f);

        vector3Filter = new OneEuroFilter<Vector3>[27];
        for (int i = 0; i < vector3Filter.Length; i++) vector3Filter[i] = new OneEuroFilter<Vector3>(60.0f); // freq, mincutoff, beta, dcutoff

    }

    // Update is called once per frame
    void Update()
    {
        
        // Transform strings to Vector3
        if (data.Length <= 2) return; // "" or "[]"
        data = data.Remove(0, 1);
        data = data.Remove(data.Length-1, 1);
        string[] points = data.Split(",");
        if (points.Length != 63) return; // Hand points data incomplete
        for (int i = 0; i < 21; i++)
            input_coordenadas[i] = new Vector3(float.Parse(points[i * 3]), float.Parse(points[i * 3 + 1]), float.Parse(points[i * 3 + 2]) * -1.0f);



        // Get center coordinates
        int handpoint_center = 9; // The handpoint 9 is the center (Middle proximal)
        Vector3 centro_nuevo = input_coordenadas[handpoint_center];
        Vector3 centro_anterior = handPoints[handpoint_center].transform.position;
        if (centro_nuevo.z > rangoMinimoDeteccion){
            centro = centro_anterior;
        }
        if(Vector3.Distance(centro_anterior, centro_nuevo) >= distanciaPrevenidaCentro) {
            centro = centro_nuevo;
            input_coordenadas_correctas[handpoint_center] = centro;
            handPoints[handpoint_center].transform.position = centro;
            transform.position = centro;

        } else {
            centro = centro_anterior;
        }

        // Get wrist coordinates
        adjustPosition(0, 9, 390.25f);

        // Get other handpoints coordinates
        // Thumb
        adjustPosition(1, 0, 162.60f);
        adjustPosition(2, 1, 134.83f);
        adjustPosition(3, 2, 112.07f);
        adjustPosition(4, 3, 122.35f);
        // Index
        adjustPosition(5, 0, 396.45f);
        adjustPosition(6, 5, 104.40f);
        adjustPosition(7, 6, 92.03f);
        adjustPosition(8, 7, 89.19f);
        // Middle
        // adjustPosition(9, 0, 390.25f);
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

    }

    void adjustPosition(int handpoint, int handpont_padre, float factor)
    {
        // Get wrist coordinates
        Vector3 point_nueva = input_coordenadas[handpoint];
        Vector3 point_antigua = handPoints[handpoint].transform.position;
        if (Vector3.Distance(point_antigua, point_nueva) >= distanciaPrevenidaArticulaciones
             && point_nueva.z < rangoMinimoDeteccion
             && Vector3.Distance(centro, point_nueva) <= radio
            )
        {
            input_coordenadas_correctas[handpoint] = point_nueva;

        }
        Vector3 point = handPoints[handpont_padre].transform.position + (Vector3.Normalize(input_coordenadas_correctas[handpoint] - input_coordenadas_correctas[handpont_padre]) * factor * tamanhoMano);
        handPoints[handpoint].transform.position = point;
    }

    void adjustWrist(GameObject a, GameObject b, GameObject c, string handedness)
    {
        Vector3 forwardDirection = b.transform.position - a.transform.position;
        Vector3 rightDirection = new Vector3();
        if (handedness.Equals("Left"))
            rightDirection = a.transform.position - c.transform.position;
        if (handedness.Equals("Right"))
            rightDirection = c.transform.position - a.transform.position;
        Vector3 upDirection = Vector3.Cross(forwardDirection, rightDirection);

        Quaternion orientation = Quaternion.LookRotation(forwardDirection, upDirection);

        a.transform.rotation = orientation;
    }

    void sendToHandModel(int a, int b)
    {
        destination[a].transform.position = vector3Filter[b].Filter(handPoints[b].transform.position / 1000.0f + ajuste);
    }

}
