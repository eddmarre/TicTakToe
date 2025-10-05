using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Fields

    private static GameManager _instance;

    private GameObject _x;
    private GameObject _o;

    public PlayerInput PlayerGameInput;
    private Board _board;

    private LineRenderer _lineRenderer;

    private Tuple<GameObject[], GameObject[], GameObject[], GameObject[]> _currentBoardConditions;

    private WaitForSeconds _waitForSeconds;

    private bool _isPlayer1Turn = true;
    private int _turnCount;
    
    private readonly float _secondsToWait = 2f;

    public bool IsGameActive;
    public event Action<string> OnUpdateGameText;

    #endregion

    public static GameManager Instance => _instance;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            PlayerGameInput = new PlayerInput();
            IsGameActive = true;
        }
    }

    private void Start()
    {
        PlayerGameInput.OnPlayerRestart += RestartGame;

        _x = Resources.Load<GameObject>("X");
        _o = Resources.Load<GameObject>("O");
        _board = FindFirstObjectByType<Board>();

        _waitForSeconds = new WaitForSeconds(_secondsToWait);
    }

    #region Private Methods

    private void RestartGame()
    {
        OnUpdateGameText?.Invoke("");
        _board.ReDrawBoard();
        _isPlayer1Turn = true;
        IsGameActive = true;
        _turnCount = 0;
        if (_lineRenderer != null)
            Destroy(_lineRenderer);
    }


    public void EndTurn(NeutralTile neutralTile)
    {
        var objectToSpawn =
            _isPlayer1Turn ? Instantiate(_x, neutralTile.transform) : Instantiate(_o, neutralTile.transform);

        var coords = neutralTile.GetCoordinates();

        if (_isPlayer1Turn)
        {
            XTile xTile = neutralTile.gameObject.AddComponent<XTile>();
            xTile.SetCoordinates(coords.Item1, coords.Item2);
        }
        else
        {
            OTile oTile = neutralTile.gameObject.AddComponent<OTile>();
            oTile.SetCoordinates(coords.Item1, coords.Item2);
        }

        _currentBoardConditions = _board.CheckBoard(coords.Item1, coords.Item2);

        Destroy(neutralTile);

        var horizontalWin = _currentBoardConditions.Item1;
        var verticalWin = _currentBoardConditions.Item2;
        var rightDiagnolWin = _currentBoardConditions.Item3;
        var leftDiagnolWin = _currentBoardConditions.Item4;

        if (WinCheck(horizontalWin) || WinCheck(verticalWin) || WinCheck(rightDiagnolWin) || WinCheck(leftDiagnolWin))
        {
            char player = _isPlayer1Turn ? 'X' : 'O';
            OnUpdateGameText?.Invoke($"{player} won game!");
            IsGameActive = false;
            StartCoroutine(WaitBeforeGameResets());
        }

        if (_turnCount >= 8)
        {
            OnUpdateGameText?.Invoke("Draw No Winner!");
            StartCoroutine(WaitBeforeGameResets());
            return;
        }

        _isPlayer1Turn = !_isPlayer1Turn;
        _turnCount++;
    }

    private bool WinCheck(GameObject[] gOs)
    {
        bool xWin = true;
        bool oWin = true;
        foreach (var tile in gOs)
        {
            if (tile == null) return false;

            if (!tile.TryGetComponent(out XTile xTile))
            {
                xWin = false;
            }

            if (!tile.TryGetComponent(out OTile oTile))
            {
                oWin = false;
            }
        }

        if (xWin || oWin)
            DrawWinningLine(gOs);

        return xWin || oWin;
    }

    private void DrawWinningLine(GameObject[] gOs)
    {
        _lineRenderer = _board.AddComponent<LineRenderer>();

        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        _lineRenderer.startColor = Color.black;
        _lineRenderer.endColor = Color.black;

        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.2f;

        _lineRenderer.positionCount = 2;

        _lineRenderer.SetPosition(0, gOs[0].transform.position);
        _lineRenderer.SetPosition(1, gOs[2].transform.position);
    }

    private IEnumerator WaitBeforeGameResets()
    {
        yield return _waitForSeconds;
        RestartGame();
    }

    #endregion
}