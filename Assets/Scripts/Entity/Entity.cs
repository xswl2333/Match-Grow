using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Entity : MonoBehaviour
{


}

public abstract class Entity<T> : Entity where T : Entity<T>
{


}