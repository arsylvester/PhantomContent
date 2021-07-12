using UnityEngine;

[CreateAssetMenu(fileName = "noclip command", menuName = "Assets/Scripts/ConsoleCmds/noclip")]
public class cmd_noclip : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        ConsoleManager console = GameObject.FindObjectOfType<ConsoleManager>();
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        
        console.UpdateLog(player.toggleNoclip() ? "noclip on" : "noclip off");

        return true;
    }
}