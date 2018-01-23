using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchActivateDeactivate : MonoBehaviour {

    public GameObject obj1;
//    public GameObject obj2;

    public void action()
    {
        obj1.SetActive(!obj1.activeInHierarchy);
//        obj2.SetActive(!obj2.activeInHierarchy);
    }
}
