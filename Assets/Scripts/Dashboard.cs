using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dashboard : MonoBehaviour
{
    public Text TotalCrossed;
    public Text CurrentlyIn;

    public void Update() {
        transform.forward = -Camera.main.transform.forward;
    }

    public void Update_TotalCrossed(int val){
        TotalCrossed.text = val.ToString();
    }

    public void Update_CurrentlyIn(int val) {
        CurrentlyIn.text = val.ToString();
    }
}
