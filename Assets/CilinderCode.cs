using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CilinderCode : MonoBehaviour
{
    public GameObject objOrigin;
    public GameObject objDestination;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Make a cilinder that joins two points
        Vector3 v3Start = objOrigin.transform.position;
        Vector3 v3End = objDestination.transform.position;

        transform.position = (v3End - v3Start) / 2.0f + v3Start;

        Vector3 v3T = transform.localScale;      // Scale it
        v3T.y = (v3End - v3Start).magnitude /2.0f;
        transform.localScale = v3T;

        transform.rotation = Quaternion.FromToRotation(Vector3.up, v3End - v3Start);
    }
}
