using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyCombat
{
    [SerializeField]
    private Transform _orientation;
    [SerializeField]
    private Transform _attackPoint;
    [SerializeField]
    private float _hitRadius;
    [SerializeField]
    private LayerMask _targetLayer;
    [SerializeField]
    private ParticleSystem _slashFx;

    public override void Attack(EnemyController context)
    {
        StartCoroutine(AttackCoroutine(context));
    }

    IEnumerator AttackCoroutine(EnemyController context)
    {
        float angle = context.GetFacingDirection();

        _orientation.rotation = Quaternion.Euler(0, 0, angle); //Rotate attack point to facing direction
        _slashFx.transform.eulerAngles = new Vector3(0, 0, angle + 180); //Rotate attack effect to facing direction
        _slashFx.Play();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_attackPoint.position, _hitRadius, _targetLayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(1);
            }
        }

        yield return new WaitForSeconds(1f);

        context.OnAttacked();
    }
}
