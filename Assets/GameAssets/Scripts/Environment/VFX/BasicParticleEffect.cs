using UnityEngine;

public class BasicParticleEffect : MonoBehaviour
{
    ParticleSystem m_selfParticleSystem;

    private void Awake()
    {
        m_selfParticleSystem = this.GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        m_selfParticleSystem.Play();
        this.Invoke("resetAll",1.5f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void resetAll()
    {
        m_selfParticleSystem.Stop();
        this.gameObject.SetActive(false);
    }
}
