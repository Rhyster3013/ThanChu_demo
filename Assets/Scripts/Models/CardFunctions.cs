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
        func.loseHP(target, damage);
        Debug.Log("The Attack has dealt damage");

        target.isAfterTargetted = null;

        func.DiscardCard(user);
    }

    public void Dodge(Player user)
    {
        if (user.isAfterTargetted.Name == "Attack")
        {
            user.isAfterTargetted = null;
            Debug.Log("Dodged");

            func.DiscardCard(user);
        }
    }

    public void Heal(Player user, Player target)
    {
        target.HP++;

        target.isAfterTargetted = null;
        func.DiscardCard(user);
    }
}
