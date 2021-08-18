namespace DefaultNamespace {
    using UnityEngine;

    public class MessageSystem : IMessageSystem {
        private ObjectPool objectPool;

        public MessageSystem(ObjectPool objectPool) {
            this.objectPool = objectPool;
        }

        public void DisplayMessage(string message, Color color, float textSize = 0.5f) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            DisplayMessageAt(pos, message, color, textSize);
        }

        public void DisplayMessageAt(Vector3 position, string message, Color color, float textSize = 0.5f, float randomHorizontalOffset = 0) {
            float offset = 0;

            if (randomHorizontalOffset != 0) {
                offset = Random.Range(-randomHorizontalOffset, randomHorizontalOffset);
            }

            objectPool.CreateFloatingText(position + new Vector3(offset, 0, 0), message, color, textSize);
        }

        public void DisplayMessageAtCursor(string message, Color color, float textSize = 0.5f) {
            DisplayMessageAt(Camera.main.ScreenToWorldPoint(Input.mousePosition), message, color, textSize);
        }
    }
}
