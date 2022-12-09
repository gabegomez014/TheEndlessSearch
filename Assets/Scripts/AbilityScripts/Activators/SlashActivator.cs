using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashActivator : Activator
{
    private float _slashDistance;

    public void SetSlashDistance(float distance) {
        _slashDistance = distance;
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
