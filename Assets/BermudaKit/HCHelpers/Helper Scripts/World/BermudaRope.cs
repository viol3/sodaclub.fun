using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class BermudaRope : MonoBehaviour
    {
        //It works with Character Joints. You need to prepare
        // a skinned mesh rope with Blender/Maya/3dsmax
        // You can watch here : https://www.youtube.com/watch?v=Cecp4a7FsTU
        [SerializeField] private Transform _headNode;
        [SerializeField] private Transform _tailNode;
        void Awake()
        {
            AttachRopeSystem(_tailNode);
        }

        void AttachRopeSystem(Transform node)
        {

            Rigidbody nodeRB = node.GetComponent<Rigidbody>();
            if (nodeRB == null)
            {
                nodeRB = node.gameObject.AddComponent<Rigidbody>();
            }
            SphereCollider collider = node.gameObject.AddComponent<SphereCollider>();
            collider.radius = 0.1f;
            nodeRB.mass = 0.1f;
            nodeRB.drag = 0.2f;
            nodeRB.angularDrag = 0.2f;
            //nodeRB.constraints = RigidbodyConstraints.FreezeRotationY;
            if (node.childCount > 0 && node != _tailNode)
            {
                node.GetChild(0).GetComponent<CharacterJoint>().connectedBody = nodeRB;
            }

            if (node == _headNode)
            {
                nodeRB.isKinematic = true;
                nodeRB.useGravity = false;
                return;
            }
            else
            {
                CharacterJoint nodeSJ = node.gameObject.AddComponent<CharacterJoint>();
                //nodeSJ.spring = _boneSpring;
                //nodeSJ.damper = _boneDamper;
                nodeSJ.enableCollision = true;
                nodeSJ.enableProjection = true;
                AttachRopeSystem(node.parent);
            }
        }

    }
}