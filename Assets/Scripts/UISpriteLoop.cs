using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OneNight.UI
{
    public class UISpriteLoop : MonoBehaviour
    {
        public Image image;
        public Sprite[] sprites;
        public float fps;

        private Coroutine animateRoutine = null;
        private void OnEnable()
        {
            OnDisable();
            animateRoutine = StartCoroutine(Co_Animate());
        }

        private void OnDisable()
        {
            if (animateRoutine != null)
            {
                StopCoroutine(animateRoutine);
            }
        }

        IEnumerator Co_Animate()
        {
            float timePerFrame = 1f / fps;
            while (true)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    image.sprite = sprites[i];

                    float elapsedTime = 0;
                    while (elapsedTime < timePerFrame)
                    {
                        elapsedTime += Time.unscaledDeltaTime; //WaitForSecondsRealtime can't be used without creating garbage every time it's used.
                        yield return null;
                    }
                }
            }
        }
    }
}
