using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Player1 player1;
    public Player2 player2;
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
    public float attack1Cool;
    public float attack2Cool;
    public float block1Cool;
    public float block2Cool;
    public float dodge1Cool;
    public float dodge2Cool;

    void Update()
    {
        attack1Cool -= Time.deltaTime;
        attack2Cool -= Time.deltaTime;
        block1Cool -= Time.deltaTime;
        block2Cool -= Time.deltaTime;
        dodge1Cool -= Time.deltaTime;
        dodge2Cool -= Time.deltaTime;
    }

    void LateUpdate()
    {
        player1HealthBar.localScale = new Vector3(player1.curHealth / player1.maxHealth, 1, 1);
        player2HealthBar.localScale = new Vector3(player2.curHealth / player2.maxHealth, 1, 1);
        combo1Txt.text = player1.currentAttack.ToString();
        combo2Txt.text = player2.currentAttack.ToString();

        if (attack1Cool < 0)
        {
            player1.canAttack = true;
            attack1Txt.gameObject.SetActive(false);
            attack1Img.color = new Color(1, 1, 1, 1);
        }
        else
        {
            player1.canAttack = false;
            attack1Txt.gameObject.SetActive(true);
            attack1Txt.text = string.Format("{0:N1}", attack1Cool);
            attack1Img.color = new Color(1, 1, 1, 0.4f);
        }

        if (attack2Cool < 0)
        {
            player2.canAttack = true;
            attack2Txt.gameObject.SetActive(false);
            attack2Img.color = new Color(1, 1, 1, 1);
        }
        else
        {
            player2.canAttack = false;
            attack2Txt.gameObject.SetActive(true);
            attack2Txt.text = string.Format("{0:N1}", attack2Cool);
            attack2Img.color = new Color(1, 1, 1, 0.4f);
        }

        if (block1Cool < 0)
        {
            player1.canBlock = true;
            block1Txt.gameObject.SetActive(false);
            block1Img.color = new Color(1, 1, 1, 1);
        }
        else
        {
            player1.canBlock = false;
            block1Txt.gameObject.SetActive(true);
            block1Txt.text = string.Format("{0:N1}", block1Cool);
            block1Img.color = new Color(1, 1, 1, 0.4f);
        }

        if (block2Cool < 0)
        {
            player2.canBlock = true;
            block2Txt.gameObject.SetActive(false);
            block2Img.color = new Color(1, 1, 1, 1);
        }
        else
        {
            player2.canBlock = false;
            block2Txt.gameObject.SetActive(true);
            block2Txt.text = string.Format("{0:N1}", block2Cool);
            block2Img.color = new Color(1, 1, 1, 0.4f);
        }

        if (dodge1Cool < 0)
        {
            player1.canDodge = true;
            dodge1Txt.gameObject.SetActive(false);
            dodge1Img.color = new Color(1, 1, 1, 1);
        }
        else
        {
            player1.canDodge = false;
            dodge1Txt.gameObject.SetActive(true);
            dodge1Txt.text = string.Format("{0:N1}", dodge1Cool);
            dodge1Img.color = new Color(1, 1, 1, 0.4f);
        }

        if (dodge2Cool < 0)
        {
            player2.canDodge = true;
            dodge2Txt.gameObject.SetActive(false);
            dodge2Img.color = new Color(1, 1, 1, 1);
        }
        else
        {
            player2.canDodge = false;
            dodge2Txt.gameObject.SetActive(true);
            dodge2Txt.text = string.Format("{0:N1}", dodge2Cool);
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
        SceneManager.LoadScene("SampleScene");
    }
}
