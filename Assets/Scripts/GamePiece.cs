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

    private void Awake()
    {
        timeToSleep = PieceMouseManager.Instance.currentSleepTime + Random.Range(-5f, 5f);
        PieceMouseManager.Instance.currentSleepTime += Random.Range(-1f, 3f);
        t = transform;
        PieceMouseManager.Instance.RegisterPiece(this);
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
                theRender.sprite = sleepingSprite;
                timeInWarmth += Time.deltaTime;
                if (timeInWarmth >= timeToSleep)
                {
                    Rested = true;
                }
            }
            else
            {
                theRender.sprite = hitSprite;
                timeInWarmth = 0;
            }
        }
        else
        {
            if (!HeatManager.instance.WithinHeatRange(t.position))
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnMouseEnter()
    {
        PieceMouseManager.Instance.SetCurrentPiece(this);
    }

    private void OnMouseExit()
    {
        PieceMouseManager.Instance.SetCurrentPiece(null);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Rested)
        {
            GamePiece thePiece = collision.gameObject.GetComponent<GamePiece>();
            if (thePiece != null)
            {
                float otherVelocity = thePiece.currentVelocity.magnitude;
                if (currentVelocity.magnitude > otherVelocity && getHitRoutine == null)
                {
                    getHitRoutine = StartCoroutine(GetHit());
                }
            }
        }
    }

    private Coroutine getHitRoutine = null;
    private static readonly WaitForSeconds hitWait = new WaitForSeconds(1.2f);
    private static readonly WaitForSeconds invulnerableWait = new WaitForSeconds(0.8f);
    private IEnumerator GetHit()
    {
        theRender.sprite = hitSprite;
        yield return hitWait;
        theRender.sprite = sleepingSprite;
        yield return invulnerableWait;
        getHitRoutine = null;
    }

    private void OnDestroy()
    {
        PieceMouseManager m = PieceMouseManager.Instance;
        if(m != null)
        {
            m.UnregisterPiece(this);
        }
    }
}
