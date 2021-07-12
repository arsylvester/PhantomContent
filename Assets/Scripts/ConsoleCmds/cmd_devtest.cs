using UnityEngine;

[CreateAssetMenu(fileName = "New test command", menuName = "Assets/Scripts/ConsoleCmds/devtest")]
public class cmd_devtest : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        Debug.Log("thank you gamers");
        return true;
    }
}