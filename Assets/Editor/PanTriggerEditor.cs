using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PanTrigger))]
public class PanTriggerEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		PanTrigger myScript = (PanTrigger)target;

        if (!myScript.defaultCamPos){
			if (GUILayout.Button("Show Default Pos")) {
				myScript.defaultCamPos = true;
				myScript.StoreVectors();
                myScript.GoToStoredPos();
			}
        } else {
			if (GUILayout.Button("Hide Default Pos")) {
				myScript.defaultCamPos = false;
				myScript.GoToStoredPos();
			}
        }

		
	}

}