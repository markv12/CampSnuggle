using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour {

    public HeatManager heatManager;
    public PieceSpawner pieceSpawner;

    public Button startButton;
    public RectTransform container;
    private static readonly Vector2 containerStartPosition = Vector2.zero;
    private static readonly Vector2 containerEndPosition = new Vector2(0, -720);

    void Start () {
        heatManager.enabled = false;
        pieceSpawner.enabled = false;
        startButton.onClick.AddListener(delegate { StartGame(); });
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
        }
	}

    private void StartGame()
    {
        if (!heatManager.enabled)
        {
            StartCoroutine(RemoveTitleScreen());
        }
    }

    public const float moveTime = 1f;
    private IEnumerator RemoveTitleScreen()
    {
        //heatManager.enabled = true;
        pieceSpawner.enabled = true;
        pieceSpawner.StartSpawning();
        float progress = 0;
        float elapsedTime = 0;
        while (progress <= 1)
        {
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / moveTime;
            float easedProgress = Easing.easeInSine(0, 1, progress);
            container.anchoredPosition = Vector2.Lerp(containerStartPosition, containerEndPosition, easedProgress);
            yield return null;
        }
        container.anchoredPosition = containerEndPosition;
        container.gameObject.SetActive(false);
    }

    public void GoToTitleScreen(System.Action onComplete)
    {
        StartCoroutine(_GoToTitleScreen(onComplete));
    }

    private IEnumerator _GoToTitleScreen(System.Action onComplete)
    {
        container.gameObject.SetActive(true);
        float progress = 0;
        float elapsedTime = 0;
        while (progress <= 1)
        {
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / moveTime;
            float easedProgress = Easing.easeInSine(1, 0, progress);
            container.anchoredPosition = Vector2.Lerp(containerStartPosition, containerEndPosition, easedProgress);
            yield return null;
        }
        container.anchoredPosition = containerStartPosition;
        onComplete();
    }
}
