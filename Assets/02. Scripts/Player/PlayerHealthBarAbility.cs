using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarAbility : PlayerAbility
{
    [SerializeField] private Slider _slider;

    private void Start()
    {
        Refresh(_player.CurrentHealth, _player.GetStat(EStatType.MaxHealth));

        _player.PlayerHpEvent += Refresh;
    }

    public void Refresh(float cur, float max)
    {
        _slider.value = cur / max;
    }
}