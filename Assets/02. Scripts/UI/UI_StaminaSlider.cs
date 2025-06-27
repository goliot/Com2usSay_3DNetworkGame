using UnityEngine;
using UnityEngine.UI;

public class UI_StaminaSlider : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.value = 1;
    }

    public void Init(PlayerStatHolder holder)
    {
        holder.PlayerStaminaEvent += UpdateSlider;
    }

    private void UpdateSlider(float cur, float max)
    {
        _slider.value = cur / max;
    }
}
