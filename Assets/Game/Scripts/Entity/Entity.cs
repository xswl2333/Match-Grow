using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Entity : MainController
{
    public virtual void Destroy()
    {
        UnityEngine.Object.Destroy(gameObject);
    }

    public virtual void SetActive(bool isShow)
    {
        gameObject.SetActive(isShow);
    }
}

public abstract class Entity<T> : Entity where T : Entity<T>
{
  

}