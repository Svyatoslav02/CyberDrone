using UnityEngine;

public class DroneAudioController : MonoBehaviour
{
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioSource audioSource3;
    public AudioSource audioSource4;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            PlayAudio(audioSource1);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            PlayAudio(audioSource3);
        }
        else
        {
            PlayAudio(audioSource2);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("City"))
        {
            PlayAudio(audioSource4);
        }
    }

    private void PlayAudio(AudioSource audioSource)
    {
        StopAllAudio();

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void StopAllAudio()
    {
        audioSource1.Stop();
        audioSource2.Stop();
        audioSource3.Stop();
        audioSource4.Stop();
    }
}//audio
