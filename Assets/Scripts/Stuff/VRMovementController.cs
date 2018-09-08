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

    float _controllersAngle
    {
        get
        {
            Vector3 controllersAxis = _rightController.transform.position - _leftController.transform.position;
            controllersAxis.y = 0.0f;

            float angle = Vector3.Angle(controllersAxis, transform.right);
            return Vector3.Dot(controllersAxis, transform.forward) < 0 ? -angle : angle;
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

    float _deltaControllersAngle;
    float _lastControllersAngle;

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
        // Handle input
        _leftPressed = _leftController.isValid && _leftController.device.GetPress(SteamVR_Controller.ButtonMask.Grip);
        _rightPressed = _rightController.isValid && _rightController.device.GetPress(SteamVR_Controller.ButtonMask.Grip);
    }

    void FixedUpdate()
    {
        // Calculate deltas
        Vector3 middlePosition = _middlePosition;
        _deltaMiddlePosition = middlePosition - _lastMiddlePosition;
        _lastMiddlePosition = middlePosition;

        float controllersDistance = _controllersDistance;
        _deltaControllersDistance = controllersDistance - _lastControllersDistance;
        _lastControllersDistance = controllersDistance;

        float controllersAngle = _controllersAngle;
        _deltaControllersAngle = controllersAngle - _lastControllersAngle;
        _lastControllersAngle = controllersAngle;

        // Handle movement
        if (_leftPressed || _rightPressed)
        {
            Vector3 delta;

            if (_leftPressed && _rightPressed)
            {
                delta = _deltaMiddlePosition;

                // Rotate
                Vector3 controllersMiddle = (_leftController.transform.position + _rightController.transform.position) * 0.5f;
                Vector3 direction = transform.position - controllersMiddle;
                direction = Quaternion.Euler(0.0f, _deltaControllersAngle, 0.0f) * direction;
                transform.position = controllersMiddle + direction;
                Vector3 eulerAngles = transform.rotation.eulerAngles;
                eulerAngles.y += _deltaControllersAngle;
                transform.rotation = Quaternion.Euler(eulerAngles);

                // Relative scale
                float scale = Mathf.Clamp(transform.localScale.x - _deltaControllersDistance * speed * 10.0f, scaleRange.x, scaleRange.y);
                float relativeScale = scale / transform.localScale.x;

                transform.localScale = Vector3.one * scale;
                transform.position = controllersMiddle + (transform.position - controllersMiddle) * relativeScale;
            }
            else if (_leftPressed)
            {
                delta = _leftController.deltaPosition;
            }
            else
            {
                delta = _rightController.deltaPosition;
            }

            transform.position -= transform.rotation * (delta * speed * transform.localScale.x);
        }
    }
}
