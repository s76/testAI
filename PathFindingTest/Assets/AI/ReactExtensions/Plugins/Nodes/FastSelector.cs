using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace React {
    [ReactNode]
    public class FastSelector : BranchNode {
        public override IEnumerator<NodeResult> NodeTask() {
            //Succeed if one node succeeds.
            foreach (var child in ActiveChildren()) {
                var task = child.GetNodeTask();
                while (task.MoveNext()) {
                    var t = task.Current;
                    if (t == NodeResult.Continue) {
                        yield return NodeResult.Continue;
                    } else if (t == NodeResult.Failure) {
                        break;
                    } else if (t == NodeResult.Success) {
                        yield return NodeResult.Success;
                        yield break;
                    }
                }
            }
            yield return NodeResult.Failure;
        }

        public override string ToString() {
            return "FastSelector";
        }
#if UNITY_EDITOR
        public new static string GetHelpText() {
            return "Runs childs in order in one time step. Succeeding on the first child that succeeds, and failing if none succeed.";
        }
#endif
    }

}