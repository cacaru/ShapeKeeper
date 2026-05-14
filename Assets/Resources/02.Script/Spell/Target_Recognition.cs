using System;
using System.Collections.Generic;
using UnityEngine;


public class Target_Recognition : MonoBehaviour
{
    List<ISpell_Recognition_Observer> observers = new();

    public void Add_Observer(ISpell_Recognition_Observer observer) {
        if (!observers.Contains(observer))
            observers.Add(observer);
    }

    public void Remove_Observer(ISpell_Recognition_Observer observer) {
        if (observers.Contains(observer)) {
            observers.Remove(observer);
        }
    }

    public void Remove_Current_Tower() {
        current = null;
    }

    private GameObject current = null;

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Tower")) {
            if (current != collision.gameObject) {
                current = collision.gameObject;
                foreach (var ob in observers) {
                    ob.Tower_Lock_On(current);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject == current) {
            current = null;
            foreach (var ob in observers) {
                ob.Tower_Lock_Off();
            }
        }
    }


}
