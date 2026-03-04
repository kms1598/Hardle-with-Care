using UnityEngine;
using System;

public class CustomWaitForSeconds : CustomYieldInstruction
{
    private float waitTime;
    private Func<bool> pauseCondition;

    public CustomWaitForSeconds(float seconds, Func<bool> pauseCondition)
    {
        waitTime = seconds;
        this.pauseCondition = pauseCondition;
    }

    public override bool keepWaiting
    {
        get
        {
            if (!pauseCondition())
            {
                waitTime -= Time.deltaTime;
            }

            return waitTime > 0;
        }
    }
}
