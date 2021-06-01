using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSystem : IMessageSystem {
    private ObjectPool objectPool;

    public MessageSystem(ObjectPool objectPool) {
        this.objectPool = objectPool;
    }

    public void DisplayMessage(string message, Color color) {
        //  spawn floating text using objectpool
        Debug.Log("Message system: " + message);
    }
}
