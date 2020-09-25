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
    public AK.Wwise.Event Tutorial_CityDestroyeds;

    public AK.Wwise.Event[] tutorialLogs;

    private int tutoStepCounter;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance)
            Destroy(gameObject);
        else
            instance = this;

        tutoStepCounter = 0;
    }

    public void IncLogCounter()
    {
        tutoStepCounter++;
    }

    public void TriggerVoice()
    {
        tutorialLogs[tutoStepCounter].Post(gameObject);
    }
}
