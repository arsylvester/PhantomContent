using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class QuestMaster : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public InMemoryVariableStorage storage;

    QuestStep mainQuestStep;
    QuestStep fishQuestStep;
    QuestStep appleQuestStep;
    QuestStep raceQuestStep;
    QuestStep escortQuestStep;
    QuestStep deliveryQuestStep;

    //Quest variables
    [SerializeField] int questsTotal = 5;
    int questsComplete;

    public enum QuestStep
    {
        NotStarted,
        InProgress,
        Completed
    }

    // Start is called before the first frame update
    void Start()
    {
        mainQuestStep = (QuestStep)PlayerPrefs.GetInt("MainStep");
        fishQuestStep = (QuestStep)PlayerPrefs.GetInt("FishStep");
        appleQuestStep = (QuestStep)PlayerPrefs.GetInt("AppleStep");
        raceQuestStep = (QuestStep)PlayerPrefs.GetInt("RaceStep");
        escortQuestStep = (QuestStep)PlayerPrefs.GetInt("EscortStep");
        deliveryQuestStep = (QuestStep)PlayerPrefs.GetInt("DeliveryStep");

        questsComplete = PlayerPrefs.GetInt("QuestsComplete");
        print("Quests completed: " + questsComplete);

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
                SetMainQuestStep(QuestStep.InProgress);
                break;
            case "fish":
                SetFishQuestStep(QuestStep.InProgress);
                break;
            case "apple":
                SetAppleQuestStep(QuestStep.InProgress);
                break;
            case "race":
                SetRaceQuestStep(QuestStep.InProgress);
                break;
            case "escort":
                SetEscortQuestStep(QuestStep.InProgress);
                break;
            case "delivery":
                SetDeliveryQuestStep(QuestStep.InProgress);
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
                    UpdateQuestsComplete();
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
                break;
            default:
                break;
        }

        // Call the completion handler
        onComplete();
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
        if (questsComplete >= questsTotal)
            storage.SetValue("$all_quests_complete", true);
    }

    public void FailDeliveryQuest()
    {
        storage.SetValue("$delivery_failed", true);
    }
}
