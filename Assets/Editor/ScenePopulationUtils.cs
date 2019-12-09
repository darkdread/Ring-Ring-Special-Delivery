using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Developed by Koh Guan Zeh
namespace XellExtraUtils
{
	public class ScenePopulationUtils : EditorWindow
	{
		//For Random Rotation
		enum Axis {X = 0, Y = 1, Z = 2};
		Axis rotationAxis;
		float minRot, maxRot;
		bool adjLocalRot;

		//For Object Alignment
		Axis alignmentAxis;
		Transform referenceTransform;

		[MenuItem("Window/Scene Utils")]
		static void ShowWindow()
		{
			GetWindow<ScenePopulationUtils>("Scene Utils");
		}

		private void OnGUI()
		{
			//For Random Rotation
			GUILayout.Label("For Randomising Rotation", EditorStyles.boldLabel);
			rotationAxis = (Axis)EditorGUILayout.EnumPopup("Rotation Axis", rotationAxis);
			minRot = EditorGUILayout.FloatField("Minimum Rotation", minRot);
			maxRot = EditorGUILayout.FloatField("Minimum Rotation", maxRot);
			adjLocalRot = EditorGUILayout.Toggle("Adjust Local Rotation", adjLocalRot);

			if (GUILayout.Button("Randomise Rotation"))
			{
				if (adjLocalRot) RandomiseRotationLocal();
				else RandomiseRotationGlobal();
			}

			GUILayout.Space(10);

			GUILayout.Label("For Object Alignment", EditorStyles.boldLabel);
			alignmentAxis = (Axis)EditorGUILayout.EnumPopup("Position Axis", alignmentAxis);
			referenceTransform = EditorGUILayout.ObjectField("Reference Transform", referenceTransform, typeof(Transform), true) as Transform;

			if (GUILayout.Button("Align To Ref Transform")) AlignToRefTransform();
		}

		void RandomiseRotationLocal()
		{
			foreach (GameObject obj in Selection.objects)
			{
				Undo.RecordObject(obj.transform, "Undo Rotation");
				obj.transform.localEulerAngles = ResetAxisRotation(rotationAxis, obj.transform.localEulerAngles);
				obj.transform.localEulerAngles += GetAxisVector(rotationAxis) * Random.Range(minRot, maxRot);
				EditorUtility.SetDirty(obj);
			}
		}

		void RandomiseRotationGlobal()
		{
			foreach (GameObject obj in Selection.objects)
			{
				Undo.RecordObject(obj.transform, "Undo Rotation");
				obj.transform.eulerAngles = ResetAxisRotation(rotationAxis, obj.transform.eulerAngles);
				obj.transform.eulerAngles += GetAxisVector(rotationAxis) * Random.Range(minRot, maxRot);
				EditorUtility.SetDirty(obj);
			}
		}

		void AlignToRefTransform()
		{
			foreach (GameObject obj in Selection.objects)
			{
				Undo.RecordObject(obj.transform, "Undo Object Alignment");
				float val = GetAxisRefPosition(alignmentAxis, referenceTransform);
				obj.transform.position = ResetAxisPosition(alignmentAxis, obj.transform.position) + GetAxisVector(alignmentAxis) * val;
				EditorUtility.SetDirty(obj);
			}
		}

		Vector3 GetAxisVector(Axis axis)
		{
			switch (axis)
			{
				case Axis.X:
					return new Vector3(1, 0, 0);
				case Axis.Y:
					return new Vector3(0, 1, 0);
				case Axis.Z:
					return new Vector3(0, 0, 1);
				default:
					return Vector3.zero;
			}
		}

		Vector3 ResetAxisRotation(Axis axis, Vector3 eulerAngle)
		{
			Vector3 newEulerAngle = eulerAngle;

			switch (axis)
			{
				case Axis.X:
					newEulerAngle.x = 0;
					break;
				case Axis.Y:
					newEulerAngle.y = 0;
					break;
				case Axis.Z:
					newEulerAngle.z = 0;
					break;
			}

			return newEulerAngle;
		}

		float GetAxisRefPosition(Axis axis, Transform refTransform)
		{
			switch (axis)
			{
				case Axis.X:
					return referenceTransform.position.x;
				case Axis.Y:
					return referenceTransform.position.y;
				case Axis.Z:
					return referenceTransform.position.z;
				default:
					return 0;
			}
		}

		Vector3 ResetAxisPosition(Axis axis, Vector3 position)
		{
			Vector3 newPosition = position;

			switch (axis)
			{
				case Axis.X:
					newPosition.x = 0;
					break;
				case Axis.Y:
					newPosition.y = 0;
					break;
				case Axis.Z:
					newPosition.z = 0;
					break;
			}

			return newPosition;
		}
	}
}
