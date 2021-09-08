using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadManager : MonoBehaviour
{
    Transform anchorPoint = null;
    private bool isAnchored = false;
    public Transform trackingSpace;
    private Transform trackingSpaceParent;
    public Transform centerEyeAnchor;
    private Transform centerEyeParent;

    private Vector3 m_anchorOffsetPosition;
    private Quaternion m_anchorOffsetRotation;

    private Quaternion controllerInitialRot;

    private bool isGrabbedByRight;

    void Awake()
    {
        m_anchorOffsetPosition = transform.localPosition;
        m_anchorOffsetRotation = transform.localRotation;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnchored)
        {
            Vector3 handPosition = Vector3.zero;
            if (isGrabbedByRight)
            {
                handPosition = trackingSpace.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch));
            }
            else
            {
                handPosition = trackingSpace.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch));
            }
            Vector3 headsetPosition = trackingSpace.TransformPoint(OVRManager.tracker.GetPose().position);
            Vector3 distFromAnchor = handPosition - headsetPosition;

            transform.position = anchorPoint.position - distFromAnchor;

            // none of these feel good
            //transform.rotation = Quaternion.Inverse(anchorPoint.rotation) * OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
            //transform.rotation = anchorPoint.rotation;
            //transform.rotation = Quaternion.Inverse(controllerInitialRot); * OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
        }
    }

    public void setAnchor(Transform theHand, Transform theAnchor, bool thisIsRight)
    { 
        anchorPoint = theHand;
        GetComponent<Rigidbody>().isKinematic = true;
        isAnchored = true;
        isGrabbedByRight = thisIsRight;
        controllerInitialRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);

        //trackingSpaceParent = trackingSpace.parent;
        //trackingSpace.SetParent(theAnchor);
        //centerEyeParent = centerEyeAnchor.parent;
        //centerEyeAnchor.SetParent(transform);
    }

    public void releaseAnchor()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        isAnchored = false;
        //trackingSpace.SetParent(trackingSpaceParent);
        //trackingSpace.localPosition = Vector3.zero;
        //centerEyeAnchor.SetParent(centerEyeParent);
        //centerEyeAnchor.localPosition = Vector3.zero;


        // calculate the "escape velocities"
        // these lines were taken from OVRGrabber.GrabEnd()
        OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch), orientation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) };
        OVRPose offsetPose = new OVRPose { position = m_anchorOffsetPosition, orientation = m_anchorOffsetRotation };
        localPose = localPose * offsetPose;
        OVRPose OVRTrackingSpace = transform.ToOVRPose() * localPose.Inverse();
        Vector3 linearVelocity = OVRTrackingSpace.orientation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        Vector3 angularVelocity = OVRTrackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.LTouch);
        GetComponent<Rigidbody>().velocity = -linearVelocity;
        GetComponent<Rigidbody>().angularVelocity = angularVelocity/100.0f;
    }
}
