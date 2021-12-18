using UnityEngine;
using UnityEngine.Rendering;

public class SpaceDenPlayermonEffectHandler : MonoBehaviour
{
    [SerializeField] private Transform eatingParticlePoint;
    [SerializeField] private Transform playingParticlePoint;
    [SerializeField] private Transform bathingParticlePoint;
    [SerializeField] private Transform loveParticlePoint;
    [SerializeField] private float loveParticleDestroyDelay = 5.0f;

    private GameObject eatingParticleObj = null;
    private Transform eatingParticleTransform = null;
    private ParticleSystem eatingParticleSystem = null;
    private GameObject playingParticleObj = null;
    private Transform playingParticleTransform = null;
    private ParticleSystem playingParticleSystem = null;
    private GameObject bathingParticleObj = null;
    private Transform bathingParticleTransform = null;
    private ParticleSystem bathingParticleSystem = null;

    public void ShowEatingEffect( GameObject eatingParticle, SortingGroup targetSortingGroup )
    {
        if (eatingParticleObj == null)
        {
            eatingParticleObj = ShowEffect( eatingParticle, eatingParticlePoint );
            eatingParticleTransform = eatingParticleObj.transform;
            eatingParticleSystem = eatingParticleObj.GetComponent<ParticleSystem>();
        }
        else
        {
            eatingParticleTransform.position = eatingParticlePoint.position;

            if (eatingParticleSystem != null)
            {
                if (eatingParticleSystem.isPlaying == false)
                {
                    eatingParticleSystem.Play();
                }
            }
            else
            {
                eatingParticleObj.SetActive( true );
            }
        }

        SetUpParticleSystemRendererSorting( eatingParticleObj, targetSortingGroup );
    }

    public void HideEatingEffect()
    {
        if (eatingParticleSystem != null)
        {
            eatingParticleSystem.Stop();
        }
        else if (eatingParticleObj != null)
        {
            eatingParticleObj.SetActive( false );
        }
    }

    public void ShowPlayingEffect( GameObject playingParticle, SortingGroup targetSortingGroup )
    {
        if (playingParticleObj == null)
        {
            playingParticleObj = ShowEffect( playingParticle, playingParticlePoint );
            playingParticleTransform = playingParticleObj.transform;
            playingParticleSystem = playingParticleObj.GetComponent<ParticleSystem>();
        }
        else
        {
            playingParticleTransform.position = playingParticlePoint.position;

            if (playingParticleSystem != null)
            {
                if (playingParticleSystem.isPlaying == false)
                {
                    playingParticleSystem.Play();
                }
            }
            else
            {
                playingParticleObj.SetActive( true );
            }
        }

        SetUpParticleSystemRendererSorting( playingParticleObj, targetSortingGroup );
    }

    public void HidePlayingEffect()
    {
        if (playingParticleSystem != null)
        {
            playingParticleSystem.Stop();
        }
        else if (playingParticleObj != null)
        {
            playingParticleObj.SetActive( false );
        }
    }

    public void ShowBathingEffect( GameObject bathingParticle, SortingGroup targetSortingGroup )
    {
        if (bathingParticleObj == null)
        {
            bathingParticleObj = ShowEffect( bathingParticle, bathingParticlePoint );
            bathingParticleTransform = bathingParticleObj.transform;
            bathingParticleSystem = bathingParticleObj.GetComponent<ParticleSystem>();
        }
        else
        {
            bathingParticleTransform.position = bathingParticlePoint.position;

            if (bathingParticleSystem != null)
            {
                if (bathingParticleSystem.isPlaying == false)
                {
                    bathingParticleSystem.Play();
                }
            }
            else
            {
                bathingParticleObj.SetActive( true );
            }
        }

        SetUpParticleSystemRendererSorting( bathingParticleObj, targetSortingGroup );
    }

    public void HideBathingEffect()
    {
        if (bathingParticleSystem != null)
        {
            bathingParticleSystem.Stop();
        }
        else if (bathingParticleObj != null)
        {
            bathingParticleObj.SetActive( false );
        }
    }

    public void ShowLoveEffect( GameObject loveParticle, SortingGroup targetSortingGroup )
    {
        GameObject _loveParticleObj = ShowEffect( loveParticle, loveParticlePoint, true, loveParticleDestroyDelay );
        SetUpParticleSystemRendererSorting( _loveParticleObj, targetSortingGroup );
    }

    private GameObject ShowEffect( GameObject particleObject, Transform particlePoint, bool autoDestroy = false, float destroyDelay = 0.0f )
    {
        GameObject _particleObj = Instantiate( particleObject );
        _particleObj.transform.position = particlePoint.position;

        if (autoDestroy == true)
        {
            Destroy( _particleObj, destroyDelay );
        }

        return _particleObj;
    }

    private void SetUpParticleSystemRendererSorting( GameObject particleObject, SortingGroup targetSortingGroup )
    {
        ParticleSystemRenderer[] _particleSystemRenderers = particleObject.GetComponentsInChildren<ParticleSystemRenderer>();

        for (int i = 0; i < _particleSystemRenderers.Length; i++)
        {
            ParticleSystemRenderer _particleSystemRenderer = _particleSystemRenderers[ i ];
            _particleSystemRenderer.sortingLayerName = targetSortingGroup.sortingLayerName;
            _particleSystemRenderer.sortingOrder = targetSortingGroup.sortingOrder + 1;
        }
    }
}
