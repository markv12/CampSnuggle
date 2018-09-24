using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HudManager : MonoBehaviour {

    public TMP_Text scoreText;
    public TMP_Text comboText;
    public TMP_Text coldOssanWarning;
    public TMP_Text coldOssanTimer;
    public RawImage warningBlackout;
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
    public void SetColdOssanLevel(int coldOssanCount, int coldOssanLimit)
    {
        bool overLimit = coldOssanCount >= coldOssanLimit;
        if (overLimit)
        {
            coldOssanWarning.text = "Too Many Cold Ossan! " + coldOssanCount + "/" + coldOssanLimit;
        }
        coldOssanWarning.enabled = overLimit;
        FadeWarningBlackout(overLimit);
    }

    public void SetColdTimeRemaining(int time)
    {
        if (time == -1)
        {
            coldOssanTimer.enabled = false;
        }
        else
        {
            coldOssanTimer.text = time.ToString();
            coldOssanTimer.enabled = true;
        }
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

    public void FadeWarningBlackout(bool fadeIn)
    {
        this.EnsureCoroutineStopped(ref warningBlackoutRoutine);
        warningBlackoutRoutine = StartCoroutine(_FadeWarningBlackout(fadeIn));
    }
    private bool blackoutVisible = false;
    private Coroutine warningBlackoutRoutine = null;
    private IEnumerator _FadeWarningBlackout(bool fadeIn)
    {
        if ((fadeIn && !blackoutVisible) || (!fadeIn && blackoutVisible))
        {
            if (fadeIn)
            {
                warningBlackout.enabled = true;
            }

            Color startColor = warningBlackout.color;
            Color endColor = fadeIn ? new Color(0, 0, 0, 0.7f) : new Color(0, 0, 0, 0f);

            float elapsedTime = 0;
            float progress = 0;
            while (progress <= 1)
            {
                progress = elapsedTime / POP_TIME;
                elapsedTime += Time.deltaTime;
                Color currentColor = Color.Lerp(startColor, endColor, progress);
                warningBlackout.color = currentColor;
                yield return null;
            }
            warningBlackout.color = endColor;

            if (!fadeIn)
            {
                warningBlackout.enabled = false;
            }
            blackoutVisible = fadeIn;
        }
        warningBlackoutRoutine = null;
    }
}
