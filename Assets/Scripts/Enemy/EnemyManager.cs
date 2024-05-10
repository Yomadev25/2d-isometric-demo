using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float _hp;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [Header("Effects")]
    [SerializeField]
    private GameObject _spawnFx;
    [SerializeField]
    private GameObject _dieFx;

    private Color _defaultColor;

    public static Action onDie;

    private void Start()
    {
        _defaultColor = _spriteRenderer.color;
        Instantiate(_spawnFx, transform.position, Quaternion.identity);
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage;
        StartCoroutine(DamageEffectCoroutine());

        if (_hp <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageEffectCoroutine()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _spriteRenderer.color = _defaultColor;
    }

    private void Die()
    {
        Instantiate(_dieFx, transform.position, Quaternion.identity);
        onDie?.Invoke();
        Destroy(gameObject);
    }
}
