using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(CompositeBehaviour))]
//public class CompositeBehaviourEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        CompositeBehaviour cb = (CompositeBehaviour)target;

//        Rect r = EditorGUILayout.BeginHorizontal();
//        r.height = EditorGUIUtility.singleLineHeight;

//        if (cb.flockBehaviours == null || cb.flockBehaviours.Length == 0)
//        {
//            EditorGUILayout.HelpBox("No flock behaviours in array.", MessageType.Warning);
//            EditorGUILayout.EndHorizontal();
//            r = EditorGUILayout.BeginHorizontal();
//            r.height = EditorGUIUtility.singleLineHeight;
//        }
//        else
//        {
//            r.x = 30.0f;
//            r.width = EditorGUIUtility.currentViewWidth - 95.0f;
//            EditorGUI.LabelField(r, "Behaviours");

//            r.x = EditorGUIUtility.currentViewWidth - 65.0f;
//            r.width = 60.0f;
//            EditorGUI.LabelField(r, "Weights");

//            r.y += EditorGUIUtility.singleLineHeight * 1.2f;

//            EditorGUI.BeginChangeCheck();

//            for (int i = 0; i < cb.flockBehaviours.Length; i++)
//            {
//                r.x = 5.0f;
//                r.width = 20.0f;
//                EditorGUI.LabelField(r, i.ToString());

//                r.x = 30.0f;
//                r.width = EditorGUIUtility.currentViewWidth - 95.0f;
//                cb.flockBehaviours[i] = (FlockBehaviour)EditorGUI.ObjectField(r, cb.flockBehaviours[i], typeof(FlockBehaviour), false);

//                r.x = EditorGUIUtility.currentViewWidth - 65.0f;
//                r.width = 60.0f;
//                cb.weights[i] = EditorGUI.FloatField(r, cb.weights[i]);

//                r.y += EditorGUIUtility.singleLineHeight * 1.1f;
//            }

//            if (EditorGUI.EndChangeCheck())
//            {
//                EditorUtility.SetDirty(cb);
//            }
//        }

//        EditorGUILayout.EndHorizontal();
//        r.x = 5.0f;
//        r.width = EditorGUIUtility.currentViewWidth - 10.0f;
//        r.y += EditorGUIUtility.singleLineHeight * 0.5f;

//        if(GUI.Button(r, "Add Behaviour"))
//        {
//            AddBehaviour(cb);
//            EditorUtility.SetDirty(cb);
//        }

//        r.y += EditorGUIUtility.singleLineHeight * 1.5f;
//        if(cb.flockBehaviours != null && cb.flockBehaviours.Length > 0)
//        {
//            if (GUI.Button(r, "Remove Behaviour"))
//            {
//                RemoveBehaviour(cb);
//                EditorUtility.SetDirty(cb);
//            }
//        }    
//    }

//    void AddBehaviour(CompositeBehaviour cb)
//    {
//        int oldCount = (cb.flockBehaviours != null) ? cb.flockBehaviours.Length : 0;
//        FlockBehaviour[] newFlockBehaviours = new FlockBehaviour[oldCount + 1];
//        float[] newWeights = new float[oldCount + 1];

//        for(int i = 0; i < oldCount; i++)
//        {
//            newFlockBehaviours[i] = cb.flockBehaviours[i];
//            newWeights[i] = cb.weights[i];
//        }

//        newWeights[oldCount] = 1.0f;
//        cb.flockBehaviours = newFlockBehaviours;
//        cb.weights = newWeights;
//    }

//    void RemoveBehaviour(CompositeBehaviour cb)
//    {
//        int oldCount = cb.flockBehaviours.Length;
//        if(oldCount == 1)
//        {
//            cb.flockBehaviours = null;
//            cb.weights = null;
//            return;
//        }

//        FlockBehaviour[] newFlockBehaviours = new FlockBehaviour[oldCount - 1];
//        float[] newWeights = new float[oldCount - 1];

//        for (int i = 0; i < oldCount - 1; i++)
//        {
//            newFlockBehaviours[i] = cb.flockBehaviours[i];
//            newWeights[i] = cb.weights[i];
//        }

//        cb.flockBehaviours = newFlockBehaviours;
//        cb.weights = newWeights;
//    }
//}
