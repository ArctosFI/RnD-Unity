using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Net;
using System.Linq;
using TMPro;

public class ShowIP : NetworkBehaviour
{
    [SerializeField] TMP_Text ipText;

    void Start()
    {
        if (!IsHost) { this.gameObject.SetActive(false); }
        else
        {
            IPAddress[] localIPS = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            string hostIP = localIPS.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();
            ipText.SetText(hostIP);
        }
    }
}
