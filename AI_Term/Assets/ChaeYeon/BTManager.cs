using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BTManager : MonoBehaviour
{
    public AgentStatus player1;
    public AgentStatus player2;
    public RectTransform player1HealthBar;
    public RectTransform player2HealthBar;
    public GameObject mainPanel;
    public GameObject resultPanel;
    public GameObject attack1Img;
    public GameObject attack2Img;
    public GameObject block1Img;
    public GameObject block2Img;
    public GameObject dodge1Img;
    public GameObject dodge2Img;
    public Image resultImg;
    //public Text attack1Txt;
    //public Text attack2Txt;
    //public Text combo1Txt;
    //public Text combo2Txt;
    //public Text block1Txt;
    //public Text block2Txt;
    //public Text blocking1Txt;
    //public Text blocking2Txt;
    //public Text dodge1Txt;
    //public Text dodge2Txt;
    //public Text resultTxt; 
    public TextMeshProUGUI attack1Txt;
    public TextMeshProUGUI attack2Txt;
    public TextMeshProUGUI combo1Txt;
    public TextMeshProUGUI combo2Txt;
    public TextMeshProUGUI block1Txt;
    public TextMeshProUGUI block2Txt;
    public TextMeshProUGUI dodge1Txt;
    public TextMeshProUGUI dodge2Txt;
    public TextMeshProUGUI resultTxt;

    void Start()
    {
            
    }   


    void LateUpdate()
    {
        player1HealthBar.localScale = new Vector3(player1.currentHealth / player1.maxHealth, 1, 1);
        player2HealthBar.localScale = new Vector3(player2.currentHealth / player2.maxHealth, 1, 1);

        if (player1.CanAttack())
        {
            attack1Txt.gameObject.SetActive(false);
            attack1Img.SetActive(false);
        }
        else
        {
            attack1Txt.gameObject.SetActive(true);
            attack1Txt.text = string.Format("{0:N1}", player1.attackCooldown + player1.lastAttackTime - Time.time);
            attack1Img.SetActive(true);
        }

        if (player2.CanAttack())
        {
            attack2Txt.gameObject.SetActive(false);
            attack2Img.SetActive(false);
        }
        else
        {
            attack2Txt.gameObject.SetActive(true);
            attack2Txt.text = string.Format("{0:N1}", player2.attackCooldown + player2.lastAttackTime - Time.time);
            attack2Img.SetActive(true);
        }

        if (player1.CanDefend())
        {
            block1Txt.gameObject.SetActive(false);
            block1Img.SetActive(false);
        }
        else
        {
            block1Txt.gameObject.SetActive(true);
            block1Txt.text = string.Format("{0:N1}", player1.defendCooldown + player1.lastDefendTime - Time.time);
            block1Img.SetActive(true);
        }

        if (player2.CanDefend())
        {
            block2Txt.gameObject.SetActive(false);
            block2Img.SetActive(false);
        }
        else
        {
            block2Txt.gameObject.SetActive(true);
            block2Txt.text = string.Format("{0:N1}", player2.defendCooldown + player2.lastDefendTime - Time.time);
            block2Img.SetActive(true);
        }

        if (player1.CanDodge())
        {
            dodge1Txt.gameObject.SetActive(false);
            dodge1Img.SetActive(false);
        }
        else
        {
            dodge1Txt.gameObject.SetActive(true);
            dodge1Txt.text = string.Format("{0:N1}", player1.dodgeCooldown + player1.lastDodgeTime - Time.time);
            dodge1Img.SetActive(true);
        }

        if (player2.CanDodge())
        {
            dodge2Txt.gameObject.SetActive(false);
            dodge2Img.SetActive(false);
        }
        else
        {
            dodge2Txt.gameObject.SetActive(true);
            dodge2Txt.text = string.Format("{0:N1}", player2.dodgeCooldown + player2.lastDodgeTime - Time.time);
            dodge2Img.SetActive(true);
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
