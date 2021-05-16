using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_UIHealthPoint : MonoBehaviour
{
    public List<GameObject> HealthPointSprites;

    Health He;

    void Start()
    {
        He = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }

    void Update()
    {
        for (int i = 0; i < HealthPointSprites.Count; i++)
        {
            if (He.CurrentValue <= i)
            {
                HealthPointSprites[i].SetActive(false);
            }
            else
            {
                HealthPointSprites[i].SetActive(true);
            }
        }
    }
}
