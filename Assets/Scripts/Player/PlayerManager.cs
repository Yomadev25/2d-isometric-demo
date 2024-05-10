using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float _maxHp;
    [SerializeField]
    private float _hp;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    public static Action<float, float> onUpdateHp;

    private void Start()
    {
        _hp = _maxHp;
        onUpdateHp?.Invoke(_hp, _maxHp);
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage;
        StartCoroutine(DamageEffectCoroutine());
        onUpdateHp?.Invoke(_hp, _maxHp);

        if (_hp <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageEffectCoroutine()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
