﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerPick : MonoBehaviour
{
    public RigidBodyMovement rbm;
    private Rigidbody body;
    public GameObject conch, cup, can, initialAnchor, tailOut, tailIn, pickParticles;
    public Transform newCapsulePos;
    private AudioSource playerAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        playerAudioSource = rbm.gameObject.GetComponent<AudioSource>();
        initialAnchor = transform.parent.Find("InitialAnchor").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (rbm.GetConchCount() == 0 && !tailOut.activeInHierarchy) {
            tailOut.SetActive(true);
            tailIn.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PickUpPoint")
        {
            GameObject target = other.gameObject;
            PickUpScript pus = target.GetComponent<PickUpScript>();
            if (pus != null) {
                playerAudioSource.Play();
                Instantiate(pickParticles, target.transform.position,Quaternion.identity);
                if (tailOut.activeInHierarchy) {
                    tailOut.SetActive(false);
                    tailIn.SetActive(true);
                    gameObject.transform.position = newCapsulePos.position;
                }
                GameObject toSpawn = null;
                switch (pus.GetRoot().tag) {
                    case "Conch": {
                            toSpawn = conch;
                            break;
                    }
                    case "Cup":
                        {
                            toSpawn = cup;
                            break;
                        }
                    case "Can":
                        {
                            toSpawn = can;
                            break;
                        }
                }
                Destroy(pus.GetRoot());
                GameObject lastShell = rbm.GetLastConch();
                Transform conchAnchor;
                if (lastShell != null) conchAnchor = lastShell.GetComponent<ConchScript>().GetAnchor().transform;
                else
                {
                    conchAnchor = initialAnchor.transform;
                }
                GameObject addedConch = Instantiate(toSpawn, conchAnchor.position, conchAnchor.rotation);
                addedConch.transform.parent = conchAnchor;
                rbm.AddConch(addedConch);                
            }
        }
    }

    

    public void DoPickUp()
    {

    }
}
