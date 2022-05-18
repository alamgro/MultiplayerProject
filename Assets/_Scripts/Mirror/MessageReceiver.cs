using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MessageReceiver : MonoBehaviour
{
    private void ReceiveMessage(MirrorNetworkMessage _msg)
    {
        Debug.Log("I recived a message of type MirrorNetworkManager", gameObject);
        print(_msg.numInt);
        print(_msg.numFloat);
        print(_msg.color);
    }

    void Start()
    {
        //Registro que si recibe un mensaje del tipo 'MirrorNetworkManager', esta función lo procesa
        NetworkClient.RegisterHandler<MirrorNetworkMessage>(ReceiveMessage);
    }

    
    void Update()
    {
        if (NetworkServer.active)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MirrorNetworkMessage msg = new MirrorNetworkMessage()
                {
                    numInt = 10,
                    numFloat = 123.4f,
                    color = Random.ColorHSV()
                };

                NetworkServer.SendToAll(msg);

            }
        }
    }
}
