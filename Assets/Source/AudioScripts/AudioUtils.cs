using UnityEngine;

public static class AudioUtils
{
    public static AudioClip GetRandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }

    public static void RandomiseAudioSourceParams(ref AudioSource source, bool randomiseVolume,
        bool randomisePitch, float volumeVariation, float pitchVariation, float originalVolume = 1,
        float originalPitch = 1)

    {
        if (randomiseVolume)
        {
            source.volume = Random.Range(originalVolume - volumeVariation, originalVolume + volumeVariation);
        }

        if (randomisePitch)
        {
            source.pitch = Random.Range(originalPitch - pitchVariation, originalPitch + pitchVariation);
        }
    }
}
