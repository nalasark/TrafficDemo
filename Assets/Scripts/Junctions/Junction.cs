using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Junction : MonoBehaviour
{
    public Dashboard dashboard;
    public int CurrentlyInside = 0;
    public int TotalCrossed = 0;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Vehicle") return;
        CurrentlyInside++;
        dashboard.Update_CurrentlyIn(CurrentlyInside);
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Vehicle") return;
        CurrentlyInside--;
        TotalCrossed++;
        dashboard.Update_CurrentlyIn(CurrentlyInside);
        dashboard.Update_TotalCrossed(TotalCrossed);
    }
}
