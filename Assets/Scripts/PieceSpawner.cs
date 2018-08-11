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
            newPiece.transform.position = RandomSpawnPosition();
        }
    }

    private Vector3 RandomSpawnPosition()
    {
        return spawnLocations[Random.Range(0, spawnLocations.Length)].position;
    }
}
