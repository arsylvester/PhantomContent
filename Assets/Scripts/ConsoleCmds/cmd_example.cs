using UnityEngine;

// fileName replaces the text that shows up in the "create" menu
// Replace "COMMAND_NAME" at the end of menuName with the name of your new command
[CreateAssetMenu(fileName = "New EXAMPLE command", menuName = "Assets/Scripts/ConsoleCmds/COMMAND_NAME")]

// Replace "cmd_example" with "cmd_[your command name]"
public class cmd_example : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        // put command functionality here...
        
        // example of printing something to the console...
        ConsoleManager console = GameObject.FindObjectOfType<ConsoleManager>();
        console.UpdateLog("Your text that you want to print to the console");
        
        return true;
    }
}