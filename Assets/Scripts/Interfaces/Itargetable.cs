using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Itargetable{

    /// <summary>
    /// Fires when target is destroyed
    /// </summary>
    public event EventHandler TargetDisabled;
}
