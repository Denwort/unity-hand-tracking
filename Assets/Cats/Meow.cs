using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;

public class Meow : MonoBehaviour
{
    public AudioSource audiosource;
    public AudioClip[] clips;
    public VideoPlayer player;
    public VideoClip videoclip;
    private int meowcount;
    private bool isPlayerStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        meowcount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerStarted == false && player.isPlaying == true)
        {
            isPlayerStarted = true;
        }
        if (isPlayerStarted == true && player.isPlaying == false)
        {
            isPlayerStarted = false;
            player.Stop();
        }
    }

    public void meow(int n)
    {
        meowcount++;
        audiosource.PlayOneShot(clips[n]);
        print(meowcount);
        if (meowcount == 8)
        {
            player.Play();
            meowcount = 0;
        }
    }

    public void meow1() { meow(0);}
    public void meow2() { meow(1);}
    public void meow3() { meow(2);} 

}
