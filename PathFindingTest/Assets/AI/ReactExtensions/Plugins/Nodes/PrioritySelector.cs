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
    public class PrioritySelector : BranchNode {
        public override IEnumerator<NodeResult> NodeTask() {
            return NodeTask(null);
        }

        private IEnumerator<NodeResult> NodeTask(BaseNode endChild = null) {
            foreach (var child in ActiveChildren()) {
                if (child == endChild) break;
                var task = child.GetNodeTask();
                while (task.MoveNext()) {
                    var t = task.Current;
                    if (t == NodeResult.Continue) {
                        yield return t;
                        var newTask = NodeTask(child);
                        while (newTask.MoveNext()) {
                            if (newTask.Current == NodeResult.Failure) continue;
                            yield return newTask.Current;
                        }
                    } else if (t == NodeResult.Failure) {
                        break;
                    } else if (t == NodeResult.Success) {
                        yield return t;
                        yield break;
                    }
                }
            }
            yield return NodeResult.Failure;
        }

        public override string ToString() {
            return "PrioritySelector";
        }
#if UNITY_EDITOR
        public new static string GetHelpText() {
            return "Like normal selector but will always try to do things with highest priority, even if this would make him stop executing current behavior.";
        }
#endif
    }

}