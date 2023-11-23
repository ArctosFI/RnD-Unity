using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class MenuUI : MonoBehaviour
{
    [SerializeField] TMP_Text ipText;
    [SerializeField] NetworkManager nm;
    [SerializeField] UnityTransport utp;
    [SerializeField] GameObject xrRig;
    string ip = "";

    private void Update()
    {
        ipText.SetText(ip);
    }

    public void Host()
    {
        nm.StartHost();
        if(xrRig !=  null) { Destroy(xrRig); }
    }

    public void Join()
    {
        utp.SetConnectionData(ip, 7777);
        nm.StartClient();
        if (xrRig != null) { Destroy(xrRig); }
    }

    public void KPInput(string input)
    {
        ip += input;
    }

    public void KPErase()
    {
        ip = ip.Remove(ip.Length - 1, 1);
    }
}
