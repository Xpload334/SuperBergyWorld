using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Contains a string of text, typer information and additional properties of the dialogue line
//Multiple of these are sent to the dialogue manager's queue for display when needed
public class Dialogue
{
    public TyperContents Typer; //Contents of typer

    public string Text { get; set; }

    public AudioClip Sound => Typer.Sound;

    public List<Sprite> Sprites => Typer.SpriteList;

    public bool ShouldAutoSkip { get; set; }

    public float AutoSkipWait { get; set; }

    public int SpriteInterval => Typer.SpriteInterval;

    public int SoundInterval => Typer.SoundInterval;
}
