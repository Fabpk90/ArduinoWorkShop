using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AK.Wwise.Event PlayMainMusic;
    public AK.Wwise.Event PlayPositiveFB;
    public AK.Wwise.Event PlayNegativeFB;
    public AK.Wwise.Event PlayStartCityPlugged;
    public AK.Wwise.Event PlayEndCityPlugged;
    public AK.Wwise.Event PlayVocalLog;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance)
            Destroy(gameObject);
        else
            instance = this;
    }
}
