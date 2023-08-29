using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScreen : MonoBehaviour
{

    public GameObject Skin;

    public void togle()
    {
        if (Skin.gameObject.activeSelf)
        {
            Skin.gameObject.SetActive(false);
            return;
        }
        Skin.gameObject.SetActive(true);
    }
}
