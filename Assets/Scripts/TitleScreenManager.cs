using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour {

    public HeatManager heatManager;
    public PieceSpawner pieceSpawner;

    public Button startButton;
    public RectTransform container;

    public AudioSource titleAudio;

    void Start () {
        heatManager.enabled = false;
        pieceSpawner.enabled = false;
        startButton.onClick.AddListener(delegate { StartGame(); });
        StartCoroutine(FadeAudioSourceVolume(titleAudio, 0.8f, 2f));
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
        }
	}

    private void StartGame()
    {
        if (!pieceSpawner.IsSpawning())
        {
            pieceSpawner.StartSpawning();
            LoadingScreen.instance.Show(_StartGame(), 0.666f);
            StartCoroutine(FadeAudioSourceVolume(titleAudio, 0f, 2f));
        }
    }

    private IEnumerator _StartGame()
    {
        container.gameObject.SetActive(false);
        yield return null;
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

    public static IEnumerator FadeAudioSourceVolume(AudioSource source, float endVolume, float fadeTime)
    {
        float startVolume = source.volume;
        if(startVolume == 0 && endVolume != 0)
        {
            source.Play();
        }
        float elapsedTime = 0;
        float progress = 0;
        while (progress <= 1)
        {
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / fadeTime;
            float easedProgress = Easing.easeInOutSine(0, 1, progress);
            source.volume = Mathf.Lerp(startVolume, endVolume, easedProgress);
            yield return null;
        }
        source.volume = endVolume;
        if(endVolume == 0)
        {
            source.Stop();
        }
    }
}
