using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public List<Character> players;

    // Start is called before the first frame update
    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this);

        players = new List<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
