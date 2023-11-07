using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class HandManager : MonoBehaviour
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
        sendToHandModel(destination[26], handPoints[0]);
        sendToHandModel(destination[0], handPoints[0]);
        // Index
        sendToHandModel(destination[1], handPoints[21]);
        sendToHandModel(destination[2], handPoints[5]);
        sendToHandModel(destination[3], handPoints[6]);
        sendToHandModel(destination[4], handPoints[7]);
        sendToHandModel(destination[5], handPoints[8]);
        // Little
        sendToHandModel(destination[6], handPoints[24]);
        sendToHandModel(destination[7], handPoints[17]);
        sendToHandModel(destination[8], handPoints[18]);
        sendToHandModel(destination[9], handPoints[19]);
        sendToHandModel(destination[10], handPoints[20]);
        // Middle
        sendToHandModel(destination[11], handPoints[22]);
        sendToHandModel(destination[12], handPoints[9]);
        sendToHandModel(destination[13], handPoints[10]);
        sendToHandModel(destination[14], handPoints[11]);
        sendToHandModel(destination[15], handPoints[12]);
        // Palm
        sendToHandModel(destination[16], handPoints[25]);
        // Ring
        sendToHandModel(destination[17], handPoints[23]);
        sendToHandModel(destination[18], handPoints[13]);
        sendToHandModel(destination[19], handPoints[14]);
        sendToHandModel(destination[20], handPoints[15]);
        sendToHandModel(destination[21], handPoints[16]);
        // Thumb
        sendToHandModel(destination[22], handPoints[1]);
        sendToHandModel(destination[23], handPoints[2]);
        sendToHandModel(destination[24], handPoints[3]);
        sendToHandModel(destination[25], handPoints[4]);
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

    void sendToHandModel(GameObject a, GameObject b)
    {
        a.transform.position = b.transform.position / 1000.0f + ajuste;
    }

}
