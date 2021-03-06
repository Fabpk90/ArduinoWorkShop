﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public struct City
{
    public List<int> cities;
    string name;
    public int ledIndex;

    public ECityState state;

    public City(List<int> cities, string name, int ledIndex)
    {
        this.cities = cities;
        this.name = name;
        this.ledIndex = ledIndex;
        state = ECityState.Dead;
    }
}

public class ArduinoManager : MonoBehaviour
{
    // Start is called before the first frame update

    public ArduinoTest arduinoA;
    public ArduinoTest arduinoB;

    public EventHandler OnGameStart;
    public EventHandler OnGameWin;

    private List<City> allCities;

    public int startingIndex;
    public int endIndex;

    private int[,] adjacencyMatrix;
    private int[] distances;

    public int numOfCables;

    public int toSend;

    void Start()
    {
        OnGameStart?.Invoke(this, null);

        OnGameWin += (sender, args) =>
        {
            print("T'as gagné, t'es trop fort !");
        };
        
        allCities = new List<City>();

        allCities.Add(new City(new List<int>() {12, 15, 1, 2, 13, 14}, "City_0", 0));
        allCities.Add(new City(new List<int>() {0, 3, 2, 4}, "City_1", 1));
        allCities.Add(new City(new List<int>() {6, 0, 3, 1, 4}, "City_2", 2));
        allCities.Add(new City(new List<int>() {1, 2, 4}, "City_3", 3));
        allCities.Add(new City(new List<int>() {5, 6, 3, 1, 2}, "City_4", 4));
        allCities.Add(new City(new List<int>() {6, 7, 4}, "City_5", 5));
        allCities.Add(new City(new List<int>() {12, 5, 2, 7, 4}, "City_6", 6));
        allCities.Add(new City(new List<int>() {12, 5, 6, 8, 10}, "City_7", 7));
        allCities.Add(new City(new List<int>() {12, 9, 19, 7, 10}, "City_8", 8));
        allCities.Add(new City(new List<int>() {12, 8, 19, 20, 11, 10}, "City_9", 9));
        allCities.Add(new City(new List<int>() {12, 9, 8, 19, 7, 13, 14, 11}, "City_10", 10));
        allCities.Add(new City(new List<int>() {12, 9, 15, 19, 16, 13, 14, 10}, "City_11", 11));
        allCities.Add(new City(new List<int>() {9,6,0,15,8,7,13,14,11,10}, "City_12", 12));
        allCities.Add(new City(new List<int>() {12, 0, 15, 19, 16, 14, 11, 10}, "City_13", 13));
        allCities.Add(new City(new List<int>() {12, 0, 15, 16, 13, 11, 10}, "City_14", 14));
        allCities.Add(new City(new List<int>() {12, 0, 16, 13, 14, 11}, "City_15", 15));
        allCities.Add(new City(new List<int>() {15, 19, 18, 17, 13, 14, 11}, "City_16", 16));
        allCities.Add(new City(new List<int>() {19, 16, 18}, "City_17", 17));
        allCities.Add(new City(new List<int>() {19, 21, 16, 17, 20}, "City_18", 18));
        allCities.Add(new City(new List<int>() {9, 8, 16, 18, 17, 13, 20, 11, 10}, "City_19", 19));
        allCities.Add(new City(new List<int>() {9, 19, 21, 18}, "City_20", 20));
        allCities.Add(new City(new List<int>() {18, 20}, "City_21", 21));
        
        arduinoA.InitCities(allCities.ToArray());
        arduinoB.InitCities(allCities.ToArray());
        
        arduinoA.OnMsgReceived += OnMessageReceived;
        arduinoB.OnMsgReceived += OnMessageReceived;

        arduinoA.OnWireConnected += (sender, tuple) =>
        {
            print("Wire connected for A " +tuple);
            
            arduinoA.serial.SendSerialMessage("C" + tuple.Item1);
            arduinoA.serial.SendSerialMessage("C" + tuple.Item2);
            
            var city = allCities[tuple.Item1];
            city.state = ECityState.Connected;
            allCities[tuple.Item1] = city;
            
            city = allCities[tuple.Item2];
            city.state = ECityState.Connected;
            allCities[tuple.Item2] = city;
            
            CheckForWin();
            
            arduinoB._cityStates[tuple.Item2] = ECityState.Available;
        };
        
        arduinoA.OnWireFriendlyPlugged += (sender, i) =>
        {
            print("Player 1 Connected the player 2 city");
            arduinoA.serial.SendSerialMessage("P" + i);
        };

        arduinoA.OnWirePlugged += (sender, i) =>
        {
            print("Wire plugged for arduino A " + i);
            print("Neighboors " +allCities[i].cities.Count);
            arduinoA.serial.SendSerialMessage("P" + i);
            foreach (int city in allCities[i].cities)
            {
                //not already connected
                if (allCities[city].state != ECityState.Available
                && city != i)
                {
                    print("Sending " + city);
                    arduinoA.serial.SendSerialMessage("A" + city);
                    arduinoA._cityStates[city] = ECityState.Available;
                }
            }
        };

        arduinoA.OnWireDisconnected += (sender, i) =>
        {
            print("disconnected " + i);

            foreach (int city in allCities[i].cities)
            {
                print("City state " + arduinoA._cityStates[city] +" " + city);
                if (arduinoA._cityStates[city] == ECityState.Available 
                    && city != startingIndex && city != endIndex)
                {
                    print("City shuting down " + city);
                    arduinoA._cityStates[city] = ECityState.Dead;
                    arduinoA.serial.SendSerialMessage("D"+city);
                }
            }
        };
        
        arduinoB.OnWireFriendlyPlugged += (sender, i) =>
        {
            print("Player 2 Connected the player 1 city");
            arduinoB.serial.SendSerialMessage("P" + i);
        };
        
        arduinoB.OnWireConnected += (sender, tuple) =>
        {
            print("Wire connected for B " +tuple);
            arduinoB.serial.SendSerialMessage("C" + tuple.Item1);
            arduinoB.serial.SendSerialMessage("C" + tuple.Item2);

            var city = allCities[tuple.Item1];
            city.state = ECityState.Connected;
            allCities[tuple.Item1] = city;
            
            city = allCities[tuple.Item2];
            city.state = ECityState.Connected;
            allCities[tuple.Item2] = city;
            
            CheckForWin();

            arduinoA._cityStates[tuple.Item2] = ECityState.Available;
        };
        
        arduinoB.OnWirePlugged += (sender, i) =>
        {
            print("Wire plugged for arduino B " + i);
            print("Neighboors " +allCities[i].cities.Count);
            arduinoB.serial.SendSerialMessage("P" + i);
            foreach (int city in allCities[i].cities)
            {
                //not already connected
                if (arduinoB._cityStates[city] == ECityState.Available 
                    && city != startingIndex && city != endIndex)
                {
                    print("City shuting down " + city);
                    arduinoB._cityStates[city] = ECityState.Dead;
                    arduinoB.serial.SendSerialMessage("D"+city);
                }
            }
        };
        
        arduinoB.OnWireDisconnected += (sender, i) =>
        {
            print("disconnected " + i);
            
            foreach (int city in allCities[i].cities)
            {
                if (arduinoB._cities[city].state == ECityState.Available
                    && (city != startingIndex && city != endIndex))
                {
                    var aCity = arduinoB._cities[city];
                    aCity.state = ECityState.Dead;
                    arduinoB._cities[city] = aCity;
                    arduinoB.serial.SendSerialMessage("D"+city);
                }
            }
        };
        
        adjacencyMatrix = new int[allCities.Count, allCities.Count];
        ComputeAdjacency();
        ChooseRandomCities();

        arduinoA.OnConnected += (sender, controller) =>
        {
            controller.SendSerialMessage("S");
            controller.SendSerialMessage("Z" + startingIndex);
            controller.SendSerialMessage("Z" + endIndex);

            arduinoA._cityStates[startingIndex] = ECityState.Available;
            arduinoA._cityStates[endIndex] = ECityState.Available;
        };

        arduinoB.OnConnected += (sender, controller) =>
        {
            controller.SendSerialMessage("S");
            
            arduinoB._cityStates[startingIndex] = ECityState.Available;
            arduinoB._cityStates[endIndex] = ECityState.Available;
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnMessageReceived(arduinoA, ""+startingIndex);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            OnMessageReceived(arduinoA, ""+toSend);
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            OnMessageReceived(arduinoB, ""+toSend);
        }
    }

