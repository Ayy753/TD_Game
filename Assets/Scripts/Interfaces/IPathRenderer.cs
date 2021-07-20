using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IPathRenderer{
    public void RenderPath(List<Vector3Int> path);
}
