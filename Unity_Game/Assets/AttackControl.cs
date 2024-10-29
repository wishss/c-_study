using UnityEngine;

public class AttackControl : MonoBehaviour
{
    Animator myAni;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myAni = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            myAni.SetTrigger("ATTACK");
        }
    }
}
