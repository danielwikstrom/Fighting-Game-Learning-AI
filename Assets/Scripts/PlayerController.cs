using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character
{


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Move(-1);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Move(1);
        }
        if (Input.GetKey(KeyCode.L))
        {
            if (Input.GetKey(KeyCode.S))
            {
                LowerBlock();
            }
            else
            {
                UpperBlock();
            }
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            EndBlock();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            UpperPunch(); 
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            LowerPunch();
        }

    }
}
