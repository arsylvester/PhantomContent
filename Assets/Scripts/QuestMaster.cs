using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class QuestMaster : MonoBehaviour
{
    public DialogueRunner dialogueRunner;

    QuestStep mainQuestStep;
    QuestStep fishQuestStep;
    QuestStep appleQuestStep;
    QuestStep raceQuestStep;
    QuestStep escortQuestStep;
    QuestStep deliveryQuestStep;

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

        dialogueRunner.AddCommandHandler(
            "start_quest",
            StartQuest
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
}
