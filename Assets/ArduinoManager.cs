using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

struct City
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

    private int startingIndex;
    private int endIndex;

    private int[,] adjacencyMatrix;

    public int numOfCables;

    void Start()
    {
        OnGameStart?.Invoke(this, null);
        
        allCities = new List<City>();

        allCities.Add(new City(new List<int>() { 1 }, "CityA", 0));
        allCities.Add(new City(new List<int>() { 0 }, "CityB", 0));
        allCities.Add(new City(new List<int>() { 2 }, "CityC", 0));
        
        arduinoA.OnMsgReceived += OnMessageReceived;
        arduinoB.OnMsgReceived += OnMessageReceived;

        arduinoA.OnConnected += ChooseStartingCities;
        arduinoB.OnConnected += ChooseStartingCities;

        arduinoA.OnWireConnected += (sender, tuple) =>
        {
            arduinoA.serial.SendSerialMessage("D" + tuple.Item1);
            arduinoA.serial.SendSerialMessage("D" + tuple.Item2);
        };
        
        arduinoA.OnWireFriendlyPlugged += (sender, i) =>
        {
            arduinoA.serial.SendSerialMessage("P" + i);
        };

        arduinoA.OnWirePlugged += (sender, i) =>
        {
            arduinoA.indexPlugged = i;
            arduinoA.serial.SendSerialMessage("P" + i);
            foreach (int city in allCities[i].cities)
            {
                if (allCities[city].state == ECityState.Available)
                {
                    arduinoA.serial.SendSerialMessage("A" + i);
                }
            }
        };

        arduinoA.OnWireDisconnected += (sender, i) =>
        {
            print("disconnected " + i);
        };
        
        arduinoB.OnWireFriendlyPlugged += (sender, i) =>
        {
            arduinoB.serial.SendSerialMessage("P" + i);
        };
        
        arduinoB.OnWireConnected += (sender, tuple) =>
        {
            arduinoB.serial.SendSerialMessage("D" + tuple.Item1);
            arduinoB.serial.SendSerialMessage("D" + tuple.Item2);
        };
        
        arduinoB.OnWirePlugged += (sender, i) =>
        {
            arduinoB.serial.SendSerialMessage("P" + i);
            
            //activate all adjacent cities

            foreach (int city in allCities[i].cities)
            {
                if (allCities[city].state == ECityState.Available)
                {
                    arduinoA.serial.SendSerialMessage("A" + i);
                }
            }
        };
        
        arduinoB.OnWireDisconnected += (sender, i) =>
        {
            print("disconnected " + i);
        };
        
        adjacencyMatrix = new int[allCities.Count, allCities.Count];
        ComputeAdjacency();
    }

    private void ChooseRandomCities()
    {
        do
        {
            startingIndex = (int) (Random.value * allCities.Count);
            endIndex = (int) (Random.value * allCities.Count);
        } while (startingIndex == endIndex && ComputeDistance(startingIndex, endIndex) > numOfCables);

        var city = allCities[startingIndex];
        city.state = ECityState.Available;
        allCities[startingIndex] = city;

        city = allCities[endIndex];
        city.state = ECityState.Available;
        allCities[endIndex] = city;
    }

    private int ComputeDistance(int startIndex, int endIndex)
    {
        var distances = Dijkstra.DijkstraAlgo(adjacencyMatrix, startIndex, allCities.Count);
        
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
            print("T'as gagné, t'es trop fort !");
            OnGameWin?.Invoke(this, null);
        }
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

                arduino.OnWireDisconnected?.Invoke(this, index);
            }
            else if (arduino.isCityAvailable(index))
            {
                //TODO: see if the city is connected
                foreach (int i in allCities[index].cities)
                {
                    if (allCities[i].state != ECityState.Available)
                        arduino.serial.SendSerialMessage("D" + i);
                }
                arduino.SetCityState(index, ECityState.Plugged);

                //TODO: send signal to all available cities, to shut them off
                
                var city = allCities[index];
                city.state = ECityState.Available;// for the other player
                allCities[index] = city;
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

            CheckForWin();
        }
        else
        {
            //BEEP !
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
        
        arduinoA.OnConnected -= ChooseStartingCities;
        arduinoB.OnConnected -= ChooseStartingCities;
    }
}
