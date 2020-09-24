using UnityEngine;
using System.IO.Ports;
using System.Collections.Generic;
using System.Collections;
using System;

public enum ECityState
{
    Plugged,
    Dead,
    Available,
    Connected
}

public class ArduinoTest : MonoBehaviour 
{
    public SerialController serial;
    private List<City> _cities;
    private List<ECityState> _cityStates;

    public List<Tuple<int, int>> cables;
    private Tuple<int, int> cableInPlugs;

    public bool isArduinoA;

    public EventHandler<string> OnMsgReceived;

    public EventHandler<SerialController> OnConnected;

    public EventHandler OnWirePlugged;
    public EventHandler OnWireFriendlyPlugged;
    public EventHandler OnWireConnected;

    public EventHandler<bool> OnToggleCity;

    private void Start() 
    {
        serial = GetComponent<SerialController>();
        
        _cityStates = new List<ECityState>();
        for (int i = 0; i < 2; i++)
        {
            _cityStates.Add(ECityState.Available);
        }
        
        cables = new List<Tuple<int, int>>();

        cableInPlugs = null;

        //cities.Add(new City());
    }

    public int GetPluggedCities()
    {
        int plugged = 0;

        foreach (ECityState state in _cityStates)
        {
            if (state == ECityState.Plugged)
                plugged++;
        }

        return plugged;
    }

    public void SetCityState(int index, ECityState state)
    {
        Debug.Log("Index " + index + " is set to" + state);
        _cityStates[index] = state;

        if (state == ECityState.Plugged)
        {
            if (cableInPlugs == null)
            {
                cableInPlugs = new Tuple<int, int>(index, -1);
                OnWirePlugged?.Invoke(this , null);

                foreach (Tuple<int,int> cable in cables)
                {
                    if (cable.Item2 == index)
                    {
                        OnWireFriendlyPlugged?.Invoke(this, null);
                        return;
                    }
                }
            }
            else
            {
                int index0 = cableInPlugs.Item1;
                cableInPlugs = new Tuple<int, int>(index0, index);
                
                cables.Add(cableInPlugs);
                cableInPlugs = null;
                OnWireConnected?.Invoke(this, null);
            }
        }
    }

    public bool isCityNotDead(int index)
    {
        return _cityStates[index] != ECityState.Dead;
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

    public bool isCityPlugged(int index)
    {
        return _cityStates[index] == ECityState.Plugged;
    }

    public bool isCityAvailable(int index)
    {
        return _cityStates[index] == ECityState.Available;
    }

    public bool isCityConnected(int index)
    {
        return _cityStates[index] == ECityState.Connected;
    }
}