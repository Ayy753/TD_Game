using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerGUI : MonoBehaviour
{
    private BuildManager buildManager;

    private Image targetIcon;
    private Button btnTargetFurthest;
    private Button btnTargetClosest;
    private Button btnTargetHighest;
    private Button btnTargetLowest;
    private Button btnTargetRandom;
    private Text txtTargetDescription;
    private GameObject pnlTarget;
    private Button btnSellStructure;
    private List<Button> towerTargetButtons;

    private Tower targettedTower;

    // Start is called before the first frame update
    void Start()
    {
        buildManager = GameManager.Instance.BuildManager;
            
        //  Targetting panel UI
        targetIcon = GameObject.Find("imgTargetIcon").GetComponent<Image>();
        btnTargetFurthest = GameObject.Find("btnFurthest").GetComponent<Button>();
        btnTargetClosest = GameObject.Find("btnClosest").GetComponent<Button>();
        btnTargetHighest = GameObject.Find("btnMaxHP").GetComponent<Button>();
        btnTargetLowest = GameObject.Find("btnMinHP").GetComponent<Button>();
        btnTargetRandom = GameObject.Find("btnRandom").GetComponent<Button>();
        txtTargetDescription = GameObject.Find("txtDescription").GetComponent<Text>();
        towerTargetButtons = new List<Button>() { btnTargetFurthest, btnTargetClosest, btnTargetHighest, btnTargetLowest, btnTargetRandom };
        pnlTarget = GameObject.Find("pnlTarget");
        btnSellStructure = GameObject.Find("btnSellStructure").GetComponent<Button>();

        pnlTarget.SetActive(false);
    }

    /// <summary>
    /// Populate target window with selected tower's info
    /// </summary>
    /// <param name="tower"></param>
    public void TargetTower(Tower tower)
    {
        targettedTower = tower;
        targetIcon.sprite = targettedTower.TowerData.Icon;

        UnhighlightTowerButtons();

        switch (tower.SelectedTargetMode)
        {
            case Tower.TargetMode.Closest:
                btnTargetClosest.image.color = Color.cyan;
                break;
            case Tower.TargetMode.Furthest:
                btnTargetFurthest.image.color = Color.cyan;
                break;
            case Tower.TargetMode.Random:
                btnTargetRandom.image.color = Color.cyan;
                break;
            case Tower.TargetMode.LowestHealth:
                btnTargetLowest.image.color = Color.cyan;
                break;
            case Tower.TargetMode.HighestHealth:
                btnTargetHighest.image.color = Color.cyan;
                break;
            default:
                throw new Exception("Target mode invalid");
        }

        txtTargetDescription.text = tower.GetDisplayText();
        float value = Mathf.Round(tower.TowerData.Cost * 0.66f);
        btnSellStructure.GetComponentInChildren<Text>().text = "Sell for " + value.ToString() + " Gold";
        pnlTarget.SetActive(true);
    }

    /// <summary>
    /// Sells the targetted tower
    /// </summary>
    public void SellTower()
    {
        buildManager.DemolishStructure(Vector3Int.FloorToInt(targettedTower.transform.position));
        pnlTarget.SetActive(false);
    }

    /// <summary>
    /// Clears the highlighted target mode button
    /// </summary>
    public void UnhighlightTowerButtons()
    {
        foreach (Button button in towerTargetButtons)
        {
            button.image.color = Color.white;
        }
    }

    /// <summary>
    /// Tower targets nearest enemy
    /// </summary>
    public void TargetNearest()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.Closest);
            btnTargetClosest.image.color = Color.cyan;
        }
    }

    /// <summary>
    /// Tower targets nearest enemy
    /// </summary>
    public void TargetFurthest()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.Furthest);
            btnTargetFurthest.image.color = Color.cyan;
        }
    }

    /// <summary>
    /// Tower targets enemy with lowest health
    /// </summary>
    public void TargetMinHP()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.LowestHealth);
            btnTargetLowest.image.color = Color.cyan;
        }
    }

    /// <summary>
    /// Tower targets enemy with highest health
    /// </summary>
    public void TargetMaxHP()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.HighestHealth);
            btnTargetHighest.image.color = Color.cyan;
        }
    }

    /// <summary>
    /// Tower targets random enemy within range
    /// </summary>
    public void TargetRandom()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.Random);
            btnTargetRandom.image.color = Color.cyan;
        }
    }

    /// <summary>
    /// Unselects the targetted tower
    /// </summary>
    public void ClearTarget()
    {
        pnlTarget.SetActive(false);
        targettedTower = null;
    }

    /// <summary>
    /// Shows tower information panel
    /// </summary>
    public void ShowTargetPanel()
    {
        pnlTarget.SetActive(true);
    }

    /// <summary>
    /// Hides tower information panel
    /// </summary>
    public void HideTargetPanel()
    {
        pnlTarget.SetActive(false);
    }
}


