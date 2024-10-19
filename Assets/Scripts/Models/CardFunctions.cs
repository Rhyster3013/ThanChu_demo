using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardName : MonoBehaviour
{
    FunctionController func;

    private void Start()
    {
        func = GetComponent<FunctionController>();
    }

    public void Attack(Player player, int damage)
    {
        if (player == null || player.isAfterTargetted) return;
        else
        {
            func.loseHP(player, damage);
        }
    }
}
