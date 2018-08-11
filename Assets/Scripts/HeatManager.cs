using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatManager : MonoBehaviour {

    public Transform heatborder;
    private float currentHeat;

    public static HeatManager instance;

    private void Awake()
    {
        instance = this;
        currentHeat = heatborder.localScale.x;
    }

    // Update is called once per frame
    void Update () {
        currentHeat -= (0.015f * Time.deltaTime);
        heatborder.localScale = new Vector3(currentHeat, currentHeat, currentHeat);
	}

    public bool WithinHeatRange(Vector3 pos)
    {
        return pos.magnitude <= currentHeat;
    }
}
