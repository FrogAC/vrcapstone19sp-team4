using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    [SerializeField] private float m_speed = 1.0f;
    IEnumerator spin()
    {
        while (true)
        {
            transform.Rotate(0, m_speed * Time.deltaTime, 0);
            yield return null;
        }
    }

    void Start()
    {
        StartCoroutine(spin());
    }
}
