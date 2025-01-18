using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewSubUnitRenderer : MonoBehaviour
{
    public Transform stunObjectParent;
    [SerializeField] public SpriteRenderer spriteRenderer;
    public Canvas atkSpeedCanvas;
    public Slider atkSpeedSlider;
    public TMPro.TMP_Text atkSpeedText;
}
