using UnityEngine;

[CreateAssetMenu(fileName = "help command", menuName = "Assets/Scripts/ConsoleCmds/help")]
public class cmd_help : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        ConsoleManager console = GameObject.FindObjectOfType<ConsoleManager>();
        
        foreach (var x in console.cmds)
        {
            console.UpdateLog(" " + x.CommandStr + x.CommandDesc);
        }        
        
        return true;
    }
}