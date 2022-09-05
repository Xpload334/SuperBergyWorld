using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip neutral;
    [Header("Sounds")] 
    public AudioClip sound;
    [Header("Timing")]
    public int soundWaitTime; //time in frames to wait before a sound can play
    public int soundWaitIndex; //current index progress towards soundWaitTime

    // Start is called before the first frame update
    void Start()
    {
        ResetWaitTime();
    }

    //Reset the current wait index to 0
    public void ResetWaitTime()
    {
        soundWaitIndex = 0;
    }

    //Set the current wait index to wait time
    public void SetToPlayNext()
    {
        soundWaitIndex = soundWaitTime;
    }
    
    //Main call for changing the face
    //Increment wait index by 1 for each call
    //If equal to the wait time, reset back to 0 and play sound
    public void PlaySound()
    {
        soundWaitIndex++; //Increment wait index
        //If index equal to wait time, play current sound
        if(soundWaitIndex >= soundWaitTime)
        {
            PlayCurrentSound();
            ResetWaitTime(); //Reset wait index to 0
        }
    }

    void PlayCurrentSound()
    {
        audioSource.PlayOneShot(sound);
    }

    public void SetSound(AudioClip newSound)
    {
        this.sound = newSound;
    }

    public void SetAudioSource(AudioSource source)
    {
        audioSource = source;
    }

    public void SetNeutral()
    {
        SetSound(neutral);
    }
}
