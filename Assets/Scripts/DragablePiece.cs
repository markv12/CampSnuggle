using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragablePiece : MonoBehaviour {
    protected Transform t;
    public Transform center;
    public Rigidbody2D rigid;
    public PolygonCollider2D mainCollider;

    protected virtual void Awake()
    {
        t = transform;
    }

    private void OnMouseEnter()
    {
        PieceMouseManager.instance.SetCurrentPiece(this);
    }

    private void OnMouseExit()
    {
        PieceMouseManager.instance.SetCurrentPiece(null);
    }

    protected Coroutine moveInRoutine = null;
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

    protected Coroutine moveOutRoutine = null;
    private static readonly WaitForSeconds moveWait = new WaitForSeconds(1f);
    private const float MOVE_TIME = 2f;
    protected IEnumerator MovePieceOut()
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
