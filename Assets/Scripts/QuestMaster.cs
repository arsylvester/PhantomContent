using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;
using Yarn.Unity.Example;

public class QuestMaster : MonoBehaviour
{
    public static QuestMaster instance;
    public DialogueRunner dialogueRunner;
    public InMemoryVariableStorage storage;
    PlayerController player;

    QuestStep mainQuestStep;
    QuestStep fishQuestStep;
    QuestStep appleQuestStep;
    QuestStep raceQuestStep;
    QuestStep escortQuestStep;
    QuestStep deliveryQuestStep;
    QuestStep carKeysQuestStep;

    //Quest variables
    //Main
    [SerializeField] public int questsTotal = 6;
    public int questsComplete;
    //Race
    [SerializeField] float raceTimeLimit = 30f;
    float currentRaceTime;
    [SerializeField] Text raceTimeText;
    public bool isInRace = false;
    [SerializeField] GameObject RaceFinishLine;
    [SerializeField] NPC RaceNPC;
    //Apple
    [SerializeField] int applesNeeded = 7;
    int apples;
    [SerializeField] int fishNeeded = 5;
    int fish;
    //Escort
    public bool isEscorting = false;
    [SerializeField] GameObject escortFinishLine;
    //Keys
    //Delivering
    bool package = false;
    [SerializeField] GameObject packageObject;
    [SerializeField] Transform packageLocation;
    public bool keys = false;

    [Header("Hud Elements")] 
    [SerializeField] private GameObject fishUI;
    [SerializeField] private GameObject appleUI;
    [SerializeField] private GameObject carkeyUI;
    [SerializeField] private GameObject packageUI;

    public enum QuestStep
    {
        NotStarted,
        InProgress,
        Completed
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        player = FindObjectOfType<PlayerController>();

        mainQuestStep = (QuestStep)PlayerPrefs.GetInt("MainStep");
        fishQuestStep = (QuestStep)PlayerPrefs.GetInt("FishStep");
        appleQuestStep = (QuestStep)PlayerPrefs.GetInt("AppleStep");
        raceQuestStep = (QuestStep)PlayerPrefs.GetInt("RaceStep");
        escortQuestStep = (QuestStep)PlayerPrefs.GetInt("EscortStep");
        deliveryQuestStep = (QuestStep)PlayerPrefs.GetInt("DeliveryStep");
        carKeysQuestStep = (QuestStep)PlayerPrefs.GetInt("KeysStep");

        questsComplete = PlayerPrefs.GetInt("QuestsComplete");
        fish = PlayerPrefs.GetInt("fish");
        apples = PlayerPrefs.GetInt("apples");
        keys = PlayerPrefs.GetInt("keys") == 1 ? true : false;
        package = PlayerPrefs.GetInt("package") == 1 ? true : false;
        FindObjectOfType<PlayerController>().hasKeys = keys;
        
        fishUI.SetActive(fish > 0);
        fishUI.GetComponentInChildren<Text>().text = (fish == 0) ? "" : "" + fish;
        appleUI.SetActive(apples > 0);
        appleUI.GetComponentInChildren<Text>().text = (apples == 0) ? "" : "" + apples;
        carkeyUI.SetActive(keys);
        packageUI.SetActive(package);

        storage = GetComponent<InMemoryVariableStorage>();
        StartCoroutine(DelaySetYarnVars());

        dialogueRunner.AddCommandHandler(
            "start_quest",
            StartQuest
        );
        dialogueRunner.AddCommandHandler(
            "complete_quest",
            CompleteQuest
        );
        dialogueRunner.AddCommandHandler(
            "spawn_package",
            SpawnPackage
        );
        dialogueRunner.AddCommandHandler(
            "return_camera",
            ReturnCameraView
        );
    }

