![](https://i.imgur.com/vuIbypq.png)

[Click Here To Play](https://kpone.itch.io/tictaktoe)

Want to See inside the project? Take a quick look at the scripts I wrote that allow this game to work.

- [ ] Board.cs    


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
- [ ] DirectionalBoardChecker.cs 


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
- [ ] GameManager.cs 


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
                 return;
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

- [ ] Selector.cs


    public class Selector : MonoBehaviour
    {
        private Collider _hitCollider;
        private GameManager _gameManager;
    
        private void Start()
        {
            _gameManager = GameManager.Instance;
            _gameManager.PlayerGameInput.OnPlayerClick += CastRayFromMousePosition;
        }
    
        private void CastRayFromMousePosition()
        {
            if (!_gameManager.IsGameActive) return;
            if (Camera.main == null) return;
    
            Ray cameraRay = Camera.main.ScreenPointToRay(Mouse.current.position.value);
            Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue);
    
            if (hit.collider == null) return;
            if (hit.collider.TryGetComponent(out NeutralTile tile))
            {
                tile.OnSelect();
            }
        }
    }
- [ ] ITile.cs


    public interface ITile
    {
        public void OnSelect();
        public void SetCoordinates(int x, int y);
        public Tuple<int, int> GetCoordinates();
    }

- [ ] NeutralTile.cs


    public class NeutralTile : MonoBehaviour, ITile
    {
        private Tuple<int, int> _coords;
    
        public void OnSelect() => GameManager.Instance.EndTurn(this);
    
        public void SetCoordinates(int x, int y) => _coords = new Tuple<int, int>(x, y);
    
        public Tuple<int, int> GetCoordinates() => _coords;
    }
- [ ] XTile.cs and OTile.cs


    public class OTile : MonoBehaviour, ITile
    {
        private Tuple<int, int> _coords;
    
        public void OnSelect()
        {
        }
    
        public void SetCoordinates(int x, int y) => _coords = new Tuple<int, int>(x, y);
    
        public Tuple<int, int> GetCoordinates() => _coords;
    }
- [ ] MessageManager.cs


    public class MessageManager : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshPro;
    
        private void Awake() => _textMeshPro = GetComponent<TextMeshProUGUI>();
    
        private void Start() => GameManager.Instance.OnUpdateGameText += OnUpdateMessage;
    
        private void OnUpdateMessage(string newMessage) => _textMeshPro.text = newMessage;
    }
- [ ] PlayerInput.cs


    public class PlayerInput
    {
        private InputSystem_Actions _inputSystemActions;

        public event Action OnPlayerClick;
        public event Action OnPlayerRestart;

        public PlayerInput()
        {
            _inputSystemActions = new InputSystem_Actions();
            _inputSystemActions.Player.Enable();
            _inputSystemActions.Player.Click.performed += (context) => OnPlayerClick?.Invoke();
            _inputSystemActions.Player.Restart.performed += (context) => OnPlayerRestart?.Invoke();
        }
    }