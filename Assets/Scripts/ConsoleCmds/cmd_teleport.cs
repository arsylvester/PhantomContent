using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "teleport command", menuName = "Assets/Scripts/ConsoleCmds/teleport")]
public class cmd_teleport : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        ConsoleManager console = GameObject.FindObjectOfType<ConsoleManager>();
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();

        if (args.Length != 0 && args[0] == "spawn")
        {
            player.Teleport(new Vector3(460, 12, 460));
        }

        else if (args.Length == 3)
        {
            float x = float.Parse(args[0]);
            float y = float.Parse(args[1]);
            float z = float.Parse(args[2]);
            
            //player.transform.position = new Vector3(x, y, z);
            player.Teleport(new Vector3(x, y, z));
        }
        
        else
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(player.transform.position, player.transform.forward, Mathf.Infinity, -1,
                QueryTriggerInteraction.Ignore);

            if (hits.Length != 0)
            {
                System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
                RaycastHit h = hits[0];
                
                var hpos = h.transform.position;
                //player.transform.position = hpos;
                player.Teleport(hpos);
            }
            else console.UpdateLog("no target found");
        }

        return true;
    }
}