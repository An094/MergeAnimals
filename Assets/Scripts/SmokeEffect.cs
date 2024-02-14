using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeEffect : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(ReturnToPoolAfterTime());
    }

    public IEnumerator ReturnToPoolAfterTime()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
