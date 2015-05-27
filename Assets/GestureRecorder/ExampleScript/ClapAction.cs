using UnityEngine;
using System.Collections;

public class ClapAction: MonoBehaviour, GestureListener {

    // Use this for initialization


    GestureAnalyzer analyser;

    void Start()
    {
        analyser = new GestureAnalyzer();
        analyser.Mode = GestureAnalyzer.ModeEnum.ANALYSING;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnComplete(DynGesture gesture)
    {
        throw new System.NotImplementedException();
    }

    public void OnRecomplete(DynGesture gesture)
    {
        throw new System.NotImplementedException();
    }

    public void OnRelease(DynGesture gesture)
    {
        throw new System.NotImplementedException();
    }
}
