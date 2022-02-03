using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TMPro;
using Unity.VisualScripting;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Application = UnityEngine.Application;

public class ChangeOpacity : MonoBehaviour
{

    public GameObject currentGameObject;
    public float alpha = 0.5f;//half transparency
    //Get current material
    private Material currentMat;

    // Start is called before the first frame update
    void Start()
    {

    }

    void ChangeAlpha(Material mat, float alphaVal)
    {
        Color oldColor = mat.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaVal);
        mat.SetColor("_Color", newColor);
    }

    public void ChangeOpacityLattice(Slider slider)
    {
        foreach (Transform child in currentGameObject.transform)
        {
            currentGameObject = gameObject;
            currentMat = child.GetComponent<Renderer>().material;
            child.GetComponent<Renderer>().material.color = new Color(.5f, .5f, .5f, slider.value) ;
            ChangeAlpha(currentMat, slider.value);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
