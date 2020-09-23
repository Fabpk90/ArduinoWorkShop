using UnityEngine;
using System.IO.Ports;
using System.Collections.Generic;
using System.Collections;
using System;

public class ArduinoTest : MonoBehaviour 
{
    public SerialController serial;
    private List<City> cities;

    public bool isArduinoA;

    public EventHandler<string> OnMsgReceived;

    public EventHandler<SerialController> OnConnected;

    public EventHandler<bool> OnToggleCity;

    private void Start() 
    {
        serial = GetComponent<SerialController>();

        //cities.Add(new City());
    }

    void OnMessageArrived(string msg)
    {
        OnMsgReceived?.Invoke(this, msg);
    }
    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        if(success)
            OnConnected?.Invoke(this, serial);
        Debug.Log(success ? "Device connected" : "Device disconnected");
    }

}