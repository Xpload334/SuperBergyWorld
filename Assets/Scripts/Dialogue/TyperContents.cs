using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Contains a sound file and list of images for a dialogue line
public class TyperContents
{
    public string TyperName;
    public List<Sprite> SpriteList = new();
    public AudioClip Sound { get; set; }

    public int SpriteInterval;
    public int SoundInterval;
    
    

    public void AddImage(Sprite sprite)
    {
        SpriteList.Add(sprite);
    }
    
}
