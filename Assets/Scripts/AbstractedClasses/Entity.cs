using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int TotalHealth;
    public int TotalMana;

    protected int _currentHealth;
    protected int _currentMana;
    protected bool _isBuffing;

    protected float _dir = 1;

    protected void Start() {
        _currentHealth = TotalHealth;
        _currentMana = TotalMana;
    }

    public virtual void Heal(int change) {
        _currentHealth += change;

        if (_currentHealth > TotalHealth) {
            _currentHealth = TotalHealth;
        }

        if (transform.tag == "Player") {
            GameUIManager.Instance.UpdateHealth((float) _currentHealth / (float) TotalHealth);
        }
    }

    public virtual void ManaChange(int change) {
        _currentMana += change;

        if (_currentMana > TotalMana) {
            _currentMana = TotalMana;
        }

        if (transform.tag == "Player") {
            GameUIManager.Instance.UpdateMana((float) _currentMana / (float) TotalMana);
        }
    }

    public bool GetBuffing() {
        return _isBuffing;
    }

    public void SetBuffing(bool state) {
        _isBuffing = state;
    }

    public float GetDirection() {
        return _dir;
    }
}
