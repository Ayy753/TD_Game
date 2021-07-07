using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Listens for IDisplayable objects getting hovered/unhovered and controls the tooltip accordingly
/// </summary>
public class TooltipManager: IInitializable, IDisposable{
    ToolTip toolTip;

    public TooltipManager(ToolTip toolTip) {
        this.toolTip = toolTip;
    }

    public void Initialize() {
        MouseManager.OnHoveredNewTooltipable += ShowToolTip;
        MouseManager.OnUnhoveredTooltipable += HideToolTip;
    }

    public void Dispose() {
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
