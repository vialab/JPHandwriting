using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSocketTagInteractor : XRSocketInteractor
{
   // XRSocketInteractor, but for specific tags instead of layers
   [SerializeField] private string _targetTag;

   public override bool CanHover(IXRHoverInteractable interactable) {
      return base.CanHover(interactable) && interactable.transform.CompareTag(_targetTag);
   }

   public override bool CanSelect(IXRSelectInteractable interactable) {
      return base.CanSelect(interactable) && interactable.transform.CompareTag(_targetTag);
   }
}
