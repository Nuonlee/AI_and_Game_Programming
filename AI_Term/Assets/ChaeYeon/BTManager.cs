using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BTManager : MonoBehaviour
{
    public AgentStatus player1;
    public AgentStatus player2;
    public RectTransform player1HealthBar;
    public RectTransform player2HealthBar;
    public GameObject mainPanel;
    public GameObject resultPanel;
    public Image attack1Img;
    public Image attack2Img;
    public Image block1Img;
    public Image block2Img;
    public Image dodge1Img;
    public Image dodge2Img;
    public Image resultImg;
    public Text attack1Txt;
    public Text attack2Txt;
    public Text combo1Txt;
    public Text combo2Txt;
    public Text block1Txt;
    public Text block2Txt;
    public Text blocking1Txt;
    public Text blocking2Txt;
    public Text dodge1Txt;
    public Text dodge2Txt;
    public Text resultTxt;

    void LateUpdate()
    {
        player1HealthBar.localScale = new Vector3(player1.currentHealth / player1.maxHealth, 1, 1);
        player2HealthBar.localScale = new Vector3(player2.currentHealth / player2.maxHealth, 1, 1);

        if (player1.CanAttack())
        {
            attack1Txt.gameObject.SetActive(false);
            attack1Img.color = new Color(1, 1, 1, 1);
        }
        else
        {
            attack1Txt.gameObject.SetActive(true);
            attack1Txt.text = string.Format("{0:N1}", player1.attackCooldown + player1.lastAttackTime - Time.time);
            attack1Img.color = new Color(1, 1, 1, 0.4f);
        }

        if (player2.CanAttack())
        {
            attack2Txt.gameObject.SetActive(false);
            attack2Img.color = new Color(1, 1, 1, 1);
        }
        else
        {
            attack2Txt.gameObject.SetActive(true);
            attack2Txt.text = string.Format("{0:N1}", player2.attackCooldown + player2.lastAttackTime - Time.time);
            attack2Img.color = new Color(1, 1, 1, 0.4f);
        }

        if (player1.CanDefend())
        {
            block1Txt.gameObject.SetActive(false);
            block1Img.color = new Color(1, 1, 1, 1);
        }
        else
        {
            block1Txt.gameObject.SetActive(true);
            block1Txt.text = string.Format("{0:N1}", player1.defendCooldown + player1.lastDefendTime - Time.time);
            block1Img.color = new Color(1, 1, 1, 0.4f);
        }

        if (player2.CanDefend())
        {
            block2Txt.gameObject.SetActive(false);
            block2Img.color = new Color(1, 1, 1, 1);
        }
        else
        {
            block2Txt.gameObject.SetActive(true);
            block2Txt.text = string.Format("{0:N1}", player2.defendCooldown + player2.lastDefendTime - Time.time);
            block2Img.color = new Color(1, 1, 1, 0.4f);
        }

        if (player1.CanDodge())
        {
            dodge1Txt.gameObject.SetActive(false);
            dodge1Img.color = new Color(1, 1, 1, 1);
        }
        else
        {
            dodge1Txt.gameObject.SetActive(true);
            dodge1Txt.text = string.Format("{0:N1}", player1.dodgeCooldown + player1.lastDodgeTime - Time.time);
            dodge1Img.color = new Color(1, 1, 1, 0.4f);
        }

        if (player2.CanDodge())
        {
            dodge2Txt.gameObject.SetActive(false);
            dodge2Img.color = new Color(1, 1, 1, 1);
        }
        else
        {
            dodge2Txt.gameObject.SetActive(true);
            dodge2Txt.text = string.Format("{0:N1}", player2.dodgeCooldown + player2.lastDodgeTime - Time.time);
            dodge2Img.color = new Color(1, 1, 1, 0.4f);
        }
    }

    public void Result()
    {
        resultPanel.SetActive(true);
        mainPanel.SetActive(false);

        if (player1.isDead)
        {
            resultImg.color = new Color(0.4f, 0.4f, 1, 0.4f);
            resultTxt.text = "Blue Win!";
            resultTxt.color = new Color(0.4f, 0.4f, 1, 1);
        }

        if (player2.isDead)
        {
            resultImg.color = new Color(1, 0.4f, 0.4f, 0.4f);
            resultTxt.text = "Red Win!";
            resultTxt.color = new Color(1, 0.4f, 0.4f, 1);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("ChaeYeon_Demo");
    }
}
