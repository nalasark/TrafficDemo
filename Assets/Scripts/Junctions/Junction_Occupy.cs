using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Junction_Occupy : MonoBehaviour
{
    public int OccupyCount = 0;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Vehicle") OccupyCount++;
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Vehicle") OccupyCount--;
    }

    public bool IsOccupied() {
        return OccupyCount > 0;
    }
}
