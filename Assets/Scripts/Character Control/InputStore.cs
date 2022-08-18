using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStore
{
    public Vector2 MovementVector2;
    public bool IsJumpPressed;

    public InputStore(Vector2 movementVector2, bool isJumpPressed)
    {
        this.MovementVector2 = movementVector2;
        this.IsJumpPressed = isJumpPressed;
    }
}
