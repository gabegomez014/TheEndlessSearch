

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/BuffAbility")]
public class BuffAbility : Ability {

    [Header("Buff Ability Information")]
    public Stats StatToEffect;
    public int IncreaseAmount = 10;

    private BuffActivator _activator;
    public override void Initialize(GameObject obj)
    {
        _activator = obj.GetComponent<BuffActivator>();

        // Base activator set
        _activator.SetAbilityCooldown(AbilityBaseCoolDown);
        _activator.SetMaxAbilityDuration(MaxAbilityDuration);
        _activator.SetKeyToActivate(KeyToActivate);
        _activator.SetAnticipationDuration(AnticipationDuration);
        _activator.SetCastDuration(CastDuration);
        _activator.SetDeathDuration(DeathDuration);
        _activator.SetAnticipationParameterName(AnticipationAnimationParameterName);
        _activator.SetCastParameterName(CastAnimationParameterName);
        _activator.SetAnticipationFeedbackLabel(AnticipationFeedbackLabel);
        _activator.SetCastFeedbackLabel(CastFeedbackLabel);
        _activator.SetFeedbackPlayer(FeedbacksPlayerName);
        _activator.SetManaCost(ManaCost);

        // Buff activator specific sets
        _activator.SetIncreaseAmount(IncreaseAmount);
        _activator.SetStatToEffect(StatToEffect);
    }

    public override void TriggerAbility()
    {
        _activator.Activate();
    }

}
