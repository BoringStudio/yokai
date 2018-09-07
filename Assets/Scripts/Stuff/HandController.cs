using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class HandController : MonoBehaviour
{
    public bool isValid
    {
        get
        {
            return _trackedObject && _trackedObject.isValid;
        }
    }

    public SteamVR_Controller.Device device
    {
        get
        {
            return SteamVR_Controller.Input((int)_trackedObject.index);
        }
    }

    public Vector3 deltaPosition { get; private set; }
    Vector3 _lastPosition;

    SteamVR_TrackedObject _trackedObject;

    void Start ()
    {
        _trackedObject = GetComponent<SteamVR_TrackedObject>();
    }

    void FixedUpdate()
    {
        deltaPosition = transform.localPosition - _lastPosition;
        _lastPosition = transform.localPosition;
    }
}
