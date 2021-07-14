using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyComponent : MonoBehaviour
{
    public HandBehaviour Hand;
    private Color a = new Color(1,1,1,1),b = new Color(1,1,1,0);
    private Image _renderer;
    void OnEnable() => HandBehaviour.Notify += SetColor;
    void OnDisable() => HandBehaviour.Notify -= SetColor;
    void Awake() => _renderer = GetComponent<Image>();
    void SetColor() => _renderer.color = Hand.MoveHasTaken ? b : a;
}
