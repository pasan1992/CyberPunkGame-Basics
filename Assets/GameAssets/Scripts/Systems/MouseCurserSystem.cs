using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCurserSystem : MonoBehaviour
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    private void Update()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
}
