using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Text Text { get; private set; }
    RectTransform canvasRect;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
        Text  = gameObject.GetComponent<Text>();
    }

    public void Initialize(Vector3 position, string text, Color color)
    {
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(position);
        Vector2 worldPosition = new Vector2((viewportPos.x * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x * 0.5f),
            (viewportPos.y * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y * 0.5f));

        Text.rectTransform.anchoredPosition = worldPosition;
        Text.text = text; 
        Text.color = color;
        StartCoroutine(FloatUp());
    }

    private IEnumerator FloatUp()
    {
        for (int i = 100; i > 0; i--)
        {
            transform.position = transform.position += Vector3.up;
            Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, Text.color.a - 0.01f);
            yield return new WaitForSeconds(0.025f);
        }
        Destroy(this.gameObject);
    }
}
