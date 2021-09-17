using System;
using UnityEngine;

namespace DefaultNamespace {
    public class NewTileReachedEventArgs : EventArgs {
        public Vector3Int Position { get; private set; }

        public NewTileReachedEventArgs( Vector3Int position) {
            Position = position;
        }
    }

    public interface IUnitMovement {
        event EventHandler<NewTileReachedEventArgs> OnNewTileReached;
        public void Move();
    }
}
