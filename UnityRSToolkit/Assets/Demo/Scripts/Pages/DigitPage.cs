using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.UI.Paging;
using RSToolkit.UI.Controls;
using RSToolkit.Helpers;

public class DigitPage : UIPage
{
    public UIDigit UIDigitDemo;
    public UIDigits UIDigitsDemo; 
    // Start is called before the first frame update
    void Start()
    {
       InvokeRepeating("RandomizeDigits", 2f, 2f); 
    }

    void RandomizeDigits(){
        UIDigitsDemo.Digits = (uint)RandomHelpers.RandomInt(100);
        UIDigitDemo.Digit = (uint)RandomHelpers.RandomInt(10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
