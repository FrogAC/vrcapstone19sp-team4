using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LeaderboardTextManager : MonoBehaviour
{
    public Text boardText;
    public Leaderboard board;
    // Start is called before the first frame update
    void Start()
    {
        boardText.text = board.ToFormatedString();
    }

    
}
