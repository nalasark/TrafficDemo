using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Junction_Entry : MonoBehaviour
{
    public Path[] Paths;
    public Junction_Occupy[] OccupySlots;
    public Transform Junction;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Vehicle") {
            //decide path to take
            Path p = GetRandomPath();
            //get vehicleBehavior
            VehicleBehavior vehicle = other.transform.GetComponent<VehicleBehavior>();
            //junction check
            bool junctionOK = vehicle.CheckJunction(Junction);
            if (!junctionOK) return;
            //decide when to move vehicle
            StartCoroutine(TakePath(vehicle, p));
        }
    }

    private Path GetRandomPath(){
        //get random path
        int index = (Paths.Length == 1) ? 0 : Randomer.Instance.GetInt(0, Paths.Length);
        return Paths[index];
    }

    IEnumerator TakePath(VehicleBehavior v, Path p) {
        while (IsOccupied()) { //stop car
            v.Stop();
            yield return null;
        }
        v.MovePath(p); //move car
        yield return null;
    }

    private bool IsOccupied(){
        foreach (Junction_Occupy slot in OccupySlots) {
            if (slot.IsOccupied()) return true;
        }
        return false;
    }
}

[System.Serializable]
public class Path {
    public Dir[] Dirs;
}

public enum Dir {
    forward, left, right
}
