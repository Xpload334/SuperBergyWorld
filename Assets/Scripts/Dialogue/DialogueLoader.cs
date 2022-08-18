using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml;

//Class to read Dialogue Objects from a given name and distribute Queue<Dialogue> to the Dialogue Manager
public class DialogueLoader : MonoBehaviour
{
    [Header("Loaders")] 
    public DialogueObjectLoader DialogueObjectLoader;
    public DialogueTyperLoader DialogueTyperLoader;
    public DialogueSoundLoader DialogueSoundLoader;
    
    [Header("Filepaths")]
    public string spritesFolderPath = "DialogueSprites\\Faces";
    public string soundsFolderPath = "DialogueSounds";
    
    private void Awake()
    {
        LoadAllFiles();
    }

    [ContextMenu(nameof(LoadAllFiles))]
    public void LoadAllFiles()
    {
        DialogueSoundLoader = GetComponentInChildren<DialogueSoundLoader>();
        DialogueTyperLoader = GetComponentInChildren<DialogueTyperLoader>();
        DialogueObjectLoader = GetComponentInChildren<DialogueObjectLoader>();

        DialogueSoundLoader.soundsFolderPath = soundsFolderPath;
        DialogueTyperLoader.spriteFolderPath = spritesFolderPath;
        
        DialogueSoundLoader.LoadFile();
        DialogueTyperLoader.LoadFile();
        DialogueObjectLoader.LoadFile();
    }


    public DialogueObject GetDialogueObject(int dialogueID)
    {
        return DialogueObjectLoader.GetDialogueObject(dialogueID);
    }

    public Typer GetTyper(int typerID)
    {
        return DialogueTyperLoader.GetTyper(typerID);
    }

    public List<Sprite> GetTyperSprites(Typer typer)
    {
        return DialogueTyperLoader.GetTyperSprites(typer);
    }

    public AudioClip GetSound(int soundID)
    {
        return DialogueSoundLoader.GetSound(soundID);
    }

}