    private void ChooseRandomCities()
    {
        int distance = 0;
        bool isPathPossible = false;
        do
        {
            startingIndex = (int) (Random.value * allCities.Count);
            endIndex = (int) (Random.value * allCities.Count);
            distance = ComputeDistance(startingIndex, endIndex);

            isPathPossible = distances[endIndex] != int.MaxValue;

        } while (startingIndex == endIndex || distance > numOfCables || distance < 3 || !isPathPossible);

        var city = allCities[startingIndex];
        city.state = ECityState.Available;
        allCities[startingIndex] = city;

        city = allCities[endIndex];
        city.state = ECityState.Available;
        allCities[endIndex] = city;
    }

    private int ComputeDistance(int startIndex, int endIndex)
    {
        distances = Dijkstra.DijkstraAlgo(adjacencyMatrix, startIndex, allCities.Count);
        
        return distances[endIndex];
    }

    private void ChooseStartingCities(object sender, SerialController serial)
    {
        serial.SendSerialMessage("S");
        
        ChooseRandomCities();

        arduinoA.serial.SendSerialMessage("Z" + startingIndex);
        arduinoA.serial.SendSerialMessage("Z" + endIndex);
        
        arduinoB.serial.SendSerialMessage("Z" + startingIndex);
        arduinoB.serial.SendSerialMessage("Z" + endIndex);
    }

