

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/BuffAbility")]
public class BuffAbility : Ability {

    [Header("Buff Ability Information")]
    public Stats StatToEffect;
    public int IncreaseAmount = 10;

    private BuffActivator activator;
    public override void Initialize(GameObject obj)
    {
        activator = obj.GetComponent<BuffActivator>();

        // Base Activator set
        activator.SetAbilityCooldown(AbilityBaseCoolDown);
        activator.SetMaxAbilityDuration(MaxAbilityDuration);
        activator.SetAnticipationDuration(AnticipationDuration);
        activator.SetCastDuration(CastDuration);
        activator.SetDeathDuration(DeathDuration);

        // Buff Activator specific sets
        activator.SetIncreaseAmount(IncreaseAmount);
        activator.SetStatToEffect(StatToEffect);
    }

    public override void TriggerAbility()
    {
        activator.Activate();
    }

}
