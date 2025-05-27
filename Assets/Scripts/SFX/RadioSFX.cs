using System;
using UnityEngine;
using FMODUnity;
using FMOD;
using FMOD.Studio;

public class RadioSFX : MonoBehaviour
{
    [SerializeField]
    private string radioEventPath = "event:/Radio_Announcement";

    private EventInstance currentEvent;

    public void PlayRadioLines()
    {
        currentEvent = RuntimeManager.CreateInstance(radioEventPath);
        currentEvent.setCallback(OnEventStopped, EVENT_CALLBACK_TYPE.STOPPED);
        currentEvent.setParameterByName("Radio", 0, false);
        var res = currentEvent.start();
    }

    private RESULT OnEventStopped(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameters)
    {
        if (type == EVENT_CALLBACK_TYPE.STOPPED)
        {
            currentEvent = RuntimeManager.CreateInstance(radioEventPath);
            currentEvent.setParameterByName("Radio", 1, false);
            var res = currentEvent.start();
            currentEvent.release();
        }
        return RESULT.OK;
    }
}