    // Update is called once per frame
    void Update()
    {
        
        //TESTING AND DO ACTUALLY REMOVE THIS LATER
        if(Input.GetKeyDown(KeyCode.F6))
        {
            PlayerPrefs.DeleteAll();
            Debug.LogWarning("Player Pref Reset");
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            UpdateQuestsComplete();
            Debug.Log("Quest Complete");
        }
        
    }

    private void StartQuest(string[] parameters, System.Action onComplete)
    {
        switch (parameters[0])
        {
            case "main":
                if (mainQuestStep == QuestStep.NotStarted)
                    SetMainQuestStep(QuestStep.InProgress);
                break;
            case "fish":
                if (mainQuestStep == QuestStep.NotStarted)
                    SetFishQuestStep(QuestStep.InProgress);
                break;
            case "apple":
                if (mainQuestStep == QuestStep.NotStarted)
                    SetAppleQuestStep(QuestStep.InProgress);
                break;
            case "race":
                if (mainQuestStep == QuestStep.NotStarted)
                    SetRaceQuestStep(QuestStep.InProgress);
                StartRace();
                break;
            case "escort":
                if (mainQuestStep == QuestStep.NotStarted)
                    SetEscortQuestStep(QuestStep.InProgress);
                StartEscort();
                break;
            case "delivery":
                if (mainQuestStep == QuestStep.NotStarted)
                    SetDeliveryQuestStep(QuestStep.InProgress);
                break;
            case "keys":
                if (mainQuestStep == QuestStep.NotStarted)
                    SetKeysQuestStep(QuestStep.InProgress);
                break;
            default:
                break;
        }

        // Call the completion handler
        onComplete();
    }

    private void CompleteQuest(string[] parameters, System.Action onComplete)
    {
        switch (parameters[0])
        {
            case "main":
                if (mainQuestStep != QuestStep.Completed)
                    UpdateQuestsComplete();
                SetMainQuestStep(QuestStep.Completed);
                break;
            case "fish":
                if (fishQuestStep != QuestStep.Completed)
                    UpdateQuestsComplete();
                SetFishQuestStep(QuestStep.Completed);
                break;
            case "apple":
                if (appleQuestStep != QuestStep.Completed)
                {
                    AppleQuestComplete();
                    UpdateQuestsComplete();
                }
                SetAppleQuestStep(QuestStep.Completed);
                break;
            case "race":
                if (raceQuestStep != QuestStep.Completed)
                    UpdateQuestsComplete();
                SetRaceQuestStep(QuestStep.Completed);
                break;
            case "escort":
                if (escortQuestStep != QuestStep.Completed)
                    UpdateQuestsComplete();
                SetEscortQuestStep(QuestStep.Completed);
                break;
            case "delivery":
                if (deliveryQuestStep != QuestStep.Completed)
                    UpdateQuestsComplete();
                SetDeliveryQuestStep(QuestStep.Completed);
                packageUI.SetActive(false);
                package = false;
                break;
            case "keys":
                if (carKeysQuestStep != QuestStep.Completed)
                    UpdateQuestsComplete();
                SetKeysQuestStep(QuestStep.Completed);
                break;
            default:
                break;
        }

        // Call the completion handler
        onComplete();
    }

    public bool isQuestStarted(string quest)
    {
        switch (quest)
        {
            case "main":
                return (mainQuestStep == QuestStep.InProgress);
                break;
            case "fish":
                return (fishQuestStep == QuestStep.InProgress);
                break;
            case "apple":
                return (appleQuestStep == QuestStep.InProgress);
                break;
            case "race":
                return (raceQuestStep == QuestStep.InProgress);
                break;
            case "escort":
                return (escortQuestStep == QuestStep.InProgress);
                break;
            case "delivery":
                return (deliveryQuestStep == QuestStep.InProgress);
                break;
            case "keys":
                return (carKeysQuestStep == QuestStep.InProgress);
                break;
            default:
                return false;
                break;
        }
    }

    public void SetMainQuestStep(QuestStep step)
    {
        mainQuestStep = step;
        PlayerPrefs.SetInt("MainStep", (int)step);
        PlayerPrefs.Save();
    }

