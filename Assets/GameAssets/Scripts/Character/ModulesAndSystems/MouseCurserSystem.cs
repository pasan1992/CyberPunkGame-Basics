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

    
    #region TargetLine

    private Transform m_targetLineStart;
    private Transform m_targetLineEnd;

    private bool m_targetLineEnable = false;

    public float squreSize = 0.05f;

    private bool onTarget = false;

    #endregion

    void Awake()
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
                onTarget = false;
                //Cursor.visible = true;
            break;
            case CURSOR_STATE.AIMED:
                Cursor.SetCursor(aimedTexture, hotSpot, cursorMode);
                onTarget = false;
                //Cursor.visible = false;
            break;
            case CURSOR_STATE.ONTARGET:
                Cursor.SetCursor(aimedOnTargetTexture, hotSpot, cursorMode);
                onTarget = true;
                //Cursor.visible = false;
            break;
        }
        
    }

    public void OnPostRender()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);

        if(m_targetLineEnable)
        {
            GL.PushMatrix();
            GL.Begin(GL.LINES);

            if(onTarget)
            {
                GL.Color(Color.red);
            }
            else
            {
                GL.Color(Color.white);
            }
            GL.Vertex3(m_targetLineStart.position.x,m_targetLineStart.position.y,m_targetLineStart.position.z);
            GL.Vertex3(m_targetLineEnd.position.x,m_targetLineEnd.position.y,m_targetLineEnd.position.z);
            GL.End();
//         GL.Begin(GL.QUADS);

//         if(onTarget)
//         {
//             GL.Color(Color.red);
//         }
//         else
//         {
//             GL.Color(Color.white);
//         }

        
//         GL.Vertex3(m_targetLineEnd.position.x+ squreSize, m_targetLineEnd.position.y,m_targetLineEnd.position.z+ squreSize);
//         GL.Vertex3(m_targetLineEnd.position.x+squreSize,  m_targetLineEnd.position.y,m_targetLineEnd.position.z- squreSize);
//         GL.Vertex3(m_targetLineEnd.position.x-squreSize,  m_targetLineEnd.position.y,m_targetLineEnd.position.z -squreSize);
//         GL.Vertex3(m_targetLineEnd.position.x -squreSize, m_targetLineEnd.position.y, m_targetLineEnd.position.z + squreSize);
// GL.End();

        GL.Begin(GL.LINES);

        if(onTarget)
        {
            GL.Color(Color.red);
        }
        else
        {
            GL.Color(Color.white);
        }
        GL.Vertex3(m_targetLineEnd.position.x,m_targetLineEnd.position.y,m_targetLineEnd.position.z);
        GL.Vertex3(m_targetLineEnd.position.x,m_targetLineEnd.position.y-4,m_targetLineEnd.position.z);
        GL.End();

        GL.Begin(GL.QUADS);

        if(onTarget)
        {
            GL.Color(Color.red);
        }
        else
        {
            GL.Color(Color.white);
        }
       
        GL.Vertex3(m_targetLineEnd.position.x+ squreSize, m_targetLineEnd.position.y-1.2f,m_targetLineEnd.position.z+ squreSize);
        GL.Vertex3(m_targetLineEnd.position.x+squreSize,  m_targetLineEnd.position.y-1.2f,m_targetLineEnd.position.z- squreSize);
        GL.Vertex3(m_targetLineEnd.position.x-squreSize,  m_targetLineEnd.position.y-1.2f,m_targetLineEnd.position.z -squreSize);
        GL.Vertex3(m_targetLineEnd.position.x -squreSize, m_targetLineEnd.position.y-1.2f, m_targetLineEnd.position.z + squreSize);
        GL.End();
        GL.PopMatrix();
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

    public void setTargetLineTrasforms(Transform start, Transform end)
    {
        m_targetLineEnd = end;
        m_targetLineStart = start;
    }

    public void enableTargetLine(bool enable)
    {
        m_targetLineEnable = enable;
    }



    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }
}
