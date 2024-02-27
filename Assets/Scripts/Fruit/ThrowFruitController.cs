using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrowFruitController : MonoBehaviour
{
    public static ThrowFruitController instance;

    public GameObject CurrentFruit { get; set; }
    [SerializeField] private Transform _fruitTransform;
    [SerializeField] private Transform _parentAfterThrow;
    [SerializeField] private FruitSelector _selector;
    [SerializeField] private float minDurationToThrow = 1f;

    private float throwTimer = 0f;
    private PlayerController _playerController;

    private Rigidbody2D _rb;
    private PolygonCollider2D _collider;

    public Bounds Bounds { get; private set; }

    private const float EXTRA_WIDTH = 0.03f;

    public bool CanThrow { get; set; } = true;

    // for detection whether touch over UI
    private PointerEventData _eventDataCurrentPosition;
    private List<RaycastResult> _result;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();

        SpawnAFruit(_selector.PickRandomFruitForThrow());
    }

    private void Update()
    {
        throwTimer += Time.deltaTime;

        if (throwTimer > minDurationToThrow )
        {
            bool isOverUI = IsOverUI();
            if (Application.isMobilePlatform && UserInput.WasReleasedThisFrame && !isOverUI && CanThrow
            || !Application.isMobilePlatform && UserInput.IsThrowPressed && CanThrow)
            {
                GameManager.instance.PlaySFXWhenThrow();
                SpriteIndex index = CurrentFruit.GetComponent<SpriteIndex>();
                Quaternion rot = CurrentFruit.transform.rotation;

                GameObject go = Instantiate(FruitSelector.instance.Fruits[index.Index], CurrentFruit.transform.position, rot);
                go.transform.SetParent(_parentAfterThrow);

                Destroy(CurrentFruit);
                CanThrow = false;

                throwTimer = 0f;
            }
        }
        
    }

    public void SpawnAFruit(GameObject fruit)
    {
        GameObject go = Instantiate(fruit, _fruitTransform);
        CurrentFruit = go;

        _collider = CurrentFruit.GetComponent<PolygonCollider2D>();
        if(_collider != null)
        {
            Bounds = _collider.bounds;
        }
        _playerController.ChangeBoundary(EXTRA_WIDTH);
    }

    private bool IsOverUI()
    {
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = UserInput.TouchPosition };
        _result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _result);
        return _result.Count > 0;
    }
}
