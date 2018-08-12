using System;
using System.Collections;
using UnityEngine;
public class LoadingScreen : MonoBehaviour
{
    public CanvasGroup mainGroup;

    public static LoadingScreen instance;

    private void Awake()
    {
        mainGroup.alpha = 0;
        instance = this;
    }

    public void Show(IEnumerator load, float fadeTime)
    {
        gameObject.SetActive(true);
        StartCoroutine(CO_Show(load, fadeTime));
    }

    private IEnumerator CO_Show(IEnumerator load, float fadeTime)
    {
        float elapsedTime = 0;
        float progress = 0;

        while (progress <= 1)
        {
            progress = elapsedTime / fadeTime;
            elapsedTime += Time.unscaledDeltaTime;
            mainGroup.alpha = progress;
            yield return null;
        }
        mainGroup.alpha = 1;
        yield return null;

        if (load != null)
        {
            yield return StartCoroutine(load);
        }
        yield return null;
        Time.timeScale = 1;

        elapsedTime = 0;
        progress = 0;

        while (progress <= 1)
        {
            progress = elapsedTime / fadeTime;
            float dTime = Time.unscaledDeltaTime;
            if (dTime > 0.1f)
            {
                dTime = 0.01666f; //Don't jump the fade too far on long frames.
            }
            elapsedTime += dTime;
            mainGroup.alpha = 1 - progress;
            yield return null;
        }
        mainGroup.alpha = 0;
        gameObject.SetActive(false);
    }
}
