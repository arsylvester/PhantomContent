using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class SpawnDictionaryBuilder : MonoBehaviour
{
    [SerializeField] List<GameObject> items;
    [SerializeField] List<string> names;
    [SerializeField] private TextAsset alt_dictionary;
    

    public static Dictionary<string, GameObject> objectDictionary = new Dictionary<string, GameObject>();
    public static List<string> activeDictionary = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < items.Count; i++)
        {
            objectDictionary.Add(names[i].ToLower(), items[i]);
        }
        
        // nonsense
        //WriteAllGameObjectsToFile("Assets/Dictionaries/autofill_dictionary.txt");
        
        // build dictionary from file
        BuildActiveDictionary();
    }

    void BuildActiveDictionary()
    {
        string[] lines = Regex.Split( alt_dictionary.text, "\n|\r|\r\n" );
        foreach (var n in lines)
        {
            activeDictionary.Add(n);
        }
    }

    void WriteAllGameObjectsToFile(string path)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        StreamWriter writer = new StreamWriter(path, true);
        foreach (var go in allObjects)
        {
            if (!go.scene.isLoaded)
                continue;
            writer.WriteLine(go.name);
        }
        writer.Close();
    }
}
