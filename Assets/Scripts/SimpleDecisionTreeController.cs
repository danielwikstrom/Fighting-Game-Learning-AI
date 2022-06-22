using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDecisionTreeController : AIController
{


    // Update is called once per frame
    void Update()
    {
        UpdateDecisionTree();
    }

    private void UpdateDecisionTree()
    {
        if (!oponent)
            return;
        if ( !IsInRange(GetDistanceToOponent()))
        {
            if (isTooFar)
            {
                MoveTowardsOponent();
            }
            else
            {
                MoveAwayFromOponent();
                AIUpperBlock();
            }
        }
        else
        {
            if (oponent.isPunching)
            {
                if (oponent.upperPunch)
                {
                    AIUpperBlock();
                }
                else
                {
                    AILowerBlock();
                }
            }
            else if (oponent.isBlocking)
            {
                if (oponent.upperBlock)
                {
                    LowerPunch();
                }
                else
                {
                    UpperPunch();
                }
            }
            else if (oponent.wasBlocked)
            {
                int rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    UpperPunch();
                }
                else
                {
                    LowerPunch();
                }
            }

        }
    }
}
