using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class VRMovementController : MonoBehaviour
{
    Vector3 _middlePosition
    {
        get
        {
            return (_leftController.transform.localPosition + _rightController.transform.localPosition) * 0.5f;
        }
    }

    float _controllersDistance
    {
        get
        {
            return (_leftController.transform.localPosition - _rightController.transform.localPosition).magnitude;
        }
    }

    public float speed = 1.0f;
    public Vector2 scaleRange = new Vector2(1.0f, 20.0f);

    SteamVR_Camera _headCamera;

    HandController _leftController;
    HandController _rightController;
    
    Vector3 _deltaMiddlePosition;
    Vector3 _lastMiddlePosition;

    float _deltaControllersDistance;
    float _lastControllersDistance;

    bool _leftPressed;
    bool _rightPressed;

    void Start()
    {
        _headCamera = FindObjectOfType<SteamVR_Camera>();
        Assert.IsNotNull(_headCamera, "There is not head assigned");

        SteamVR_ControllerManager controllerManager = GetComponent<SteamVR_ControllerManager>();
        Assert.IsNotNull(controllerManager, "There is no SteamVR_ControllerManager assigned");

        _leftController = controllerManager.left.GetComponent<HandController>();
        Assert.IsNotNull(controllerManager, "There is no left hand controller assigned");

        _rightController = controllerManager.right.GetComponent<HandController>();
        Assert.IsNotNull(controllerManager, "There is no right hand controller assigned");
    }
    
	void Update ()
    {
        _leftPressed = _leftController.isValid && _leftController.device.GetPress(SteamVR_Controller.ButtonMask.Grip);
        _rightPressed = _rightController.isValid && _rightController.device.GetPress(SteamVR_Controller.ButtonMask.Grip);
    }

    void FixedUpdate()
    {
        _deltaMiddlePosition = _middlePosition - _lastMiddlePosition;
        _lastMiddlePosition = _middlePosition;

        _deltaControllersDistance = _controllersDistance - _lastControllersDistance;
        _lastControllersDistance = _controllersDistance;

        if (_leftPressed || _rightPressed)
        {
            Vector3 delta;

            if (_leftPressed && _rightPressed)
            {
                Vector3 A = transform.position;
                Vector3 B = _middlePosition;

                float scale = Mathf.Clamp(transform.localScale.x - _deltaControllersDistance * speed * 10.0f, scaleRange.x, scaleRange.y);
                float relativeScale = scale / transform.localScale.x;

                transform.localScale = Vector3.one * scale;
                transform.position = B + (A - B) * relativeScale;

                delta = _deltaMiddlePosition;
            }
            else if (_leftPressed)
            {
                delta = _leftController.deltaPosition;
            }
            else
            {
                delta = _rightController.deltaPosition;
            }

            transform.position -= delta * speed * transform.localScale.x;
        }
    }
}
