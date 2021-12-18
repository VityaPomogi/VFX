using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas _canvas = null;
    [SerializeField] private RectTransform _itemSlot = null;
    [SerializeField, Range(0.1f, 1.0f)] private float _alphaOnDrag = 0.75f;
    [SerializeField] private float _circleCastRadius = 0.5f; 
    [SerializeField] private FillBarProgress _fillBarProgress = null;

    [SerializeField] private AudioClip _interactSFX = null;

    private PointerEventData _lastPointerData = null;
    private float _alphaEndDrag = 0.0f;

    private RectTransform _rectTransfrom = null;
    private CanvasGroup _canvasGroup = null;
    private ItemProperties _itemProperties = null;
    private SpaceDenPlayermon _target = null;

    void Awake()
    {
        _rectTransfrom = this.GetComponent<RectTransform>();
        _canvasGroup = this.GetComponent<CanvasGroup>();
        _itemProperties = this.GetComponent<ItemProperties>();
        _alphaEndDrag = _canvasGroup.alpha;
    }

    private void OnEndDragAction()
    {
        //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        RaycastHit2D[] _hits = Physics2D.CircleCastAll( Camera.main.ScreenToWorldPoint( Input.mousePosition ), _circleCastRadius, Vector2.zero );

        for (int i = 0; i < _hits.Length; i++)
        {
            RaycastHit2D _hit = _hits[ i ];
            if (_hit.collider != null)
            {
                SpaceDenPlayermon _spaceDenPlayermon = _hit.collider.gameObject.GetComponent<SpaceDenPlayermon>();

                if (_target == _spaceDenPlayermon)
                {
                    switch (_itemProperties.Type)
                    {
                        case ItemType.Feed:
                            _target.StopFeeding();
                            break;

                        case ItemType.Bath:
                            _target.StopBathing();
                            break;

                        case ItemType.Play:
                            _target.StopPlaying();
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }

    private void OnDragAction()
    {
        //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        RaycastHit2D[] _hits = Physics2D.CircleCastAll( Camera.main.ScreenToWorldPoint( Input.mousePosition ), _circleCastRadius, Vector2.zero );

        for (int i = 0; i < _hits.Length; i++)
        {
            RaycastHit2D _hit = _hits[ i ];
            if (_hit.collider != null)
            {
                SpaceDenPlayermon _spaceDenPlayermon = _hit.collider.gameObject.GetComponent<SpaceDenPlayermon>();

                if (_target != null)
                {
                    if (_target != _spaceDenPlayermon)
                    {
                        switch (_itemProperties.Type)
                        {
                            case ItemType.Feed:
                                _target.StopFeeding();
                                break;

                            case ItemType.Bath:
                                _target.StopBathing();
                                break;

                            case ItemType.Play:
                                _target.StopPlaying();
                                break;

                            default:
                                break;
                        }
                    }
                }

                _target = _spaceDenPlayermon;
                if (_target != null)
                {
                    SpaceDenPlayermon.RequestedAction _requestedAction = _target.GetCurrentRequestedAction();

                    switch (_itemProperties.Type)
                    {
                        case ItemType.Feed:

                            _target.StartFeeding();

                            if (_requestedAction == SpaceDenPlayermon.RequestedAction.FEED)
                            {
                                _fillBarProgress.SetTarget( _target, ItemType.Feed );
                            }

                            break;

                        case ItemType.Bath:

                            _target.StartBathing();

                            if (_requestedAction == SpaceDenPlayermon.RequestedAction.BATH)
                            {
                                _fillBarProgress.SetTarget( _target, ItemType.Bath );
                            }

                            break;

                        case ItemType.Play:

                            _target.StartPlaying();

                            if (_requestedAction == SpaceDenPlayermon.RequestedAction.PLAY)
                            {
                                _fillBarProgress.SetTarget( _target, ItemType.Play );
                            }

                            break;

                        default:
                            break;
                    }
                }
            }
            else
            {
                if (_target != null)
                {
                    switch (_itemProperties.Type)
                    {
                        case ItemType.Feed:
                            _target.StopFeeding();
                            break;

                        case ItemType.Bath:
                            _target.StopBathing();
                            break;

                        case ItemType.Play:
                            _target.StopPlaying();
                            break;

                        default:
                            break;
                    }

                    _target = null;
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastPointerData = eventData;

        _canvasGroup.alpha = _alphaOnDrag;
        _canvasGroup.blocksRaycasts = false;
        _target = null;
        //AudioManager.instance.Play("InteractiveObjectSFX");
        SoundManager.Instance.PlaySoundEffect(_interactSFX);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransfrom.anchoredPosition += eventData.delta / _canvas.scaleFactor;

        OnDragAction();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDrag();
    }

    public void OnEndDrag()
    {
        _canvasGroup.alpha = _alphaEndDrag;
        _canvasGroup.blocksRaycasts = true;
        _rectTransfrom.anchoredPosition = _itemSlot.anchoredPosition;

        OnEndDragAction();
    }

    public void CancelDrag()
    {
        if (_lastPointerData != null)
        {
            _lastPointerData.pointerDrag = null;
        }

        OnEndDrag();
    }

    public FillBarProgress GetFillBarProgress()
    {
        return _fillBarProgress;
    }
}
