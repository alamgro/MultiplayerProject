using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 1f, -10f);

    private Transform playerToFollow;

    void Start()
    {
        this.enabled = false;
    }

    
    void LateUpdate()
    {
        transform.position = playerToFollow.position + offset;
    }

    public void FollowLocalPlayer()
    {
        playerToFollow = MirrorPlayer.Instance.transform;
        //playerToFollow = Mirror.NetworkClient.localPlayer.transform;
        this.enabled = true;
    }
}
