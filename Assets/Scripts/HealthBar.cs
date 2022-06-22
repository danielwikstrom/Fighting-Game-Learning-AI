using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    Character _char;

    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }
    // Update is called once per frame
    void Update()
    {
        _slider.value = (float)_char.GetHealth() / 100.0f;
    }
}
