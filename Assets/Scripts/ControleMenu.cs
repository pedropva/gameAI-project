using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleMenu : MonoBehaviour {

    public void IniciarJogo()
    {
        // Carrega cena da fase 1
    }

    public void FecharGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
