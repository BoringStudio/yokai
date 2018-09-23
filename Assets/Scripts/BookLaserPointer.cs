using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class BookLaserPointer : MonoBehaviour
{
    public Text testLabel;

    public bool cursorInside { get; private set; }
    public Vector2 cursorPosition { get; private set; }

    LineRenderer _lineRenderer;

	void Start ()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
	}
	
	void Update ()
    {
        Vector3 origin = transform.position;

        _lineRenderer.SetPosition(0, origin);

        RaycastHit hit;
        if (Physics.Raycast(origin, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("UI")))
        {
            cursorInside = true;
            cursorPosition = hit.textureCoord;

            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            cursorInside = false;
            _lineRenderer.SetPosition(1, origin);
        }

        testLabel.text = cursorPosition.ToString();
    }
}
