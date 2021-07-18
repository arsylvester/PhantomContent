using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDictionaryBuilder : MonoBehaviour
{
    [SerializeField] List<GameObject> items;
    [SerializeField] List<string> names;
    

    public static Dictionary<string, GameObject> objectDictionary = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < items.Count; i++)
        {
            objectDictionary.Add(names[i].ToLower(), items[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