    private void CheckForWin()
    {
        if(allCities[endIndex].state == ECityState.Connected)
        {
            OnGameWin?.Invoke(this, null);
        }
    }

    public ArduinoTest GetOtherArduino(ArduinoTest a)
    {
        return a == arduinoA ? arduinoB : arduinoA;
    }

    public void OnMessageReceived(object o, string msg)
    {
        ArduinoTest arduino = o as ArduinoTest;
        var otherArduino = GetOtherArduino(arduino);

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
                arduino.OnWireDisconnected?.Invoke(this, index);
            }
            else if (arduino.isCityAvailable(index))
            {
                //connected city
                if (otherArduino.cables.Count > 0 
                    && otherArduino.cables[otherArduino.cables.Count - 1].Item2 == index)
                {
                    //TODO: positive feedback sound
                    arduino.OnWireFriendlyPlugged?.Invoke(this, index);
                    arduino._cityStates[index] = ECityState.Plugged;
                    arduino.cableInPlugs = new Tuple<int, int>(index, -1);
                }
                else
                {
                    //disconnect previously on leds
                    foreach (int i in allCities[index].cities)
                    {
                        if (allCities[i].state != ECityState.Available)
                            arduino.serial.SendSerialMessage("D" + i);
                    }
                    arduino.SetCityState(index, ECityState.Plugged);

                    var city = allCities[index];
                    city.state = ECityState.Available;// for the other player
                    allCities[index] = city;
                }
            }
            else if (arduino.isCityConnected(index))
            {
                arduino.SetCityState(index, ECityState.Available);
                arduino.OnWireDisconnected?.Invoke(this, index);
                
                Tuple<int, int> c = null;
                
                foreach (Tuple<int,int> cable in arduino.cables)
                {
                    if (cable.Item1 == index)
                    {
                        //chiant
                        c = cable;

                        arduino.serial.SendSerialMessage("D" + cable.Item2);
                    }
                    else if (cable.Item2 == index)
                    {
                        foreach (var city in allCities[index].cities)
                        {
                            arduino.serial.SendSerialMessage("A" + city);
                        }
                    }
                }

                arduino.cables.Remove(c);
                //available for this city and plugged for the other not connected
            }
        }
        else
        {
            //BEEP !
            print("bad feedback");
        }
    }

    public void ComputeAdjacency()
    {
        for (int i = 0; i < allCities.Count; i++)
        {
            for (int j = 0; j < allCities.Count; j++)
            {
                adjacencyMatrix[i, j] = allCities[i].cities.Contains(j) ? 1 : 0;
            }
        }
    }

    private void OnDisable()
    {
        //not working cause the threads are stopped during disable
        arduinoA.serial.SendSerialMessage("R");
        arduinoB.serial.SendSerialMessage("R");
        
        arduinoA.OnMsgReceived -= OnMessageReceived;
        arduinoB.OnMsgReceived -= OnMessageReceived;
        
    }
}
