using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    int playerID = 0;
    [SerializeField]
    int health = 100;
    [SerializeField]
    float speed = 2;
    [SerializeField]
    private GameObject Fist;
    [SerializeField]
    private Transform FistPos;
    [SerializeField]
    private Transform UpperPunchPos;
    [SerializeField]
    private Transform LowerPunchPos;
    [SerializeField]
    private Transform UpperBlockPos;
    [SerializeField]
    private Transform LowerBlockPos;

    protected Transform _transform;
    private Fist _fist;
    private Vector3 initFistScale;
    private int _currentHealth;
    private bool canMove = true;
    private Transform Obstacle;

    public int damagePerPunch = 5;
    [HideInInspector]
    public bool isPunching = false;
    [HideInInspector]
    public bool upperPunch = true;
    [HideInInspector]
    public bool isBlocking;
    [HideInInspector]
    public bool upperBlock;
    // Start is called before the first frame update
    void Awake()
    {
        _transform = gameObject.transform;
        isPunching = false;
        isBlocking = false;
        upperBlock = true;
        initFistScale = Fist.transform.localScale;
        _currentHealth = health;
        _fist = GetComponentInChildren<Fist>();
    }

    protected virtual void Start()
    {
        Debug.Log("heyyo");
        GameManager.instance.players.Add(this);
    }


    protected void Move(float input)
    {
            Vector3 newPos = new Vector3();
            newPos = _transform.position;
            newPos.x += speed * Time.deltaTime * input;
        if (canMove)
        {
            _transform.position = newPos;
        }
        else
        {
            float distance = (_transform.position - Obstacle.position).magnitude;
            if (distance < (newPos - Obstacle.position).magnitude)
            {
                _transform.position = newPos;
            }
        }
    }

    protected void UpperPunch()
    {
        if (!isPunching && !isBlocking)
        {
            Fist.transform.position = UpperPunchPos.position;
            isPunching = true;
            upperPunch = true;
            _fist.ToggleCollider(true);
            StartCoroutine("ResetPunchTimer");
        }
    }

    protected void LowerPunch()
    {
        if (!isPunching && !isBlocking)
        {
            Fist.transform.position = LowerPunchPos.position;
            isPunching = true;
            _fist.ToggleCollider(true);
            upperPunch = false;
            StartCoroutine("ResetPunchTimer");
        }
    }

    private void EndPunch()
    {
        Fist.transform.position = FistPos.position;
        isPunching = false;
        _fist.ToggleCollider(false);
    }

    protected void UpperBlock()
    {
        if (!isPunching)
        {
            Fist.transform.position = UpperBlockPos.position;
            Vector3 newScale = Fist.transform.localScale;
            newScale.y = initFistScale.y * 2;
            Fist.transform.localScale = newScale;
            isBlocking = true;
            upperBlock = true;
        }
    }

    protected void LowerBlock()
    {
        if (!isPunching)
        {
            Fist.transform.position = LowerBlockPos.position;
            Vector3 newScale = Fist.transform.localScale;
            newScale.y = initFistScale.y * 2;
            Fist.transform.localScale = newScale;
            isBlocking = true;
            upperBlock = false;
        }
    }

    protected void EndBlock()
    {
        Fist.transform.position = FistPos.position;
        Fist.transform.localScale = initFistScale;
        isBlocking = false;
    }

    IEnumerator ResetPunchTimer()
    {
        yield return new WaitForSeconds(0.2f);
        EndPunch();
    }

    public void GetHit(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }


    }

    public int getID()
    {
        return playerID;
    }

    public int GetHealth()
    {
        return _currentHealth;
    }

        private void Die()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Character>())
        {
            Obstacle = collision.gameObject.transform;
            canMove = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Character>())
        {
            canMove = true;
        }
    }
}
