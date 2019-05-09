using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HRGameManager : MonoBehaviour
{
    public delegate void HomeRunDelegate();
    public event HomeRunDelegate homeRunEvent;

    private int homeruns;


    public void HomeRun()
    {
        if (homeRunEvent != null)
        {
            homeRunEvent();
        }
    }
}
