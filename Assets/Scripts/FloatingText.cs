using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingText : MonoBehaviour
{
    private TMP_Text theText;

    private void Start()
    {
        //  Move render order on top of sprites
        gameObject.GetComponent<MeshRenderer>().sortingOrder = 400;
    }

    private void OnEnable()
    {
        theText = GetComponent<TMP_Text>();
    }

    /// <summary>
    /// Initialize the floating text
    /// </summary>
    /// <param name="position"></param>
    /// <param name="text"></param>
    /// <param name="color"></param>
    public void Initialize(Vector3 position, string text, Color color, float textSize = 0.5f)
    {
        gameObject.SetActive(true);
        
        //  Solves bug where mouse position causes text to be hidden from camera
        transform.position = new Vector3(position.x, position.y, 1);

        //  textmesh pro text scale doesnt match the previous text scale
        theText.fontSize = textSize * 10;
        theText.text = text;
        theText.color = color;
        StartCoroutine(FloatUp());
    }

    private IEnumerator FloatUp()
    {
        for (int i = 50; i > 0; i--)
        {
            //  Float up
            transform.position = transform.position += new Vector3(0, 0.2f, 0);
            //  Fade out
            theText.color = new Color(theText.color.r, theText.color.g, theText.color.b, theText.color.a - 0.02f);
            yield return new WaitForSecondsRealtime(0.05f);
        }
        gameObject.SetActive(false);
    }
}
