using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "New delete command", menuName = "Assets/Scripts/ConsoleCmds/delete")]
public class cmd_delete : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        ConsoleManager console = GameObject.FindObjectOfType<ConsoleManager>();

        if (args.Length != 0)
        {
            GameObject target;
            string str = args[0];
            
            if (args.Length > 1)
            {
                for (int z = 1; z < args.Length; z++)
                    str += " " + args[z];
            }
            
            target = GameObject.Find(str);

            if (target != null)
                Destroy(target);
            else
                console.UpdateLog("\"" + str + "\" not found.");

            return true;
        }
        
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, player.transform.forward, out hit, Mathf.Infinity, -1,
            QueryTriggerInteraction.Ignore))
        {
            Destroy(hit.collider.gameObject);
        }
        else console.UpdateLog("No target found.");

        return true;
    }
}