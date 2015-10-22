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
    [Serializable]
    public enum MutationPolicy
    {
        MoveToTop,
        MoveToBottom
    }

    [ReactNode]
    public class MutatingSelector : BranchNode
    {
        [ReactVar("Mutation")]
        public MutationPolicy
            mutation;
        public override IEnumerator<NodeResult> NodeTask ()
        {
            //Succeed if one node succeeds.
            foreach (var child in ActiveChildren().ToArray()) {
                var task = child.GetNodeTask ();
                while (task.MoveNext ()) {
                    var t = task.Current;
                    if (t == NodeResult.Continue) {
                        yield return NodeResult.Continue;
                    } else if (t == NodeResult.Failure) {
                        yield return NodeResult.Continue;
                        break;
                    } else if (t == NodeResult.Success) {
                        children.Remove (child);
                        if (mutation == MutationPolicy.MoveToTop)
                            children.Insert (0, child);
                        else
                            children.Add (child);
                        yield return NodeResult.Success;
                        yield break;
                    }
                }
            }
            yield return NodeResult.Failure;
        }

        public override string ToString ()
        {
            return "MutatingSelector";
        }
#if UNITY_EDITOR
        public new static string GetHelpText ()
        {
            return "Just like a normal selector, except that the first child that succeeds is also moved into first or last place.";
        }			
#endif		
    }

}







