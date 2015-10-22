React
-----

React implements behaviour trees in Unity3D. Behaviour trees are used to implement complex AI behaviours. The editor uses normal cut, copy paste etc operations. 


Demo
----

The demo scene contains a single player character and multiple enemy characters. The enemy characters will move randomly whenever they can see the player character, attempting to find a hiding spot.


Usage Notes
-----------

- The Edit->Delete command (and it's shortcut) will delete the selected node.
- You can use arrow keys to change which node is selected.
- You can drag and drop nodes. 
- Reorder nodes by dragging and dropping onto the parent nodde.
- You can right click on a node to get a context menu. This provides options for changing the node type and other editing operations.


Workflow
--------


1. Right click in the Project Window, Select Create -> Reactable.

This is the file which will contain the behaviour tree.

2. Select the created Reactable asset, and give it a meaningful name.

3. You will want to use Action and Condition methods with your Behaviour tree. In order for the Reactable to know about these, you will need to assign any scripts you intend to use in the Behaviour tree. Select the asset, and then in the inspector you will notice an item called 'Behaviours'. Drag and drop your scripts into this list.

If you want a method to be used as an Action, it must have this signature:

public IEnumerator<NodeResult> ActionName();

When you want to signal that your action completed successfully, use:

yield return NodeResult.Success;

When you want to signal that your action could not be completed, use:

yield return NodeResult.Failure;

When you want to wait till the next frame to continue the action use:

yield return NodeResult.Continue;

If you want to use a method as a Condition, it must have this signature:

public bool ConditionName();

Then return true if the condition is true, else return false;

There are examples of these methods in the ReactorTemplateScript.cs component.

4. Right click the Reactable asset, and select "Edit Reactable". A window will appear. This is the behaviour tree editor. You might want to dock this window behind your scene window.

5. The top of the window has a field where you can change the asset being edited, and undo and redo buttons. The undo and redo buttons are located here as the Unity Undo/Redo system does not work with ScriptableObjects (which is what a Reactable asset is derived from).

6. The Root node should be selected (coloured red) and have some warning text: "Missing Child!". You can right click the root node and add a branch node, such as a selector or sequence.

7. You build the tree by creating new nodes using the right click context menu or by using the buttons at the bottom of the inspector panel.

8. Node properties are changed in the inspector panel. For example, an Action node has a dropdown which lists the possible methods you can use. These methods are extracted from the scripts you assigned in step 3. You can always assign more scripts at any time, and they will become available in the drop down.

9. In order to use your Reactable asset, you must assign a Reactor component to the GameObject you want to control. You must also assign the scripts from step 3. A shortcut for this is available by right clicking the Reactor component and choosing "Add reactable components".

10. Assign the reactable asset you have created into the available field on the reactor component. The Tick duration is how long you would like each node in the behaviour tree to execute. Eg, 0.1 means that 10 nodes would be executed per second. You can set to 0 to enable 1 node per frame.

11. Press Play, your GameObject now has a mind of it's own!
