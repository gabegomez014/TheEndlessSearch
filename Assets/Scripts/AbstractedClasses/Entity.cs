using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int TotalHealth;
    public int TotalMana;

    protected int _currentHealth;
    protected int _currentMana;

    protected void Start() {
        _currentHealth = TotalHealth;
        _currentMana = TotalMana;
    }
}
