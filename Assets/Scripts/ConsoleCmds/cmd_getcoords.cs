using UnityEngine;

[CreateAssetMenu(fileName = "getcoords command", menuName = "Assets/Scripts/ConsoleCmds/getcoords")]
public class cmd_getcoords : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        ConsoleManager console = GameObject.FindObjectOfType<ConsoleManager>();

        console.UpdateLog("player's coordinates: " + player.transform.position);

        return true;
    }
}