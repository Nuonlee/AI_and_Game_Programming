using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Player1 player1;
    public Player2 player2;
    public RectTransform player1HealthBar;
    public RectTransform player2HealthBar;
    public Image attack1Img;
    public Image attack2Img;
    public Image block1Img;
    public Image block2Img;
    public Image dodge1Img;
    public Image dodge2Img;
    public float attackCool;
    public float blockCool;
    public float dodgeCool;

    void LateUpdate()
    {
        player1HealthBar.localScale = new Vector3(player1.curHealth / player1.maxHealth, 1, 1);
        player2HealthBar.localScale = new Vector3(player2.curHealth / player2.maxHealth, 1, 1);
    }
}