    public void SetFishQuestStep(QuestStep step)
    {
        fishQuestStep = step;
        PlayerPrefs.SetInt("FishStep", (int)step);
        PlayerPrefs.Save();
    }
    public void SetAppleQuestStep(QuestStep step)
    {
        appleQuestStep = step;
        PlayerPrefs.SetInt("AppleStep", (int)step);
        PlayerPrefs.Save();
    }
    public void SetRaceQuestStep(QuestStep step)
    {
        raceQuestStep = step;
        PlayerPrefs.SetInt("RaceStep", (int)step);
        PlayerPrefs.Save();
    }
    public void SetEscortQuestStep(QuestStep step)
    {
        escortQuestStep = step;
        PlayerPrefs.SetInt("EscortStep", (int)step);
        PlayerPrefs.Save();
    }

    public void SetDeliveryQuestStep(QuestStep step)
    {
        deliveryQuestStep = step;
        PlayerPrefs.SetInt("DeliveryStep", (int)step);
        PlayerPrefs.Save();
    }

    public void SetKeysQuestStep(QuestStep step)
    {
        carKeysQuestStep = step;
        PlayerPrefs.SetInt("KeysStep", (int)step);
        PlayerPrefs.Save();
    }

    private void UpdateQuestsComplete()
    {
        questsComplete++;
        if (questsComplete >= questsTotal)
        {
            print("ALL QUESTS COMPLETED!");
            storage.SetValue("$all_quests_complete", true);
        }
        //print("Storage has: " + storage.GetValue("$all_quests_complete").AsBool);
        PlayerPrefs.SetInt("QuestsComplete", questsComplete);
        PlayerPrefs.Save();
    }

    IEnumerator DelaySetYarnVars()
    {
        yield return new WaitForSeconds(.1f);
        //Main
        if (questsComplete >= questsTotal)
            storage.SetValue("$all_quests_complete", true);
        //Race
        storage.SetValue("$time_to_beat", raceTimeLimit);
        //Fish
        if (fish >= fishNeeded)
        {
            storage.SetValue("$fish_completed", true);
        }
        //apple
        if (apples >= applesNeeded)
        {
            storage.SetValue("$apple_completed", true);
            print("Apples: " + storage.GetValue("$apple_completed").AsBool);
        }
        //Delivery
        storage.SetValue("$isDelivering", package);
        if(deliveryQuestStep == QuestStep.Completed)
            storage.SetValue("$delivery_complete", package);
        //Escort
        if (escortQuestStep == QuestStep.Completed)
            storage.SetValue("$trip_finished", true);
        //Keys
        storage.SetValue("$hasKeys", keys);
    }

    public void FailDeliveryQuest()
    {
        storage.SetValue("$delivery_failed", true);
        package = false;
        packageUI.SetActive(false);
        storage.SetValue("$isDelivering", false);
        PlayerPrefs.SetInt("package", 0);
        PlayerPrefs.Save();
    }

    public void PickupPackage()
    {
        package = true;
        packageUI.SetActive(true);
        storage.SetValue("$isDelivering", true);
        PlayerPrefs.SetInt("package", 1);
        PlayerPrefs.Save();
    }

    private void SpawnPackage(string[] parameters, System.Action onComplete)
    {
        Instantiate(packageObject, packageLocation.position, packageLocation.rotation);
        string objectList = "";
        foreach(InteractableObject var in InteractableObject.allInteractable)
        {
            objectList += var + ", ";
        }
        print(objectList);
        // Call the completion handler
        onComplete();
    }

    public void StartRace()
    {
        currentRaceTime = raceTimeLimit;
        raceTimeText.gameObject.SetActive(true);
        isInRace = true;
        storage.SetValue("$race_failed", false);
        RaceFinishLine.SetActive(true);
        StartCoroutine(RaceCountDown());
    }

