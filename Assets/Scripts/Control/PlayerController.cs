using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;

namespace RPG.Control
{
  public class PlayerController : MonoBehaviour
  {
    Fighter fighter;
    Health health;
    private void Start()
    {
      fighter = GetComponent<Fighter>();
      health = GetComponent<Health>();
    }
    private void Update()
    {
      if (health.IsDead()) { return; }
      if (InteractWithCombat()) { return; }
      if (InteractWithMovement()) { return; }
    }

    private bool InteractWithCombat()
    {
      RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

      foreach (RaycastHit hit in hits)
      {
        CombatTarget target = hit.transform.GetComponent<CombatTarget>();

        if (target == null) { continue; }

        if (!fighter.CanAttack(target.gameObject)) { continue; }

        if (Input.GetMouseButton(0))
        {
          fighter.Attack(target.gameObject);
        }
        return true;
      }
      return false;
    }



    private bool InteractWithMovement()
    {
      RaycastHit hit;
      bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
      if (hasHit)
      {
        if (Input.GetMouseButton(0))
        {
          if (IsStationary())
          {
            transform.LookAt(hit.point);
          }
          else
          {
            GetComponent<Mover>().StartMoveAction(hit.point);
          }
        }
        return true;
      }
      return false;
    }

    private static Ray GetMouseRay()
    {
      return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    private bool IsStationary()
    {
      return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
  }
}
