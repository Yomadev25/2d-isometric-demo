using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum State
    {
        NONE,
        IDLE,
        CHASE,
        ATTACK
    }

    [SerializeField]
    private State _currentState;

    [Header("Field Of View")]
    [SerializeField]
    private float _viewRadius;
    [SerializeField]
    private float _chaseRadius;
    [SerializeField]
    private float _combatRadius;
    [SerializeField]
    private LayerMask _targetLayer;

    [Header("Movement")]
    [SerializeField]
    private float _speed;

    [Header("References")]
    [SerializeField]
    private AIPath _aiPath;
    [SerializeField]
    private AIDestinationSetter _aiDestination;
    [SerializeField]
    private Animator _anim;

    private Vector2 _lastDirection;
    private int _lastDirectionIndex;
    private string[] _staticDirections = { "Static N", "Static NW", "Static W", "Static SW", "Static S", "Static SE", "Static E", "Static NE" };
    private string[] _runDirections = { "Run N", "Run NW", "Run W", "Run SW", "Run S", "Run SE", "Run E", "Run NE" };

    //PUBLIC VARIABLES
    public AIPath aiPath => _aiPath;
    public Vector2 lastDirection => _lastDirection;

    private void Start()
    {
        ChangeState(State.IDLE);
    }

    private void Update()
    {
        UpdateState();

        _lastDirection = _aiPath.velocity.normalized;
        SetAnimation(_lastDirection);
    }

    #region STATE MACHINE HANDLER
    private void ChangeState(State state)
    {
        if (state == _currentState) return;

        ExitState();
        _currentState = state;
        EnterState();
    }

    private void EnterState()
    {
        switch (_currentState)
        {
            case State.IDLE:
                EnterIdleState();
                break;
            case State.CHASE:
                EnterChaseState();
                break;
            case State.ATTACK:
                EnterAttackState();
                break;
            default:
                break;
        }
    }

    private void UpdateState()
    {
        switch (_currentState)
        {
            case State.IDLE:
                UpdateIdleState();
                break;
            case State.CHASE:
                UpdateChaseState();
                break;
            case State.ATTACK:
                UpdateAttackState();
                break;
            default:
                break;
        }
    }

    private void ExitState()
    {
        switch (_currentState)
        {
            case State.IDLE:
                ExitIdleState();
                break;
            case State.CHASE:
                ExitChaseState();
                break;
            case State.ATTACK:
                ExitAttackState();
                break;
            default:
                break;
        }
    }
    #endregion

    #region IDLE STATE
    private void EnterIdleState()
    {
        if (_viewRadius == 0 && _chaseRadius == 0)
        {
            StartCoroutine(PatrolCoroutine());
        }
    }

    private void UpdateIdleState()
    {
        if (GetVisibleTarget() != null)
        {
            ChangeState(State.CHASE);
        }
    }

    private void ExitIdleState()
    {
        _aiPath.isStopped = true;
    }

    private IEnumerator PatrolCoroutine()
    {
        Vector2 dirWalk = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

        _aiPath.isStopped = false;
        _aiPath.destination = (Vector3)dirWalk + transform.position;

        yield return new WaitForSeconds(2f);

        ChangeState(State.CHASE);
    }
    #endregion

    #region CHASE STATE
    private void EnterChaseState()
    {
        Transform target = GetChasedTarget();
        _aiDestination.target = target;
        _aiPath.isStopped = false;
    }

    private void UpdateChaseState()
    {
        if (GetChasedTarget() == null)
        {
            ChangeState(State.IDLE);
        }

        if (GetCombatTarget() != null)
        {
            ChangeState(State.ATTACK);
        }
    }

    private void ExitChaseState()
    {
        _aiDestination.target = null;
        _aiPath.isStopped = true;
    }
    #endregion

    #region ATTACK STATE
    private void EnterAttackState()
    {
        if (TryGetComponent(out EnemyCombat combat))
        {
            combat.Attack(this);
        }
        else
        {
            ChangeState(State.CHASE);
        }
    }

    private void UpdateAttackState()
    {

    }

    private void ExitAttackState()
    {

    }

    public void OnAttacked()
    {
        ChangeState(State.CHASE);
    }
    #endregion

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

    #region ENEMY VISUALIZATION
    public Transform GetVisibleTarget()
    {
        Collider2D[] targetInViewRadius = Physics2D.OverlapCircleAll(transform.position, _viewRadius, _targetLayer);

        foreach (Collider2D collider in targetInViewRadius)
        {
            Transform target = collider.transform;
            return target;
        }

        return null;
    }

    public Transform GetChasedTarget()
    {
        Collider2D[] targetInChaseRadius = Physics2D.OverlapCircleAll(transform.position, _chaseRadius, _targetLayer);

        foreach (Collider2D collider in targetInChaseRadius)
        {
            return collider.transform;
        }

        return null;
    }

    public Transform GetCombatTarget()
    {
        Collider2D[] targetInCombatRadius = Physics2D.OverlapCircleAll(transform.position, _combatRadius, _targetLayer);

        foreach (Collider2D collider in targetInCombatRadius)
        {
            return collider.transform;
        }

        return null;
    }
    #endregion

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _combatRadius);
    }
}
