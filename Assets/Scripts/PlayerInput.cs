using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool CanProcessInput()
    {
        return true;
        // TODO: Lock the input during specific events
    }

    public Vector3 GetMoveInput()
    {
        if (!CanProcessInput()) return Vector3.zero;
        return new Vector3(0f, 0f, Input.GetAxisRaw("Vertical"));
    }

    public float GetRotationInput()
    {
        if (!CanProcessInput()) return 0f;
        
        float rotataion = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) rotataion += -1f;
        if (Input.GetKey(KeyCode.RightArrow)) rotataion += 1f;

        return rotataion;
    }
    
    public bool GetFireInputDown()
    {
        return Input.GetButtonDown("Fire1");
    }


    public bool GetSpaceBarDown()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public bool GetCarModeDown()
    {
        return Input.GetKeyDown(KeyCode.C);
    }

    public bool GetTildeDown()
    {
        return Input.GetKeyDown(KeyCode.BackQuote);
    }

    public bool GetEscDown()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }

    public bool GetQuestModeDown()
    {
        return Input.GetKeyDown(KeyCode.V);
    }

    public bool GetQuestModeUp()
    {
        return Input.GetKeyUp(KeyCode.V);
    }
}
