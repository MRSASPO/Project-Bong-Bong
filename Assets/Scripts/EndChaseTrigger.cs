﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class EndChaseTrigger : MonoBehaviour {
    public UnityEvent ue = new UnityEvent();

    void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Lethal")) {
            ue.Invoke();
        }
    }
}
