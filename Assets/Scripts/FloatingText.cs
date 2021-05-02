using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour, IPoolable
{
    private TextMesh textMesh;

    [SerializeField]
    private GameObject prefab;
    public GameObject Prefab { get { return prefab; } }
    public GameObject GameObjectRef { get { return gameObject; } }

    private void OnEnable()
    {
        //  Move render order on top of sprites
        gameObject.GetComponent<MeshRenderer>().sortingOrder = 6;
        textMesh = GetComponent<TextMesh>();
    }

    /// <summary>
    /// Initialize the floating text
    /// </summary>
    /// <param name="position"></param>
    /// <param name="text"></param>
    /// <param name="color"></param>
    public void Initialize(Vector3 position, string text, Color color, float textSize = 0.5f)
    {
        //  Solves bug where mouse position causes text to be hidden from camera
        transform.position = new Vector3(position.x, position.y, 1);

        textMesh.characterSize = textSize;
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
        //Destroy(this.gameObject);
        gameObject.SetActive(false);
    }

    public void Spawn(Vector3 position, Transform parent)
    {
        transform.position = position;
        transform.parent = parent;
    }
}
