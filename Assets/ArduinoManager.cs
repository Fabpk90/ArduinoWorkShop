using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct City
{
    List<int> cities;
    string name;

    public City(List<int> cities, string name)
    {
        this.cities = cities;
        this.name = name;
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
        //serial.SendSerialMessage("S");
        
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

        if (arduino.isCityNotDead(index))
        {
            if (arduino.isCityPlugged(index))
            {
                //TODO: see if the city really available
                arduino.SetCityState(index, ECityState.Available);
            }
            else if (arduino.isCityAvailable(index))
            {
                //TODO: see if the city is connected
                arduino.SetCityState(index, ECityState.Plugged);
                
                var pluggedCities = arduino.GetPluggedCities();
                if ((pluggedCities & 1) == 0) //odd
                {
                    print("CONNECTED");
                    //check if good city
                    //if good connect
                }
            }
            else if (arduino.isCityConnected(index))
            {
                arduino.SetCityState(index, ECityState.Available);
                //TODO: see how to know to which is connected (list of pairs ?)
                //available for this city and plugged for the other not connected
            }

            CheckForWin();
        }
        else
        {
            //BEEP !
        }

        
    }

    private void OnDisable()
    {
        arduinoA.OnMsgReceived -= OnMessageReceived;
        arduinoB.OnMsgReceived -= OnMessageReceived;
        
        arduinoA.OnConnected -= ChooseStartingCities;
        arduinoB.OnConnected -= ChooseStartingCities;
    }
}
