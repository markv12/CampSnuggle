using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class EndScreenManager : MonoBehaviour
{
    public TitleScreenManager titleScreen;

    public TMP_Text scoreText;
    public TMP_Text[] highScoreTexts;
    public TMP_Text rankText;
    public TMP_Text nextLevelText;

    public Button retryButton;
    public RectTransform container;

    void Start()
    {
        retryButton.onClick.AddListener(delegate { Retry(); });
    }

    private const string HIGH_SCORE_KEY = "highscorelist";
    public void RecordScore(int score)
    {
        List<float> highScores = new List<float>(PlayerPrefsX.GetFloatArray(HIGH_SCORE_KEY));
        highScores.Add(score);
        highScores.Sort((a, b) => -1 * a.CompareTo(b));
        PlayerPrefsX.SetFloatArray(HIGH_SCORE_KEY, highScores.ToArray());
        for (int i = 0; i < highScoreTexts.Length; i++)
        {
            if(i < highScores.Count)
            {
                highScoreTexts[i].text = highScores[i].ToString();
            }
            else
            {
                highScoreTexts[i].text = "";
            }
        }

        scoreText.text = "Score: " + score.ToString();
        SetRankText(score);
    }

    private void SetRankText(int score)
    {
        string rank;
        string next;
        if (score < 200)
        {
            rank = "Bumbling Intern";
            next = "Next Level at: 200";
        }
        else if (score < 500)
        {
            rank = "Trainee Soother";
            next = "Next Level at: 500";
        }
        else if (score < 800)
        {
            rank = "Associate Ossan Bed Keeper";
            next = "Next Level at: 800";
        }
        else if (score < 1000)
        {
            rank = "Professional Nuzzle Attendant";
            next = "Next Level at: 1000";
        }
        else
        {
            rank = "God Snuggle Hustler";
            next = "";
        }
        rankText.text = rank;
        nextLevelText.text = next;
    }

    private void Retry()
    {
        LoadingScreen.instance.Show(_Retry(), 0.666f);
    }

    private IEnumerator _Retry()
    {
        container.gameObject.SetActive(false);
        titleScreen.container.gameObject.SetActive(true);

        yield return null;
    }
}
