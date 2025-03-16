using UnityEngine;

public class DroneAudioController : MonoBehaviour
{
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioSource audioSource3;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            SetAudioLevels(0.31f, 0.3f, 0.3f); // Movement sound active
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            SetAudioLevels(0.3f, 0.3f, 0.31f); // Boost sound active
        }
        else
        {
            SetAudioLevels(0.3f, 1.0f, 0.3f); // Idle sound active
        }
    }

    private void SetAudioLevels(float movementVolume, float idleVolume, float boostVolume)
    {
        audioSource1.volume = movementVolume;
        audioSource2.volume = idleVolume;
        audioSource3.volume = boostVolume;

        if (!audioSource1.isPlaying)
        {
            if (movementVolume < 0.31f)
            {
            }
            else
            {
                audioSource1.Play();
            }
        }
        if (!audioSource2.isPlaying) {

            if (idleVolume < 0.31f)
            {
            }
            else
            {
                audioSource2.Play();
            }
        }
        if (!audioSource3.isPlaying) {
            if (boostVolume < 0.31f)
            {
            }
            else
            {
                audioSource3.Play();
            }
        }
    }

    //private void StopAllAudio()
    //{
    //    audioSource1.Stop();
    //    audioSource2.Stop();
    //    audioSource3.Stop();
    //}
}//audio
