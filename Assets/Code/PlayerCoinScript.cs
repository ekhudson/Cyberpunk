﻿using UnityEngine;
using System.Collections;

public class PlayerCoinScript : MonoBehaviour 
{
    public bool FreshCoin = true;

    private const float kCurveScaleAmount = -0.05f;
    private AudioSource mAudioSource;

    public void PlayLaunchSound()
    {
        if (mAudioSource == null)
        {
            return;
        }

        mAudioSource.Play();
    }

    private void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    public void FixedUpdate()
    {
        if (CoinUserInterfaceManager.Instance.DoCurving && rigidbody.angularVelocity != Vector3.zero && rigidbody.velocity != Vector3.zero)
        {
            rigidbody.AddForce(kCurveScaleAmount*Vector3.Cross(rigidbody.velocity,rigidbody.angularVelocity), ForceMode.Force);
        }
    }

	public void OnCollisionEnter(Collision collision)
    {
        EventManager.Instance.Post(new PlayerCoinImpactEvent(this, this, collision));
    }
}
