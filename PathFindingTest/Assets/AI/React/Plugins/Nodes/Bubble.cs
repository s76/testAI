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
    public class Bubble : LeafNode
    {
        [ReactVar]
        public string
            text;
        
        public override IEnumerator<NodeResult> NodeTask ()
        {
            gameObject.SendMessage ("ShowSpeechBubble", text, SendMessageOptions.RequireReceiver);
            yield return NodeResult.Success;
            
        }

        public override string ToString ()
        {
            return "Bubble";
        }
#if UNITY_EDITOR
        public new static string GetHelpText ()     {
            return "Shows a speech bubble above the gameObject";
        }           
#endif
    }
    


}







