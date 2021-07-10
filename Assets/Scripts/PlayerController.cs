using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public float maxSpeedOnGround = 10f;
    public float movementSharpnessOnGround = 15;
    float footstepDistanceCounter;
    public float footStepInterval = 1;
    
    PlayerInput m_InputHandler;
    CharacterController m_Controller;
    
    public Vector3 CharacterVelocity { get; set; }
    
    
    // Start is called before the first frame update
    void Start()
    {
        m_InputHandler = GetComponent<PlayerInput>();
        m_Controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        PlayerInteraction();
    }

    void PlayerMovement()
    {
        // rotate the player
        transform.Rotate(new Vector3(0f, (m_InputHandler.GetRotationInput() * rotationSpeed * Time.deltaTime), 0f), Space.Self);

        // converts move input to a worldspace vector based on our character's transform orientation
        Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());
        
        // calculate the desired velocity from inputs, max speed, and current slope
        Vector3 targetVelocity = worldspaceMoveInput * maxSpeedOnGround;
         
        // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
        CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);
        
        // keep track of distance traveled for footsteps sound
        footstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
        
        // footsteps sound
        if (footstepDistanceCounter >= 1f / footStepInterval)
        {
            footstepDistanceCounter = 0f;
            //AkSoundEngine.PostEvent("FootStep", gameObject); // Play footstep sound
        }
        
        //TODO: add head bobbing
        
        m_Controller.Move(CharacterVelocity * Time.deltaTime);
    }

    void PlayerInteraction()
    {
        if (m_InputHandler.GetSpaceBarDown()) Debug.Log("Space Bar Pressed");
    }
}
