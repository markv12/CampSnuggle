using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndScreenManager : MonoBehaviour
{
    public TitleScreenManager titleScreen;

    public TMP_Text scoreText;

    public Button retryButton;
    public RectTransform container;

    void Start()
    {
        retryButton.onClick.AddListener(delegate { Retry(); });
    }

    public void ShowScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
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
