using UnityEngine;
using UnityEngine.UI;

public class FloatingTimerBar_Script : MonoBehaviour
{
    public Slider timerSlider;



    private void Update()
    {
        
    }

    public void UpdateTimer(float _currentValue, float _maxValue)
    {
        timerSlider.value = _currentValue / _maxValue;
    }
}
