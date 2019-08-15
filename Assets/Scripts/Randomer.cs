using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomer : MonoBehaviour
{
    public static Randomer Instance;
    private void Awake() { Instance = this; }

    System.Random R = new System.Random();

    public int GetInt(int min, int max) {
        return R.Next(min, max);
    }
}
