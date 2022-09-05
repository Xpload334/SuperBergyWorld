using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml;

//Class to read Dialogue Objects from a given name and distribute Queue<Dialogue> to the Dialogue Manager
public class DialogueLoader : MonoBehaviour
{
    public static DialogueLoader Instance;
    [Header("Loaders")] 
    public DialogueObjectLoader dialogueObjectLoader;
    public DialogueTyperLoader dialogueTyperLoader;
    public DialogueSoundLoader dialogueSoundLoader;
    
    [Header("File Paths")]
    public string spritesFolderPath = "DialogueSprites\\Faces";
    public string soundsFolderPath = "DialogueSounds";
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
        LoadAllFiles();
    }

    [ContextMenu(nameof(LoadAllFiles))]
    public void LoadAllFiles()
    {
        dialogueSoundLoader = GetComponentInChildren<DialogueSoundLoader>();
        dialogueTyperLoader = GetComponentInChildren<DialogueTyperLoader>();
        dialogueObjectLoader = GetComponentInChildren<DialogueObjectLoader>();

        dialogueSoundLoader.soundsFolderPath = soundsFolderPath;
        dialogueTyperLoader.spriteFolderPath = spritesFolderPath;
        
        dialogueSoundLoader.LoadFile();
        dialogueTyperLoader.LoadFile();
        dialogueObjectLoader.LoadFile();
    }


    public DialogueObject GetDialogueObject(int dialogueID)
    {
        return dialogueObjectLoader.GetDialogueObject(dialogueID);
    }

    public Typer GetTyper(int typerID)
    {
        return dialogueTyperLoader.GetTyper(typerID);
    }

    public List<Sprite> GetTyperSprites(Typer typer)
    {
        return dialogueTyperLoader.GetTyperSprites(typer);
    }

    public AudioClip GetSound(int soundID)
    {
        return dialogueSoundLoader.GetSound(soundID);
    }

}