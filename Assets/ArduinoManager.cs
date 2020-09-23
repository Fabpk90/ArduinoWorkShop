using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class City
{
    List<int> cities;
    string name;
    public bool isConnected;

    public City(List<int> cities, string name)
    {
        this.cities = cities;
        this.name = name;

        isConnected = false;
    }

    public void Toggle()
    {
        isConnected = !isConnected;
    }
}

public class ArduinoManager : MonoBehaviour
{
    // Start is called before the first frame update

    public ArduinoTest arduinoA;
    public ArduinoTest arduinoB;

    public EventHandler OnGameStart;

    private List<City> allCities;

    private int startingIndex;
    private int endIndex;

    void Start()
    {
        OnGameStart?.Invoke(this, null);
        arduinoA.OnMsgReceived += OnMessageReceived;
        arduinoB.OnMsgReceived += OnMessageReceived;

        arduinoA.OnConnected += ChooseStartingCities;
        arduinoB.OnConnected += ChooseStartingCities;
    }

    private void ChooseStartingCities(object sender, SerialController serial)
    {
        serial.SendSerialMessage("S");
        
        //TODO: assign start and end, and send them
    }

    private void CheckForWin()
    {
        
    }

    public void OnMessageReceived(object o, string msg)
    {
        ArduinoTest arduino = o as ArduinoTest;

        if (arduino.isArduinoA)
        {
            print("A " + msg);
        }
        else
            print("B " + msg);
        
        var index = Convert.ToInt32(msg);
        allCities[index].Toggle();
        
        print(allCities[index].isConnected ? "connected" : "disconnected");
        
        CheckForWin();
    }

    private void OnDisable()
    {
        arduinoA.OnMsgReceived -= OnMessageReceived;
        arduinoB.OnMsgReceived -= OnMessageReceived;
        
        arduinoA.OnConnected -= ChooseStartingCities;
        arduinoB.OnConnected -= ChooseStartingCities;
    }
}
