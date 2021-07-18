using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuoteManager : MonoBehaviour
{
    public TextAsset jsonFile;
    public ArrayList horoscopes = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
        buildQuoteList();
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
        if (horoscopes.Count <= 0)
            buildQuoteList();

        int index = Random.Range(0, horoscopes.Count);
        Horoscope q = (Horoscope)horoscopes[index];
        horoscopes.RemoveAt(index);

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