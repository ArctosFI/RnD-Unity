using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] float moveAmount;

    private void Start()
    {
        if (IsHost) { transform.position -= new Vector3(0, 0, moveAmount); }
        else { transform.position += new Vector3(0, 0, moveAmount); transform.Rotate(transform.up * 180); }
    }
}
