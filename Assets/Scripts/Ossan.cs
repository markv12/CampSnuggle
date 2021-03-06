﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ossan : DragablePiece {
    public SpriteRenderer theRender;
    public SpriteRenderer maskHole;
    public Sprite sleepingSprite;
    public Sprite hitSprite;
    public Sprite restedSprite;
    public Sprite coldSprite;
    public Image sleepCountdown;
    public Image heart;
    public AudioSource theSource;

    private float timeInWarmth = 0f;
    private float timeToSleep;

    private static AudioClip[] gruntClips = null;
    private static int currentGruntIndex = 0;
    private static int[] gruntIndices;
    private static AudioClip GetNextGruntClip()
    {
        int index = gruntIndices[currentGruntIndex];
        currentGruntIndex++;
        if(currentGruntIndex >= gruntIndices.Length)
        {
            currentGruntIndex = 0;
            PieceSpawner.SortInPlaceRandom(gruntIndices);
        }
        return gruntClips[index];
    }

    private static AudioClip[] scoreClips = null;
    private static int currentScoreIndex = 0;
    private static int[] scoreIndices;
    private static AudioClip GetNextScoreClip()
    {
        int index = scoreIndices[currentScoreIndex];
        currentScoreIndex++;
        if (currentScoreIndex >= scoreIndices.Length)
        {
            currentScoreIndex = 0;
            PieceSpawner.SortInPlaceRandom(scoreIndices);
        }
        return scoreClips[index];
    }

    private bool rested = false;
    private bool Rested {
        get
        {
            return rested;
        }
        set
        {
            rested = value;
            if (rested == true)
            {
                if(getHitRoutine != null)
                {
                    StopCoroutine(getHitRoutine);
                    getHitRoutine = null;
                }
                theRender.sprite = restedSprite;
                heart.enabled = true;
            }
        }
    }

    private bool withinFire = true;
    private bool WithinFire
    {
        get
        {
            return withinFire;
        }
        set
        {
            withinFire = value;
            maskHole.enabled = !withinFire;
            if (getHitRoutine == null)
            {
                theRender.sprite = withinFire ? sleepingSprite : coldSprite;
            }
            if (withinFire)
            {
                PieceMouseManager.instance.UnregisterColdPerson(this);
                if (countdownFlashRoutine != null)
                {
                    StopCoroutine(countdownFlashRoutine);
                    countdownFlashRoutine = null;
                }
                sleepCountdown.color = Color.white;
            }
            else
            {
                PieceMouseManager.instance.RegisterColdPerson(this);
                if(countdownFlashRoutine != null)
                {
                    StopCoroutine(countdownFlashRoutine);
                    countdownFlashRoutine = null;
                }
                sleepCountdown.color = HudManager.COLD_COLOR;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        heart.enabled = false;
        if (gruntClips == null)
        {
            AudioList theList = (Resources.Load("Grunts") as AudioList);
            gruntClips = theList.clips;
            gruntIndices = new int[gruntClips.Length];
            for (int i = 0; i < gruntIndices.Length; i++)
            {
                gruntIndices[i] = i;
            }
            PieceSpawner.SortInPlaceRandom(gruntIndices);
        }
        if (scoreClips == null)
        {
            AudioList theList = (Resources.Load("ScoreSounds") as AudioList);
            scoreClips = theList.clips;
            scoreIndices = new int[scoreClips.Length];
            for (int i = 0; i < scoreIndices.Length; i++)
            {
                scoreIndices[i] = i;
            }
            PieceSpawner.SortInPlaceRandom(scoreIndices);
        }
        sleepCountdown.fillAmount = 0;
    }

    private void Start()
    {
        timeToSleep = PieceMouseManager.instance.currentSleepTime + Random.Range(-5f, 5f);
        PieceMouseManager.instance.currentSleepTime += Random.Range(-1f, 2.5f);
    }

    private float timeSinceLastShiver = 0;
    private const float SHIVER_INTERVAL = 3;
    private void Update()
    {
        if (!Rested)
        {
            if (HeatManager.instance.WithinHeatRange(center.position))
            {
                if (!WithinFire)
                {
                    WithinFire = true;
                }
                timeInWarmth += Time.deltaTime;
                sleepCountdown.fillAmount = 1f-(timeInWarmth / timeToSleep);
                if (timeInWarmth >= timeToSleep)
                {
                    Rested = true;
                }
            }
            else
            {
                if (WithinFire)
                {
                    WithinFire = false;
                }
                if(moveInRoutine == null)
                {
                    Vector3 pos = center.position;
                    if(Mathf.Abs(pos.x) > 9.5f || Mathf.Abs(pos.y) > 17f)
                    {
                        rigid.AddForce(-pos.normalized * 50);
                    }
                }
                timeSinceLastShiver += Time.deltaTime;
                if(timeSinceLastShiver >= SHIVER_INTERVAL)
                {
                    timeSinceLastShiver = 0;
                    StartCoroutine(Shiver());
                }
            }
        }
        else
        {
            if (moveOutRoutine == null && !HeatManager.instance.WithinHeatRange(center.position))
            {
                HudManager.instance.MarkScore();
                theSource.PlayOneShot(GetNextScoreClip());
                moveOutRoutine = StartCoroutine(MovePieceOut());
            }
            float heartSize = 0.8f + (Mathf.Sin(Time.time*6.666f) * 0.18f);
            heart.rectTransform.sizeDelta = new Vector2(heartSize, heartSize);
        }
    }

    private const float LOWER_HIT_LIMIT = 5.5f;
    private const float UPPER_HIT_LIMIT = 40f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Rested)
        {
            if (getHitRoutine == null && HeatManager.instance.WithinHeatRange(center.position))
            {
                Vector2 vel1 = (collision.rigidbody == null) ? Vector2.zero : collision.rigidbody.velocity;
                Vector2 vel2 = (collision.otherRigidbody == null) ? Vector2.zero : collision.otherRigidbody.velocity;
                float hitSpeed = (vel1 + vel2).magnitude;
                if (hitSpeed > LOWER_HIT_LIMIT)
                {
                    hitSpeed = Mathf.Min(UPPER_HIT_LIMIT, hitSpeed);
                    float hitStrength = Mathf.InverseLerp(LOWER_HIT_LIMIT, UPPER_HIT_LIMIT, hitSpeed);
                    float timePenalty = Mathf.Lerp(3f, 8f, hitStrength);
                    timeInWarmth = Mathf.Max(0, timeInWarmth - timePenalty);
                    getHitRoutine = StartCoroutine(GetHit());
                }
            }
        }
    }

    private Coroutine getHitRoutine = null;
    private const float HIT_RED_TIME = 0.4f;
    private static readonly WaitForSeconds hitWait = new WaitForSeconds(HIT_RED_TIME);
    private static readonly WaitForSeconds invulnerableWait = new WaitForSeconds(0.5f);
    private IEnumerator GetHit()
    {
        countdownFlashRoutine = StartCoroutine(FlashCountdownColor(sleepCountdown));
        theSource.PlayOneShot(GetNextGruntClip());
        theRender.sprite = hitSprite;
        yield return hitWait;
        theRender.sprite = WithinFire ? sleepingSprite : coldSprite;
        yield return invulnerableWait;
        theRender.sprite = WithinFire ? sleepingSprite : coldSprite;
        getHitRoutine = null;
    }

    private static readonly Color popColor = Color.red;
    private static readonly Color normalColor = Color.white;
    private Coroutine countdownFlashRoutine = null;
    private IEnumerator FlashCountdownColor(Image countdown)
    {
        countdown.color = normalColor;
        yield return null;
        countdown.color = (normalColor + popColor) / 2f;
        yield return null;
        countdown.color = popColor;
        yield return null;

        float elapsedTime = 0;
        float progress = 0;
        while (progress <= 1)
        {
            progress = elapsedTime / HIT_RED_TIME;
            elapsedTime += Time.deltaTime;
            Color currentColor = Color.Lerp(popColor, normalColor, progress);
            countdown.color = currentColor;
            yield return null;
        }
        countdown.color = normalColor;
        countdownFlashRoutine = null;
    }

    private static readonly WaitForSeconds shiverWait = new WaitForSeconds(0.06f);
    private IEnumerator Shiver()
    {
        float magnitude = 4f;
        t.Rotate(0, 0, -magnitude);
        yield return shiverWait;
        t.Rotate(0, 0, magnitude);
        yield return shiverWait;
        t.Rotate(0, 0, -magnitude);
        yield return shiverWait;
        t.Rotate(0, 0, magnitude);
        yield return shiverWait;
        t.Rotate(0, 0, -magnitude);
        yield return shiverWait;
        t.Rotate(0, 0, magnitude);
        yield return shiverWait;
    }
}
