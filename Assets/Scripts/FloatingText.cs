using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    private TextMesh textMesh;

    private void OnEnable()
    {
        //  Move render order on top of sprites
        gameObject.GetComponent<MeshRenderer>().sortingOrder = 5;
        textMesh = GetComponent<TextMesh>();
    }

    /// <summary>
    /// Initialize the floating text
    /// </summary>
    /// <param name="position"></param>
    /// <param name="text"></param>
    /// <param name="color"></param>
    public void Initialize(Vector3 position, string text, Color color)
    {
        transform.position = position;
        textMesh.text = text;
        textMesh.color = color;
        StartCoroutine(FloatUp());
    }

    private IEnumerator FloatUp()
    {
        for (int i = 50; i > 0; i--)
        {
            //  Float up
            transform.position = transform.position += new Vector3(0, 0.2f, 0);
            //  Fade out
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, textMesh.color.a - 0.02f);
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(this.gameObject);
    }
}
