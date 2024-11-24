using DS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedMessagesScript : MonoBehaviour
{
    public DSDialogue ImmediateDialogue;
    // Start is called before the first frame update
    void Start()
    {
        ImmediateDialogue.SubmitDialogue();
    }
}
