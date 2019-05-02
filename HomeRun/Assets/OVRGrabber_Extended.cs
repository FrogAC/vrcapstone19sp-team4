using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRGrabber_Extended : OVRGrabber
{

    private void RemoveNullsFromGrabCandidates()
    {
        List<OVRGrabbable> toRemove = new List<OVRGrabbable>();
        foreach (OVRGrabbable grabbable in m_grabCandidates.Keys)
        {
            if (grabbable == null)
            {
                toRemove.Add(grabbable);
            }
        }
        foreach (OVRGrabbable grabbable in toRemove)
        {
            m_grabCandidates.Remove(grabbable);
        }
    }

    new protected virtual void GrabBegin()
    {
        RemoveNullsFromGrabCandidates();
        base.GrabBegin();
    }
}
