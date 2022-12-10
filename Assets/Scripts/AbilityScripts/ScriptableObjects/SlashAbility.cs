using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/SlashAbility")]
public class SlashAbility : Ability
{
    [Header("Slash Ability mechanics")]
    [Tooltip("How far this slash affects")]
    public float SlashDistance;
    public int Damage;

    private SlashActivator _activator;
    public override void Initialize(GameObject obj) {
        _activator = obj.GetComponent<SlashActivator>();

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

        // Slash activator specific sets
        _activator.SetSlashDistance(SlashDistance);
        _activator.SetDamage(Damage);
    }
    public override void TriggerAbility() {
        _activator.Activate();
    }
}
