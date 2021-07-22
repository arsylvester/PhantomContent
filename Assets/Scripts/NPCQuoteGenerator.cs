using System.Collections;
using UnityEngine;
using Yarn.Unity;
using System.Collections.Generic;
using UnityEngine.UI;

public class NPCQuoteGenerator : MonoBehaviour {    

    // Drag and drop your Dialogue Runner into this variable.
    
    public DialogueRunner dialogueRunner;
    public PlayerController playerController;
    public InMemoryVariableStorage storage;
    public TextAsset jsonFile;
    public TextAsset jsonTips;
    public ArrayList uniqueQuotes = new ArrayList();
    public ArrayList tips = new ArrayList();


    public void Awake() {
        dialogueRunner.AddCommandHandler(
            "generate_quote",
            GenerateQuote
        );
        dialogueRunner.AddCommandHandler(
            "generate_tip",
            GenerateTip
        );

        storage = GetComponent<InMemoryVariableStorage>();

        buildQuoteList();
    }

    public void buildQuoteList()
    {
        NPCQuotes quoteList = JsonUtility.FromJson<NPCQuotes>(jsonFile.text);

        foreach (var q in quoteList.quotes)
        {
            uniqueQuotes.Add(q);
        }

        GameTips tipsList = JsonUtility.FromJson<GameTips>(jsonTips.text);

        foreach (var t in tipsList.tips)
        {
            tips.Add(t);
        }
    }

    private void GenerateQuote(string[] parameters) {
        string quote = "";

        if (uniqueQuotes.Count <= 0)
            buildQuoteList();

        int index = Random.Range(0, uniqueQuotes.Count);
        randQuote q = (randQuote)uniqueQuotes[index];
        uniqueQuotes.RemoveAt(index);

        quote = playerController.characterName + ": " + q.quote;

        storage.SetValue("$random_quote", quote);
    }

    private void GenerateTip(string[] parameters)
    {
        string tip = "";

        if (tips.Count <= 0)
            buildQuoteList();

        int index = Random.Range(0, tips.Count);
        randTips q = (randTips)tips[index];
        uniqueQuotes.RemoveAt(index);

        tip = playerController.characterName + ": " + q.quote;

        storage.SetValue("$current_tip", tip);
    }
}

[System.Serializable]
public class NPCQuotes
{
    public randQuote[] quotes;
}

[System.Serializable]
public class randQuote
{
    public string quote;
    public string author;
}

[System.Serializable]
public class GameTips
{
    public randTips[] tips;
}

[System.Serializable]
public class randTips
{
    public string quote;
    public string author;
}