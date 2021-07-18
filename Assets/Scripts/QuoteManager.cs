using System;
using System.Collections;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuoteManager : MonoBehaviour
{
    public TextAsset jsonFile;
    public ArrayList horoscopes = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
        buildQuoteList();
    }

    public void buildQuoteOrder()
    {
        string order = "";
        ArrayList<int> indexes = new ArrayList<int>();
        for (int x = 0; x < horoscopes.Count; x++)
            indexes.Add(x);

        while (indexes.Count != 0)
        {
            int rng = Random.Range(0, indexes.Count);
            order += indexes[rng] + ",";
            indexes.RemoveAt(rng);
        }
        
        PlayerPrefs.SetString("QUOTE_ORDER", order);
        PlayerPrefs.Save();
    }

    public void buildQuoteList()
    {
        Quotes quoteList = JsonUtility.FromJson<Quotes>(jsonFile.text);

        foreach (var q in quoteList.quotes)
        {
            horoscopes.Add(q);
        }
    }

    public string getQuote()
    {
        if (!PlayerPrefs.HasKey("QUOTE_ORDER") || PlayerPrefs.GetString("QUOTE_ORDER") == "")
            buildQuoteOrder();

        string quoteOrder = PlayerPrefs.GetString("QUOTE_ORDER");
        string[] indexes = quoteOrder.Split(',');
        string i = quoteOrder.Substring(0, quoteOrder.IndexOf(",", StringComparison.Ordinal));
        string remainingIndexes = quoteOrder.Substring(quoteOrder.IndexOf(",", StringComparison.Ordinal) + 1);

        int index = int.Parse(i);
        Horoscope q = (Horoscope)horoscopes[index];
        
        if (remainingIndexes == "")
            buildQuoteOrder();
        else
        {
            PlayerPrefs.SetString("QUOTE_ORDER", remainingIndexes);
            PlayerPrefs.Save();
        }
        
        return q.quote + "\n\n — " + q.author;
    }
}

[System.Serializable]
public class Quotes
{
    public Horoscope[] quotes;
}

[System.Serializable]
public class Horoscope
{
    public string quote;
    public string author;
}