using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack2 : MonoBehaviour
{
    public Player1 player1;

    void OnTriggerEnter(Collider other)
    {
        player1.TakeDamage();
    }
}
