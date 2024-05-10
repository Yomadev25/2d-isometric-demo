using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private string _targetTag;
    [SerializeField]
    private Rigidbody2D _rigidBody;
    [SerializeField]
    private GameObject _hitFx;

    private void Start()
    {
        _rigidBody.AddForce(transform.up * _speed, ForceMode2D.Impulse);
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_targetTag))
        {
            if (collision.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(1);
                Instantiate(_hitFx, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }       
    }
}
