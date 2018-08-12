using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatManager : MonoBehaviour {

    public Button logButton;
    public Transform heatborder;
    private float currentHeat;

    public static HeatManager instance;

    private void Awake()
    {
        instance = this;
        logButton.onClick.AddListener(delegate { AddLog(); });
    }

    void Update () {
        currentHeat -= (0.032f * Time.deltaTime);
        heatborder.localScale = new Vector3(currentHeat, currentHeat, currentHeat);
	}

    private void AddLog()
    {
        currentHeat = Mathf.Min(MAX_FIRE, currentHeat + 0.30f);
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
