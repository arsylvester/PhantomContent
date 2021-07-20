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

    [Header("Skybox")] [SerializeField] private Material skyboxDay;
    [SerializeField] private Material skyboxNight;
    [SerializeField] private Material skyboxDawn;
    [SerializeField] private Material skyboxDusk;
    [SerializeField] Light directionalLight;
    [SerializeField] AudioSource ambinaceAudio;
    [SerializeField] AudioClip dayClip;
    [SerializeField] AudioClip nightClip;
    [SerializeField] AudioClip[] clockTicks;
    private int tickIndex = 0;


    [Header("Footsteps")] [SerializeField] float footStepInterval = 1;
    [SerializeField] float cameraBobAmplitude = 0.02f;
    [SerializeField] float stepMinVel = 2.5f;
    [SerializeField] AudioClip[] footStepsClips;
    int lastStep = 0;
    private float defaultCamHeight;

    [SerializeField] Camera playerCam;
    [SerializeField] float CarCamHeightDif;

    PlayerInput m_InputHandler;
    CharacterController m_Controller;
    private MenuManager m_MenuManager;
    private ConsoleManager m_Console;
    AudioSource m_audioSource;

    private InteractText interactionText;
    private bool wasLookingAt;
    
    public Vector3 CharacterVelocity { get; set; }
    public bool IsGrounded { get; private set; }

    [SerializeField] float interactionRadius = 5;

    [Header("Car")] [SerializeField] Image carOverlay; //Move this later
    [SerializeField] float carSpeed = 20f;
    [SerializeField] float carMovementSharpnessOnGround = 3;
    [SerializeField] AudioClip carAudioClip;
    [SerializeField] AudioClip carStartupAudioClip;
    [SerializeField] float carPitchMod = 500;
    [SerializeField] Sprite straightCarSprite;
    [SerializeField] Sprite leftCarSprite;
    [SerializeField] Sprite rightCarSprite;
    [SerializeField] GameObject playerCar;
    [SerializeField] AudioClip carLockClip;
    [SerializeField] GameObject carHitBox; 

    [Header("Quest Highlight Mode")]
    [SerializeField] NPC[] npcs;
    private bool inQuestMode;

    private Yarn.Unity.DialogueRunner Dialogue;
    private Yarn.Unity.DialogueUI DialogueUI;
    private List<GameObject> lookingAt;
    public String characterName;
    private bool carMode = false;
    public bool isNoclip = false;
    public bool hasKeys = false;


    [Header("Clock")] public float DEFAULT_TIMESCALE = 1;
    public int hours = 0;
    public double minutes = 0;
    [SerializeField] private Text clock;
    [SerializeField] public Text dayText;

    // Start is called before the first frame update
    void Start()
    {
        m_InputHandler = GetComponent<PlayerInput>();
        m_Controller = GetComponent<CharacterController>();
        m_audioSource = GetComponent<AudioSource>();
        m_Console = FindObjectOfType<ConsoleManager>();
        m_MenuManager = FindObjectOfType<MenuManager>();
        Dialogue = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        DialogueUI = FindObjectOfType<Yarn.Unity.DialogueUI>();
        characterName = "";
        m_Console.toggleVisable();
        m_Console.toggleFocus();

        interactionText = FindObjectOfType<InteractText>();

        defaultCamHeight = playerCam.transform.localPosition.y;

        hours = 6;
        minutes = 0;
        RenderSettings.skybox = skyboxDawn;
        directionalLight.color = new Vector4(0.9339623f, 0.790913f, 0.5771182f, 1);
        directionalLight.transform.rotation = Quaternion.Euler(53.584f, 11.114f, 176.684f);
        directionalLight.intensity = 0.75f;
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
        if(hasKeys)
            SwapCar();
        updateTime();
        QuestMode();
        
        if (m_InputHandler.GetEscDown())
            m_MenuManager.ToggleGamePaused();
    }

    void PlayerMovement()
    {
        // rotate the player
        transform.Rotate(new Vector3(0f, (m_InputHandler.GetRotationInput() * rotationSpeed * Time.deltaTime), 0f),
            Space.Self);

        if (isNoclip)
        {
            transform.position += transform.TransformVector(m_InputHandler.GetMoveInput()) *
                                  (maxSpeedOnGround * Time.deltaTime);
        }
        else
        {
            
            // converts move input to a worldspace vector based on our character's transform orientation
            Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

            // calculate the desired velocity from inputs, max speed, and current slope
            Vector3 targetVelocity = worldspaceMoveInput * maxSpeedOnGround;

            // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
            CharacterVelocity =
                Vector3.Lerp(CharacterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);

            //print(CharacterVelocity.magnitude);
            // keep track of distance traveled for footsteps sound
            if (CharacterVelocity.magnitude > stepMinVel && IsGrounded)
                footstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;

            if (!IsGrounded) CharacterVelocity += Vector3.down * GravityModifier;
            
            m_Controller.Move(CharacterVelocity * Time.deltaTime);

            if (worldspaceMoveInput == Vector3.zero)
            {
                //footstepDistanceCounter = 0;
                Vector3 defaultCam = new Vector3(playerCam.transform.localPosition.x, defaultCamHeight,
                    playerCam.transform.localPosition.z);
                
                playerCam.transform.localPosition = Vector3.Lerp(playerCam.transform.localPosition, defaultCam, Time.deltaTime * 2);
            } else {
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
                
                playerCam.transform.localPosition = new Vector3(playerCam.transform.localPosition.x,
                    -Mathf.Cos(2 * Mathf.PI * footstepDistanceCounter / footStepInterval) / (2 * cameraBobAmplitude),
                    playerCam.transform.localPosition.z);
            }
            
            //if (!IsGrounded) CharacterVelocity += Vector3.down * GravityModifier;
        }
    }

    void CarMovement()
    {
        // rotate the player
        transform.Rotate(new Vector3(0f, (m_InputHandler.GetRotationInput() * rotationSpeed * Time.deltaTime), 0f),
            Space.Self);

        // converts move input to a worldspace vector based on our character's transform orientation
        Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

        // calculate the desired velocity from inputs, max speed, and current slope
        Vector3 targetVelocity = worldspaceMoveInput * carSpeed;

        // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
        CharacterVelocity =
            Vector3.Lerp(CharacterVelocity, targetVelocity, carMovementSharpnessOnGround * Time.deltaTime);

        if (!IsGrounded) CharacterVelocity += Vector3.down * GravityModifier;

        // Car sounds
        if (CharacterVelocity.magnitude > .1f)
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
        if (m_InputHandler.GetRotationInput() < 0)
        {
            carOverlay.sprite = leftCarSprite;
        }
        else if (m_InputHandler.GetRotationInput() > 0)
        {
            carOverlay.sprite = rightCarSprite;
        }
        else
        {
            carOverlay.sprite = straightCarSprite;
        }

        m_Controller.Move(CharacterVelocity * Time.deltaTime);
    }

    void PlayerInteraction()
    {
        InteractableObject lookingAt = GetLookingAt();

        if (m_InputHandler.GetSpaceBarDown())
        {
            //Debug.Log("Space Bar Pressed");
            if (Dialogue.IsDialogueRunning)
            {
                DialogueUI.MarkLineComplete();
                return;
            }
            if (lookingAt != null)
            {
                if (lookingAt.type == InteractableObject.InteractableTypes.NPC)
                {
                    characterName = lookingAt.GetComponent<NPC>().characterName;
                    Dialogue.StartDialogue(lookingAt.GetComponent<NPC>().GetTalkToNode());
                    interactionText.SetText("");
                    interactionText.enabled = false;
                }
                else
                {
                    lookingAt.pickUpItem();
                }
            }
        }
        
        if (lookingAt != null && !Dialogue.IsDialogueRunning)
        {
            interactionText.enabled = true;
            interactionText.SetText(lookingAt.name);
            wasLookingAt = true;
        }
        else if (wasLookingAt)
        {
            interactionText.SetText("");
            interactionText.enabled = false;
            wasLookingAt = false;
        }
        

        if (m_InputHandler.GetFireInputDown())
        {
            RaycastHit hit;
            Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
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
        for (int i = 0; i < InteractableObject.allInteractable.Count; i++)
        {
            Vector3 viewPos = playerCam.WorldToViewportPoint(InteractableObject.allInteractable[i].transform.position);
            if (viewPos.z > 0 && viewPos.x > 0 && viewPos.x < 1) // if camera is looking at object
            {
                if ((target == null && (InteractableObject.allInteractable[i].transform.position - this.transform.position).magnitude <
                        interactionRadius) || // get target within range
                    (target != null && (InteractableObject.allInteractable[i].transform.position - this.transform.position).magnitude <
                        distance)) // get closest target
                {
                    target = InteractableObject.allInteractable[i];
                    distance = (InteractableObject.allInteractable[i].transform.position - this.transform.position).magnitude;
                }
            }
        }

        /*if (target != null)
        {
            Debug.Log("Looking at " + target.name);
        }*/

        return target;
    }

    public void SwapCar()
    {
        if(playerCar == null)
        {
            playerCar = GameObject.FindGameObjectWithTag("playerCar");
        }
        if (m_InputHandler.GetCarModeDown() && !m_Console.isActive)
        {
            CharacterVelocity = Vector3.zero;
            if (carMode)
            {
                carMode = false;
                playerCam.transform.position = new Vector3(playerCam.transform.position.x, playerCam.transform.position.y + CarCamHeightDif, playerCam.transform.position.z);
                carOverlay.gameObject.SetActive(false);
                m_audioSource.Stop();
                carHitBox.SetActive(true);
                
                playerCar.SetActive(false);
                playerCar.transform.rotation = this.transform.rotation;
                playerCar.transform.parent = gameObject.transform;
                playerCar.transform.localPosition = new Vector3(2, -0.5f, 0);
                playerCar.transform.parent = null;
                m_audioSource.PlayOneShot(carLockClip, .5f);
            }
            else if(playerCar != null)
            {
                carMode = true;
                carHitBox.SetActive(true);
                playerCam.transform.position = new Vector3(playerCam.transform.position.x, playerCam.transform.position.y - CarCamHeightDif, playerCam.transform.position.z);
                carOverlay.gameObject.SetActive(true);
                m_audioSource.PlayOneShot(carStartupAudioClip);
                playerCar.SetActive(false);
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

    private void updateTime()
    {
        minutes += DEFAULT_TIMESCALE * Time.deltaTime;
        if (minutes >= 60)
        {
            hours += 1;
            minutes = 0;
            ambinaceAudio.PlayOneShot(clockTicks[tickIndex]);
            if (++tickIndex >= clockTicks.Length)
                tickIndex = 0;
            
            switch (hours) // change the skybox at certain hours
            {
                case 6:
                    RenderSettings.skybox = skyboxDawn;
                    directionalLight.color = new Vector4(0.9339623f, 0.790913f, 0.5771182f, 1);
                    directionalLight.transform.rotation = Quaternion.Euler(53.584f, 11.114f, 176.684f);
                    directionalLight.intensity = 0.75f;
                    ambinaceAudio.clip = dayClip;
                    ambinaceAudio.Play();
                    break;
                case 10:
                    RenderSettings.skybox = skyboxDay;
                    directionalLight.color = new Vector4(1, 0.9568627f, 0.8392157f, 1);
                    directionalLight.transform.rotation = Quaternion.Euler(62.242f, -162.474f, 4.228f);
                    directionalLight.intensity = 1;
                    break;
                case 18:
                    RenderSettings.skybox = skyboxDusk;
                    directionalLight.color = new Vector4(0.9716981f, 0.706511f, 0.6004361f, 1);
                    directionalLight.transform.rotation = Quaternion.Euler(35.589f, -164.808f, 2.42f);
                    directionalLight.intensity = 0.65f;
                    ambinaceAudio.clip = nightClip;
                    ambinaceAudio.Play();
                    break;
                case 22:
                    RenderSettings.skybox = skyboxNight;
                    directionalLight.color = new Vector4(0.8470589f, 0.7882354f, 0.6705883f, 1);
                    directionalLight.transform.rotation = Quaternion.Euler(42.407f, -183.328f, -8.39f);
                    directionalLight.intensity = 0.2f;
                    break;
            }
            
            if (hours >= 24)
            {
                // trigger end of day
                hours = 0;
                minutes = 0;
                m_MenuManager.NextDay();
                m_MenuManager.RunDayEndSequence();
            }
        }
        
        string h = (hours < 10) ? "0" + hours : "" + hours;
        string m = (minutes < 10) ? "0" + Math.Floor(minutes) : "" + Math.Floor(minutes);


        clock.text = h + ":" + m;
    }

    public void MoveToSpeed(NPC TalkTo)
    {
        Teleport(new Vector3(193, 24, 602));
        Dialogue.StartDialogue(TalkTo.GetTalkToNode());
        interactionText.SetText("");
        interactionText.enabled = false;
    }

    public void QuestMode()
    {
        if (m_InputHandler.GetQuestModeDown() && !m_Console.isActive)
        {
            inQuestMode = true;
            foreach(NPC npc in npcs)
            {
                npc.SetExclamationPoint(true);
            }
        }
        else if(m_InputHandler.GetQuestModeUp() && inQuestMode)
        {
            inQuestMode = false;
            foreach (NPC npc in npcs)
            {
                npc.SetExclamationPoint(false);
            }
        }
    }
}