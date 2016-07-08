using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{

    public float MaxStamina = 100;
    [Range(0, 1)]
    public float StaminaRegenRate = 0.5f;
    public float RegenStartTime = 2f;
    public Slider StaminaSlider;

    private float _currentStamina;
    private float _lastUseTime;

    void Awake()
    {
        _currentStamina = MaxStamina;
        StaminaSlider.maxValue = MaxStamina;
    }

    public bool ConsumeStamina(int amount)
    {
        if (_currentStamina >= amount)
        {
            _lastUseTime = Time.time;
            _currentStamina -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    void FixedUpdate()
    {
        if (Time.time > _lastUseTime + RegenStartTime)
        {
            _currentStamina = Mathf.Lerp(_currentStamina, MaxStamina, StaminaRegenRate);
        }

        StaminaSlider.value = _currentStamina;
    }
}
