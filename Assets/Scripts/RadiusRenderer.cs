using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusRenderer : MonoBehaviour{

    private LineRenderer line;

    public void Awake() {
        line = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Renders a line of a given radius around a point to draw a circle
    /// used to show the radius of things
    /// </summary>
    /// <param name="center">Center of tower</param>
    /// <param name="radius">Attack radius of tower</param>
    public void RenderRadius(Vector3 center, float radius) {
        line.enabled = true;
        float x;
        float y;

        //  Drawing a line around the given position
        for (int i = 0; i < line.positionCount; i++) {
            x = center.x + radius * Mathf.Sin(Mathf.Deg2Rad * (360 / line.positionCount * i));
            y = center.y + radius * Mathf.Cos(Mathf.Deg2Rad * (360 / line.positionCount * i));

            line.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    /// <summary>
    /// Hides the line
    /// </summary>
    public void HideRadius() {
        line.enabled = false;
    }
}
