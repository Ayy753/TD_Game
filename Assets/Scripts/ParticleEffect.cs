using System.Collections;
using UnityEngine;

public class ParticleEffect : MonoBehaviour{
    [SerializeField] private float effectDuration;

    private ParticleSystem particles;
    private Coroutine effectCoroutine;
    private bool isAvailable = false;

    private void Awake() {
        particles = transform.GetComponentInChildren<ParticleSystem>();
        SetEffectDuration(effectDuration);
    }

    private void SetEffectDuration(float duration) {
        effectDuration = duration;
        ParticleSystem.MainModule main = particles.main;
        main.startLifetime = effectDuration;
    }

    private IEnumerator ActivateEffect() {
        EmitParticles();
        yield return new WaitForSeconds(effectDuration);
        StopEmittingParticles();
    }

    private void SetEffectRadius(float radius) {
        ParticleSystem.MainModule main = particles.main;
        main.startSpeed = radius;
        main.startLifetime = 1;
    }

    private void EmitParticles() {
        Debug.Log("Emitting particles");
        isAvailable = false;
        ParticleSystem.EmissionModule emission = particles.emission;
        emission.enabled = true;
        particles.Play();
    }

    private void StopEmittingParticles() {
        Debug.Log("Stopped emitting particles");
        ParticleSystem.EmissionModule emission = particles.emission;
        emission.enabled = false;
        particles.Clear();
        isAvailable = true;
    }

    public void Activate(Vector3 position, float radius) {
        transform.position = position;
        SetEffectRadius(radius);
        effectCoroutine = StartCoroutine(ActivateEffect());
    }

    public void Disable() {
        StopCoroutine(effectCoroutine);
        isAvailable = true;
    }

    public bool IsAvailable() {
        return isAvailable;
    }
}