    IEnumerator RaceCountDown()
    {
        while (currentRaceTime > 0)
        {
            currentRaceTime -= Time.deltaTime;
            raceTimeText.text = "" + Mathf.Floor(currentRaceTime);
            yield return new WaitForEndOfFrame();
        }
        raceTimeText.gameObject.SetActive(false);
        storage.SetValue("$race_failed", true);
        isInRace = false;
        player.MoveToSpeed(RaceNPC);
    }

    public void RaceWin()
    {
        if (raceQuestStep != QuestStep.Completed)
            UpdateQuestsComplete();
        SetRaceQuestStep(QuestStep.Completed);
        StopAllCoroutines();
        storage.SetValue("$race_win", true);
        raceTimeText.color = Color.green;
        RaceFinishLine.SetActive(false);
        StartCoroutine(DelayTextDisappear());
    }

    IEnumerator DelayTextDisappear()
    {
        yield return new WaitForSeconds(5);
        raceTimeText.gameObject.SetActive(false);
    }

    public void FishUpdated()
    {
        //int fish = InventoryManager.inventory[(int)InteractableObject.InteractableTypes.FISH];
        fish++;
        PlayerPrefs.SetInt("fish", fish);
        PlayerPrefs.Save();

        if (fish > 0)
        {
            fishUI.SetActive(true);
            if (fish == 0)
                fishUI.GetComponentInChildren<Text>().text = "";
            else
                fishUI.GetComponentInChildren<Text>().text = "" + fish;
        }

        if (fish >= fishNeeded)
        {
            storage.SetValue("$fish_completed", true);
        }
    }
    
    public void SetFish(int x)
    {
        fish = x;
        fishUI.SetActive(fish > 0);
        PlayerPrefs.SetInt("fish", fish);
        PlayerPrefs.Save();
        fishUI.GetComponentInChildren<Text>().text = fish == 0 ? "" : "" + fish;
        storage.SetValue("$fish_completed", fish >= fishNeeded);
    }

    public void AppleUpdated()
    {
        apples++;
        PlayerPrefs.SetInt("apples", apples);
        PlayerPrefs.Save();
        
        if (apples > 0)
        {
            appleUI.SetActive(true);
            appleUI.GetComponentInChildren<Text>().text = apples == 0 ? "" : "" + apples;
        }
        
        if (apples >= applesNeeded)
        {
            storage.SetValue("$apple_completed", true);
        }
    }

    public void SetApples(int x)
    {
        apples = x;
        appleUI.SetActive(apples > 0);
        PlayerPrefs.SetInt("apples", apples);
        PlayerPrefs.Save();
        appleUI.GetComponentInChildren<Text>().text = apples == 0 ? "" : "" + apples;
        storage.SetValue("$apple_completed", apples >= applesNeeded);
    }

    public void AppleQuestComplete()
    {
        apples -= applesNeeded;
        appleUI.GetComponentInChildren<Text>().text = apples == 0 ? "" : "" + apples;
        PlayerPrefs.SetInt("apples", apples);
        PlayerPrefs.Save();
    }

    public void StartEscort()
    {
        isEscorting = true;
        escortFinishLine.SetActive(true);
        FindObjectOfType<Escort>().StartMoving();
    }

    public void FinishEscort()
    {
        isEscorting = false;
        escortFinishLine.SetActive(false);
        FindObjectOfType<Escort>().StopMoving();
        storage.SetValue("$trip_finished", true);
    }

    public void FoundKeys()
    {
        FindObjectOfType<PlayerController>().hasKeys = true;
        keys = true;
        storage.SetValue("$hasKeys", true);
        PlayerPrefs.SetInt("keys", 1);
        PlayerPrefs.Save();
        carkeyUI.SetActive(true);
    }

    private void ReturnCameraView(string[] parameters, System.Action onComplete)
    {
        player.ReturnToNormalCam();
        onComplete();
    }
}
