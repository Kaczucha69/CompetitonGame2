﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using UnityEngine.UI;

public class DragZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    private Player player;
    private Card d;

    public void Start()
    {
        player = GameObject.Find("Gracz").GetComponent<Player>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (GameObject.Find("Player").GetComponent<whoseTurn>().isMyTurn) // is my Turn ?
        {
            d = eventData.pointerDrag.GetComponent<Card>();

            if (this.name != "EnemiesHand" && this.name != "EnemiesArchers" && this.name != "EnemiesMelee")
            {
                if (this.name == d.getProperties().type.ToString())
                    if (checkIfEnoughResources(player.getResources(), d.getResources()))
                    {
                        drainResources(player.getResources(), d.getResources());
                        GameObject.Find("Gracz").GetComponent<Player>().cardsInHand--;
                        d.parentToReturnTo = this.transform;

                        PhotonView photonView = PhotonView.Get(this);
                        photonView.RPC("updateMana", PhotonTargets.Others, d.getResources().like, d.getResources().snap, d.getResources().tweet);
                        photonView.RPC("ChangeCardParent", PhotonTargets.Others, this.name, d.GetComponent<Properties>().CardId);
                    }
            }
        }
        else
        {
            Transform panel = GameObject.Find("Canvas").transform.FindChild("Waiting");
            panel.GetComponent<Text>().text = "Nie twoja tura !";
        }
    }

    [PunRPC]
    void updateMana(int l, int s, int t)
    {
        GameObject.Find("Enemy").GetComponent<EnemyStats>().drainMana(l, s, t);
    }

    public bool checkIfEnoughResources(CardResources player, CardResources karta)
    {
        if (player.like >= karta.like && player.snap >= karta.snap && player.tweet >= karta.tweet)
            return true;
        return false;
    }
    public void drainResources(CardResources player, CardResources karta)
    {
        player.like -= karta.like;
        player.snap -= karta.snap;
        player.tweet -= karta.tweet;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log(gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log(gameObject.name);
    }

    [PunRPC]
    void ChangeCardParent(string panelName, int cardID)
    {
        panelName = "Enemies" + panelName;
        var gameWorld = GameObject.Find("GameWorld");
        GameObject panel = GameObject.Find(panelName);

        GameObject x = gameWorld.GetComponent<AssignID>().allCards.Find(card => card.GetComponent<Properties>().CardId.Equals(cardID));
        x.transform.SetParent(panel.transform, true);
        x.GetComponent<Card>().parentToReturnTo = panel.transform;
        x.transform.Find("BackOfCard").gameObject.SetActive(false);
    }
}
