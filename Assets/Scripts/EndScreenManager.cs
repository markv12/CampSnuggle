using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour
{

    public HeatManager heatManager;
    public PieceSpawner pieceSpawner;

    public Button startButton;
    public RectTransform container;
    private static readonly Vector2 containerStartPosition = Vector2.zero;
    private static readonly Vector2 containerEndPosition = new Vector2(0, -720);

    void Start()
    {
        startButton.onClick.AddListener(delegate { EndGame(); });
    }

    private void EndGame()
    {
        if (!pieceSpawner.enabled)
        {
            pieceSpawner.enabled = true;
            LoadingScreen.instance.Show(_EndGame(), 0.666f);
        }
    }

    private IEnumerator _EndGame()
    {
        //heatManager.enabled = true;
        container.gameObject.SetActive(false);
        yield return null;
        pieceSpawner.StartSpawning();
    }
}
