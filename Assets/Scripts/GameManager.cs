using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public List<Character> players;

    public float mapMinX = -18;
    public float mapMaxX = 18;

    public float TimeMultiplier = 1.0f;

    public Dictionary<QLearningController.GAMESTATE, float[,]> Q;

    public int MatchesPerRotation = 50;
    public float GoalWinPercentage = 0.8f;
    [HideInInspector]
    public int currentMatches = 0;
    [HideInInspector]
    public int numWins = 0;

    [HideInInspector]
    public int numRotations;

    [HideInInspector]
    public int numImprovments = 0;

    [HideInInspector]
    public float AvgHealth;

    public QLearningController learningAgent;
    public QLearningController nonLearningAgent;
    public SimpleDecisionTreeController RandomAgent;

    public float timer = 0.0f;

    // Start is called before the first frame update
    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this);

        players = new List<Character>();
        numRotations = 1;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 10000)
        {

            players[0].Reset(true);
            players[1].Reset(true);
            timer = 0.0f;
        }
        Time.timeScale = TimeMultiplier;
    }

    public void CharacterDied(int id, bool trainingMode = true)
    {
        if (id != learningAgent.getID())
            numWins++;
        if(trainingMode)
            ResetMatch();
    }

    public void ResetMatch()
    {
        currentMatches++;
        AvgHealth += (learningAgent.GetHealth());
        players[0].Reset();
        players[1].Reset();

        if (currentMatches >= MatchesPerRotation)
        {
            numRotations++;

            float currentWinRate = numWins / (float)currentMatches;
            Debug.Log("CURRENT WINRATE: " + currentWinRate + " | " + GoalWinPercentage);
            if (currentWinRate >= GoalWinPercentage)
            {
                Debug.Log("yowegothere");
                this.Q = new Dictionary<QLearningController.GAMESTATE, float[,]>();
                this.Q = learningAgent.Q;
                //change ai with another
                //learningAgent.ResetQMatrix();
                nonLearningAgent.gameObject.SetActive(true);
                RandomAgent.gameObject.SetActive(false);
                players = new List<Character>();
                players.Add(learningAgent);
                players.Add(nonLearningAgent);
                numImprovments++;
            }

            numWins = 0;
            currentMatches = 0;
            AvgHealth = 0.0f;
        }
    }

}
