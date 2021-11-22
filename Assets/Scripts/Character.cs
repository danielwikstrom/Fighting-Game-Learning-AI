using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character: MonoBehaviour
{
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

    private Transform _transform;
    private Vector3 initFistScale;
    private bool isPunching;
    private bool isBlocking;
    private bool upperBlock;
    // Start is called before the first frame update
    void Awake()
    {
        _transform = gameObject.transform;
        isPunching = false;
        isBlocking = false;
        upperBlock = true;
        initFistScale = Fist.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
    }

    protected void Move(float input)
    {
        Vector3 newPos = new Vector3();
        newPos = _transform.position;
        newPos.x += speed * Time.deltaTime * input;
        _transform.position = newPos;
    }

    protected void UpperPunch()
    {
        if (!isPunching && !isBlocking)
        {
            Fist.transform.position = UpperPunchPos.position;
            isPunching = true;
            StartCoroutine("ResetPunchTimer");
        }
    }

    protected void LowerPunch()
    {
        if (!isPunching && !isBlocking)
        {
            Fist.transform.position = LowerPunchPos.position;
            isPunching = true;
            StartCoroutine("ResetPunchTimer");
        }
    }

    private void EndPunch()
    {
        Fist.transform.position = FistPos.position;
        isPunching = false;
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
}
