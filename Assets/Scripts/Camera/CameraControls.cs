using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    Transform cam;

    public float PanFactor = 0.05f;
    public float PanZoomRatio = 0.5f;
    public float PanZoomMin = 0.5f;
    public float RotFactor = 0.5f;
    public float ZoomFactor = 0.3f;
    public float ZoomBackFactor = 0.3f;

    public float TapBillboardZoom = 0.5f;
    float LerpElapsed = 0;
    float LerpTime = 0.5f;
    bool Lerping = false;
    Vector3 BillboardMovePos;

    MultiTouchState multiTouch = MultiTouchState.Ended;
    enum MultiTouchState {Init, Began, Ongoing, Ended}
    public float MultiTouchInitDelay = 0.1f;
    float MultiTouchInitStart;
    public float RotZoomLockMin = 0.1f;
    bool StartedWithRot = false;

    private void Start() {
        cam = Camera.main.transform;
    }

    private void LateUpdate() {
        if (!Lerping) {
            TapBillboard();
            Calculate();
            UpdateTouchState();
            CheckPan();
            CheckRotation();
            CheckPinch();
        }
    }

    void UpdateTouchState(){
        if (multiTouch == MultiTouchState.Ended){ //reset
            StartedWithRot = false;
        }
    }

    void TapBillboard(){
        if (Input.touchCount > 1) return;
        if (!Input.GetMouseButtonDown(0)) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f)){
            if(hit.collider.tag == "Dashboard") {
                Transform billboard = hit.collider.transform.parent;
                //temporary reset of x rotation
                Vector3 billboardEuler = billboard.localEulerAngles;
                Vector3 temp = billboard.localEulerAngles;
                temp.x = 0;
                billboard.localEulerAngles = temp;
                Debug.Log(billboard.localEulerAngles);
                //get forward vector
                Vector3 forward = billboard.forward;
                Debug.Log(billboard.forward);
                //undo reset of x rotation
                billboard.localEulerAngles = billboardEuler;
                //get move position
                BillboardMovePos = billboard.position + forward * 2f;
                Debug.Log(BillboardMovePos);
                StartCoroutine(DoLerp());
            }
        }
    }
    IEnumerator DoLerp(){
        Lerping = true;
        LerpElapsed = 0;
        float startZoom = CameraBehavior.Instance.GetZoom();
        Vector3 startPos = cam.parent.position;
        float finalX;
        float finalZ;
        while (LerpElapsed < LerpTime){
            float perc = LerpElapsed / LerpTime;
            //set zoom
            float newZoom = Mathf.Lerp(startZoom, TapBillboardZoom, perc);
            CameraBehavior.Instance.SetZoom(newZoom);
            //set x position
            finalX = Mathf.Lerp(startPos.x, BillboardMovePos.x, perc);
            //set z position
            finalZ = Mathf.Lerp(startPos.z, BillboardMovePos.z, perc);
            //set position
            Vector3 pos = cam.parent.position;
            pos.x = finalX;
            pos.z = finalZ;
            cam.parent.position = pos;
            //increment
            LerpElapsed += Time.deltaTime;
            yield return null;
        }
        Lerping = false;
    }

    void CheckPan(){
        if (Input.touchCount != 1) return;
        Touch touch = Input.GetTouch(0);
        //forward or back
        Vector3 forward = cam.parent.forward.normalized;
        forward.y = 0;
        Vector3 right = cam.parent.right.normalized;
        right.y = 0;
        float zoomMultiplier = CameraBehavior.Instance.GetZoom() * PanZoomRatio + PanZoomMin;
        float newX = -touch.deltaPosition.x * PanFactor * zoomMultiplier;
        float newY = -touch.deltaPosition.y * PanFactor * zoomMultiplier;
        cam.parent.position += forward * newY;
        cam.parent.position += right * newX;
    }

    void CheckRotation(){
        if (multiTouch == MultiTouchState.Init) return;
        if (multiTouch == MultiTouchState.Began) Debug.Log(turnAngleDelta);
        if (multiTouch == MultiTouchState.Began && turnAngleDelta > RotZoomLockMin) StartedWithRot = true;
        Vector3 camEuler = cam.parent.eulerAngles;
        camEuler.y += turnAngleDelta * RotFactor;
        cam.parent.eulerAngles = camEuler;
    }

    void CheckPinch(){
        if (multiTouch == MultiTouchState.Init) return;
        if (StartedWithRot) return; //rotZoomLock
        float pinchAmount = 0;
        if (Mathf.Abs(pinchDistanceDelta) > 0) { // zoom
            pinchAmount = pinchDistanceDelta;
        }
        bool zoomSuccess = CameraBehavior.Instance.UpdateZoom(-pinchAmount*ZoomFactor);
        //move back
        if (zoomSuccess) cam.parent.position -= cam.parent.forward * ZoomBackFactor * pinchDistanceDelta;
    }

    //detect touch pinch & turn
    const float pinchTurnRatio = Mathf.PI / 2;
    const float minTurnAngle = 0;

    const float pinchRatio = 1;
    const float minPinchDistance = 0;

    const float panRatio = 1;
    const float minPanDistance = 0;

    //The delta of the angle between two touch points
    static public float turnAngleDelta;

    //The angle between two touch points
    static public float turnAngle;

    //The delta of the distance between two touch points that were distancing from each other
    static public float pinchDistanceDelta;

    //The distance between two touch points that were distancing from each other
    static public float pinchDistance;

    //Calculates Pinch and Turn - This should be used inside LateUpdate
    void Calculate() {
        pinchDistance = pinchDistanceDelta = 0;
        turnAngle = turnAngleDelta = 0;

        // if two fingers are touching the screen at the same time ...
        if (Input.touchCount == 2) {
            //update multiTouch
            if (multiTouch == MultiTouchState.Ended) {
                multiTouch = MultiTouchState.Init;
                MultiTouchInitStart = Time.time;
            }
            else if (multiTouch == MultiTouchState.Init && Time.time-MultiTouchInitStart > MultiTouchInitDelay) {
                multiTouch = MultiTouchState.Began;
            }
            else if (multiTouch == MultiTouchState.Began) {
                multiTouch = MultiTouchState.Ongoing;
            }

            Touch touch1 = Input.touches[0];
            Touch touch2 = Input.touches[1];

            // ... if at least one of them moved ...
            if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved) {
                // ... check the delta distance between them ...
                pinchDistance = Vector2.Distance(touch1.position, touch2.position);
                float prevDistance = Vector2.Distance(touch1.position - touch1.deltaPosition,
                                                      touch2.position - touch2.deltaPosition);
                pinchDistanceDelta = pinchDistance - prevDistance;

                // ... if it's greater than a minimum threshold, it's a pinch!
                if (Mathf.Abs(pinchDistanceDelta) > minPinchDistance) pinchDistanceDelta *= pinchRatio;
                else pinchDistance = pinchDistanceDelta = 0;

                // ... check the delta angle between them ...
                turnAngle = Angle(touch1.position, touch2.position);
                float prevTurn = Angle(touch1.position - touch1.deltaPosition,
                                       touch2.position - touch2.deltaPosition);
                turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);

                // ... if it's greater than a minimum threshold, it's a turn!
                if (Mathf.Abs(turnAngleDelta) > minTurnAngle) turnAngleDelta *= pinchTurnRatio;
                else turnAngle = turnAngleDelta = 0;
            }
        }
        else {
            multiTouch = MultiTouchState.Ended;
        }
    }

    static private float Angle(Vector2 pos1, Vector2 pos2) {
        Vector2 from = pos2 - pos1;
        Vector2 to = new Vector2(1, 0);

        float result = Vector2.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);

        if (cross.z > 0) result = 360f - result;

        return result;
    }
}
