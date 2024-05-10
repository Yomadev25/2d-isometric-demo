using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyCombat : MonoBehaviour
{
    public abstract void Attack(EnemyController context);
}
