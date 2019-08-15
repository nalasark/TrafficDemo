using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleDespawn : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vehicle") {
            VehicleManager.Instance.DeSpawn();
            Destroy(other.gameObject);
        }
    }
}
