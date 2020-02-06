﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Entity))]
public class EntityEditor : Editor
{
	private string _currentTagAddString = "";

	public override void OnInspectorGUI()
	{
		Entity entity = target as Entity;

		if(entity == null)
		{
			return;
		}

		EditorGUILayout.LabelField(string.Concat("Entity: ", entity.gameObject.name));

		EditorGUILayout.Space();

		string[] tags = entity.GetTags();
		EditorGUILayout.BeginHorizontal();
		_currentTagAddString = EditorGUILayout.TextField("Add Tag: ", _currentTagAddString);
		if (GUILayout.Button("+", GUILayout.Width(30)))
		{
			entity.AddTag(_currentTagAddString);
			_currentTagAddString = string.Empty;
		}

		EditorGUILayout.EndHorizontal();

		for (int i = 0; i < tags.Length; i++)
		{
			EditorGUILayout.BeginHorizontal();
			GUIStyle s = new GUIStyle(GUI.skin.label);
			s.normal.textColor = new Color(0.5f, 0.3f, 0.6f);
			GUILayout.Label(" * " + tags[i], s);
			s = new GUIStyle(GUI.skin.button);
			s.normal.textColor = Color.red;
			if (GUILayout.Button("x", s, GUILayout.Width(25)))
			{
				entity.RemoveTag(tags[i]);
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Space();
	}
}
