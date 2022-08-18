using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Contains a sound file and list of images for a dialogue line
public class TyperContents
{
    public string TyperName;
    private AudioClip _sound;
    public List<Sprite> spriteList = new();
    public AudioClip Sound {
        get { return _sound; }
        set { _sound = value; }
    }
    public int SpriteInterval;
    public int SoundInterval;
    
    

    public void AddImage(Sprite sprite)
    {
        spriteList.Add(sprite);
    }
    
}
