using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioClip m_laserPistol;
    private AudioClip m_laserRifal;

    void Awake()
    {
        m_laserPistol = Resources.Load<AudioClip>("Sounds/LaserPistol");
        m_laserRifal = Resources.Load<AudioClip>("Sounds/LaserPistol");
    }

    public AudioClip getLaserPistolAudioClip()
    {
        return m_laserPistol;
    }

    public AudioClip getLaserRifalAudioClip()
    {
        return m_laserRifal;
    }
}
