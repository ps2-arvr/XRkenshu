using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundDicePrefab : MonoBehaviour
{
    public AudioClip DiceSoundEffect;
    public AudioSource DiceSound;
    // Start is called before the first frame update
    void Start()
    {
      DiceSound =  GetComponent<AudioSource>();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        //衝突した際に音を流す関数
    public void OnCollisionEnter(Collision collision)
    {
        PlaySound();
    }

    public void PlaySound()
    {
        DiceSound.PlayOneShot(DiceSoundEffect);
    }
}
