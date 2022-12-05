using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int TotalHealth;

    protected int _currentHealth;

    protected void Start() {
        _currentHealth = TotalHealth;
    }

    public abstract void AttackAnticipation();
    public abstract void Attack();
    public abstract void AttackRecovery();
}
