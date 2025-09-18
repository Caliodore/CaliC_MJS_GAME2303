using Unity.VisualScripting;
using UnityEngine;

public class EnumCompareTest : MonoBehaviour
{
    private enum TestValues
    {
        State0 = 0,
        State1 = 1,
        State2 = 2,
        State3 = 3,
        State4 = 4,
    }

    [SerializeField] TestValues enumValueOne;
    [SerializeField] TestValues enumValueTwo;

    [SerializeField] bool testTrigger;
    [SerializeField] bool isEnumTestTrue;
    [SerializeField] bool isIntTestTrue;

    // Update is called once per frame
    void Update()
    {
        if(testTrigger)
        {
            int intEnumOne = (int)enumValueOne;
            int intEnumTwo = (int)enumValueTwo;
            testTrigger = false;
            Debug.Log($"The two enum values are: {enumValueOne} and {enumValueTwo}.");
            Debug.Log($"The two enum values as integers are: {intEnumOne} and {intEnumTwo}.");
            if(enumValueOne.ToString() == enumValueTwo.ToString())
            { 
                isEnumTestTrue = true;
            }
            if(intEnumOne == intEnumTwo)
            { 
                isIntTestTrue = true;
            }
        }
    }
}
