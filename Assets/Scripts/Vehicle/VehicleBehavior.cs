using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleBehavior : MonoBehaviour
{
    public float speed = 0.02f;

    public float minSpeed = 0.01f;
    public float maxSpeed = 0.04f;

    Transform Junction = null;

    private void Start() {
        speed = Random.Range(minSpeed, maxSpeed);
    }

    void FixedUpdate() {
        DefaultMove();
    }

    public int BumpCount = 0;
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Bumper") BumpCount++;
    }
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Bumper") BumpCount--;
    }
    public bool IsBumping() {
        return BumpCount > 0;
    }

    bool Default = true;
    void DefaultMove() {
        if (!Default) return;
        if (IsBumping()) return;
        transform.position += transform.forward * speed;
    }

    public void Stop(){
        Default = false;
    }

    public void MovePath(Path path) {
        StartCoroutine(IMovePath(path));
    }

    public bool CheckJunction(Transform J){
        if (Junction == null) {
            Junction = J;
            return true;
        }
        else if (Junction != J){
            Junction = J;
            return true;
        }
        return false;
    }

    float gridSize = 0.41f;
    float initialDistance = 0.1f;

    IEnumerator IMovePath(Path path){
        Default = false; //pause default move behavior
        //move initial forward
        float startTime = Time.time;
        float duration = initialDistance / speed / 50;
        while (Time.time - startTime < duration) {
            transform.position += transform.forward * speed;
            yield return new WaitForFixedUpdate();
        }
        //move in path
        foreach (Dir dir in path.Dirs){
            startTime = Time.time;
            //move forward
            if (dir == Dir.forward){
                //Debug.Log("forward");
                duration = gridSize / speed / 50;
                while (Time.time-startTime < duration){
                    transform.position += transform.forward * speed;
                    yield return new WaitForFixedUpdate();
                }
            }
            //move right or left
            else {
                //Debug.Log("left/right");
                duration = (gridSize/2) / speed / 50;
                while (Time.time - startTime < duration) {
                    transform.position += transform.forward * speed;
                    yield return new WaitForFixedUpdate();
                }
                float angle = (dir == Dir.right) ? 90 : -90;
                transform.Rotate(0,angle, 0);
                startTime = Time.time;
                while (Time.time - startTime < duration) {
                    transform.position += transform.forward * speed;
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        Default = true;
    }
}
