using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public static VehicleManager Instance;
    private void Awake() { Instance = this; }

    public int MaxVehicleCount = 20;
    public float MinSpawnDelay = 1f;
    public float MaxSpawnDelay = 3f;

    public Transform[] SpawnPoints;
    public GameObject[] Vehicles;

    public int CurVehicleCount = 0;
    float SpawnStart;
    float SpawnDelay;

    public void Start() {
        DoSpawn();
    }

    public void FixedUpdate() {
        if (Time.time - SpawnStart > SpawnDelay) DoSpawn();
    }

    void ResetValues() {
        SpawnStart = Time.time;
        SpawnDelay = Random.Range(MinSpawnDelay, MaxSpawnDelay);
    }

    void DoSpawn(){
        if (CurVehicleCount >= MaxVehicleCount) return;
        //get vehicle to spawn
        int vIndex = (Vehicles.Length > 1) ? Randomer.Instance.GetInt(0, Vehicles.Length) : 0;
        //get location to spawn
        int sIndex = (SpawnPoints.Length > 1) ? Randomer.Instance.GetInt(0, SpawnPoints.Length) : 0;
        //spawn vehicle
        Instantiate(Vehicles[vIndex], SpawnPoints[sIndex].position, SpawnPoints[sIndex].rotation);
        //reset values
        ResetValues();
        CurVehicleCount++;
    }

    public void DeSpawn(){
        CurVehicleCount--;
    }

}
