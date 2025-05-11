using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface ITurnActor { // must define what can be done when taking turn
    IEnumerator TakeTurn();
}
