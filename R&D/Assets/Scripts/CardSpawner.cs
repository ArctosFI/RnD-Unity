using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CardSpawner : NetworkBehaviour
{
    List<int> cardIDs;
    [SerializeField] GameObject card;

    [ServerRpc]
    public void AddCardServerRPC(int id)
    {
        if (!cardIDs.Contains(id)) 
        { 
            cardIDs.Add(id);
            GameObject _card = Instantiate(card, transform.position + new Vector3(0.15f * (cardIDs.Count - 2), 0, 0), card.transform.rotation);
            _card.GetComponent<NetworkObject>().Spawn();
        }
    }
}
