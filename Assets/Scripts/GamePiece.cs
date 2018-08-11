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

    private void Start()
    {
        timeToSleep = PieceMouseManager.instance.currentSleepTime + Random.Range(-5f, 5f);
        PieceMouseManager.instance.currentSleepTime += Random.Range(-1f, 3f);
        t = transform;
        PieceMouseManager.instance.RegisterPiece(this);
    }

    private Vector3 prevPos = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;
    private void Update()
    {
        Vector3 pos = t.position;
        currentVelocity = (prevPos - pos) * Time.deltaTime;
        prevPos = pos;

        if (!Rested)
        {
            if (HeatManager.instance.WithinHeatRange(t.position))
            {
                if (getHitRoutine == null)
                    theRender.sprite = sleepingSprite;
                timeInWarmth += Time.deltaTime;
                if (timeInWarmth >= timeToSleep)
                {
                    Rested = true;
                }
            }
            else
            {
                if (getHitRoutine == null)
                    theRender.sprite = hitSprite;
                timeInWarmth = 0;
            }
        }
        else
        {
            if (moveOutRoutine == null && !HeatManager.instance.WithinHeatRange(t.position))
            {
                PieceMouseManager.instance.Score += 1;
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
            if (getHitRoutine == null)
            {
                timeInWarmth -= 1f;
                getHitRoutine = StartCoroutine(GetHit());
            }
        }
    }

    private Coroutine getHitRoutine = null;
    private static readonly WaitForSeconds hitWait = new WaitForSeconds(0.5f);
    private static readonly WaitForSeconds invulnerableWait = new WaitForSeconds(1f);
    private IEnumerator GetHit()
    {
        theRender.sprite = hitSprite;
        yield return hitWait;
        theRender.sprite = sleepingSprite;
        yield return invulnerableWait;
        getHitRoutine = null;
    }

    private Coroutine moveOutRoutine = null;
    private static readonly WaitForSeconds moveWait = new WaitForSeconds(1f);
    private const float MOVE_TIME = 2f;
    private IEnumerator MovePieceOut() 
    {
        yield return moveWait;
        mainCollider.enabled = false;
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

    private void OnDestroy()
    {
        PieceMouseManager m = PieceMouseManager.instance;
        if(m != null)
        {
            m.UnregisterPiece(this);
        }
    }
}
