using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System;

[CustomEditor(typeof(Reactable))]
[CanEditMultipleObjects]
public class ReactableEditor : InspectorEditor 
{

    MonoScript[] behaviours = new MonoScript[0];

    public void OnEnable() {
        behaviours = ((Reactable)target).behaviourTypes
            .Select(t => Resources
                .FindObjectsOfTypeAll(typeof(MonoScript))
                .Cast<MonoScript>()
                .First(c => c.hideFlags == 0 && c.GetClass() != null && c.GetClass().Name == t))
            .ToArray();
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        int size = IntField("Size", behaviours.Length);
        if (size != behaviours.Length) {
            Array.Resize<MonoScript>(ref behaviours, size);
            EditorUtility.SetDirty(target);
        }

        for(int i=0; i < behaviours.Length; i++) {
            var newB = ObjectView(behaviours[i], typeof(MonoScript), "Element: ");
            if (newB != behaviours[i]) {
                behaviours[i] = (MonoScript)newB;
                EditorUtility.SetDirty(target);
            }
        }


        if (Button("Serialize type names")) {
            foreach (Reactable reactable in targets.Cast<Reactable>()) {
                reactable.behaviourTypes = reactable.Behaviours.Select(b => b.Name).ToArray();
                EditorUtility.SetDirty(reactable);
            }
        }
    }
}
