using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showSliderValue : MonoBehaviour
{
    InputField slider_text;
    Slider parent_slider;
    // Start is called before the first frame update
    void Start()
    {
        ControlPanel cp = GameObject.Find("Canvas").GetComponent<ControlPanel>();
        slider_text = GetComponent<InputField>();
        parent_slider = GetComponentInParent<Slider>();
        parent_slider.onValueChanged.AddListener((float v) => cp.setLEDcolor(parent_slider.name, v));
    }

    public void updateValue(float v)
    {
        slider_text = GetComponent<InputField>();
        slider_text.text = v.ToString();
    }
    public void updateSlider(string s)
    {
        if (int.TryParse(s, out int v))
        {
            GetComponentInParent<Slider>().value = v;
        }
    }
}
