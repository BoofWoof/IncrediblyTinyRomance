using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MessagingVariables
{
    public static float DefaultTimeBetweenMessages = 1.2f;
    public static float DefaultTimePerCharacter = 0.01f;
    public static bool DefaultForceSelect = false;

    public static float TimeBetweenMessages = DefaultTimeBetweenMessages;
    public static float TimePerCharacter = DefaultTimePerCharacter;
    public static bool ForceSelect = DefaultForceSelect;

    public static void InstantMessaging()
    {
        TimeBetweenMessages = 0f;
        TimePerCharacter = 0f;
        ForceSelect = true;
    }
    public static void SemiInstantMessaging()
    {
        TimeBetweenMessages = 0f;
        TimePerCharacter = 0f;
        ForceSelect = false;
    }

    public static void DefaultMessaging()
    {
        TimeBetweenMessages = DefaultTimeBetweenMessages;
        TimePerCharacter = DefaultTimePerCharacter;
        DefaultForceSelect = ForceSelect;
    }
}
