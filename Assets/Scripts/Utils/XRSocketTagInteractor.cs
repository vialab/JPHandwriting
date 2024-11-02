using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// XRSocketInteractor, but only allows specific tags.
/// </summary>
public class XRSocketTagInteractor : XRSocketInteractor
{
   [SerializeField] private string _targetTag;

   public override bool CanHover(IXRHoverInteractable interactable) {
      return base.CanHover(interactable) && interactable.transform.CompareTag(_targetTag);
   }

   public override bool CanSelect(IXRSelectInteractable interactable) {
      return base.CanSelect(interactable) && interactable.transform.CompareTag(_targetTag);
   }
}
