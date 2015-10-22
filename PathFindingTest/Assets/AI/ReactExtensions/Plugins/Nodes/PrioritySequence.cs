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
    public class PrioritySequence : BranchNode {
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
                            if (newTask.Current == NodeResult.Success) continue;
                            yield return newTask.Current;
                        }
                    } else if (t == NodeResult.Failure) {
                        yield return t;
                        yield break;
                    } else if (t == NodeResult.Success) {
                        break;
                    }
                }
            }
            yield return NodeResult.Success;
        }

        public override string ToString() {
            return "PrioritySequence";
        }
#if UNITY_EDITOR
        public new static string GetHelpText() {
            return "Like normal sequence but will always try to do things with highest priority, even if this would make him stop executing current behavior.";
        }
#endif
    }

}