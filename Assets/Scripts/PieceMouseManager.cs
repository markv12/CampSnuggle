using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PieceMouseManager : MonoBehaviour {

    [NonSerialized]
    public float currentSleepTime = 18;

    private List<GamePiece> spawnedPieces = new List<GamePiece>();
    private GamePiece currentPiece;
    private bool piecePickedUp = false;
    private Vector3 pickUpOffset;

    public static PieceMouseManager instance;

    void Awake()
    {
        instance = this;
    }

	void LateUpdate () {
        if (Input.GetMouseButtonDown(0) && currentPiece != null)
        {
            piecePickedUp = true;
            pickUpOffset = currentPiece.transform.position - GetWorldMousePosition();
        }
        if (Input.GetMouseButtonUp(0))
        {
            piecePickedUp = false;
        }
        if(piecePickedUp && currentPiece != null)
        {
            Vector3 v3 = GetWorldMousePosition();
            Vector3 diff = (v3 + pickUpOffset) - currentPiece.transform.position;
            currentPiece.rigid.velocity = diff*15;

            var d = Input.GetAxis("Mouse ScrollWheel");
            currentPiece.transform.Rotate(0, 0, d * 50);
        }
    }

    private static Vector3 GetWorldMousePosition()
    {
        Vector3 v3 = Input.mousePosition;
        v3.z = 10.0f;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        return v3;
    }

    public void RegisterPiece(GamePiece piece)
    {
        spawnedPieces.Add(piece);
    }

    public void UnregisterPiece(GamePiece piece)
    {
        spawnedPieces.Remove(piece);
    }

    public void SetCurrentPiece(GamePiece piece)
    {
        if (!piecePickedUp)
        {
            currentPiece = piece;
        }
    }
}
