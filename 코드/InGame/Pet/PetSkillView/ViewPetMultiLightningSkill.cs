using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ViewPetMultiLightningSkill : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;
    public void Fire()
    {
        particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
        particle.Play();
    }
}
