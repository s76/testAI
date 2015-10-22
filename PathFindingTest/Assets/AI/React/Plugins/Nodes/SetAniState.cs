using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

//TODO: Add rest of mecanim state changes.
namespace React
{
    [ReactNode]
    public class SetAniState : LeafNode
    {
        [ReactVar]
        public string fieldName;
        [ReactVar]
        public bool value;

        int fieldID;

        Animator animator;

        Animator Animator {
            get {
                if(animator == null) {
                    animator = gameObject.GetComponent<Animator>();
                    fieldID = Animator.StringToHash(fieldName);
                }
                return animator;
            }
        }

        public override IEnumerator<NodeResult> NodeTask ()
        {
            Animator.SetBool(fieldID, value);
            yield return NodeResult.Success;
        }

        public override string ToString ()
        {
            return string.Format("SetAnim {0} {1}", fieldName, value);
        }

#if UNITY_EDITOR
        public new static string GetHelpText ()     {
            return "Change a bool state on the Animator component.";
        }           
#endif
    }
    


}







