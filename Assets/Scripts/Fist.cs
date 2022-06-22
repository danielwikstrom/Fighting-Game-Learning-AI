using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : MonoBehaviour
{

    private Character _char;
    private int charID;
    private Collider _col;
    private int damageDealt;

    // Start is called before the first frame update
    void Awake()
    {
        _col = GetComponent<Collider>();
        //_col.enabled = false;

        _char = GetComponentInParent<Character>();
        charID = _char.getID();
        damageDealt = _char.damagePerPunch;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleCollider(bool active)
    {
        _col.enabled = active;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("CharacterBody"))
        {
            return;
        }
        var oponent = other.gameObject.GetComponentInParent<Character>();
        if (_char.isPunching && oponent && oponent.getID() != _char.getID())
        {
            if (oponent.isBlocking)
            {
                if (oponent.upperBlock && _char.upperPunch)
                {
                    _char.PunchGotBlocked();
                    oponent.BlockedPunch();
                    return;
                }
                else if (!oponent.upperBlock && !_char.upperPunch)
                {
                    _char.PunchGotBlocked();
                    oponent.BlockedPunch();
                    return;
                }
            }
            oponent.GetHit(damageDealt);
            _char.OponentGotHit();
        }
    }
}
