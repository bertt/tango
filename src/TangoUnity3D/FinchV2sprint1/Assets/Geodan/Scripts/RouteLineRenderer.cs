using UnityEngine;
using System.Collections;

public class RouteLineRenderer : MonoBehaviour
{
    static Material lineMaterial;
    public bool useQuads = false;

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
        if (GameManager.Markers.Count > 1)
        {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform

            if (useQuads)
            {
                //  GL.LoadOrtho();
                GL.Begin(GL.QUADS);
                for (int i = 1; i < GameManager.Markers.Count; ++i)
                {
                    // Vertex colors change from red to green based on distance
                    //GL.Color(new Color(a, 1 - a, 0, 0.8F));

                    GL.Color(Color.green);

                    
                    // startpos
                    Vector3 sRight = GameManager.Markers[i - 1].transform.position + (0.02f * GameManager.Markers[i - 1].transform.right);
                    Vector3 sLeft = GameManager.Markers[i - 1].transform.position + (0.02f * -GameManager.Markers[i - 1].transform.right);
                    GL.Vertex3(sRight.x, sRight.y, sRight.z);
                    GL.Vertex3(sLeft.x, sLeft.y, sLeft.z);
                    // endpos
                    Vector3 eRight = GameManager.Markers[i].transform.position + (-0.02f * GameManager.Markers[i].transform.right);
                    Vector3 eLeft = GameManager.Markers[i].transform.position + (0.02f * GameManager.Markers[i].transform.right);
                    GL.Vertex3(eRight.x, eRight.y, eRight.z);
                    GL.Vertex3(eLeft.x, eLeft.y, eLeft.z);
                }
                GL.End();

                //float width = 2;
                //Vector3 perpendicular = (new Vector3(linePoints[i + 1].y, linePoints[i].x, nearClip) -
                //                        new Vector3(linePoints[i].y, linePoints[i + 1].x, nearClip)).normalized * width;
                //Vector3 v1 = new Vector3(linePoints[i].x, linePoints[i].y, nearClip);
                //Vector3 v2 = new Vector3(linePoints[i + 1].x, linePoints[i + 1].y, nearClip);
                //GL.Vertex(cam.ViewportToWorldPoint(v1 - perpendicular));
                //GL.Vertex(cam.ViewportToWorldPoint(v1 + perpendicular));
                //GL.Vertex(cam.ViewportToWorldPoint(v2 + perpendicular));
                //GL.Vertex(cam.ViewportToWorldPoint(v2 - perpendicular));
            }
            else
            {
                GL.MultMatrix(transform.localToWorldMatrix);

                // Draw lines
                GL.Begin(GL.LINES);
                for (int i = 1; i < GameManager.Markers.Count; ++i)
                {
                    // Vertex colors change from red to green based on distance
                    //GL.Color(new Color(a, 1 - a, 0, 0.8F));

                    GL.Color(Color.green);
                    // startpos
                    Vector3 s = GameManager.Markers[i - 1].transform.position;
                    GL.Vertex3(s.x, s.y, s.z);
                    // endpos
                    Vector3 e = GameManager.Markers[i].transform.position;
                    GL.Vertex3(e.x, e.y, e.z);
                }
                GL.End();
            }
            GL.PopMatrix();
        }
    }
}
