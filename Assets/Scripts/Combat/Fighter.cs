using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
  public class Fighter : MonoBehaviour, IAction
  {
    [SerializeField] float weaponRange = 2f;
    [SerializeField] float timeBetweenAttacks = 1f;
    [SerializeField] float weaponDamage = 5f;

    Mover mover;
    Animator animator;
    Health target;
    float timeSinceLastAttack = Mathf.Infinity;

    private void Start()
    {
      mover = GetComponent<Mover>();
      animator = GetComponent<Animator>();
    }

    private void Update()
    {
      timeSinceLastAttack += Time.deltaTime;
      if (target == null) { return; }
      if (target.IsDead()) { return; }
      if (!GetIsInRange())
      {
        mover.MoveTo(target.transform.position);
      }
      else
      {
        mover.Cancel();
        AttackBehavior();
      }
    }

    private void AttackBehavior()
    {
      transform.LookAt(target.transform);
      if (timeSinceLastAttack > timeBetweenAttacks)
      {
        // Trigger Hit() event
        TriggerAttack();
        timeSinceLastAttack = 0;
      }
    }

    private void TriggerAttack()
    {
      animator.ResetTrigger("stopAttack");
      animator.SetTrigger("attack");
    }

    // Animation event
    void Hit()
    {
      if (target == null) { return; }
      target.TakeDamage(weaponDamage);
    }

    private bool GetIsInRange()
    {
      return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
    }

    public bool CanAttack(GameObject combatTarget)
    {
      if (combatTarget == null) { return false; }

      Health combatTargetHealth = combatTarget.GetComponent<Health>();
      return combatTargetHealth != null && !combatTargetHealth.IsDead();
    }

    public void Attack(GameObject combatTarget)
    {
      GetComponent<ActionScheduler>().StartAction(this);
      target = combatTarget.GetComponent<Health>();
    }

    public void Cancel()
    {
      StopAttack();
      target = null;
    }

    private void StopAttack()
    {
      animator.ResetTrigger("attack");
      animator.SetTrigger("stopAttack");
    }
  }
}
