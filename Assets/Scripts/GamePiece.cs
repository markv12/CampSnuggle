using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePiece : MonoBehaviour {
    private Transform t;
    public Transform center;
    public SpriteRenderer theRender;
    public Sprite sleepingSprite;
    public Sprite hitSprite;
    public Sprite restedSprite;
    public Image sleepCountdown;
    public Rigidbody2D rigid;
    public PolygonCollider2D mainCollider;
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
                sleepCountdown.color = Color.white;
                theRender.sprite = restedSprite;
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
            if (getHitRoutine == null)
            {
                theRender.sprite = withinFire ? sleepingSprite : hitSprite;
            }
            if (withinFire)
            {
                PieceMouseManager.instance.UnregisterColdPerson(this);
            }
            else
            {
                PieceMouseManager.instance.RegisterColdPerson(this);
            }
        }
    }

    private void Awake()
    {
        if(gruntClips == null)
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
        t = transform;
        sleepCountdown.fillAmount = 0;
    }

    private void Start()
    {
        timeToSleep = PieceMouseManager.instance.currentSleepTime + Random.Range(-5f, 5f);
        PieceMouseManager.instance.currentSleepTime += Random.Range(-1f, 2f);
    }

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
                    if(Mathf.Abs(pos.x) > 18f || Mathf.Abs(pos.y) > 10f)
                    {
                        rigid.AddForce(-pos.normalized * 50);
                    }
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
        }
    }

    private void OnMouseEnter()
    {
        PieceMouseManager.instance.SetCurrentPiece(this);
    }

    private void OnMouseExit()
    {
        PieceMouseManager.instance.SetCurrentPiece(null);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Rested)
        {
            if (getHitRoutine == null && HeatManager.instance.WithinHeatRange(center.position))
            {
                timeInWarmth = Mathf.Max(0, timeInWarmth - 1.8f);
                getHitRoutine = StartCoroutine(GetHit());
            }
        }
    }

    private Coroutine getHitRoutine = null;
    private static readonly WaitForSeconds hitWait = new WaitForSeconds(0.4f);
    private static readonly WaitForSeconds invulnerableWait = new WaitForSeconds(1f);
    private IEnumerator GetHit()
    {
        theSource.PlayOneShot(GetNextGruntClip());
        theRender.sprite = hitSprite;
        sleepCountdown.color = Color.red;
        yield return hitWait;
        theRender.sprite = WithinFire ? sleepingSprite : hitSprite;
        sleepCountdown.color = Color.white;
        yield return invulnerableWait;
        getHitRoutine = null;
    }

    private Coroutine moveInRoutine = null;
    public void MovePieceIn(Vector3 startPos, Vector3 endPos)
    {
        moveInRoutine = StartCoroutine(_MovePieceIn(startPos, endPos));
    }

    private IEnumerator _MovePieceIn(Vector3 startPos, Vector3 endPos)
    {
        t.position = startPos;
        float elapsedTime = 0;
        float progress = 0;
        while (progress <= 1)
        {
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / MOVE_TIME;
            float easedProgress = Easing.easeOutSine(0, 1, progress);
            t.position = Vector3.Lerp(startPos, endPos, easedProgress);
            yield return null;
        }
        t.position = endPos;
        moveInRoutine = null;
    }

    private Coroutine moveOutRoutine = null;
    private static readonly WaitForSeconds moveWait = new WaitForSeconds(1f);
    private const float MOVE_TIME = 2f;
    private IEnumerator MovePieceOut() 
    {
        mainCollider.enabled = false;
        yield return moveWait;
        Vector3 startPos = t.position;
        Vector3 endPos = startPos * 3;
        float elapsedTime = 0;
        float progress = 0;
        while (progress <= 1)
        {
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / MOVE_TIME;
            float easedProgress = Easing.easeInSine(0, 1, progress);
            t.position = Vector3.Lerp(startPos, endPos, easedProgress);
            yield return null;
        }
        Destroy(gameObject);
    }
}
