using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace React
{
    [ReactNode]
    public class WithAniState : DecoratorNode
    {
        [ReactVar]
        public string fieldName;
        [ReactVar]
        public bool value;
        
        int fieldID;
        bool previousState;

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
            if (Child == null) {
                yield return NodeResult.Failure;
                yield break;
            }
            previousState = Animator.GetBool(fieldID);
            Animator.SetBool(fieldID, value);

            var task = Child.GetNodeTask ();
            while (task.MoveNext ()) {
                if(task.Current != NodeResult.Continue) {
                    Animator.SetBool(fieldID, previousState);
                }
                yield return task.Current;
            }
        }

        public override string ToString ()
        {
            return string.Format("WithAnim {0} {1}", fieldName, value);
        }

        public override void Abort ()
        {
            Animator.SetBool(fieldID, previousState);
            base.Abort();
        }

#if UNITY_EDITOR
        public new static string GetHelpText ()
        {
            return "Set some animator flag for the child, returns to previous state when done.";
        }           
#endif
    }
    


}







