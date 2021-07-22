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

        float temp;

        if (args.Length != 0 && args[0] == "spawn")
        {
            player.Teleport(new Vector3(529, 16, 530));
        }

        else if(args.Length != 0 && !float.TryParse(args[0], out temp))
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
                player.Teleport(target.transform.position);
            else
                console.UpdateLog("\"" + str + "\" not found.");

            return true;
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
            RaycastHit hit;
            
            if (Physics.Raycast(player.transform.position, player.transform.forward, out hit, Mathf.Infinity, -1,
                QueryTriggerInteraction.Ignore))
            {
                var hpos = hit.transform.position;
                hpos.y += hit.transform.localScale.y;
                //player.transform.position = hpos;
                player.Teleport(hpos);
            }
            else console.UpdateLog("no target found");
        }

        return true;
    }
}