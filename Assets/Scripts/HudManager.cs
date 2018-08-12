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
            if (value > currentCombo)
            {
                StartCoroutine(FlashText(comboText));
            }
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
    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            if (value > score)
            {
                StartCoroutine(FlashText(scoreText));
            }
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

    private const float COMBO_TIME = 4f;
    private static readonly Vector2 comboTimerSmallSize = new Vector2(0, 8);
    private static readonly Vector2 comboTimerLargeSize = new Vector2(255, 8);
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

    private static readonly Color popColor = Color.green;
    private static readonly Color normalColor = Color.white;
    private const float POP_TIME = 0.333f;
    private IEnumerator FlashText(TMP_Text text)
    {
        text.color = normalColor;
        yield return null;
        text.color = (normalColor + popColor) / 2f;
        yield return null;
        text.color = popColor;
        yield return null;

        float elapsedTime = 0;
        float progress = 0;
        while (progress <= 1)
        {
            progress = elapsedTime / POP_TIME;
            elapsedTime += Time.deltaTime;
            Color currentColor = Color.Lerp(popColor, normalColor, progress);
            text.color = currentColor;
            yield return null;
        }
        text.color = normalColor;
    }
}
