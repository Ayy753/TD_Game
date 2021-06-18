using UnityEngine;

public class TickManager : MonoBehaviour {
    public const float tickFrequency = 1 / 3;
    private float accumulatedTime = 0;

    public delegate void Tick();
    public static event Tick OnTick;

    private void Update() {
        accumulatedTime += Time.deltaTime;
        if (accumulatedTime >= tickFrequency) {
            if (OnTick != null) {
                OnTick.Invoke();
            }
            accumulatedTime = 0;
        }
    }
}
