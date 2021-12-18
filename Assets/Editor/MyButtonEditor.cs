using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[CustomEditor(typeof( MyButton ) )]
public class MyButtonEditor : UnityEditor.UI.ButtonEditor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

        MyButton _component = ( MyButton )target;

        EditorGUILayout.LabelField( "UGUI", EditorStyles.boldLabel );
        _component.buttonLabel = ( Text )EditorGUILayout.ObjectField( "Label", _component.buttonLabel, typeof( Text ), true );

        EditorGUILayout.Space();

        EditorGUILayout.LabelField( "TextMesh Pro UGUI", EditorStyles.boldLabel );
        _component.textMeshProButtonLabel = ( TextMeshProUGUI )EditorGUILayout.ObjectField( "TMPro Label", _component.textMeshProButtonLabel, typeof( TextMeshProUGUI ), true );
        _component.textMeshProButtonLabelEnabled = ( Material )EditorGUILayout.ObjectField( "TMPro Label Enabled", _component.textMeshProButtonLabelEnabled, typeof( Material ), true );
        _component.textMeshProButtonLabelDisabled = ( Material )EditorGUILayout.ObjectField( "TMPro Label Disabled", _component.textMeshProButtonLabelDisabled, typeof( Material ), true );

        if (GUI.changed == true)
        {
            EditorUtility.SetDirty( _component );
            EditorSceneManager.MarkSceneDirty( SceneManager.GetActiveScene() );
        }
    }
}
