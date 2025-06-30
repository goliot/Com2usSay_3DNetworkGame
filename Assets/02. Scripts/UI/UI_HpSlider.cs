using UnityEngine;
using UnityEngine.UI;

public class UI_HpSlider : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.value = 1;
    }

    public void Init(PlayerStatHolder holder)
    {
        holder.PlayerHpEvent += UpdateSlider;
        _slider.value = 1;
    }

    private void UpdateSlider(float cur, float max)
    {
        _slider.value = cur / max;
    }
}