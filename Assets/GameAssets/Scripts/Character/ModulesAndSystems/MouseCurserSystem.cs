using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCurserSystem : MonoBehaviour
{
    public Texture2D aimedTexture;
    public Texture2D aimedOnTargetTexture;
    public Texture2D idleTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    private static MouseCurserSystem mouseSystem;

    public enum CURSOR_STATE { IDLE,AIMED,ONTARGET}
    private CURSOR_STATE m_currentState = CURSOR_STATE.IDLE;

    void Start()
    {
        Cursor.SetCursor(aimedTexture, hotSpot, cursorMode);
        mouseSystem = this;
    }

    private void Update()
    {
        switch (m_currentState)
        {
            case CURSOR_STATE.IDLE:
                Cursor.SetCursor(idleTexture, hotSpot, cursorMode);
            break;
            case CURSOR_STATE.AIMED:
                Cursor.SetCursor(aimedTexture, hotSpot, cursorMode);
            break;
            case CURSOR_STATE.ONTARGET:
                Cursor.SetCursor(aimedOnTargetTexture, hotSpot, cursorMode);
            break;
        }
        
    }


    public static MouseCurserSystem getInstance()
    {
        return mouseSystem;
    }

    public void setMouseCurserState(CURSOR_STATE state)
    {
        m_currentState = state;
    }
}
