using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoManager : MonoBehaviour
{
    // Start is called before the first frame update

    public ArduinoTest arduinoA;
    public ArduinoTest arduinoB;

    void Start()
    {
        arduinoA.OnMsgReceived += OnMessageReceived;
        arduinoB.OnMsgReceived += OnMessageReceived;
    }

    public void OnMessageReceived(object o, string msg)
    {
        ArduinoTest arduino = o as ArduinoTest;

        if (arduino.isArduinoA)
            print("A");
        else
            print("B");
    }

    private void OnDisable()
    {
        arduinoA.OnMsgReceived -= OnMessageReceived;
        arduinoB.OnMsgReceived -= OnMessageReceived;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
