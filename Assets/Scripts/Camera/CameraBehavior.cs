using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform CamRoot;
    public Transform CamT;
    [Range(0.0f, 10.0f)]
    public float Zoom = 10f;
    public float MinZoom = 0f;
    public float MaxZoom = 10f;
    public float MaxHeight = 25f;
    public float MinHeight = 0.5f;

    public static CameraBehavior Instance;
    private void Awake() { Instance = this; }

    void Update() {
        HandleZoom();
    }

    void HandleZoom(){        
        if (Zoom > MaxZoom) Zoom = MaxZoom;
        if (Zoom < MinZoom) Zoom = MinZoom;
        //handle rotation
        Vector3 Rot = CamT.localEulerAngles;
        Rot.x = Zoom * 8.9f;
        CamT.localEulerAngles = Rot;
        //handle position
        Vector3 Pos = CamRoot.position;
        Pos.y = Zoom / (MaxZoom - MinZoom) * (MaxHeight - MinHeight) + MinHeight;
        CamRoot.position = Pos;
    }

    public bool UpdateZoom(float increment){
        if (Zoom == MaxZoom && increment > 0) return false;
        else if (Zoom == MinZoom && increment < 0) return false;
        //do zoom
        if (Zoom + increment > MaxZoom) Zoom = MaxZoom;
        else if (Zoom + increment < MinZoom) Zoom = MinZoom;
        else Zoom += increment;
        return true;
    }

    public void SetZoom(float value){
        if (value > MaxZoom) Zoom = MaxZoom;
        else if (value < MinZoom) Zoom = MinZoom;
        else Zoom = value;
    }

    public float GetZoom(){
        return Zoom;
    }
}
