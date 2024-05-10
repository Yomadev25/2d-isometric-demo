using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float _speed;

    [Header("Spell Cast")]
    [SerializeField]
    private GameObject _magicPrefab;

    [Header("References")]
    [SerializeField]
    private Rigidbody2D _rigidbody;
    [SerializeField]
    private InputHandler _input;
    [SerializeField]
    private Animator _anim;

    private Vector2 _lastDirection;
    private int _lastDirectionIndex;
    private string[] _staticDirections = { "Static N", "Static NW", "Static W", "Static SW", "Static S", "Static SE", "Static E", "Static NE" };
    private string[] _runDirections = { "Run N", "Run NW", "Run W", "Run SW", "Run S", "Run SE", "Run E", "Run NE" };

    private void Awake()
    {
        if (!_rigidbody)
        {
            Debug.LogError("Doesn't has Rigidbody2D");
        }

        if (!_input)
        {
            Debug.LogError("Doesn't has Input Handler");
        }
        else
        {
            _input.onFire.AddListener(Fire);
        }

        if (!_anim)
        {
            Debug.LogError("Doesn't has Animator");
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 currentPos = _rigidbody.position;
        Vector2 inputAxis = _input.move;
        Vector2 movement = inputAxis * _speed;
        Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;

        _rigidbody.MovePosition(newPos);
        SetAnimation(movement);
    }

    private void Fire()
    {
        Instantiate(_magicPrefab, transform.position, Quaternion.Euler(new Vector3(0f, 0f, GetFacingDirection())));       
    }

    private void SetAnimation(Vector2 dir)
    {
        string[] directionArray = null;

        if (dir.magnitude < .01f)
        {
            directionArray = _staticDirections;
        }
        else
        {
            directionArray = _runDirections;            
            _lastDirectionIndex = DirectionToIndex(dir, 8);
        }

        _anim.Play(directionArray[_lastDirectionIndex]);
    }

    private int DirectionToIndex(Vector2 dir, int sliceCount)
    {
        Vector2 normDir = dir.normalized;
        _lastDirection = normDir;

        float step = 360f / sliceCount;
        float halfstep = step / 2;
        float angle = Vector2.SignedAngle(Vector2.up, normDir);
        angle += halfstep;
        if (angle < 0)
        {
            angle += 360;
        }
        float stepCount = angle / step;

        return Mathf.FloorToInt(stepCount);
    }

    public int GetFacingDirection()
    {
        float angle = Vector2.SignedAngle(Vector2.up, _lastDirection);

        if (angle < 0)
        {
            angle += 360f;
        }

        int roundedAngle = Mathf.RoundToInt(angle / 45f) * 45;

        if (roundedAngle >= 360)
        {
            roundedAngle = 0;
        }

        return roundedAngle;
    }
}
