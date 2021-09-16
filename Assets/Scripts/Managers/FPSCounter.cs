namespace DefaultNamespace.GUI {
    using UnityEngine;
    using TMPro;

    public class FPSCounter : MonoBehaviour {
        private const float UPDATE_RATE = 2.0f;
        private int frameCount;
        private float dt;
        private float fps;

        TMP_Text fpsText;

        private void Awake() {
            fpsText = GameObject.Find("txtFPS").GetComponent<TMP_Text>();
            Debug.Log("starting fps counter");
        }

        public void Update() {
            frameCount++;
            dt += Time.unscaledDeltaTime;
            if (dt > 1.0f / UPDATE_RATE) {
                fps = frameCount / dt;
                frameCount = 0;
                dt -= 1.0f / UPDATE_RATE;

                fpsText.text = fps.ToString();
            }
        }
    }
}
