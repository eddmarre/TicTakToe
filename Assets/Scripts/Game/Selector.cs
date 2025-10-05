using UnityEngine;
using UnityEngine.InputSystem;

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