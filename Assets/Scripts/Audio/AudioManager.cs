using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AK.Wwise.Event Music_Main;
    public AK.Wwise.Event Feedback_Apparition;
    public AK.Wwise.Event Feedback_FinalCity;
    public AK.Wwise.Event Feedback_Negative;
    public AK.Wwise.Event Feedback_Neutre;
    public AK.Wwise.Event Feedback_Positive;
    public AK.Wwise.Event Feedback_Start;

    [Space]
    public AK.Wwise.Event Tutorial_CityDestroyed;
    public AK.Wwise.Event VO_EndOfTrip;
    public AK.Wwise.Event VO_NewTrip;

    [Space]
    public AK.Wwise.Event[] tutorialLogs;

    public Dictionary<AK.Wwise.Event, bool> emittedLog = new Dictionary<AK.Wwise.Event, bool>();

    private bool[] logEmitted;

    private int tutoStepCounter;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance)
            Destroy(gameObject);
        else
            instance = this;

        tutoStepCounter = 0;

        logEmitted = new bool[tutorialLogs.Length];

        /*
        foreach (AK.Wwise.Event wwiseEvent in tutorialLogs)
        {
            emittedLog.Add(wwiseEvent, false);
        }
        */
    }

    public void IncLogCounter() => tutoStepCounter++;

    public void ResetLogCounter() => tutoStepCounter = 0;

    public void TriggerVoice()
    {
        if (!logEmitted[tutoStepCounter])
        {
            tutorialLogs[tutoStepCounter].Post(gameObject);
            logEmitted[tutoStepCounter] = true;
        }

        /*
        bool containValue = emittedLog.TryGetValue(tutorialLogs[tutoStepCounter], out bool hasBeenEmitted);
        if (!hasBeenEmitted && containValue)
        {
            tutorialLogs[tutoStepCounter].Post(gameObject);
            emittedLog[tutorialLogs[tutoStepCounter]] = true;
        }
        */
    }
}
