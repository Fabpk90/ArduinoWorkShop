using UnityEngine;
using System.IO.Ports;
using System.Collections.Generic;
using System.Collections;
using System;

class City
{
    List<int> cities;
    string name;
    bool isConected;

    public City(List<int> cities, string name)
    {
        this.cities = cities;
        this.name = name;

        isConected = false;
    }

}
public class ArduinoTest : MonoBehaviour 
{
    private SerialController serial;
    private List<City> cities;

    public bool isArduinoA;

    private bool arduinoConnected;

    public EventHandler<string> OnMsgReceived;

    private void Start() 
    {
        serial = GetComponent<SerialController>();

        //cities.Add(new City());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            serial.SendSerialMessage("salut");
        }
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
        Debug.Log(success ? "Device connected" : "Device disconnected");
    }

}