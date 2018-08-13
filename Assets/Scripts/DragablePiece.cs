using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragablePiece : MonoBehaviour {
    protected Transform t;
    public Transform center;
    public Rigidbody2D rigid;
    public PolygonCollider2D mainCollider;

    private void OnMouseEnter()
    {
        PieceMouseManager.instance.SetCurrentPiece(this);
    }

    private void OnMouseExit()
    {
        PieceMouseManager.instance.SetCurrentPiece(null);
    }
}
