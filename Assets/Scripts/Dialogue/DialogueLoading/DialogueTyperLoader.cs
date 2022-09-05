using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
[XmlRoot]
public class TypersRoot
{
    [XmlElement(ElementName = "SpriteFolderPath")] 
    public string spriteFolderPath;
    [XmlArray(ElementName = "Typers"), XmlArrayItem(ElementName = "Typer")] public List<Typer> Typers;
}

[Serializable]
[XmlRoot(ElementName = "Typer")]
public class Typer
{
    [XmlAttribute(AttributeName = "id")] public int typerID;
    [XmlElement(ElementName = "Name")] public string typerName;
    [XmlArray(ElementName = "Sprites"), XmlArrayItem(ElementName = "Sprite")] public List<string> spritePaths;
    [XmlElement(ElementName = "SoundID")] public int soundID;
    [XmlElement(ElementName = "SpriteInterval")] public int spriteInterval;
    [XmlElement(ElementName = "SoundInterval")] public int soundInterval;
}


//Uses the typerStore file
public class DialogueTyperLoader : MonoBehaviour
{
    [Header("Input")]
    public string xmlFileUrl = Path.Combine("Resources", "typerStore_2022.xml");

    public string spriteFolderPath;

    [Header("Result")] 
    public TypersRoot root;

    [ContextMenu(nameof(LoadFile))]
    public void LoadFile()
    {
        // Open the file as a stream
        using (var stream = File.Open(xmlFileUrl, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            // create an XMLSerializer according to the root type
            var serializer = new XmlSerializer(typeof(TypersRoot));

            // Deserialize the file according to your implemented Root class structure
            root = (TypersRoot) serializer.Deserialize(stream);
            Debug.Log("Loaded file "+xmlFileUrl);
        }
    }

    //Return a typer from a given typer ID
    public Typer GetTyper(int typerID)
    {
        foreach (var typer in root.Typers.Where(typer => typer.typerID == typerID))
        {
            return typer;
        }
        Debug.LogError("Typer with ID="+typerID+" not found");
        return null;
    }
    
    
    //Get the list of sprites from a given typer ID
    public List<Sprite> GetTyperSprites(int typerID)
    {
        var typer = GetTyper(typerID);
        if (typer == null) return null;
        
        var sprites = GetTyperSprites(typer);
        if (sprites != null)
        {
            return sprites;
        }

        Debug.LogError("Sprites for typer with ID="+typerID+" not found");
        return null;
    }
    
    //Get the list of sprites from a given typer
    public List<Sprite> GetTyperSprites(Typer typer)
    {
        List<Sprite> sprites = new List<Sprite>();
        foreach (string spritePath in typer.spritePaths)
        {
            string fullSpritePath = Path.Combine(spriteFolderPath, spritePath);
            Debug.Log("Getting sprite from "+fullSpritePath);
            Sprite sprite = Resources.Load<Sprite>(fullSpritePath);
            sprites.Add(sprite);
        }
        Debug.Log("Finished all sprites for typer ID "+typer.typerID);
        return sprites;
    }
    
    
}
