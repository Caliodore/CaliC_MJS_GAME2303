using System.Collections;
using UnityEngine;

public class GuardStateChange_3 : MonoBehaviour
{
    [SerializeField] GuardBrain_3 attachedBrain;

    public GuardState activeGuardState;
    private GuardState changeToState;
    private GuardState previousGuardState;

    private int activeStateInt;
    private int previousStateInt;

    private void Start()
    {
        activeGuardState = GuardState.ResumePatrol;
        changeToState = GuardState.ActivePatrol;
        attachedBrain = gameObject.GetComponent<GuardBrain_3>();
        ChangeState(changeToState);
    }

    public void InitializeStateValues(GuardState ags)
    { 
        activeGuardState = ags;
    }

    public void ChangeState(GuardState stateToChangeTo)         //Method to handle changing the state of the guard, and sharing that change across all other components through the brain.
    {
        if(activeGuardState != stateToChangeTo)
        { 
            StateChangedLogicHandler(stateToChangeTo);
            attachedBrain.OnStateChange(activeGuardState);
        }
    }

    /*private void StateUpdater(GuardState stateToChangeTo)
    { 
        switch (stateToChangeTo)
        { 
            case (GuardState.ResumePatrol):
                activeGuardState = GuardState.ResumePatrol;
                break;

            case (GuardState.ActivePatrol):
                activeGuardState = GuardState.ActivePatrol;
                break;

            case (GuardState.Waiting):
                activeGuardState = GuardState.Waiting;
                break;

            case (GuardState.Investigating):
                activeGuardState = GuardState.Investigating;
                break;

            case (GuardState.Pursuing):
                activeGuardState = GuardState.Pursuing;
                break;
        }    
    }*/
    
    /// <summary>
    /// Handles what to do with movement in the current moment when swapping from previousState to the inputState (stateChangingTo).
    /// Pursuing has highest priority and overwrites all others. Investigating overwrites everything other than pursuing.
    /// The other 3 (RP, AP, and Waiting) do not overwrite and instead wait for certain timers after other states to swap.
    /// </summary>
    /// <param name="stateChangingTo"></param>
    private void StateChangedLogicHandler(GuardState stateChangingTo)
    {
        int convertedEnum = (int)stateChangingTo;
        int enumPriorityTest = convertedEnum - 3;

        if(enumPriorityTest >= 0)     //If the value of enumPriorityTest is negative then it cannot be Investigating or Pursuing. 
        {                             //To access this method as well the previous state cannot be the same as the one being changed to.
            switch(previousStateInt)  //Therefore, if we are at this line the previousGuardState has one less option (being Investigating or Pursuing removed).
            {
                case(4):    //---------This means the previousState was Pursuing, which has ultimate priority. It will only be overwritten by time passing as according to coroutines.     
                    Debug.Log("Previous state was pursuing, which cannot be overwritten by investigating.");
                    break;

                case(3):    //---------The previousState was Investigating, which can be overwritten by Pursuing.
                    if(convertedEnum == 4)
                    {
                        previousGuardState = activeGuardState;
                        Debug.Log("The new state is Pursuing, overwriting Investigating.");
                        activeGuardState = stateChangingTo;
                    }
                    else
                        Debug.Log("This line shouldn't be accessible. Investigating cannot overwrite Investigating.");
                    break;

                case(2):    //----------All of these can be overwritten by either Investigating or Pursuing.
                case(1):    //----------We don't define the individual cases to leave it to fall-through.
                case(0):
                    if(convertedEnum == 4)
                    {
                        previousGuardState = activeGuardState;
                        Debug.Log($"The new state is Pursuing, overwriting {previousGuardState}.");
                        activeGuardState = stateChangingTo;
                    }
                    else
                    {
                        previousGuardState = activeGuardState;
                        Debug.Log($"The new state is Investigating, overwriting {previousGuardState}.");
                        activeGuardState = stateChangingTo;
                    }
                    break;
            }
        }
        else                //----------If stateChangingTo =/= a priority state. So returning to patrol states mainly.
        { 
            switch(previousStateInt)  //ChangingTo choices: ResumePatrol, ActivePatrol, Waiting
            {
                case(4):    //----------To transition from Pursuing/Investigating to the other states it must go through ResumePatrol.
                case(3):
                    {
                        previousGuardState = activeGuardState;
                        Debug.Log($"The new state is Resume Patrol, overwriting {previousGuardState}.");
                        activeGuardState = stateChangingTo;
                    }
                    break;
                case(2):    //----------Previous state = Waiting, so a timer has finished and guard is on patrol as usual.
                    {
                        previousGuardState = activeGuardState;
                        Debug.Log($"The new state is Active Patrol, overwriting {previousGuardState}.");
                        activeGuardState = stateChangingTo;
                    }    
                    break;
                case(1):    //----------Previous state = ActivePatrol, which can only transition to Waiting out of the choices.
                case(0):    //----------Previous state = Resume patrol, which can only transition to Waiting out of the choices.
                    {
                        previousGuardState = activeGuardState;
                        Debug.Log($"The new state is Waiting, overwriting {previousGuardState}.");
                        activeGuardState = stateChangingTo;
                    }
                    break;
            }
        }
    }

    IEnumerator TimerFromWaitingToActivePatrol()
    { 
        yield return null;    
    }
}
