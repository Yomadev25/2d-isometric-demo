using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Input Values")]
    public Vector2 move;
    public UnityEvent onFire;

    private void Awake()
    {
        SubscribeMobileInputHud();
    }

    private void OnDestroy()
    {
        UnsubscribeMobileInputHud();
    }

    private void MoveInput(Vector2 moveDir)
    {
        move = moveDir;
    }

    private void FireInput()
    {
        onFire?.Invoke();
    }

    #region PLAYER INPUT ACTION BEHAVIOURS
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            FireInput();
        }
    }
    #endregion

    #region MOBILE INPUT ACTION
    private void SubscribeMobileInputHud()
    {
        MobileInputHud.onMove += MoveInput;
        MobileInputHud.onFire += FireInput;
    }

    private void UnsubscribeMobileInputHud()
    {
        MobileInputHud.onMove -= MoveInput;
        MobileInputHud.onFire -= FireInput;
    }
    #endregion
}
