using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFunctions : MonoBehaviour
{
    FunctionController func;

    private void Start()
    {
        func = GetComponent<FunctionController>();
    }

    public void Attack(Player user, Player target, int damage)
    {
        Debug.Log("The Attack has dealt damage");
        func.loseHP(target, damage);

        target.isAfterTargetted = null;
    }

    public void Dodge(Player user)
    {
        if (user.isAfterTargetted.Name == "Attack")
        {
            user.isAfterTargetted = null;
            Debug.Log(user + " dodged");

            func.FinishProcess();
        }
    }

    public void Heal(Player user, Player target)
    {
        func.HealHP(target);
        Debug.Log("Player " + target + "has healed 1 HP");

        target.isAfterTargetted = null;

        if (func.processCase != 1)
        {
            func.FinishProcess();
        }
    }
}
