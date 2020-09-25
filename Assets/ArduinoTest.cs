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
    public List<City> _cities;
    public List<ECityState> _cityStates;

    public List<Tuple<int, int>> cables;
    public Tuple<int, int> cableInPlugs;

    public bool isArduinoA;

    public EventHandler<string> OnMsgReceived;

    public EventHandler<SerialController> OnConnected;

    public EventHandler<int> OnWirePlugged;
    public EventHandler<int> OnWireFriendlyPlugged;
    public EventHandler<Tuple<int, int>> OnWireConnected;

    public EventHandler<int> OnWireDisconnected;

    private void Start() 
    {
        serial = GetComponent<SerialController>();

        cables = new List<Tuple<int, int>>();

        cableInPlugs = null;
    }

    public void InitCities(City[] cities)
    {
        _cities = new List<City>(cities.Length);
        _cities.AddRange(cities);

        _cityStates = new List<ECityState>(cities.Length);
        
        for (int i = 0; i < cities.Length; i++)
        {
            _cityStates.Add(ECityState.Dead);
        }
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
        if (ECityState.Plugged == _cityStates[index] && state == ECityState.Available)
        {
            cableInPlugs = null;
            
            _cityStates[index] = ECityState.Available;
            Debug.Log("UnPlugged");
        }
        else
        {
            _cityStates[index] = state;

            if (state == ECityState.Plugged)
            {
                if (cableInPlugs == null)
                {
                    cableInPlugs = new Tuple<int, int>(index, -1);
                    OnWirePlugged?.Invoke(this , index);
                }
                else
                {
                    Debug.Log("Index " + index + " is set to" + ECityState.Connected);
                    int index0 = cableInPlugs.Item1;
                    cableInPlugs = new Tuple<int, int>(index0, index);
                
                    _cityStates[index] = ECityState.Connected;
                    _cityStates[index0] = ECityState.Connected;
                
                    cables.Add(cableInPlugs);
                    OnWireConnected?.Invoke(this, cableInPlugs);
                    cableInPlugs = null;
                }
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

    public int GetCableAmount()
    {
        int count = cables.Count;

        if (cableInPlugs != null)
            return count + 1;

        return count;
    }
}