using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New timescale command", menuName = "Assets/Scripts/ConsoleCmds/timescale")]

public class cmd_timescale : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        ConsoleManager console = GameObject.FindObjectOfType<ConsoleManager>();
        float x = float.Parse(args[0]);
        Time.timeScale = x;
        console.UpdateLog("Timescale: " + Time.timeScale);
        return true;
    }
}