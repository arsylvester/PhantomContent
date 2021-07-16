using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity.Example;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public float maxSpeedOnGround = 10f;
    public float movementSharpnessOnGround = 15;
    public float GravityModifier = 1000;
    float footstepDistanceCounter;

    [Header("Footsteps")]
    [SerializeField] float footStepInterval = 1;
    [SerializeField] float cameraBobAmplitude = 0.02f;
    [SerializeField] float stepMinVel = 2.5f;
    [SerializeField] AudioClip[] footStepsClips;
    int lastStep = 0;

    [SerializeField] Camera playerCam;

    PlayerInput m_InputHandler;
    CharacterController m_Controller;
    
    private ConsoleManager m_Console;
    AudioSource m_audioSource;
    
    public Vector3 CharacterVelocity { get; set; }
    public bool IsGrounded { get; private set; }

    [SerializeField] float interactionRadius = 5;

    [Header("Car")]
    [SerializeField] Image carOverlay; //Move this later
    [SerializeField] float carSpeed = 20f;
    [SerializeField] float carMovementSharpnessOnGround = 3;
    [SerializeField] AudioClip carAudioClip;
    [SerializeField] AudioClip carStartupAudioClip;
    [SerializeField] float carPitchMod = 500;
    [SerializeField] Sprite straightCarSprite;
    [SerializeField] Sprite leftCarSprite;
    [SerializeField] Sprite rightCarSprite;

    private Yarn.Unity.DialogueRunner Dialogue;
    private Yarn.Unity.DialogueUI DialogueUI;
    private List<NPC> allParticipants;
    private List<InteractableObject> allInteractable;
    private List<GameObject> lookingAt;
    private bool carMode = false;
    public bool isNoclip = false;


    // Start is called before the first frame update
    void Start()
    {
        //playerCam = GetComponent<Camera>();
        m_InputHandler = GetComponent<PlayerInput>();
        m_Controller = GetComponent<CharacterController>();
        m_audioSource = GetComponent<AudioSource>();
        m_Console = FindObjectOfType<ConsoleManager>();
        Dialogue = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        DialogueUI = FindObjectOfType<Yarn.Unity.DialogueUI>();
        allParticipants = new List<NPC>(FindObjectsOfType<NPC>());
        allInteractable = new List<InteractableObject>(FindObjectsOfType<InteractableObject>());
        m_Console.toggleVisable();
        m_Console.toggleFocus();
        


        //TODO: REMOVE THIS LATER!! FIX THE GRAVITY FOR REAL
        IsGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded = m_Controller.isGrounded;

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

            //print(CharacterVelocity.magnitude);
            // keep track of distance traveled for footsteps sound
            if (CharacterVelocity.magnitude > stepMinVel && IsGrounded)
                footstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
            
            if (!IsGrounded) CharacterVelocity += Vector3.down * GravityModifier;


            // footsteps sound
            if (footstepDistanceCounter / footStepInterval >= 1f)
            {
                footstepDistanceCounter = 0f;
                int stepSound = UnityEngine.Random.Range(0, footStepsClips.Length - 1);
                if (stepSound == lastStep)
                    stepSound = UnityEngine.Random.Range(0, footStepsClips.Length - 1);
                lastStep = stepSound;
                m_audioSource.PlayOneShot(footStepsClips[stepSound]); // Play footstep sound
            }
            
            m_Controller.Move(CharacterVelocity * Time.deltaTime);

            playerCam.transform.localPosition = new Vector3(playerCam.transform.localPosition.x, -Mathf.Cos(2 * Mathf.PI * footstepDistanceCounter / footStepInterval) / (2 * cameraBobAmplitude), playerCam.transform.localPosition.z);

            //if (!IsGrounded) CharacterVelocity += Vector3.down * GravityModifier;
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

        if (!IsGrounded) CharacterVelocity += Vector3.down * GravityModifier;

        // Car sounds
        if(CharacterVelocity.magnitude > .1f)
        {
            if (m_audioSource.clip != carAudioClip || !m_audioSource.isPlaying)
            {
                m_audioSource.clip = carAudioClip;
                m_audioSource.Play();
            }
            //var temp = CharacterVelocity * Time.deltaTime;
            //print(temp.magnitude);
            //m_audioSource.pitch = temp.magnitude / carPitchMod;
        }
        else
        {
            m_audioSource.Stop();
        }

        //Car image
        if(m_InputHandler.GetRotationInput() < 0)
        {
            carOverlay.sprite = leftCarSprite;
        }
        else if(m_InputHandler.GetRotationInput() > 0)
        {
            carOverlay.sprite = rightCarSprite;
        }
        else
        {
            carOverlay.sprite = straightCarSprite;
        }

        //TODO: add head bobbing

        m_Controller.Move(CharacterVelocity * Time.deltaTime);
    }

    void PlayerInteraction()
    {
        if (m_InputHandler.GetSpaceBarDown()) {
            //Debug.Log("Space Bar Pressed");
            if (Dialogue.IsDialogueRunning)
            {
                DialogueUI.MarkLineComplete();
                return;
            } else {
                InteractableObject lookingAt = GetLookingAt();
                if (lookingAt != null && lookingAt.type == InteractableObject.InteractableTypes.NPC)
                    Dialogue.StartDialogue(lookingAt.GetComponent<NPC>().GetTalkToNode());
                else if(lookingAt != null)
                {
                    lookingAt.pickUpItem();
                }
            }
        }

        if (m_InputHandler.GetFireInputDown())
        {
            RaycastHit hit;
            Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out hit)) {
                Transform objectHit = hit.transform;
                var position = hit.transform.position;
                m_Console.UpdateLog(objectHit.name + " [" + position.x + ", " + position.y + ", " + position.z + "]");
            }
        }
        
        if (m_InputHandler.GetTildeDown())
        {
            m_Console.toggleVisable();
            m_Console.toggleFocus();
        }
    }
    /*
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
    */

    public InteractableObject GetLookingAt()
    {
        InteractableObject target = null;
        float distance = 0;
        for (int i = 0; i < allInteractable.Count; i++)
        {
            if (allInteractable[i].isAvailable)
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
        }

        if (target != null)
        {
            Debug.Log("Looking at " + target.name);
        }
        return target;
    }

    public void SwapCar()
    {
        if(m_InputHandler.GetCarModeDown() && !m_Console.isActive)
        {
            CharacterVelocity = Vector3.zero;
            if(carMode)
            {
                carMode = false;
                carOverlay.gameObject.SetActive(false);
                m_audioSource.Stop();
            }
            else
            {
                carMode = true;
                carOverlay.gameObject.SetActive(true);
                m_audioSource.PlayOneShot(carStartupAudioClip);
            }
        }
    }

    public bool toggleNoclip()
    {
        isNoclip = !isNoclip;
        m_Controller.detectCollisions = !isNoclip; // disable/enable collisions
        m_Controller.enabled = !isNoclip;
        IsGrounded = false;
        return isNoclip;
    }

    public void Teleport(Vector3 v3)
    {
        m_Controller.enabled = false;
        transform.position = v3; // teleport the player
        m_Controller.enabled = true;
        m_Console.UpdateLog("teleporting to [" + v3.x + ", " + v3.y + ", " + v3.z + "]");
    }
}
