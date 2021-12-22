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
        if (oponent && !IsInRange(GetDistanceToOponent()))
        {
            if (isTooFar)
            {
                MoveTowardsOponent();
            }
            else
            {
                MoveAwayFromOponent();
                DoRandomAction();
            }
        }
        else
        {
            DoRandomAction();
        }
    }
}
