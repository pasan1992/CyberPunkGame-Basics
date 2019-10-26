using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawStuff : MonoBehaviour
{
      // When added to an object, draws colored rays from the
    // transform position.
    public int lineCount = 100;
    public float radius = 3.0f;

    static Material lineMaterial;

    public Transform start;
    public Transform end;


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

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);
        for (int i = 0; i < lineCount; ++i)
        {
            float a = i / (float)lineCount;
            float angle = a * Mathf.PI * 2;
            // Vertex colors change from red to green
            GL.Color(new Color(a, 1 - a, 0, 0.8F));
            // One vertex at transform position
            GL.Vertex3(0, 0, 0);
            // Another vertex at edge of circle
            GL.Vertex3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }

        GL.Begin(GL.LINES); // Line
        GL.Color(new Color(0, 0, 0, 1));
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(0.75f, 0.75f, 0);
        GL.End();

        LineBetweenObject();

        
        GL.End();
        GL.PopMatrix();
    }

    private void LineBetweenObject()
    {
        GL.MultMatrix(start.transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(new Color(0, 0, 0, 1));
        GL.Vertex3(0, 0, 0);
        Vector3 pos = end.position - start.position;
        GL.Vertex3(pos.x,pos.y,pos.z);
        GL.End();
    }
}
