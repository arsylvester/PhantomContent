using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "New spawn command", menuName = "Assets/Scripts/ConsoleCmds/spawn")]
public class cmd_spawn : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        Vector3 position = player.transform.position;
        Object prefab = SpawnDictionaryBuilder.objectDictionary[args[0].ToLower()];
        if (prefab != null)
        {
            int num = 1;
            if (args.Length != 1)
                num += int.Parse(args[1]) - 1;
            
            while (num > 0)
            {
                spawn(prefab, args[0], position);
                num--;
            }
        }
        return true;
    }

    private void spawn(Object prefab, string name, Vector3 pos)
    {
        GameObject clone = Instantiate(prefab, pos, Quaternion.identity) as GameObject;
        ConsoleManager console = GameObject.FindObjectOfType<ConsoleManager>();
        console.UpdateLog(name + " spawned @ ["+ Math.Truncate(pos.x) + ", " + Math.Truncate(pos.y) + ", " + Math.Truncate(pos.z)+ "]");
    }
}