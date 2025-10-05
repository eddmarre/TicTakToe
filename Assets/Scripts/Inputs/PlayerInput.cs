using System;

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