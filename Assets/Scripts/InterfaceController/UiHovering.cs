using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiHovering : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    GlobalController gc;
	
	void Start () {
        gc = GameObject.FindWithTag("GlobalController").GetComponent<GlobalController>();
	}

   
    public void OnPointerEnter(PointerEventData pointerEventData) {
        gc.setMouseOverUi(true);
    }


    public void OnPointerExit(PointerEventData pointerEventData) {
        gc.setMouseOverUi(false);
    }
}
