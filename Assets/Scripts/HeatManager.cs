using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatManager : MonoBehaviour {

    public Transform heatborder;
    public Transform flames;
    private float currentHeat;

    public static HeatManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Update () {
        currentHeat -= (0.032f * Time.deltaTime);
        heatborder.localScale = new Vector3(currentHeat, currentHeat, currentHeat);
        float fractionOfMaxFire = currentHeat / MAX_FIRE;
        float easedFraction = Easing.easeInCubic(0, 1, fractionOfMaxFire);
        flames.localScale = new Vector3(easedFraction, easedFraction, easedFraction);
        float y = Mathf.Lerp(0.2f, 1.05f, easedFraction);
        flames.localPosition = new Vector3(0, y, 0);
    }

    public void AddLog()
    {
        currentHeat = Mathf.Min(MAX_FIRE, currentHeat + 1f);
    }

    public bool WithinHeatRange(Vector3 pos)
    {
        return pos.magnitude <= currentHeat;
    }

    private const float START_FIRE = 8;
    private const float MAX_FIRE = 9.7f;
    private void OnEnable()
    {
        currentHeat = START_FIRE;
    }
}
