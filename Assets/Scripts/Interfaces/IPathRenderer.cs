namespace DefaultNamespace {

    using System.Collections.Generic;
    using UnityEngine;

    public interface IPathRenderer {
        public void RenderPath(List<Vector3Int> path);
    }
}
