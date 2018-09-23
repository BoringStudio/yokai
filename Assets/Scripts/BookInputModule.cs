using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BookInputModule : StandaloneInputModule
{
    public RenderTexture bookRenderTexture;
    public BookLaserPointer pointer;

    bool _lastPressed;

    Vector2 m_cursorPos;
    private readonly MouseState m_MouseState = new MouseState();
    protected override MouseState GetMousePointerEventData(int id = 0)
    {
        MouseState m = new MouseState();

        // Populate the left button...
        PointerEventData leftData;
        var created = GetPointerData(kMouseLeftId, out leftData, true);

        leftData.Reset();

        if (created)
            leftData.position = m_cursorPos;

        // Ordinarily we'd just pass the screen coordinates of the cursor through.
        //Vector2 pos = Input.mousePosition;

        Vector2 pos;
        pos.x = bookRenderTexture.width * pointer.cursorPosition.x;
        pos.y = bookRenderTexture.height * pointer.cursorPosition.y;
        m_cursorPos = pos;

        // For UV-mapped meshes, you could fire a ray against its MeshCollider 
        // and determine the UV coordinates of the struck point.

        leftData.delta = pos - leftData.position;
        leftData.position = pos;
        leftData.scrollDelta = Input.mouseScrollDelta;
        leftData.button = PointerEventData.InputButton.Left;
        eventSystem.RaycastAll(leftData, m_RaycastResultCache);
        var raycast = FindFirstRaycast(m_RaycastResultCache);
        leftData.pointerCurrentRaycast = raycast;
        m_RaycastResultCache.Clear();

        // copy the apropriate data into right and middle slots
        PointerEventData rightData;
        GetPointerData(kMouseRightId, out rightData, true);
        CopyFromTo(leftData, rightData);
        rightData.button = PointerEventData.InputButton.Right;

        PointerEventData middleData;
        GetPointerData(kMouseMiddleId, out middleData, true);
        CopyFromTo(leftData, middleData);
        middleData.button = PointerEventData.InputButton.Middle;

        HandController controller = pointer.GetComponent<HandController>();

        bool pressed = controller.isValid && pointer.GetComponent<HandController>().device.GetPress(SteamVR_Controller.ButtonMask.Trigger);

        PointerEventData.FramePressState pressState;
        if (pressed && !_lastPressed)
        {
            pressState = PointerEventData.FramePressState.Pressed;
        }
        else if (!pressed && _lastPressed)
        {
            pressState = PointerEventData.FramePressState.Released;
        }
        else {
            pressState = PointerEventData.FramePressState.NotChanged;
        }

        m_MouseState.SetButtonState(PointerEventData.InputButton.Left, pressState, leftData);
        m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), rightData);
        m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), middleData);

        _lastPressed = pressed;

        return m_MouseState;
    }
}
