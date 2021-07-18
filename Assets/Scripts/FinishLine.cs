using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity.Example;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>() && QuestMaster.instance.isInRace)
        {
            QuestMaster.instance.RaceWin();
        }
        else if(other.GetComponent<NPC>() && other.name == "Alley" && QuestMaster.instance.isEscorting)
        {
            QuestMaster.instance.FinishEscort();
        }
    }
}
