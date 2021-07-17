using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>() && QuestMaster.instance.isInRace)
        {
            QuestMaster.instance.RaceWin();
        }
    }
}
