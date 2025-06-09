using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class RL_Manager : MonoBehaviour
{
    public TextMeshProUGUI Atk1;
    public TextMeshProUGUI Def1;
    public TextMeshProUGUI Dodge1;
    public TextMeshProUGUI Atk2;
    public TextMeshProUGUI Def2;
    public TextMeshProUGUI Dodge2;
    public Image Atk1img;
    public Image Def1img;
    public Image Dodge1img;
    public Image Atk2img;
    public Image Def2img;
    public Image Dodge2img;

    public TextMeshProUGUI ATKVicCount;
    public TextMeshProUGUI DEFVicCount;

    public RectTransform player1HealthBar;
    public RectTransform player2HealthBar;

    public AgentStatus_Re p1status;
    public AgentStatus_Re p2status;

    public RL_Agent p1cool;
    public RL_Agent p2cool;

    public GameObject mainPanel;
    public GameObject resultPanel;
    public Image resultImg;
    public Text resultTxt;

    float attack1Cool;
    float defend1Cool;
    float dodge1Cool;
    float attack2Cool;
    float defend2Cool;
    float dodge2Cool;

    // Start is called before the first frame update
    void Start()
    {
        attack1Cool = p1cool.leftattackCooldownTime;
        defend1Cool = p1cool.leftdefendCooldownTime;
        dodge1Cool = p1cool.leftdodgeCooldownTime;
        attack2Cool = p2cool.leftattackCooldownTime;
        defend2Cool = p2cool.leftdefendCooldownTime;
        dodge2Cool = p2cool.leftdodgeCooldownTime;
        ATKVicCount.text = string.Format("{0:D0}", p1cool.VictoryCount);
        DEFVicCount.text = string.Format("{0:D0}", p1cool.VictoryCount);
    }

    // Update is called once per frame
    void Update()
    {
        player1HealthBar.localScale = new Vector3(p1status.currentHealth / p1status.maxHealth, 1, 1);
        player2HealthBar.localScale = new Vector3(p2status.currentHealth / p2status.maxHealth, 1, 1);
        ATKVicCount.text = string.Format("{0:N1}", p1cool.VictoryCount);
        DEFVicCount.text = string.Format("{0:N1}", p1cool.VictoryCount);

        // Cooldown
        attack1Cool = p1cool.leftattackCooldownTime;
        defend1Cool = p1cool.leftdefendCooldownTime;
        dodge1Cool = p1cool.leftdodgeCooldownTime;
        attack2Cool = p2cool.leftattackCooldownTime;
        defend2Cool = p2cool.leftdefendCooldownTime;
        dodge2Cool = p2cool.leftdodgeCooldownTime;

        if (attack1Cool < 0)
        {
            Atk1.gameObject.SetActive(false);
            Atk1img.color = new Color(1, 1, 1, 0.4f);
        }
        else
        {
            Atk1.gameObject.SetActive(true);
            Atk1.text = string.Format("{0:N1}", attack1Cool);
            Atk1img.color = new Color(1, 1, 1, 1);
        }
        if (attack2Cool < 0)
        {
            Atk2.gameObject.SetActive(false);
            Atk2img.color = new Color(1, 1, 1, 0.4f);
        }
        else
        {
            Atk2.gameObject.SetActive(true);
            Atk2.text = string.Format("{0:N1}", attack2Cool);
            Atk2img.color = new Color(1, 1, 1, 1);
        }

        if (defend1Cool < 0)
        {
            Def1.gameObject.SetActive(false);
            Def1img.color = new Color(1, 1, 1, 0.4f);
        }
        else
        {
            Def1.gameObject.SetActive(true);
            Def1.text = string.Format("{0:N1}", defend1Cool);
            Def1img.color = new Color(1, 1, 1, 1);
        }
        if (defend2Cool < 0)
        {
            Def2.gameObject.SetActive(false);
            Def2img.color = new Color(1, 1, 1, 0.4f);
        }
        else
        {
            Def2.gameObject.SetActive(true);
            Def2.text = string.Format("{0:N1}", defend2Cool);
            Def2img.color = new Color(1, 1, 1, 1);
        }

        if (dodge1Cool < 0)
        {
            Dodge1.gameObject.SetActive(false);
            Dodge1img.color = new Color(1, 1, 1, 0.4f);
        }
        else
        {
            Dodge1.gameObject.SetActive(true);
            Dodge1.text = string.Format("{0:N1}", dodge1Cool);
            Dodge1img.color = new Color(1, 1, 1, 1);
        }
        if (dodge2Cool < 0)
        {
            Dodge2.gameObject.SetActive(false);
            Dodge2img.color = new Color(1, 1, 1, 0.4f);
        }
        else
        {
            Dodge2.gameObject.SetActive(true);
            Dodge2.text = string.Format("{0:N1}", dodge2Cool);
            Dodge2img.color = new Color(1, 1, 1, 1);
        }
    }
    public void Result()
    {
        resultPanel.SetActive(true);
        mainPanel.SetActive(false);

        if (!p1status.IsAlive())
        {
            resultImg.color = new Color(0.4f, 0.4f, 1, 0.4f);
            resultTxt.text = "Blue Win!";
            resultTxt.color = new Color(0.4f, 0.4f, 1, 1);
        }

        if (!p2status.IsAlive())
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
