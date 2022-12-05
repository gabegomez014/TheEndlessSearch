using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamageable
{
    public void TakeDamage(int damage);
    public void Heal(int heal);
    public void Die();
}
