using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QlearningStatsUI : MonoBehaviour
{
    public TMP_Text winText, healthText, rotationsText;

    private GameManager gm;

    private void Start()
    {
        gm = GameManager.instance;
    }
    // Update is called once per frame
    void Update()
    {
        winText.text = "WINS: " + gm.numWins + "/" + gm.currentMatches + " | " + gm.numWins / (float)gm.currentMatches;
        healthText.text = "Health AVG: " + gm.AvgHealth/(float)gm.currentMatches + "%";
        rotationsText.text = "Cicle: " + gm.numRotations + " | Gen: " + gm.numImprovments;
    }
}
