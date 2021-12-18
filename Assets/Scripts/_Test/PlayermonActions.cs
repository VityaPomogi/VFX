using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;

public class PlayermonActions : MonoBehaviour
{
    [SerializeField] private SortingGroup _sortingGroup = null;
    [SerializeField] private GameObject _self = null;
    [SerializeField] private PlayermonAnimations _animation = null;
    [SerializeField] private Transform meleeAttackerPoint = null;
    [SerializeField] private GameObject shadow = null;

    [Header("Battle Properties")]
    [SerializeField] private float _jumpStrength = 0.25f;
    [SerializeField] private float _moveTime = 1.0f;
    [SerializeField] private float _delayTime = 2.0f;


    private GameObject _targetEnemy = null;
    private Vector3 _originalPosition = Vector3.zero;
    private Vector3 _target = Vector3.zero;
    private float _distance = 0.0f;
    private string _defaultSortingLayerName = "Default";
    private int _defaultSortingOrder = 1;


    private string TargetAttackLayerName => _targetEnemy.GetComponent<SortingGroup>().sortingLayerName.Replace("Defend", "Attack");


    public Action onBeingIdle = null;
    public Action onGettingHit = null;
    public Action onAttackStarted = null;
    public Action onAttackEnded = null;

    private bool stayThereAfterAttack = false;

    void Awake()
    {
        if (_sortingGroup == null)
        {
            _sortingGroup = GetComponent<SortingGroup>();
        }
    }

    public void AttackTarget()
    {
        if (onAttackStarted != null)
        {
            onAttackStarted();
        }

        //_isMoving = true;
        _originalPosition = transform.position;
    //    _target = _targetEnemy.transform.position + ( ( ( isPlayer == true ) ? -1 : 1 ) * Vector3.right * 2.0f );
        _target = _targetEnemy.GetComponent<PlayermonActions>().meleeAttackerPoint.position;
        _distance = Vector3.Distance( _originalPosition, _target );
        _animation.PlayJumpAnimation(Attack, Jump);
    }

    private void Jump()
    {
        float _moveDuration = (_distance / 6.0f) * ( _moveTime - 0.2f ) + 0.2f;

        LeanTween.moveLocalY(_self, _distance * _jumpStrength, _moveDuration / 2.0f)
            .setLoopPingPong(1)
            .setEase(LeanTweenType.easeOutQuad);

        float _shadowScale = 1.0f - ( ( _distance / 6.0f ) * 0.4f );

        LeanTween.scale( shadow, new Vector3( _shadowScale, _shadowScale, 1.0f ), _moveDuration / 2.0f )
            .setLoopPingPong( 1 )
            .setEase( LeanTweenType.easeOutQuad);

        LeanTween.move(gameObject, _target, _moveDuration)
            .setOnStart(() => { SwitchSortingLayer(TargetAttackLayerName, _defaultSortingOrder); });
    }

    public void JumpToTargetPosition( Vector3 targetPoistion )
    {
        float _moveDuration = (_distance / 6.0f) * (_moveTime - 0.2f) + 0.2f;

        _originalPosition = transform.position;
        _target = targetPoistion;
        _distance = Vector3.Distance( _originalPosition, _target );

        LeanTween.moveLocalY( _self, _distance * _jumpStrength, _moveDuration / 2.0f )
            .setLoopPingPong( 1 )
            .setEase( LeanTweenType.easeOutQuad);

        float _shadowScale = 1.0f - ( ( _distance / 6.0f ) * 0.4f );

        LeanTween.scale( shadow, new Vector3( _shadowScale, _shadowScale, 1.0f ), _moveDuration / 2.0f )
            .setLoopPingPong( 1 )
            .setEase( LeanTweenType.easeOutQuad);

        LeanTween.move( gameObject, _target, _moveDuration);
    }

    public void Attack()
    {
        LeanTween.delayedCall( _delayTime, () =>
        {
            _animation.PlayAttackAnimation( () =>
            {
                if (stayThereAfterAttack == false)
                {
                    _animation.PlayJumpAnimation( () =>
                    {
                        if (onAttackEnded != null)
                        {
                            onAttackEnded();
                        }

                        PlayIdleAnimation();
                    },
                    BackToOriginalPosition
                    );
                }
            },
            _targetEnemy.GetComponent<PlayermonActions>().PlayGetHitAnimation
            );
        } );
    }

    private void BackToOriginalPosition()
    {
        float _moveDuration = (_distance / 6.0f) * (_moveTime - 0.2f) + 0.2f;

        LeanTween.moveLocalY(_self, _distance * _jumpStrength, _moveDuration / 2.0f)
            .setLoopPingPong(1)
            .setEase(LeanTweenType.easeOutQuad);

        float _shadowScale = 1.0f - ( ( _distance / 6.0f ) * 0.4f );

        LeanTween.scale( shadow, new Vector3( _shadowScale, _shadowScale, 1.0f ), _moveDuration / 2.0f )
            .setLoopPingPong( 1 )
            .setEase( LeanTweenType.easeOutQuad);

        LeanTween.move(gameObject, _originalPosition, _moveDuration)
            .setOnComplete(() => { SwitchSortingLayer(_defaultSortingLayerName, _defaultSortingOrder); /*_isMoving = false;*/ /*_animator.Play("New iddle Animation");*/ });
    }

    private void PlayIdleAnimation()
    {
    //    _animation.PlayIdleAnimation();

        if (onBeingIdle != null)
        {
            onBeingIdle();
        }
    }

    private void PlayGetHitAnimation()
    {
        if (onGettingHit != null)
        {
            onGettingHit();
        }

        _animation.PlayGetHitAnimation( PlayIdleAnimation );
    }

    public void SetTargetEnemy(GameObject target)
    {
        _targetEnemy = target;
    }

    public void SetDefaultSortingLayer(string sortingLayerName, int sortingOrder)
    {
        _defaultSortingLayerName = sortingLayerName;
        _defaultSortingOrder = sortingOrder;

        SwitchSortingLayer(_defaultSortingLayerName, _defaultSortingOrder);
    }

    public void SwitchSortingLayer(string sortingLayerName, int sortingOrder)
    {
        _sortingGroup.sortingLayerName = sortingLayerName;
        _sortingGroup.sortingOrder = sortingOrder;
    }

    public void SetStayThereAfterAttack( bool stayThereAfterAttack )
    {
        this.stayThereAfterAttack = stayThereAfterAttack;
    }
}
