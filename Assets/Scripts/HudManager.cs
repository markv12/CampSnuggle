using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudManager : MonoBehaviour {

    public TMP_Text scoreText;
    public TMP_Text comboText;
    public RectTransform comboTimer;

    public static HudManager instance;

    private float currentCombo = 1;
    private float CurrentCombo
    {
        get
        {
            return currentCombo;
        }
        set
        {
            currentCombo = value;
            comboText.text = (currentCombo <= 1) ? "" : ("Combo: " + currentCombo.ToString() + "x");
        }
    }

    public void MarkScore()
    {
        Score += (int)(10f * CurrentCombo);
        if(comboRoutine != null)
        {
            StopCoroutine(comboRoutine);
            comboRoutine = null;
        }
        comboRoutine = StartCoroutine(Combo());
    }

    private int score = 0;
    private int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            scoreText.text = "Score: " + score.ToString();

        }
    }

    // Use this for initialization
    void Awake () {
        instance = this;
        comboTimer.sizeDelta = comboTimerSmallSize;
        comboText.text = "";
	}

    private const float COMBO_TIME = 3f;
    private static readonly Vector2 comboTimerSmallSize = new Vector2(0, 8);
    private static readonly Vector2 comboTimerLargeSize = new Vector2(240, 8);
    private Coroutine comboRoutine = null;
    private IEnumerator Combo()
    {
        CurrentCombo += 0.5f;
        float elapsedTime = 0;
        float progress = 0;
        while (progress <= 1)
        {
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / COMBO_TIME;
            comboTimer.sizeDelta = Vector2.Lerp(comboTimerLargeSize, comboTimerSmallSize, progress);
            yield return null;
        }
        comboTimer.sizeDelta = comboTimerSmallSize;
        CurrentCombo = 1;
        comboRoutine = null;
        comboText.text = "";
    }
}
