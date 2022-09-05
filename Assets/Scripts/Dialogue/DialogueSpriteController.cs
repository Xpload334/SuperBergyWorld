using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Used with TypeText to handle changing the sprites of the face in the dialogue box
public class DialogueSpriteController : MonoBehaviour
{
    public Image faceImage;
    public Sprite blank; //blank image to use when nothing available
    [Header("Faces")]
    public Sprite currentFace;
    public List<Sprite> spritesList;
    [Header("Timing")]
    public int spriteChangeWaitTime; //time in frames to wait before a sprite can change
    private int _spriteChangeWaitIndex; //current index progress towards spriteChangeWaitTime
    private int _spriteIndex; //current index of image to use

    // Start is called before the first frame update
    void Start()
    {
        ResetWaitTime();
    }

    // Update is called once per frame

    //Reset the current wait time to 0
    public void ResetWaitTime()
    {
        _spriteChangeWaitIndex = 0;
    }
    
    //Main call for changing the face
    //Increment wait index by 1 for each call
    //If equal to the wait time, reset back to 0 and increment the face index
    public void FaceChange()
    {
        if(CheckShouldChange())
        {
            _spriteChangeWaitIndex++; //Increment wait index
            //If index equal to wait time, do sprite change
            if(_spriteChangeWaitIndex >= spriteChangeWaitTime)
            {
                _spriteIndex++; //Increment sprite index
                //Loop index back to 0 if larger than number of sprites (last sprite = spritesList[length - 1])
                if (_spriteIndex >= spritesList.Count)
                {
                    _spriteIndex = 0;
                }
                SetSprite(spritesList[_spriteIndex]); //Set face box
                ResetWaitTime(); //Reset wait index to 0

            }
        }
    }

    public void SetToFirstSprite()
    {
        SetSprite(spritesList[0]);
    }

    public void SetSprite(Sprite newFace)
    {
        currentFace = newFace;
        faceImage.sprite = currentFace;
    }

    public void SetFaceBox(Image faceBox)
    {
        faceImage = faceBox;
    }

    //If only contains 1 or 0 sprites, do not need to change
    bool CheckShouldChange()
    {
        if (spritesList.Count > 1)
        {
            return true;
        }

        if (spritesList == null || spritesList.Count == 0)
        {
            SetBlank();
        }

        return false;
    }

    public void SetBlank()
    {
        currentFace = blank;
        SetSprite(currentFace);
    }

}
