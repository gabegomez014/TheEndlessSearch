using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffActivator : Activator
{
    private int _increaseAmount;
    private Stats _statToEffect;

    public void SetIncreaseAmount(int amount) {
        _increaseAmount = amount;
    }

    public void SetStatToEffect(Stats stat) {
        _statToEffect = stat;
    }

    public override void Activate()
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator AnticipationAction()
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator CastAction()
    {
        throw new System.NotImplementedException();
    }
}
