using System;
using UnityEngine;

public class DirectionalBoardChecker
{
    private const int ROW = 0;
    private const int COLUMN = 1;

    private readonly GameObject[,] _gameObjects;

    private GameObject[] _horizontalWin;
    private GameObject[] _verticalWin;
    private GameObject[] _rightDiagnolWin;
    private GameObject[] _leftDiagnolWin;
    
    public DirectionalBoardChecker(GameObject[,] gOs)
    {
        _gameObjects = gOs;
    }

    private void InitializeWinArrays()
    {
        _horizontalWin = new GameObject[_gameObjects.GetLength(ROW)];
        _verticalWin = new GameObject[_gameObjects.GetLength(ROW)];
        _rightDiagnolWin = new GameObject[_gameObjects.GetLength(ROW)];
        _leftDiagnolWin = new GameObject[_gameObjects.GetLength(ROW)];
    }
    
    public Tuple<GameObject[], GameObject[], GameObject[], GameObject[]> CheckPositions(Tuple<int, int> coords)
    {
        InitializeWinArrays();

        int x = coords.Item1;
        int y = coords.Item2;

        bool isCenterTile = x == 1 && y == 1;
        bool isRightDiagnolCorner = x == 0 && y == 0 || x == 2 && y == 2;
        bool isLeftDiagnolCorner = x == 2 && y == 0 || x == 0 && y == 2;

        if (isCenterTile)
        {
            CheckDiagnolLeft();
            CheckDiagnolRight();
        }

        if (isRightDiagnolCorner)
        {
            CheckDiagnolRight();
        }

        if (isLeftDiagnolCorner)
        {
            CheckDiagnolLeft();
        }

        CheckVertical(x);
        CheckHorizontal(y);

        return new Tuple<GameObject[], GameObject[], GameObject[], GameObject[]>(_horizontalWin, _verticalWin,
            _rightDiagnolWin,
            _leftDiagnolWin);
    }

    private void CheckVertical(int position)
    {
        for (int i = 0; i < _gameObjects.GetLength(COLUMN); i++)
        {
            if (_gameObjects[position, i].TryGetComponent(out XTile xTile))
            {
                _verticalWin[i] = xTile.gameObject;
            }

            if (_gameObjects[position, i].TryGetComponent(out OTile oTile))
            {
                _verticalWin[i] = oTile.gameObject;
            }
        }
    }

    private void CheckHorizontal(int position)
    {
        for (int i = 0; i < _gameObjects.GetLength(COLUMN); i++)
        {
            if (_gameObjects[i, position].TryGetComponent(out XTile xTile))
            {
                _horizontalWin[i] = xTile.gameObject;
            }

            if (_gameObjects[i, position].TryGetComponent(out OTile oTile))
            {
                _horizontalWin[i] = oTile.gameObject;
            }
        }
    }
    private void CheckDiagnolRight()
    {
        for (int i = 0; i < _gameObjects.GetLength(ROW); i++)
        {
            if (_gameObjects[i, i].TryGetComponent(out XTile xtile))
            {
                _rightDiagnolWin[i] = xtile.gameObject;
            }

            if (_gameObjects[i, i].TryGetComponent(out OTile otile))
            {
                _rightDiagnolWin[i] = otile.gameObject;
            }
        }
    }

    private void CheckDiagnolLeft()
    {
        int leftSide = _gameObjects.GetLength(ROW) - 1;
        for (int i = 0; i < _gameObjects.GetLength(ROW); i++)
        {
            if (_gameObjects[i, leftSide].TryGetComponent(out XTile xtile))
            {
                _leftDiagnolWin[i] = xtile.gameObject;
            }

            if (_gameObjects[i, leftSide].TryGetComponent(out OTile otile))
            {
                _leftDiagnolWin[i] = otile.gameObject;
            }

            leftSide--;
        }
    }
}