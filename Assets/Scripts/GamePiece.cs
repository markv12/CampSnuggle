using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour {
    private Transform t;
    public SpriteRenderer theRender;
    public Sprite sleepingSprite;
    public Sprite hitSprite;
    public Sprite restedSprite;
    public Rigidbody2D rigid;
    public PolygonCollider2D mainCollider;
    public AudioSource theSource;
    public AudioList gruntClips;

    private float timeInWarmth = 0f;
    private float timeToSleep;

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
        t = transform;
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
            if (HeatManager.instance.WithinHeatRange(t.position))
            {
                if (!WithinFire)
                {
                    WithinFire = true;
                }
                timeInWarmth += Time.deltaTime;
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
                    Vector3 pos = t.position;
                    if(Mathf.Abs(pos.x) > 18f || Mathf.Abs(pos.y) > 10f)
                    {
                        rigid.AddForce(-pos.normalized * 50);
                    }
                }
            }
        }
        else
        {
            if (moveOutRoutine == null && !HeatManager.instance.WithinHeatRange(t.position))
            {
                HudManager.instance.MarkScore();
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
            if (getHitRoutine == null && HeatManager.instance.WithinHeatRange(t.position))
            {
                timeInWarmth -= 1f;
                getHitRoutine = StartCoroutine(GetHit());
            }
        }
    }

    private Coroutine getHitRoutine = null;
    private static readonly WaitForSeconds hitWait = new WaitForSeconds(0.4f);
    private static readonly WaitForSeconds invulnerableWait = new WaitForSeconds(1f);
    private IEnumerator GetHit()
    {
        theSource.PlayOneShot(gruntClips.clips[Random.Range(0, gruntClips.clips.Length)]);
        theRender.sprite = hitSprite;
        yield return hitWait;
        theRender.sprite = WithinFire ? sleepingSprite : hitSprite;
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
