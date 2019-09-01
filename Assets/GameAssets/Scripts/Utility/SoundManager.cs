using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioClip m_laserPistol;
    private AudioClip m_laserRifal;
    private AudioClip m_droneExplosion;
    private AudioClip m_emptyGun;


    public string pistolSoundFile;
    public string emptyGunSoundFile;

    public string rifleSoundFile;

    void Awake()
    {
        m_laserPistol = Resources.Load<AudioClip>("Sounds/" + pistolSoundFile);
        m_laserRifal = Resources.Load<AudioClip>("Sounds/" + rifleSoundFile);
        m_droneExplosion = Resources.Load<AudioClip>("Sounds/DroneExplosion");
        m_emptyGun = Resources.Load<AudioClip>("Sounds/" + emptyGunSoundFile);
    }

    public AudioClip getLaserPistolAudioClip()
    {
        return m_laserPistol;
    }

    public AudioClip getLaserRifalAudioClip()
    {
        return m_laserRifal;
    }

    public AudioClip getDroneExplosion()
    {
        return m_droneExplosion;
    }

    public AudioClip getEmptyGunSound()
    {
        return m_emptyGun;
    }
}
