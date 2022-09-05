using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class DialogueRoot
{
    [XmlArray(ElementName = "DialogueObjects"), XmlArrayItem(ElementName = "DialogueObject")]
    public List<DialogueObject> dialogueGroups;
}

[Serializable]
[XmlRoot(ElementName = "DialogueObject")]
public class DialogueObject
{
    [XmlAttribute(AttributeName = "id")] public int dialogueID;
    [XmlElement(ElementName = "Name")] public string dialogueName;
    
    [XmlElement(ElementName = "DisableControlOnFinish")] public bool disableControlOnFinish;
    [XmlElement(ElementName = "DisableControlOnStart")] public bool disableControlOnStart;
    
    [XmlArray(ElementName = "Lines"), XmlArrayItem(ElementName = "Line")] 
    public List<DialogueLine> lines = new List<DialogueLine>();
}

[Serializable]
[XmlRoot(ElementName = "Line")]
public class DialogueLine
{
    [XmlElement("TyperID")] public int typerID;
    [XmlElement("Text")] public string text;
    [XmlElement("autoSkipSeconds")] public int autoSkipSeconds;
    public bool ShouldAutoSkip => autoSkipSeconds != 0;
}


public class DialogueObjectLoader : MonoBehaviour, IXMLLoader
{
    [Header("Input")] 
    public string xmlFileUrl = Path.Combine("Resources", "dialogueTest_2022.xml");

    [Header("Result")] 
    public DialogueRoot root;

    [ContextMenu(nameof(LoadFile))]
    public void LoadFile()
    {
        // Open the file as a stream
        using (var stream = File.Open(xmlFileUrl, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            // create an XMLSerializer according to the root type
            var serializer = new XmlSerializer(typeof(DialogueRoot));

            // Deserialize the file according to your implemented Root class structure
            root = (DialogueRoot) serializer.Deserialize(stream);
            Debug.Log("Loaded file "+xmlFileUrl);
        }
    }

    public DialogueObject GetDialogueObject(int dialogueID)
    {
        foreach (var dialogueObject in root.dialogueGroups.Where(dialogueObject => dialogueObject.dialogueID == dialogueID))
        {
            return dialogueObject;
        }
        Debug.LogError("Dialogue object with ID="+dialogueID+" not found");
        return null;
    }
}
