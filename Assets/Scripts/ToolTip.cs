using UnityEngine;
using UnityEngine.UI;
public class ToolTip : MonoBehaviour
{
    TileData currentStructure;
    Text tooltipText;
    private RectTransform background;
    // Start is called before the first frame update
    void Start()
    {
        tooltipText = transform.Find("Text").GetComponent<Text>();
        background = transform.Find("Background").GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
        float textPadding = 4f;
        background.sizeDelta = new Vector2(tooltipText.preferredWidth + textPadding *2f, tooltipText.preferredHeight + textPadding * 2f);
    }

    public void SetCurrentTile(TileData tile)
    {
        currentStructure = tile;
        tooltipText.text = tile.ToString();
    }
}
