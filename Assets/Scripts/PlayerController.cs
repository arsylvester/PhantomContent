using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity.Example;

public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public float maxSpeedOnGround = 10f;
    public float movementSharpnessOnGround = 15;
    public float GravityModifier = 1000;
    float footstepDistanceCounter;
    public float footStepInterval = 1;
    
    PlayerInput m_InputHandler;
    CharacterController m_Controller;
    private ConsoleManager m_Console;
    
    public Vector3 CharacterVelocity { get; set; }
    public bool IsGrounded { get; private set; }

    [SerializeField] float interactionRadius = 5;

    [Header("Car")]
    [SerializeField] GameObject carOverlay; //Move this later
    [SerializeField] float carSpeed = 20f;
    public float carMovementSharpnessOnGround = 3;

    private Yarn.Unity.DialogueRunner Dialogue;
    private Yarn.Unity.DialogueUI DialogueUI;
    private List<NPC> allParticipants;
    private bool carMode = false;
    public bool isNoclip = false;


    // Start is called before the first frame update
    void Start()
    {
        m_InputHandler = GetComponent<PlayerInput>();
        m_Controller = GetComponent<CharacterController>();
        m_Console = FindObjectOfType<ConsoleManager>();
        Dialogue = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        DialogueUI = FindObjectOfType<Yarn.Unity.DialogueUI>();
        allParticipants = new List<NPC>(FindObjectsOfType<NPC>());
        m_Console.toggleVisable();
        m_Console.toggleFocus();
    }

    // Update is called once per frame
    void Update()
    {
        if (carMode)
            CarMovement();
        else
            PlayerMovement();
        PlayerInteraction();
        SwapCar();
    }

    void PlayerMovement()
    {
        // rotate the player
        transform.Rotate(new Vector3(0f, (m_InputHandler.GetRotationInput() * rotationSpeed * Time.deltaTime), 0f), Space.Self);
        
        if (isNoclip)
        {
            transform.position += transform.TransformVector(m_InputHandler.GetMoveInput()) * (maxSpeedOnGround * Time.deltaTime);
        }

        else
        {
            // converts move input to a worldspace vector based on our character's transform orientation
            Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

            // calculate the desired velocity from inputs, max speed, and current slope
            Vector3 targetVelocity = worldspaceMoveInput * maxSpeedOnGround;

            // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
            CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);

            // keep track of distance traveled for footsteps sound
            footstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
            
            if (!IsGrounded) CharacterVelocity += Vector3.down * GravityModifier;

            // footsteps sound
            if (footstepDistanceCounter >= 1f / footStepInterval)
            {
                footstepDistanceCounter = 0f;
                //AkSoundEngine.PostEvent("FootStep", gameObject); // Play footstep sound
            }
            
            m_Controller.Move(CharacterVelocity * Time.deltaTime);
            
            //TODO: add head bobbing

            
        }
    }

    void CarMovement()
    {
        // rotate the player
        transform.Rotate(new Vector3(0f, (m_InputHandler.GetRotationInput() * rotationSpeed * Time.deltaTime), 0f), Space.Self);

        // converts move input to a worldspace vector based on our character's transform orientation
        Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

        // calculate the desired velocity from inputs, max speed, and current slope
        Vector3 targetVelocity = worldspaceMoveInput * carSpeed;

        // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
        CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity, carMovementSharpnessOnGround * Time.deltaTime);

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
        if (m_InputHandler.GetSpaceBarDown()) {
            Debug.Log("Space Bar Pressed");
            if (Dialogue.IsDialogueRunning)
            {
                DialogueUI.MarkLineComplete();
                return;
            } else {
                CheckForNearbyNPC();
            }
        }
        
        if (m_InputHandler.GetTildeDown())
        {
            m_Console.toggleVisable();
            m_Console.toggleFocus();
        }
    }

    public void CheckForNearbyNPC()
    {
        NPC target = allParticipants.Find(delegate (NPC p) {
            return string.IsNullOrEmpty(p.talkToNode) == false && // has a conversation node?
            (p.transform.position - this.transform.position).magnitude <= interactionRadius && // is in range?
            true;
        });
        if (target != null)
        {
            Debug.Log("target not null");
            // Kick off the dialogue at this node.
            Dialogue.StartDialogue(target.talkToNode);
        }
    }

    public void SwapCar()
    {
        if(m_InputHandler.GetCarModeDown() && !m_Console.isActive)
        {
            if(carMode)
            {
                carMode = false;
                carOverlay.SetActive(false);
            }
            else
            {
                carMode = true;
                carOverlay.SetActive(true);
            }
        }
    }

    public bool toggleNoclip()
    {
        isNoclip = !isNoclip;
        m_Controller.detectCollisions = !isNoclip; // disable/enable collisions
        IsGrounded = false;
        return isNoclip;
    }

    public void Teleport(Vector3 v3)
    {
        // This is not at all stupid or redundant.
        StartCoroutine(Warp(v3));
    }
    
    private IEnumerator Warp(Vector3 v3)
    {
        yield return new WaitForEndOfFrame();
        transform.position = v3;
        m_Console.UpdateLog("teleporting to [" + v3.x + ", " + v3.y + ", " + v3.z + "]");
    }
}
