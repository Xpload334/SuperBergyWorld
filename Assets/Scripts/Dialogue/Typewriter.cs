using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Typewriter : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private float typewriterSpeed = 50f;

    public float waitBeforeDialogueStart;
    public bool IsRunning {get; private set;}
    [Header("Sound")]
    public DialogueSoundController voiceBleepController;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public float audioVolume = 0.5f;
    [Header("Sprites")]
    public DialogueSpriteController spriteController;
    [SerializeField] public Image faceImage;

    //List of punctuations to wait a certain amount of time for
    private readonly List<Punctuation> _punctuations = new List<Punctuation>()
    {
        new Punctuation(new HashSet<char>(){'.', '!', '?'}, 0.5f),
        new Punctuation(new HashSet<char>(){',', ';', ':'}, 0.2f),
    };
    //Better organised structure for punctuations
    private readonly struct Punctuation
    {
        public readonly HashSet<char> Punctuations;
        public readonly float WaitTime;
        public Punctuation(HashSet<char> punctuations, float waitTime)
        {
            Punctuations = punctuations;
            WaitTime = waitTime;
        }
    }
    
    /*
     * While playing:
     *
     * Typewriter effect (adapt from previous code)
     * Sound file play (interval?)
     * Display image at index (interval?)
     * Increment index tracking image count (this makes the images cycle around)
     */
    
    private Coroutine _typingCoroutine;

    public void Run(string textToType, TMP_Text textLabel, TyperContents typer)
    {
        _typingCoroutine = StartCoroutine(TypeText(textToType, textLabel, typer));
    }

    public void Stop()
    {
        StopCoroutine(_typingCoroutine);
        IsRunning = false;
        
        spriteController.SetToFirstSprite();
        spriteController.ResetWaitTime();
        voiceBleepController.ResetWaitTime();
    }

    public void SetTypingSpeed(float speed)
    {
        typewriterSpeed = speed;
    }


    // Main typing subroutine
    public IEnumerator TypeText(string textToType, TMP_Text textLabel, TyperContents typer)
    {
        IsRunning = true;
        textLabel.text = string.Empty;

        float t = 0;
        int charIndex = 0;
        spriteController.spriteChangeWaitTime = typer.SpriteInterval; //Set sprite interval
        spriteController.spritesList = typer.SpriteList; //Add all typer sprites
        spriteController.faceImage = faceImage;
        spriteController.SetToFirstSprite();
        
        voiceBleepController.soundWaitTime = typer.SoundInterval; //Set sound interval
        voiceBleepController.SetSound(typer.Sound); //Set typer sound
        voiceBleepController.audioSource = audioSource;
        voiceBleepController.SetToPlayNext();


        yield return new WaitForSeconds(waitBeforeDialogueStart);
        while (charIndex < textToType.Length)
        {
            int lastCharIndex = charIndex;

            //Set speed
            t += Time.deltaTime * typewriterSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            for(int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i >= textToType.Length - 1;

                //Adds the character (important stuff)
                textLabel.text = textToType.Substring(0, i + 1);
                
                //Call playVoiceBleep()
                voiceBleepController.PlaySound();
                //Call faceChange()
                spriteController.FaceChange();
                //Check character is punctuation, not the last character, and the next character is not punctuation
                if(IsPunctuation(textToType[i], out float waitTime) && !isLast && !IsPunctuation(textToType[i + 1], out _))
                {
                    //If is punctuation or is last, reset face
                    if(IsPunctuation(textToType[i], out float tempTime) || isLast)
                    {
                        //Reset to first sprite
                        spriteController.SetToFirstSprite();
                        spriteController.ResetWaitTime();
                        
                        voiceBleepController.SetToPlayNext();
                    }
                    
                    yield return new WaitForSeconds(waitTime);
                }
            }
            yield return null;
        }
        IsRunning = false;
        //Sprite reset
        spriteController.SetToFirstSprite();
        spriteController.ResetWaitTime();
        //Sound reset
        voiceBleepController.ResetWaitTime();
    }
    
    private bool IsPunctuation(char character, out float waitTime)
    {   
        //Looking through the hashsets for the characters
        foreach(Punctuation punctuationCategory in _punctuations)
        {
            if(punctuationCategory.Punctuations.Contains(character))
            {
                //Return the waitTime of that character if it finds it
                waitTime = punctuationCategory.WaitTime;
                return true;
            }
        }
        //Else, return the default wait time
        waitTime = default;
        return false;
    }
}
