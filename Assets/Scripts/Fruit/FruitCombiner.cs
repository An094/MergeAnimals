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
        _layerIndex = gameObject.layer;
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
                           

                            Vector3 combinedFruitPos = (transform.position + collision.transform.position) / 2f;// + new Vector3(0f, 0.02f, 0f);
                            //GameObject go = Instantiate(SpawnCombinedFruit(_info.FruitIndex), GameManager.instance.transform);
                            //go.transform.position = combinedFruitPos;
                            GameObject go = Instantiate(SpawnCombinedFruit(_info.FruitIndex), combinedFruitPos, Quaternion.identity);

                            ColliderInformer informer = go.GetComponent<ColliderInformer>();
                            if (informer != null)
                            {
                                informer.WasCombinedIn = true;
                            }

                            GameObject appearEffect = ObjectPoolManager.SpawnObject(AppearEffect, combinedFruitPos, Quaternion.identity);
                            //appearEffect.transform.localScale = go.transform.localScale;
                            Destroy(collision.gameObject);
                            Destroy(gameObject);

                            //GameManager.instance.CombineObject(gameObject, collision.gameObject, _info.FruitIndex);
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
