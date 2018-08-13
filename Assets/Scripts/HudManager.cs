using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudManager : MonoBehaviour {

    public TMP_Text scoreText;
    public TMP_Text comboText;
    public TMP_Text coldOssanText;
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
                StartCoroutine(FlashText(comboText, Color.white, Color.green));
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
                StartCoroutine(FlashText(scoreText, Color.white, Color.green));
            }
            score = value;
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public static readonly Color COLD_COLOR = new Color(0.4f, 1f, 1f);
    private int prevOssanCount = 0;
    public void SetColdOssanLevel(int coldOssanCount, int coldOssanLimit)
    {
        coldOssanText.text = "Cold Ossan: " + coldOssanCount + "/" + coldOssanLimit;
        if (coldOssanCount > prevOssanCount)
        {
            StartCoroutine(FlashText(coldOssanText, Color.white, COLD_COLOR));
        }
        prevOssanCount = coldOssanCount;
    }

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
        CurrentCombo += 1f;
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

    private const float POP_TIME = 0.333f;
    private IEnumerator FlashText(TMP_Text text, Color normalColor, Color flashColor)
    {
        text.color = normalColor;
        yield return null;
        text.color = (normalColor + flashColor) / 2f;
        yield return null;
        text.color = flashColor;
        yield return null;

        float elapsedTime = 0;
        float progress = 0;
        while (progress <= 1)
        {
            progress = elapsedTime / POP_TIME;
            elapsedTime += Time.deltaTime;
            Color currentColor = Color.Lerp(flashColor, normalColor, progress);
            text.color = currentColor;
            yield return null;
        }
        text.color = normalColor;
    }
}
