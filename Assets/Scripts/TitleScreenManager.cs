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
        if (!pieceSpawner.enabled)
        {
            pieceSpawner.enabled = true;
            LoadingScreen.instance.Show(_StartGame(), 0.666f);
        }
    }

    private IEnumerator _StartGame()
    {
        //heatManager.enabled = true;
        container.gameObject.SetActive(false);
        yield return null;
        pieceSpawner.StartSpawning();
    }

    public void GoToTitleScreen(System.Action onComplete)
    {
        LoadingScreen.instance.Show(_GoToTitleScreen(onComplete), 0.666f);
    }

    private IEnumerator _GoToTitleScreen(System.Action onComplete)
    {
        container.gameObject.SetActive(true);
        yield return null;
        onComplete();
    }
}
