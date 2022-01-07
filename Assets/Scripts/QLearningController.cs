using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QLearningController : AIController
{
    private const int NUM_MOVEACTION = 3;
    private const int NUM_FIGHTACTION = 5;
    private const float LEARNING_RATE = 0.3f;
    private const float DISCOUNT = 0.1f;

    private const float EXPLORATION_RATE = 0.2f;

    private float reward = 0.0f;


    public bool isLearning = true;
    
    public enum MOVEACTION
    {
        MOVEAWAY = 0,
        MOVETOWARDS,
        STAY
    };

    public enum FIGHTACTION
    {
        UPPERPUNCH = 0,
        LOWERPUNCH,
        UPPERBLOCK,
        LOWERBLOCK,
        NOTHING
    };

    public struct ACTION
    {
        public MOVEACTION moveAction;
        public FIGHTACTION fightAction;
    }

    public struct GAMESTATE
    {
        public bool inRange;
        public bool isTooFar;
        public bool isPunching;
        public bool isUpperPunch;
        public bool isBlocking;
        public bool isUpperBlock;
        public bool isStunBlocked;
        public bool isOponentPunching;
        public bool oponentUpperPunch;
        public bool isOponentBlocking;
        public bool oponentUpperBlock;
        public bool isOponentStunBlocked;
    };


    //Q matrix
    public Dictionary<GAMESTATE, float[,]> Q;
    public ACTION currentAction;


    [System.Serializable]
    public class QStates
    {
        public bool inRange;
        public bool isTooFar;
        public bool isPunching;
        public bool isUpperPunch;
        public bool isBlocking;
        public bool isUpperBlock;
        public bool isStunBlocked;
        public bool isOponentPunching;
        public bool oponentUpperPunch;
        public bool isOponentBlocking;
        public bool oponentUpperBlock;
        public bool isOponentStunBlocked;
        public float[] MOVEAWAY;
        public float[] MOVETOWARDS;
        public float[] STAY;


    }


    [System.Serializable]
    public class Qdata
    {
        public QStates[] qdata;
    }

    public class JsonSerializer
    {
        public void ReadFile(TextAsset json)
        {
            Qdata qMatrixData = JsonUtility.FromJson<Qdata>(json.text);
            foreach (QStates state in qMatrixData.qdata)
            {
                Debug.Log(" STATE :");
                Debug.Log("|    IN RANGE    | " + state.inRange);
                Debug.Log("|    TOO FAR     | " + state.isTooFar);
                Debug.Log("|  IS PUNCHING   | " + state.isPunching);
                Debug.Log("|  UPPER PUNCH   | " + state.isUpperPunch);
                Debug.Log("|  IS BLOCKING   | " + state.isBlocking);
                Debug.Log("|  UPPER BLOCK   | " + state.isUpperBlock);
                Debug.Log("|   IS STUNNED   | " + state.isStunBlocked);
                Debug.Log("| OP IS PUNCHING | " + state.isOponentPunching);
                Debug.Log("| OP UPPER PUNCH | " + state.oponentUpperPunch);
                Debug.Log("| OP IS BLOCKING | " + state.isOponentBlocking);
                Debug.Log("| OP UPPER BLOCK | " + state.oponentUpperBlock);
                Debug.Log("| OP IS STUNNED  | " + state.isOponentStunBlocked);
            }
        }
    }
    public void PrintQMatrix()
    {
        string qmatrix = "";
        foreach (KeyValuePair<GAMESTATE, float[,]> entry in Q)
        {
            //Debug.Log(" STATE :");
            //Debug.Log("|    IN RANGE    | " + entry.Key.inRange);
            //Debug.Log("|    TOO FAR     | " + entry.Key.isTooFar);
            //Debug.Log("|  IS PUNCHING   | " + entry.Key.isPunching);
            //Debug.Log("|  UPPER PUNCH   | " + entry.Key.isUpperPunch);
            //Debug.Log("|  IS BLOCKING   | " + entry.Key.isBlocking);
            //Debug.Log("|  UPPER BLOCK   | " + entry.Key.isUpperBlock);
            //Debug.Log("|   IS STUNNED   | " + entry.Key.isStunBlocked);
            //Debug.Log("| OP IS PUNCHING | " + entry.Key.isOponentPunching);
            //Debug.Log("| OP UPPER PUNCH | " + entry.Key.oponentUpperPunch);
            //Debug.Log("| OP IS BLOCKING | " + entry.Key.isOponentBlocking);
            //Debug.Log("| OP UPPER BLOCK | " + entry.Key.oponentUpperBlock);
            //Debug.Log("| OP IS STUNNED  | " + entry.Key.isOponentStunBlocked);


            //qmatrix.Remove(qmatrix.Length-1);
            foreach (MOVEACTION m in Enum.GetValues(typeof(MOVEACTION)))
            {
                int i = (int)m;
                //Debug.Log(" ACTION :");
                foreach (FIGHTACTION f in Enum.GetValues(typeof(FIGHTACTION)))
                {
                    int j = (int)f;
                    //Debug.Log("| " + (MOVEACTION)i + " | " + (FIGHTACTION)j + " | " + entry.Value[i, j]);
                    
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        JsonSerializer serializer = new JsonSerializer();
        serializer.ReadFile(GameManager.instance.qMatrixJson);


        base.Start();
        StartCoroutine("GetOponent");
        Q = new Dictionary<GAMESTATE, float[,]>();
        currentAction = new ACTION();
        currentAction.moveAction = MOVEACTION.MOVEAWAY;
        currentAction.fightAction = FIGHTACTION.UPPERPUNCH;
        if (isLearning)
        {
            GameManager.instance.learningAgent = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (oponent)
        {
            if (isLearning)
            {
                reward = 0;
                UpdateQMatrix(Q);
                float rand = UnityEngine.Random.Range(0.0f, 1.0f);
                if (rand < EXPLORATION_RATE)
                {
                    int randomAction = UnityEngine.Random.Range(0, NUM_MOVEACTION - 1);
                    currentAction.moveAction = (MOVEACTION)randomAction;
                    randomAction = UnityEngine.Random.Range(0, NUM_FIGHTACTION - 1);
                    currentAction.fightAction = (FIGHTACTION)randomAction;
                }
                else
                {
                    currentAction = GetBestAction(GetCurrentGameState(), Q);
                }
                DoAction(currentAction);
                //Debug.Log("CHOSEN ACTION: " + currentAction.moveAction.ToString() + " " + currentAction.fightAction.ToString());
                UpdateQMatrix(Q);
                //Debug.Log(currentAction.moveAction);
            }
            else
            {
                UpdateQMatrix(GameManager.instance.Q);
                currentAction = GetBestAction(GetCurrentGameState(), GameManager.instance.Q);
                DoAction(currentAction);
            }
        }
    }

    private void DoAction(ACTION a)
    {
        switch (a.moveAction)
        {
            case MOVEACTION.MOVEAWAY:
                QMoveAway();
                break;
            case MOVEACTION.MOVETOWARDS:
                QMoveTowards();
                break;
            case MOVEACTION.STAY:
                break;
        }
        //if a fighting action is already taking place, the agent must wait until it has finished to perform another one
        if (!isPunching && !isBlocking)
        {
            switch (a.fightAction)
            {
                case FIGHTACTION.UPPERPUNCH:
                    UpperPunch();
                    break;
                case FIGHTACTION.LOWERPUNCH:
                    LowerPunch();
                    break;
                case FIGHTACTION.UPPERBLOCK:
                    AIUpperBlock();
                    break;
                case FIGHTACTION.LOWERBLOCK:
                    AILowerBlock();
                    break;
                case FIGHTACTION.NOTHING:
                    break;
            }
        }
    }

    private GAMESTATE GetCurrentGameState()
    {
        GAMESTATE s = new GAMESTATE();
        s.inRange = IsInRange(GetDistanceToOponent());
        s.isTooFar = isTooFar;
        s.isPunching = isPunching;
        s.isUpperPunch = upperPunch;
        s.isBlocking = isBlocking;
        s.isUpperBlock = upperBlock;
        s.isStunBlocked = wasBlocked;
        s.isOponentPunching = oponent.isPunching;
        s.oponentUpperPunch = oponent.upperPunch;
        s.isOponentBlocking = oponent.isBlocking;
        s.oponentUpperBlock = oponent.upperBlock;
        s.isOponentStunBlocked = oponent.wasBlocked;
        return s;
    }

    //This method updates the Qmatrix based on the reward obtained for the current state of the game. If there is no record of 
    // the games current state, a new tuple is added to the dictionary.
    private void UpdateQMatrix(Dictionary<GAMESTATE, float[,]> Q)
    {
        GAMESTATE s = GetCurrentGameState();
        if (!Q.ContainsKey(s))
        {
            float[,] actions = new float[NUM_MOVEACTION, NUM_FIGHTACTION];
            foreach (MOVEACTION m in Enum.GetValues(typeof(MOVEACTION)))
            {
                int i = (int)m;
                foreach (FIGHTACTION f in Enum.GetValues(typeof(FIGHTACTION)))
                {
                    int j = (int)f;
                    actions[i, j] = 0.0f;
                }
            }
            Q.Add(s, actions);
        }
        int moveaction = (int)currentAction.moveAction;
        int fightaction = (int)currentAction.fightAction;
        float bestValue = Q[s][0,0];
        for (int i = 0; i < Q[s].GetLength(0); i++)
        {
            for (int j = 0; j < Q[s].GetLength(1); j++)
            {
                if (Q[s][i, j] > bestValue)
                {
                    bestValue = Q[s][i, j];
                }
            }
        }
        if (isLearning)
            Q[s][moveaction, fightaction] += LEARNING_RATE * (reward + DISCOUNT * bestValue - Q[s][moveaction, fightaction]);
        //Reset reward after updating the matrix
        reward = 0.0f;

        this.Q = Q;
    }

    //returns the best action for the given game state based on the Q matrix
    private ACTION GetBestAction(GAMESTATE s, Dictionary<GAMESTATE, float[,]> Q)
    {
        float bestValue = Q[s][0, 0];
        ACTION action = new ACTION();
        MOVEACTION chosenMoveAction = 0;
        FIGHTACTION chosenFightAction = 0;
        for (int i = 0; i < Q[s].GetLength(0); i++)
        {
            for (int j = 0; j < Q[s].GetLength(1); j++)
            {
                if (Q[s][i, j] > bestValue)
                {
                    bestValue = Q[s][i, j];
                    chosenMoveAction = (MOVEACTION)i;
                    chosenFightAction = (FIGHTACTION)j;
                }
            }
        }

        action.moveAction = chosenMoveAction;

        //As punching and blocking take some time to see if there is a good or bad reward, until the action doesnt finish, there wont be a change of action
        if (!isPunching && !isBlocking)
            action.fightAction = chosenFightAction;
        else
            action.fightAction = currentAction.fightAction;
        return action;
    }

    public override void BlockedPunch()
    {
        base.BlockedPunch();
        reward = 0.5f;
        UpdateQMatrix(Q);
        if (isLearning)
            Debug.Log("Blocked punch");
    }

    public override void PunchGotBlocked()
    {
        base.PunchGotBlocked();
        reward = -0.4f;
        UpdateQMatrix(Q);
    }

    public override void GetHit(int damage)
    {
        base.GetHit(damage);
        reward = -0.5f;
        UpdateQMatrix(Q);
    }

    public override void OponentGotHit()
    {
        base.OponentGotHit();
        reward = 0.2f;
        UpdateQMatrix(Q);
    }

    protected override void Die()
    {
        base.Die();
        //reward = -0.3f;
        //UpdateQMatrix();
    }

    //Calls Move() method but calcultaes reward based on if the agent is now in range or not
    private void QMoveTowards()
    {
        float previousDist = GetDistanceToOponent();
        MoveTowardsOponent();
        float currentDist = GetDistanceToOponent();
        //got in range
        if (!IsInRange(previousDist) && IsInRange(currentDist))
        {
            reward = 0.05f;
        }
        //got out of range
        if (IsInRange(previousDist) && !IsInRange(currentDist))
        {
            reward = -0.05f;
        }
        else if (!IsInRange(previousDist) && !IsInRange(currentDist))
        {
            if (isTooFar)
            {
                // agent got closer to oponent
                reward = 0.05f;
            }
            else
            {
                //agent got even closer to oponent, when already too close
                reward = -0.01f;
            }
        }
        // moved but is still in range
        else
        {
            reward = 0.01f;
        }
        UpdateQMatrix(Q);
    }

    //Calls Move() method but calcultaes reward based on if the agent is now in range or not
    private void QMoveAway()
    {
        float previousDist = GetDistanceToOponent();
        MoveAwayFromOponent();
        float currentDist = GetDistanceToOponent();
        //got in range
        if (!IsInRange(previousDist) && IsInRange(currentDist))
        {
            reward = 0.05f;
        }
        //got out of range
        if (IsInRange(previousDist) && !IsInRange(currentDist))
        {
            reward = -0.05f;
        }
        //if agent is out of range after and before moving
        else if (!IsInRange(previousDist) && !IsInRange(currentDist))
        {
            //agenbt is getting further away from the oponent
            if (isTooFar)
            {
                reward = -0.01f;
            }
            else
            {
                //if agent got further away from oponent, but was too close to it
                reward = 0.01f;
            }
        }
        // moved but is still in range
        else
        {
            reward = 0.01f;
        }
        UpdateQMatrix(Q);
    }

    public void ResetQMatrix()
    {
        Q.Clear();
        Q = new Dictionary<GAMESTATE, float[,]>();
    }
}
