using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private int _boardOffset = -1;

    private List<GameObject> _createdGameObjects;
    private GameObject[,] _boardTileGameObjects;
    private DirectionalBoardChecker _boardChecker;

    private const int BOARDSIZE = 3;

    private void Start()
    {
        DrawCross();
        DrawBoard();
    }

    private void DrawBoard()
    {
        _boardTileGameObjects = new GameObject[BOARDSIZE, BOARDSIZE];
        _createdGameObjects = new List<GameObject>();

        for (int x = 0; x < BOARDSIZE; x++)
        {
            for (int y = 0; y < BOARDSIZE; y++)
            {
                var spawnedQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                spawnedQuad.transform.position = new Vector3(x + _boardOffset, y + _boardOffset);
                spawnedQuad.transform.SetParent(transform);
                spawnedQuad.name = $"{x}, {y}";
                var tile = spawnedQuad.AddComponent<NeutralTile>();
                tile.SetCoordinates(x, y);
                if (spawnedQuad.TryGetComponent(out MeshRenderer mesh))
                {
                    mesh.enabled = false;
                }
                _createdGameObjects.Add(spawnedQuad);
                _boardTileGameObjects[x, y] = spawnedQuad;
            }
        }

        _boardChecker = new DirectionalBoardChecker(_boardTileGameObjects);
    }

    private void DrawCross()
    {
        var halfQuadHeight = .5f;

        var crossParent = new GameObject
        {
            name = "BoardOutline"
        };

        Transform crossParentTransform = crossParent.transform;

        var topHorizontalLine = GameObject.CreatePrimitive(PrimitiveType.Quad);
        topHorizontalLine.transform.SetParent(crossParentTransform);
        topHorizontalLine.transform.position = new Vector3(0f, halfQuadHeight, 0f);

        var bottomHorizontalLine = GameObject.CreatePrimitive(PrimitiveType.Quad);
        bottomHorizontalLine.transform.SetParent(crossParentTransform);
        bottomHorizontalLine.transform.position = new Vector3(0f, -halfQuadHeight, 0f);

        var leftVerticalLine = GameObject.CreatePrimitive(PrimitiveType.Quad);
        leftVerticalLine.transform.SetParent(crossParentTransform);
        leftVerticalLine.transform.position = new Vector3(-halfQuadHeight, 0f, 0f);
        leftVerticalLine.transform.rotation = Quaternion.Euler(0f, 0f, 90);

        var rightVerticalLine = GameObject.CreatePrimitive(PrimitiveType.Quad);
        rightVerticalLine.transform.SetParent(crossParentTransform);
        rightVerticalLine.transform.position = new Vector3(halfQuadHeight, 0f, 0f);
        rightVerticalLine.transform.rotation = Quaternion.Euler(0f, 0f, 90);

        int childCount = 1;
        foreach (Transform child in crossParent.transform)
        {
            child.name = $"Border {childCount}";
            child.localScale = new Vector3(3f, .1f, 1f);
            child.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Black");
            MeshCollider meshCollider = child.GetComponent<MeshCollider>();
            Destroy(meshCollider);
            childCount++;
        }
    }

    public void ReDrawBoard()
    {
        foreach (var go in _createdGameObjects)
        {
            Destroy(go);
        }

        _createdGameObjects.Clear();

        DrawBoard();
    }

    public Tuple<GameObject[], GameObject[], GameObject[], GameObject[]> CheckBoard(int x, int y)
    {
        Tuple<int, int> coords = new Tuple<int, int>(x, y);
        return _boardChecker.CheckPositions(coords);
    }
}