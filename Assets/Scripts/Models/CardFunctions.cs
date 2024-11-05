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

    public void Attack(Player player, int damage)
    {
        if (func.HasNo(player, "Dodge"))
        {
            func.loseHP(player, damage);
        }
    }

    public void Dodge(Player user)
    {
        if (user.isAfterTargetted.Name == "Attack")
        {
            user.isAfterTargetted = null;
        }
    }
}
