using System.Collections;
using UnityEngine;

public class PieceSpawner : MonoBehaviour {
    public PieceSet set;
    public Transform[] spawnLocations;
    private GamePiece[] allPieces;
    void Awake()
    {
        allPieces = set.pieces;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnPieces());
    }

    private IEnumerator SpawnPieces()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 5f));
            GamePiece piece = allPieces[Random.Range(0, allPieces.Length)];
            GamePiece newPiece = Instantiate(piece);
            Vector3 spawnPos = RandomSpawnPosition();
            StartCoroutine(MovePieceIn(newPiece.transform, spawnPos * 2, spawnPos));
        }
    }

    private Vector3 RandomSpawnPosition()
    {
        return spawnLocations[Random.Range(0, spawnLocations.Length)].position;
    }

    private const float MOVE_TIME = 2f;
    private IEnumerator MovePieceIn(Transform t, Vector3 startPos, Vector3 endPos)
    {
        t.position = startPos;
        float elapsedTime = 0;
        float progress = 0;
        while(progress <= 1)
        {
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / MOVE_TIME;
            float easedProgress = Easing.easeOutSine(0, 1, progress);
            t.position = Vector3.Lerp(startPos, endPos, easedProgress);
            yield return null;
        }
        t.position = endPos;
    }
}
