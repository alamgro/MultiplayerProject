using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct MirrorNetworkMessage : NetworkMessage 
{
    public int numInt;
    public float numFloat;
    public Color color;
}
