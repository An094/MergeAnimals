using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public bool IsMovingToRight;
    public float Speed;

    // Update is called once per frame
    void Update()
    {
        Vector2 Bounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        if (transform.position.x < (-Bounds.x - 2f) && !IsMovingToRight || transform.position.x > (Bounds.x + 2f) && IsMovingToRight)
        {
            //Destroy(this.gameObject);
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
        else
        {
            if (IsMovingToRight)
            {
                transform.position = new Vector2(transform.position.x + Speed * Time.deltaTime, transform.position.y);
            }
            else
            {
                transform.position = new Vector2(transform.position.x - Speed * Time.deltaTime, transform.position.y);
            }
        }
    }
}
