using System;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR 
using UnityEditor;
#endif


public enum DamageModifier
{
    Multiplier,
    Percentage,
}

public class HitZone : MonoBehaviour, IHitable
{
    public bool IsEnabled;

    public HitZones Zone;
    public DamageModifier Modifier;
    public float ModifierValue;
    public HitController Controller;

    public void Hit(object sender, float damage)   
	{
        if (!IsEnabled) return;
        Controller.Damage(sender, new DamageEventArgs(CalculateDamage(damage), Zone));
    }

    public float CalculateDamage(float damage)
    {
        return Modifier == DamageModifier.Multiplier ? damage * ModifierValue : ModifierValue / damage * 100F;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(HitZone))]
    class HitZoneEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var obj = target as HitZone;

            GUILayout.Label("For get event OnDamage, add collider \nand setup him size. For add new zone \ntype use button \"EditZones\"");

            GUILayout.BeginHorizontal("box");

            GUI.color = !obj.IsEnabled ? Color.red : Color.green;

            obj.IsEnabled = GUILayout.Toggle(obj.IsEnabled, obj.IsEnabled ? "Enabled" : "Disabled", "Button");
            GUI.color = Color.white;

            GUILayout.FlexibleSpace();

            GUILayout.Label("Zone: ");
            obj.Zone = (HitZones)EditorGUILayout.EnumPopup(obj.Zone);


            GUILayout.EndHorizontal();

            if (!obj.IsEnabled)
                EditorGUILayout.HelpBox("Zone disabled.", MessageType.Warning);

            if (obj.GetComponent<Collider>() == null || !obj.GetComponent<Collider>().enabled)
                EditorGUILayout.HelpBox("Collider not found or disabled. Please add Collider component.",
                    MessageType.Error);
           // else if (!obj.GetComponent<Collider>().isTrigger)
                //obj.GetComponent<Collider>().isTrigger = true;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(); EditorGUILayout.Space();

            GUI.color = Color.red;
            if (GUILayout.Button("Edit zones"))
                ZoneEditor.Init();
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}
