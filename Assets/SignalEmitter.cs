using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SignalEmitter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class CustomMarker : UnityEngine.Timeline.Marker, INotification

{

    public PropertyName id { get { return new PropertyName(); } }

}
