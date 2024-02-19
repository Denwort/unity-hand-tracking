using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameObject[] lista = new GameObject[0];
    public GameObject[] objetos;
    void Start()
    {
        lista = objetos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
