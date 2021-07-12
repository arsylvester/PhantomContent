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
    float footstepDistanceCounter;
    public float footStepInterval = 1;
    
    PlayerInput m_InputHandler;
    CharacterController m_Controller;
    Camera playerCam;
    
    public Vector3 CharacterVelocity { get; set; }

    [SerializeField] float interactionRadius = 5;

    private Yarn.Unity.DialogueRunner Dialogue;
    private Yarn.Unity.DialogueUI DialogueUI;
    private List<NPC> allParticipants;
    private List<InteractableObject> allInteractable;
    private List<GameObject> lookingAt;


    // Start is called before the first frame update
    void Start()
    {
        playerCam = GetComponent<Camera>();
        m_InputHandler = GetComponent<PlayerInput>();
        m_Controller = GetComponent<CharacterController>();
        Dialogue = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        DialogueUI = FindObjectOfType<Yarn.Unity.DialogueUI>();
        allParticipants = new List<NPC>(FindObjectsOfType<NPC>());
        allInteractable = new List<InteractableObject>(FindObjectsOfType<InteractableObject>());
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
        if (m_InputHandler.GetSpaceBarDown()) {
            Debug.Log("Space Bar Pressed");
            if (Dialogue.IsDialogueRunning)
            {
                DialogueUI.MarkLineComplete();
                return;
            } else {
                InteractableObject lookingAt = GetLookingAt();
                if (lookingAt != null && lookingAt.isNPC)
                    Dialogue.StartDialogue(lookingAt.GetComponent<NPC>().talkToNode);
            }
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

    public InteractableObject GetLookingAt()
    {
        InteractableObject target = null;
        float distance = 0;
        for (int i = 0; i < allInteractable.Count; i++)
        {
            Vector3 viewPos = playerCam.WorldToViewportPoint(allInteractable[i].transform.position);
            if (viewPos.z > 0 && viewPos.x > 0 && viewPos.x < 1) // if camera is looking at object
            {
                if ((target == null && (allInteractable[i].transform.position - this.transform.position).magnitude < interactionRadius) || // get target within range
                    (target != null && (allInteractable[i].transform.position - this.transform.position).magnitude < distance)) // get closest target
                {
                    target = allInteractable[i];
                    distance = (allInteractable[i].transform.position - this.transform.position).magnitude;
                }
            }
        }

        if (target != null)
        {
            Debug.Log("Looking at " + target.name);
            return target;
        }
        else
            return null;
    }
}
