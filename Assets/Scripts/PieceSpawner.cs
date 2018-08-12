using System.Collections;
using UnityEngine;

public class PieceSpawner : MonoBehaviour {
    public PieceSet set;
    public Transform[] spawnLocations;
    private GamePiece[] allPieces;
    void Awake()
    {
        allPieces = set.pieces;
        SortInPlaceRandom(spawnOrder);
    }

    public void StartSpawning()
    {
        spawnRoutine = StartCoroutine(SpawnPieces());
    }

    private Coroutine spawnRoutine;
    private IEnumerator SpawnPieces()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 5.5f));
            GamePiece piece = allPieces[Random.Range(0, allPieces.Length)];
            GamePiece newPiece = Instantiate(piece);
            Vector3 spawnPos = RandomSpawnPosition();
            newPiece.GetComponent<GamePiece>().MovePieceIn(spawnPos * 2, spawnPos);
        }
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    public bool IsSpawning()
    {
        return spawnRoutine != null;
    }

    private int currentIndex = 0;
    private int[] spawnOrder = new int[] { 0, 1, 2, 3, 4, 5 };
    private Vector3 RandomSpawnPosition()
    {
        int loc = spawnOrder[currentIndex];
        currentIndex++;
        if(currentIndex >= spawnOrder.Length)
        {
            currentIndex = 0;
            SortInPlaceRandom(spawnOrder);
        }

        return spawnLocations[loc].position;
    }

    private void SortInPlaceRandom(int[] theArray)
    {
        for (int t = 0; t < theArray.Length; t++)
        {
            int tmp = theArray[t];
            int r = Random.Range(t, theArray.Length);
            theArray[t] = theArray[r];
            theArray[r] = tmp;
        }
    }
}
