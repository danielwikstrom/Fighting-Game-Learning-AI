using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Character
{

    public Character oponent;
    public float minBlockDuration = 0.5f;

    [SerializeField]
    private float minDistance = 3.0f;

    [SerializeField]
    private float maxDistance = 3.6f;

    protected bool isTooFar = true;
    protected bool aiIsBlocking = false;
    // Start is called before the first frame update
    void Start()
    {
        
        base.Start();
        StartCoroutine("GetOponent");

    }

    // Update is called once per frame
    void Update()
    {
    }

    protected float GetDistanceToOponent()
    {
        if (oponent)
            return Mathf.Abs(_transform.position.x - oponent.transform.position.x);
        else
            return Mathf.Infinity;
    }

    IEnumerator GetOponent()
    {
        yield return new WaitForSeconds(0.01f);
        foreach (var player in GameManager.instance.players)
        {
            if (player.getID() != this.getID())
            {
                oponent = player;
            }
        }
    }

    protected void MoveTowardsOponent()
    {
        //is to the left
        if (oponent.transform.position.x < transform.position.x)
        {
            Move(-1);
        }
        //is to the right
        else
        {
            Move(1);
        }
    }

    protected void MoveAwayFromOponent()
    {
        //is to the left
        if (oponent.transform.position.x < transform.position.x)
        {
            Move(1);
        }
        //is to the right
        else
        {
            Move(-1);
        }
    }

    // Will return true if the oponent is withing the set distance to attack or block attacks;
    protected bool IsInRange(float distance)
    {

        if (distance > minDistance && distance < maxDistance)
        {
            return true;
        }
        else
        {
            isTooFar = distance > maxDistance;
            return false;
        }
    }

    protected void DoRandomAction()
    {
        int rand = Random.Range(0, 4);
        if (!isPunching && !aiIsBlocking)
        {
            switch (rand)
            {
                case 0:
                    UpperPunch();
                    break;
                case 1:
                    LowerPunch();
                    break;
                case 2:
                    AIUpperBlock();
                    break;
                case 3:
                    AILowerBlock();
                    break;
                default:
                    break;
            }
        }
    }

    //This function is used by the AI to block, Making it so it is forced to block for a minimum amount of time;
    protected void AIUpperBlock()
    {
        UpperBlock();
        StartCoroutine("AIBlock");
    }

    protected void AILowerBlock()
    {
        LowerBlock();
        StartCoroutine("AIBlock");
    }

    public override void Reset(bool positionOnly = false)
    {
        base.Reset();
        if(!positionOnly)
            StartCoroutine("GetOponent");
    }

    IEnumerator AIBlock()
    {
        aiIsBlocking = true;
        yield return new WaitForSeconds(minBlockDuration);
        EndBlock();
        aiIsBlocking = false;
    }

}
