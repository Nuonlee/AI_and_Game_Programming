using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack1 : MonoBehaviour
{
    public Player2 player2;

    void OnTriggerEnter(Collider other)
    {
        player2.TakeDamage();
    }
}
