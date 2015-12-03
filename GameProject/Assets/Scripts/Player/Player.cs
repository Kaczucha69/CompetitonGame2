﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    CardResources zasoby;

	void Start () {
        zasoby = gameObject.GetComponent<CardResources>();
        zasoby.like = 0;
        zasoby.tweet = 0;
        zasoby.snap = 0;
	}

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            zasoby.like += 1;
            zasoby.tweet += 1;
            zasoby.snap += 1;
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("ChatMessage", PhotonTargets.All, "jup", "and jup!");
            Debug.Log("Adding +1 to all of Mana !");
        }
  
    
    }
    public CardResources getResources()
    {
        return zasoby;
    }
    [PunRPC]
    void ChatMessage(string a, string b)
    {
        Debug.Log("ChatMessage " + a + " " + b);
    }
}
