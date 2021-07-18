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
    public ArrayList uniqueQuotes = new ArrayList();


    public void Awake() {
        dialogueRunner.AddCommandHandler(
            "generate_quote",
            GenerateQuote
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