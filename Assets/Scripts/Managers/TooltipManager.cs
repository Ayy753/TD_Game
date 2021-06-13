using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Listens for IDisplayable objects getting hovered/unhovered and controls the tooltip accordingly
/// </summary>
public class TooltipManager : MonoBehaviour{

    [Inject] ToolTip toolTip;

    private void OnEnable() {
        MouseManager.OnHoveredNewTooltipable += ShowToolTip;
        MouseManager.OnUnhoveredTooltipable += HideToolTip;
    }

    private void OnDisable() {
        MouseManager.OnHoveredNewTooltipable -= ShowToolTip;
        MouseManager.OnUnhoveredTooltipable -= HideToolTip;
    }

    private void HideToolTip() {
        toolTip.HideToolTip();
    }

    private void ShowToolTip(IDisplayable displayable) {
        toolTip.ShowToolTip(displayable);
    }
}
