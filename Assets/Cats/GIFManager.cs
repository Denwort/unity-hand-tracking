using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIFManager : MonoBehaviour
{
    public GameObject texture;
    public Texture2D[] frames;
    int framesPerSecond;
    // Start is called before the first frame update
    private void Start()
    {
        framesPerSecond = 20;
    }
    // Update is called once per frame
    void Update()
    {
        int index = Mathf.RoundToInt(Time.time * framesPerSecond);
        index = index % frames.Length;
        texture.GetComponentInChildren<MeshRenderer>().material.mainTexture = frames[index];
    }
}

