using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInputHud : MonoBehaviour
{
    [Header("Mobile Input")]
    [SerializeField]
    private Joystick _moveJoystick;
    [SerializeField]
    private Button _fireButton;

    public static event Action<Vector2> onMove;
    public static event Action onFire;

    private void Awake()
    {
        if (_moveJoystick)
        {
            _moveJoystick.onDrag += OnMove;
        }
        else
        {
            Debug.LogErrorFormat("{0} not found.", nameof(Joystick));
        }

        if (_fireButton)
        {
            _fireButton.onClick.AddListener(OnFire);
        }
        else
        {
            Debug.LogErrorFormat("{0} not found.", nameof(Button));
        }
    }

    private void OnDestroy()
    {
        _moveJoystick.onDrag -= OnMove;
    }

    private void OnMove()
    {
        onMove?.Invoke(_moveJoystick.Direction);
    }

    private void OnFire()
    {
        onFire?.Invoke();
    }
}
