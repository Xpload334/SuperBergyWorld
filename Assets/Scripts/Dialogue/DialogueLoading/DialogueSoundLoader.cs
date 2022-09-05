using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
[XmlRoot(ElementName = "SoundRoot")]
public class SoundRoot
{
    [XmlElement(ElementName = "SoundFolderPath")]
    public string soundFolderPath;
    [XmlArray(ElementName = "TyperSounds"), XmlArrayItem(ElementName = "TyperSound")]
    public List<TyperSound> typerSounds;
}

[Serializable]
[XmlRoot(ElementName = "TyperSound")]
public class TyperSound
{
    [XmlAttribute(AttributeName = "id")] 
    public int soundID;
    [XmlElement(ElementName = "SoundPath")] 
    public string soundPath;
}

//Uses the typerSoundStore.xml file
public class DialogueSoundLoader : MonoBehaviour
{
    [Header("Input")] 
    public string xmlFileUrl = Path.Combine("Resources", "typerSoundStore.xml");

    public string soundsFolderPath;

    [Header("Result")] 
    public SoundRoot root;

    [ContextMenu(nameof(LoadFile))]
    public void LoadFile()
    {
        // Open the file as a stream
        using (var stream = File.Open(xmlFileUrl, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            // create an XMLSerializer according to the root type
            var serializer = new XmlSerializer(typeof(SoundRoot));

            // Deserialize the file according to your implemented Root class structure
            root = (SoundRoot) serializer.Deserialize(stream);
            Debug.Log("Loaded file "+xmlFileUrl);
        }
    }

    public AudioClip GetSound(int soundID)
    {
        foreach (var typerSound in root.typerSounds)
        {
            if (typerSound.soundID == soundID)
            {
                string soundPath = Path.Combine(soundsFolderPath, typerSound.soundPath);
                //Debug.Log("Finding sound at "+soundPath);
                AudioClip sound = Resources.Load<AudioClip>(soundPath);
                //Debug.Log(sound);
                return sound;
            }
        }
        Debug.LogError("Sound with ID="+soundID+" not found");
        return null;
    }
}
