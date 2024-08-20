using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNPC : MonoBehaviour, Interactable
{
    public GameObject npcUI;
    public void Interaction(GameObject target)
    {
        Debug.Log("NPC");
        npcUI.SetActive(true);
    }
}
