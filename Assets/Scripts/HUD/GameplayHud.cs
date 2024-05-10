using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayHud : MonoBehaviour
{
    [SerializeField]
    private Image _hpBar;

    private void Awake()
    {
        PlayerManager.onUpdateHp += UpdateHpBar;
    }

    private void OnDestroy()
    {
        PlayerManager.onUpdateHp -= UpdateHpBar;
    }

    private void UpdateHpBar(float hp, float maxHp)
    {
        _hpBar.fillAmount = hp / maxHp;
    }
}
