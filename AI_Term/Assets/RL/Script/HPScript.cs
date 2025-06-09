using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPScript : MonoBehaviour
{
    public AgentStatus_Re status;
    public float curHealth;
    public float maxHealth;

    public Slider HpSlider;
    // Start is called before the first frame update
    void Start()
    {
        SetHp();
    }

    // Update is called once per frame
    void Update()
    {
        SetHp();
        CheckHp();
    }
    public void SetHp()
    {
        maxHealth = status.maxHealth;
        curHealth = status.currentHealth;
    }

    public void CheckHp()
    {
        if(HpSlider != null)
        {
            HpSlider.value = curHealth / maxHealth;
        }
    }
}
