using UnityEngine;

namespace RPG.Core
{
  public class Health : MonoBehaviour
  {
    [SerializeField] float healthPoints = 100f;

    private bool isDead = false;

    private void Update()
    {
      if (healthPoints == 0 && !IsDead())
      {
        Die();
        isDead = true;
      }
    }

    private void Die()
    {
      GetComponent<Animator>().SetTrigger("die");
      GetComponent<ActionScheduler>().CancelCurrentAction();
    }

    public void TakeDamage(float damage)
    {
      healthPoints = Mathf.Max(healthPoints - damage, 0);
      print(healthPoints);
    }

    public bool IsDead()
    {
      return isDead;
    }
  }
}