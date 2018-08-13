using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PieceMouseManager : MonoBehaviour {

    private const float START_SLEEP_TIME = 30;
    [NonSerialized]
    public float currentSleepTime = START_SLEEP_TIME;

    public EndScreenManager endScreen;
    public PieceSpawner spawner;

    public AudioSource mainGameAudio;
    public AudioSource titleScreenAudio;

    private List<Ossan> coldPeople = new List<Ossan>();

    private DragablePiece currentPiece;
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

    private const int COLD_PERSON_LIMIT = 10;
    public void RegisterColdPerson(Ossan ossan)
    {
        if (!coldPeople.Contains(ossan))
        {
            coldPeople.Add(ossan);
        }
        if(coldPeople.Count >= COLD_PERSON_LIMIT)
        {
            EndGame();
            coldPeople.Clear();
        }
    }

    private void EndGame()
    {
        LoadingScreen.instance.Show(_EndGame(), 0.666f);
    }

    private IEnumerator _EndGame()
    {
        spawner.StopSpawning();
        DragablePiece[] remainingDraggablePieces = FindObjectsOfType(typeof(DragablePiece)) as DragablePiece[];
        for (int i = 0; i < remainingDraggablePieces.Length; i++)
        {
            Destroy(remainingDraggablePieces[i].gameObject);
        }
        endScreen.RecordScore(HudManager.instance.Score);
        StartCoroutine(TitleScreenManager.FadeAudioSourceVolume(mainGameAudio, 0f, 2f));
        yield return new WaitForSeconds(1f);
        StartCoroutine(TitleScreenManager.FadeAudioSourceVolume(titleScreenAudio, 0.8f, 2f));
        HudManager.instance.Score = 0;
        currentSleepTime = START_SLEEP_TIME;
        endScreen.container.gameObject.SetActive(true);
        yield return null;
    }

    public void UnregisterColdPerson(Ossan ossan)
    {
        coldPeople.Remove(ossan);
    }

    public void SetCurrentPiece(DragablePiece piece)
    {
        if (!piecePickedUp)
        {
            currentPiece = piece;
        }
    }
}
