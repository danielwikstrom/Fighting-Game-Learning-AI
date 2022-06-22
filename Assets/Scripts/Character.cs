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
    //The time in seconds that it takes to throw a punch
    [SerializeField]
    float punchSpeed = 0.1f;
    //After a punch gets blocked, the agent cant punch or block
    [SerializeField]
    float blockStunTime = 0.5f;
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
    private Vector3 initPos;

    public int damagePerPunch = 5;
    [HideInInspector]
    public bool isPunching = false;
    [HideInInspector]
    public bool upperPunch = true;
    [HideInInspector]
    public bool isBlocking;
    [HideInInspector]
    public bool upperBlock;
    [HideInInspector]
    public bool wasBlocked;
    // Start is called before the first frame update
    void Awake()
    {
        _transform = gameObject.transform;
        isPunching = false;
        isBlocking = false;
        upperBlock = true;
        _currentHealth = health;
        initFistScale = Fist.transform.localScale;
        _fist = GetComponentInChildren<Fist>();
        initPos = _transform.position;
    }

    protected virtual void Start()
    {
        GameManager.instance.players.Add(this);
    }


    protected void Move(float input)
    {
            Vector3 newPos = new Vector3();
            newPos = _transform.position;
            newPos.x += speed * Time.deltaTime * input;
        if (newPos.x <= GameManager.instance.mapMinX || newPos.x > GameManager.instance.mapMaxX)
        {
            return;
        }
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
        if (!isPunching && !isBlocking && !wasBlocked)
        {
            upperPunch = true;
            StartCoroutine(ThrowPunch(UpperPunchPos));
        }
    }

    protected void LowerPunch()
    {
        if (!isPunching && !isBlocking && !wasBlocked)
        {
            upperPunch = false;
            StartCoroutine(ThrowPunch(LowerPunchPos));
        }
    }

    IEnumerator ThrowPunch(Transform FistEndPos)
    {
        float delta = 0.0f;
        isPunching = true;
        while (delta < 1)
        {
            delta += Time.deltaTime/punchSpeed;
            Vector3 newPos = Vector3.Lerp(FistPos.position, FistEndPos.position, delta);
            Fist.transform.position = newPos;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        delta = 0;
        //_fist.ToggleCollider(true);
        //yield return new WaitForSeconds(Time.deltaTime);
        //_fist.ToggleCollider(false);
        while (delta < 1)
        {
            delta += Time.deltaTime / punchSpeed;
            Vector3 newPos = Vector3.Lerp(FistEndPos.position, FistPos.position, delta);
            Fist.transform.position = newPos;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        isPunching = false;
    }

    IEnumerator BlockStun()
    {
        wasBlocked = true;
        yield return new WaitForSeconds(blockStunTime);
        wasBlocked = false;
    }
  

    protected void UpperBlock()
    {
        if (!isPunching && !wasBlocked)
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
        if (!isPunching && !wasBlocked)
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


    public virtual void GetHit(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth < 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    public virtual void OponentGotHit()
    { 
    }

    public virtual void BlockedPunch()
    {
    }

    public virtual void PunchGotBlocked()
    {
        StartCoroutine("BlockStun");
    }

    public int getID()
    {
        return playerID;
    }

    public int GetHealth()
    {
        return _currentHealth;
    }

    protected virtual void Die()
    {
        GameManager.instance.CharacterDied(this.playerID);
    }

    public virtual void Reset(bool positionOnly = false)
    {
        if (!positionOnly)
        {
            isPunching = false;
            isBlocking = false;
            upperBlock = true;
            _currentHealth = health;
            EndBlock();
        }

        _transform.position = initPos;
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
