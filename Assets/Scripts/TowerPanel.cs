using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TowerPanel : IInitializable {
    GameObject pnlTowerPanel;
    
    TMP_Text txtRange, txtDamage, txtReloadTime, txtProjectileType;
    Button btnClosest, btnFurthest, btnLowHP, btnHighHP, btnRandom;
    Button[] buttons;

    Tower currentlySelectedTower;

    public void Initialize() {
        Debug.Log("initializing tower panel");

        pnlTowerPanel = GameObject.Find("pnlTower");

        txtRange = GameObject.Find("txtRangeVal").GetComponent<TMP_Text>();
        txtDamage = GameObject.Find("txtDamageVal").GetComponent<TMP_Text>();
        txtReloadTime = GameObject.Find("txtReloadTimeVal").GetComponent<TMP_Text>();
        txtProjectileType = GameObject.Find("txtProjectileTypeVal").GetComponent<TMP_Text>();

        btnClosest = GameObject.Find("btnClosest").GetComponent<Button>();
        btnFurthest = GameObject.Find("btnFurthest").GetComponent<Button>();
        btnLowHP = GameObject.Find("btnLowest").GetComponent<Button>();
        btnHighHP = GameObject.Find("btnHighest").GetComponent<Button>();
        btnRandom = GameObject.Find("btnRandom").GetComponent<Button>();

        buttons = new Button[] { btnClosest, btnFurthest, btnLowHP, btnHighHP, btnRandom };

        btnClosest.onClick.AddListener(delegate { SetTargetMode(Tower.TargetMode.Closest); });
        btnFurthest.onClick.AddListener(delegate { SetTargetMode(Tower.TargetMode.Furthest); });
        btnLowHP.onClick.AddListener(delegate { SetTargetMode(Tower.TargetMode.LowestHealth); });
        btnHighHP.onClick.AddListener(delegate { SetTargetMode(Tower.TargetMode.HighestHealth); });
        btnRandom.onClick.AddListener(delegate { SetTargetMode(Tower.TargetMode.Random); });

        pnlTowerPanel.SetActive(false);
    }

    private void UpdateTowerPanel() {
        TowerData towerData = currentlySelectedTower.TowerData;

        txtRange.text = towerData.Range.ToString();
        txtDamage.text = towerData.ProjectileData.RawTotalDamage().ToString();
        txtReloadTime.text = towerData.ReloadTime.ToString();
        txtProjectileType.text = towerData.ProjectileData.type.ToString();

        UpdateButtonColors(currentlySelectedTower.CurrentTargetMode);
    }

    private void UpdateButtonColors(Tower.TargetMode targetMode) {
        //  Remove highlight from each button
        for (int i = 0; i < buttons.Length; i++) {
            buttons[i].image.color = Color.white;
        }

        Button selectedBtn;

        switch (targetMode) {
            case Tower.TargetMode.Closest:
                selectedBtn = btnClosest;
                break;
            case Tower.TargetMode.Furthest:
                selectedBtn = btnFurthest;
                break;
            case Tower.TargetMode.Random:
                selectedBtn = btnRandom;
                break;
            case Tower.TargetMode.LowestHealth:
                selectedBtn = btnLowHP;
                break;
            case Tower.TargetMode.HighestHealth:
                selectedBtn = btnHighHP;
                break;
            default:
                throw new System.Exception("Tower panel button does not exist for target mode: " + targetMode);
        }

        selectedBtn.image.color = selectedBtn.colors.selectedColor;
    }

    public void SetTargetMode(Tower.TargetMode targetMode) {
        currentlySelectedTower.ChangeTargetMode(targetMode);
        UpdateButtonColors(targetMode);
    }

    public void TargetTower(Tower tower) {
        currentlySelectedTower = tower;
        pnlTowerPanel.SetActive(true);
        UpdateTowerPanel();
    }

    public void ClearTarget() {
        currentlySelectedTower = null;
        pnlTowerPanel.SetActive(false);
    }
}
