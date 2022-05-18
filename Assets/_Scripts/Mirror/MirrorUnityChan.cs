using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MirrorUnityChan : NetworkBehaviour
{
    [SerializeField] private int packagesPerSecond = 2;

    private float packageFrequency;
    private float frequencyDelay;
    private Animator anim;

    bool playAnim = false;
    float playAnimTimeStamp = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();

        packageFrequency = 1f / packagesPerSecond;
        frequencyDelay = packageFrequency;
    }

    
    void Update()
    {
        if (!isServer) return;

        frequencyDelay -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.U))
        {
            //anim.SetBool("Win", true);
            //Invoke(nameof(RegresarFalse), 0.2f);
            playAnim = true;
            playAnimTimeStamp = (float)NetworkTime.time;
            anim.SetTrigger("Win");
        }

        if(frequencyDelay <= 0f)
        {
            if (playAnim)
            {
                RPC_PlayAnim(playAnimTimeStamp);
                playAnim = false;
            }

            frequencyDelay = packageFrequency;
        }

    }

    private void RegresarFalse()
    {
        anim.SetBool("Win", false);
    }

    [ClientRpc]
    private void RPC_PlayAnim(float _t)
    {
        if (isServer) return;

        float lag = (float)NetworkTime.time - _t;

        //Para saber el porcentaje normalizado 
        float normalizedTime = lag / 3.967f;
        anim.Play("WIN00", 0, normalizedTime);
    }
}
