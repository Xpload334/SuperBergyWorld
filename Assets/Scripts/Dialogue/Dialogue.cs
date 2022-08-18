using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Contains a string of text, typer information and additional properties of the dialogue line
//Multiple of these are sent to the dialogue manager's queue for display when needed
public class Dialogue
{
    private string _text; //Text to present
    public TyperContents Typer; //Contents of typer
    private bool _shouldAutoSkip; //If the dialogue line should automatically skip
    private float _autoSkipWait; //If should autoskip, how long to wait (seconds)

    public string Text { get { return _text; } set { _text = value; }
    }
    public AudioClip Sound {
        get { return Typer.Sound; }
    }
    public List<Sprite> Sprites {
        get { return Typer.spriteList; }
    }
    public bool ShouldAutoSkip {
        get { return _shouldAutoSkip; } set { _shouldAutoSkip = value; }
    }
    public float AutoSkipWait {
        get { return _autoSkipWait; } set { _autoSkipWait = value; }
    }
    public int SpriteInterval {
        get { return Typer.SpriteInterval; }
    }
    public int SoundInterval {
        get { return Typer.SoundInterval; }
    }
}
