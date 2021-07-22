using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "set command", menuName = "Assets/Scripts/ConsoleCmds/set")]
public class cmd_set : ConsoleCommand
{
    public static List<string> cmd_set_arguments = new List<string> { "time", "apples", "fish", "keys", "questscomplete", "day"}; // this is hard coded and shouldn't be, but it's a game jam soooooo
    public override bool Process(string[] args)
    {
        ConsoleManager console = GameObject.FindObjectOfType<ConsoleManager>();
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        MenuManager mm = GameObject.FindObjectOfType<MenuManager>();

        if (args.Length < 2)
        {
            console.UpdateLog("Please use `set [variable] [value]`");
            return true;
        }

        switch (args[0].ToLower())
        {
            case "apples":
                QuestMaster.instance.SetApples(int.Parse(args[1]));
                break;
            case "fish":
                QuestMaster.instance.SetFish(int.Parse(args[1]));
                break;
            case "keys":
                PlayerPrefs.SetInt("keys", int.Parse(args[1]));
                PlayerPrefs.Save();
                if (int.Parse(args[1]) == 1)
                    QuestMaster.instance.FoundKeys();
                break;
            case "questscomplete":
                PlayerPrefs.SetInt("QuestsComplete", int.Parse(args[1]));
                PlayerPrefs.Save();
                break;
            case "time":
                int t = int.Parse(args[1]);
                player.hours = t / 100;
                player.minutes = t % 100;
                break;
            case "day":
                int d = int.Parse(args[1]);
                mm.SetDay(d);
                break;
            default:
                console.UpdateLog("variable " + args[0] + " not found.");
                break;
        }
        
        return true;
    }
}