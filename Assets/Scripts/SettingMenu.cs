using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SettingsMenu : MonoBehaviour
{
    [Header("space between menu items")]
    [SerializeField] Vector2 spacing;

    [Space]
    [Header("Main button rotation")]
    [SerializeField] float rotationDuration;
    [SerializeField] Ease rotationEase;

    [Space]
    [Header("Animation")]
    [SerializeField] float expandDuration;
    [SerializeField] float collapseDuration;
    [SerializeField] Ease expandEase;
    [SerializeField] Ease collapseEase;

    [Space]
    [Header("Fading")]
    [SerializeField] float expandFadeDuration;
    [SerializeField] float collapseFadeDuration;

    Button mainButton;
    SettingsMenuItem[] menuItems;

    [SerializeField] Text speedText;
    //is menu opened or not
    bool isExpanded = false;

    Vector2 mainButtonPosition;
    int itemsCount;

    void Start()
    {
        //add all the items to the menuItems array
        itemsCount = transform.childCount - 1;
        menuItems = new SettingsMenuItem[itemsCount];
        for (int i = 0; i < itemsCount; i++)
        {
            // +1 to ignore the main button
            menuItems[i] = transform.GetChild(i + 1).GetComponent<SettingsMenuItem>();
        }

        mainButton = transform.GetChild(0).GetComponent<Button>();
        mainButton.onClick.AddListener(ToggleMenu);
        //SetAsLastSibling () to make sure that the main button will be always at the top layer
        mainButton.transform.SetAsLastSibling();

        mainButtonPosition = mainButton.GetComponent<RectTransform>().anchoredPosition;

        //set all menu items position to mainButtonPosition
        ResetPositions();
    }

    void ResetPositions()
    {
        for (int i = 0; i < itemsCount; i++)
        {
            menuItems[i].rectTrans.anchoredPosition = mainButtonPosition;
        }
    }

    void ToggleMenu()
    {
        isExpanded = !isExpanded;

        if (isExpanded)
        {
            //menu opened
            for (int i = 0; i < itemsCount; i++)
            {
                menuItems[i].rectTrans.DOAnchorPos(mainButtonPosition + spacing * (i + 1), expandDuration).SetEase(expandEase);
                //Fade to alpha=1 starting from alpha=0 immediately
                menuItems[i].img.DOFade(1f, expandFadeDuration).From(0f);
            }

            mainButton.transform
              .DORotate(Vector3.forward * 90f, rotationDuration)
              .From(Vector3.zero)
              .SetEase(rotationEase);
        }
        else
        {
            //menu closed
            for (int i = 0; i < itemsCount; i++)
            {
                menuItems[i].rectTrans.DOAnchorPos(mainButtonPosition, collapseDuration).SetEase(collapseEase);
                //Fade to alpha=0
                menuItems[i].img.DOFade(0f, collapseFadeDuration);
            }

            mainButton.transform
             .DORotate(Vector3.zero, rotationDuration)
             .From(Vector3.forward * 90f)
             .SetEase(rotationEase);
        }

       
    }

    public void OnItemClick(int index)
    {
        //here you can add you logic 
        switch (index)
        {
            case 0:
                GameManager.instance.OpenReplayPopup();
                break;
            case 1:
                GameManager.instance.OpenExitPopup();
                break;
            case 2:
                GameManager.instance.OpenMusicSettingPopup();
                break;

        }
    }

    void OnDestroy()
    {
        //remove click listener to avoid memory leaks
        mainButton.onClick.RemoveListener(ToggleMenu);
    }

    public void ToggleSpeed()
    {
        if (Time.timeScale == 2f)
        {
            Time.timeScale = 1f;
            speedText.text = "x1";
        }
        else
        {
            Time.timeScale = 2f;
            speedText.text = "x2";
        }
    }
}