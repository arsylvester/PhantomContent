using UnityEngine;

[CreateAssetMenu(fileName = "getcoords command", menuName = "Assets/Scripts/ConsoleCmds/getcoords")]
public class cmd_getcoords : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        Debug.Log("thank you gamers");
        GameObject.FindObjectOfType<ConsoleManager>().UpdateLog("thank you gamers");
        return true;
    }
}