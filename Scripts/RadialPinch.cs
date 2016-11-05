﻿using UnityEngine;
using System.Collections;
using Leap.Unity;
using System.Collections.Generic;

public class RadialPinch : MonoBehaviour
{
  public RigidHand mano;
  private Dictionary<Transform, Vector3> posIniziali;
  private Transform colliso, padre;

  public void OnValidate()
  {
    IHandModel ihm = gameObject.GetComponentInParent<IHandModel>();

    if (ihm != null)
      mano = (RigidHand)ihm;
  }

  public void Start()
  {
    // Calcolo le posizioni iniziali di tutti gli oggetti presenti nella scena. All'occorrenza utilizzerò questi valori per effettuare qualche controllo
    posIniziali = new Dictionary<Transform, Vector3>();
    Transform[] objs = FindObjectsOfType<Transform>();

    foreach (Transform obj in objs)
      posIniziali.Add(obj, new Vector3(obj.position.x, obj.position.y, obj.position.z));

    colliso = null;
  }

  public void OnTriggerEnter(Collider other)
  {
    if (colliso == null && other.tag != "Imprendibile")
    {
      padre = other.transform.parent;

      if (padre != null)
      {
        colliso = other.transform;
        SendMessageUpwards("StoAfferrando", true);
      }
    }
  }

  public void OnTriggerStay(Collider other)
  {
    if (colliso != null && mano.GetLeapHand().PinchStrength == 1)
    {
      Vector3 dir = colliso.position - padre.position, dirMano = transform.position - padre.position, nuovaPosizione = padre.position + Vector3.Project(dirMano, dir);

      if (Max(nuovaPosizione, posIniziali[colliso], padre.position))
        colliso.position = nuovaPosizione;
    }
  }

  public void OnTriggerExit(Collider other)
  {
    if (colliso != null)
    {
      colliso = null;
      padre = null;
      SendMessageUpwards("StoAfferrando", false);
    }
  }

  private bool Max(Vector3 a, Vector3 b, Vector3 center)
  {
    // Controllo se la lunghezza del punto a è maggiore rispetto alla lunghezza del punto b, rispetto a un punto centrale
    Vector3 p1 = a - center, p2 = b - center;

    return p1.magnitude > p2.magnitude;
  }
}