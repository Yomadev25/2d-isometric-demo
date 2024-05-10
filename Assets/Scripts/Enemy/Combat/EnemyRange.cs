using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRange : EnemyCombat
{
    [SerializeField]
    private GameObject _magicPrefab;
    Transform _playerTransform;

    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void Attack(EnemyController context)
    {
        StartCoroutine(AttackCoroutine(context));
    }

    IEnumerator AttackCoroutine(EnemyController context)
    {
        Vector2 dirWalk = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

        context.aiPath.isStopped = false;
        context.aiPath.destination = (Vector3)dirWalk + transform.position; //Random walk
        yield return new WaitForSeconds(2f);
        context.aiPath.isStopped = true;
        

        float duration = 0.1f;
        dirWalk = (_playerTransform.position - transform.position).normalized;
        while (true)
        {          
            context.aiPath.Move((Vector3)dirWalk * 0.5f * Time.deltaTime); //Rotate to target direction (in 8 direction)
            duration -= Time.deltaTime;

            if (duration < 0) break;
            yield return null;
        }

        Instantiate(_magicPrefab, transform.position, Quaternion.Euler(new Vector3(0f, 0f, context.GetFacingDirection())));
        yield return new WaitForSeconds(1f);
        context.OnAttacked();
    }
}
