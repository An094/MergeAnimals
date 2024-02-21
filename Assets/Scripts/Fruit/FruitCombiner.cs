using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitCombiner : MonoBehaviour
{

    [SerializeField] private GameObject AppearEffect;
    public bool WasCombined { get; set;}
    private int _layerIndex;

    private FruitInfo _info;

    private void Awake()
    {
        _info = GetComponent<FruitInfo>();
       
    }

    private void OnEnable()
    {
        _layerIndex = gameObject.layer;     
        WasCombined = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == _layerIndex)
        {
            FruitInfo info = collision.gameObject.GetComponent<FruitInfo>();
            if (info != null)
            {
                if (info.FruitIndex == _info.FruitIndex)
                {
                    int thisID = gameObject.GetInstanceID();
                    int otherID = collision.gameObject.GetInstanceID();
                    FruitCombiner otherFruitCombiner = collision.gameObject.GetComponent<FruitCombiner>();

                    if (thisID > otherID && !WasCombined && !otherFruitCombiner.WasCombined)
                    {
                        WasCombined = true;
                        otherFruitCombiner.WasCombined = true;
                        GameManager.instance.IncreaseScore(_info.PointsWhenAnnihilated);

                        if (_info.FruitIndex == FruitSelector.instance.Fruits.Length -1)
                        {
                            Destroy(collision.gameObject);
                            Destroy(gameObject);
                        }

                        else
                        {
                            MergeInfo mergeInfo = new MergeInfo() { firstObj = gameObject, secondObj = collision.gameObject, currentIndex = _info.FruitIndex };
                            GameManager.mergeInfos.Add(mergeInfo);
                        }
                    }
                }
            }
        }
    }

    private GameObject SpawnCombinedFruit(int index)
    {
        GameObject go = FruitSelector.instance.Fruits[index + 1];
        return go;
    }
}
