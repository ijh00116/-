using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleOnOffActive : MonoBehaviour
{
    ParticleSystem particle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.gameObject.activeInHierarchy)
        {
            if (particle.isPlaying == false)
            {
                this.gameObject.SetActive(false);
            }
        }
        
    }
}
