using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cast : MonoBehaviour
{
    // Start is called before the first frame update
    public ConfigurableJoint configJoint;
    public SoftJointLimit softJointLimit;
    void Update()
    // For testing -- need to assign to trigger on index controller
    {
        if (Input.GetKey("="))
        {
            softJointLimit.limit = configJoint.linearLimit.limit + .5f;
            Debug.Log(softJointLimit.limit);
            configJoint.linearLimit = softJointLimit;
        }

        if (Input.GetKey("-"))
        {
            if(configJoint.linearLimit.limit > 1) {
                softJointLimit.limit = configJoint.linearLimit.limit - .5f;
                Debug.Log(softJointLimit.limit);
                configJoint.linearLimit = softJointLimit;
            } 
        }
    }
}
