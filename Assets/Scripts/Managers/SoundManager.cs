using System.Collections;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
	[SerializeField] private AudioSource backgroundMusicAudioSource;
	[SerializeField] private AudioSource soundEffectAudioSource;
	[SerializeField] private AudioClip positiveClickingClip;
	[SerializeField] private AudioClip negativeClickingClip;

	private bool isPlayingBackgroundMusic = false;
	private bool isBackgroundMusicEnabled = false;
	private bool isSoundEffectEnabled = false;

	private const string BACKGROUND_MUSIC_STATUS_KEY_NAME = "BackgroundMusicStatus";
	private const string SOUND_EFFECT_STATUS_KEY_NAME = "SoundEffectStatus";

	void Start()
	{
		isBackgroundMusicEnabled = ( PlayerPrefs.GetInt( BACKGROUND_MUSIC_STATUS_KEY_NAME, 1 ) == 1 );
		isSoundEffectEnabled = ( PlayerPrefs.GetInt( SOUND_EFFECT_STATUS_KEY_NAME, 1 ) == 1 );
	}

    public void SetBackgroundMusicAudioSourceVolume( float volumeValue )
    {
        backgroundMusicAudioSource.volume = volumeValue;
    }

    public void SetSoundEffectAudioSourceVolume( float volumeValue )
    {
        soundEffectAudioSource.volume = volumeValue;
    }

    public void EnableBackgroundMusic( bool isEnabled )
	{
		isBackgroundMusicEnabled = isEnabled;

		if (isBackgroundMusicEnabled == true)
		{
			if (backgroundMusicAudioSource.isPlaying == false)
			{
				backgroundMusicAudioSource.Play();
			}
		}
		else
		{
			if (backgroundMusicAudioSource.isPlaying == true)
			{
				backgroundMusicAudioSource.Pause();
			}
		}

		PlayerPrefs.SetInt( BACKGROUND_MUSIC_STATUS_KEY_NAME, ( ( isBackgroundMusicEnabled == true ) ? 1 : 0 ) );
	}

	public void FadeInBackgroundMusic( float toVolume = 1.0f )
	{
		LeanTween.value( backgroundMusicAudioSource.volume, toVolume, 0.2f ).setOnUpdate( SetBackgroundMusicAudioSourceVolume );
	}

	public void FadeOutBackgroundMusic( float toVolume = 0.0f )
	{
		LeanTween.value( backgroundMusicAudioSource.volume, toVolume, 0.2f ).setOnUpdate( SetBackgroundMusicAudioSourceVolume );
	}

	public void DelayToFadeInBackgroundMusic( float delay )
	{
		StartCoroutine( RunDelayingToFadeInBackgroundMusic( delay ) );
	}

	private IEnumerator RunDelayingToFadeInBackgroundMusic( float delay )
	{
		yield return new WaitForSeconds( delay );
		FadeInBackgroundMusic();
	}

	public void EnableSoundEffect( bool isEnabled )
	{
		isSoundEffectEnabled = isEnabled;

		PlayerPrefs.SetInt( SOUND_EFFECT_STATUS_KEY_NAME, ( ( isSoundEffectEnabled == true ) ? 1 : 0 ) );
	}

	public void PlayBackgroundMusic()
	{
		if (isBackgroundMusicEnabled == true)
		{
			isPlayingBackgroundMusic = true;
			backgroundMusicAudioSource.Play();
		}
	}

    public void PlayBackgroundMusic( AudioClip clip, float volumeScale = 1.0f )
    {
        backgroundMusicAudioSource.clip = clip;
        backgroundMusicAudioSource.volume = volumeScale;
        PlayBackgroundMusic();
    }

    public void PlaySoundEffect( AudioClip clip, float volumeScale = 1.0f )
	{
		if (isSoundEffectEnabled == true)
		{
			if (clip != null)
			{
				soundEffectAudioSource.PlayOneShot( clip, volumeScale );
			}
		}
	}

	public void PlayPositiveClickingClip()
	{
		PlaySoundEffect( positiveClickingClip );
	}

	public void PlayNegativeClickingClip()
	{
		PlaySoundEffect( negativeClickingClip );
	}

	public bool GetIsPlayingBackgroundMusic()
	{
		return isPlayingBackgroundMusic;
	}

	public bool GetIsBackgroundMusicEnabled()
	{
		return isBackgroundMusicEnabled;
	}

	public bool GetIsSoundEffectEnabled()
	{
		return isSoundEffectEnabled;
	}

	public bool IsPlayingBackgroundMusic()
	{
		return ( backgroundMusicAudioSource.isPlaying == true );
	}

    public float GetPositiveClickingClipLength()
    {
        return positiveClickingClip.length;
    }
}
