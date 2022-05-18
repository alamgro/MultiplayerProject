using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MirrorTransform : NetworkBehaviour
{
    [System.NonSerialized]
    public Vector3 direction;

    [SerializeField] private int packagesPerSecond = 20;

    private float packageFrequency; //La frencuencia de envío de paquetes por segundo
    private float packageDelay;
    private Vector3 nextPositionPrediction;
    private float distanceToTargetPosition;


    void Start()
    {
        packageFrequency = 1f / packagesPerSecond;
        packageDelay = packageFrequency;
    }

    
    void Update()
    {
        if (!isLocalPlayer)
        {
            //Opción A) Traslación donde vemos el pasado del player (siempre es suave, pero está en el pasado).
            //transform.position = Vector3.MoveTowards(transform.position, nextPositionPrediction, distanceToTargetPosition * packagesPerSecond * Time.deltaTime);

            //Opción B) Predicción al futuro del movimiento
            transform.Translate(direction * Time.deltaTime);
            return;
        }

        packageDelay -= Time.deltaTime;
        if(packageDelay <= 0f)
        {
            CMD_UpdatePosition(transform.position, direction, (float) NetworkTime.time); 
            packageDelay = packageFrequency;
        }
    }

    [Command]
    private void CMD_UpdatePosition(Vector3 _position, Vector3 _direction, float _t)
    {
        RPC_UpdatePosition(_position, _direction, _t);
    }

    [ClientRpc]
    void RPC_UpdatePosition(Vector3 _position, Vector3 _direction, float _t)
    {
        if (!isLocalPlayer) //No actualizar si es mi propio objeto
        {
            float lag = (float)NetworkTime.time - _t;

            //Opción A) Traslación donde vemos el pasado del player (siempre es suave, pero está en el pasado).
            //nextPositionPrediction = _position + direction * lag;
            //distanceToTargetPosition = Vector3.Distance(transform.position, _position);

            //Opción B) Predicción al futuro del movimiento
            transform.position = _position + direction * lag;
            direction = _direction;

            if(Vector3.Distance(transform.position, _position) > 2f)
            {
                transform.position = _position;
            }

        }
    }
}
