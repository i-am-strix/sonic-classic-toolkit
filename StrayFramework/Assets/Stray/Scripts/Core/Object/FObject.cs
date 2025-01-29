using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Stray.Core;

namespace Stray.Physical
{
    public class FObject : MonoBehaviour
    {
        // public UnityEvent<Player> OnEnter = new();
        // public UnityEvent<Player> OnStay = new();
        // public UnityEvent<Player> OnExit = new();

        // public virtual void EOnEnter(Player player)
        // {
        //     OnEnter.Invoke(player);
        // }
        // public virtual void EOnStay(Player player)
        // {
        //     OnStay.Invoke(player);
        // }
        // public virtual void EOnExit(Player player)
        // {
        //     OnExit.Invoke(player);
        // }

        // // Events
        // public void OnTriggerEnter(Collider other)
        // {
        //     if (other.CompareTag("Player")) EOnEnter(other.GetComponent<Player>());
        // }
        // public void OnTriggerStay(Collider other)
        // {
        //     if (other.CompareTag("Player")) EOnEnter(other.GetComponent<Player>());
        // }
        // public void OnTriggerExit(Collider other)
        // {
        //     if (other.CompareTag("Player")) EOnEnter(other.GetComponent<Player>());
        // }
    }
}
