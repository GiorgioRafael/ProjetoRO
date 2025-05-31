using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;


[DisallowMultipleComponent]
[CustomEditor(typeof(UILevelSelector))]
public class UILevelSelectorEditor : Editor
{
    UILevelSelector selector;

    void OnEnable()
    {
        selector = target as UILevelSelector;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!selector.toggleTemplate)
            EditorGUILayout.HelpBox(
                "You need to assign a toggle template for the button bellow to work properly",
                MessageType.Warning
            );

        if (GUILayout.Button("Find and Populate Levels"))
        {
            PopulateLevelsList();
            CreateLevelSelectToggles();
        }
    }

    public void PopulateLevelsList()
    {
        Undo.RecordObject(selector, "Create New SceneData structs");
        SceneAsset[] maps = UILevelSelector.GetAllMaps();

        selector.levels.RemoveAll(levels => levels.scene == null);

        foreach (SceneAsset map in maps)
        {
            if (!selector.levels.Any(sceneData => sceneData.scene == map))
            {
                Match m = Regex.Match(map.name, UILevelSelector.MAP_NAME_FORMAT, RegexOptions.IgnoreCase);
                string mapLabel = "Level", mapName = "New Map";
                if (m.Success)
                {
                    if (m.Groups.Count > 1) mapLabel = m.Groups[1].Value;
                    if (m.Groups.Count > 2) mapName = m.Groups[2].Value;
                }

                selector.levels.Add(new UILevelSelector.SceneData
                {
                    scene = map,
                    label = mapLabel,
                    displayName = mapName
                });
            }
        }
    }

    public void CreateLevelSelectToggles()
    {
        if (!selector.toggleTemplate)
        {
            Debug.LogWarning("failed to create the toggles for selecting levels. please assign the toggle template");
            return;
        }

        for (int i = selector.toggleTemplate.transform.parent.childCount - 1; i >= 0; i--)
        {
            Toggle tog = selector.toggleTemplate.transform.parent.GetChild(i).GetComponent<Toggle>();
            if (tog == selector.toggleTemplate) continue;
            Undo.DestroyObjectImmediate(tog.gameObject);
        }

        Undo.RecordObject(selector, "Updates to UILevelSelector.");
        selector.selectableToggles.Clear();

        for (int i = 0; i < selector.levels.Count; i++)
        {
            Toggle tog;
            if (i == 0)
            {
                tog = selector.toggleTemplate;
                Undo.RecordObject(tog, "Modifiying the template");
            }
            else
            {
                tog = Instantiate(selector.toggleTemplate, selector.toggleTemplate.transform.parent);
                Undo.RegisterCreatedObjectUndo(tog.gameObject, "Created a new toggle");
            }

            tog.gameObject.name = selector.levels[i].scene.name;

            Transform levelName = tog.transform.Find(selector.LevelImagePath).Find("Name Holder").Find(selector.LevelNamePath);

            if (levelName && levelName.TryGetComponent(out TextMeshProUGUI lvlName))
            {
                lvlName.text = selector.levels[i].displayName;
            }

            Transform levelNumber = tog.transform.Find(selector.LevelImagePath).Find(selector.LevelNumberPath);
            if (levelNumber && levelNumber.TryGetComponent(out TextMeshProUGUI lvlNumber))
            {
                lvlNumber.text = selector.levels[i].label;
            }

            Transform levelDescription = tog.transform.Find(selector.LevelDescriptionPath);
            if (levelDescription && levelDescription.TryGetComponent(out TextMeshProUGUI lvlDescription))
            {
                lvlDescription.text = selector.levels[i].description;
            }

            Transform levelImage = tog.transform.Find(selector.LevelImagePath);
            if (levelImage && levelImage.TryGetComponent(out Image lvlImage))
            {
                lvlImage.sprite = selector.levels[i].icon;
            }

            selector.selectableToggles.Add(tog);

            for (int j = 0; j < tog.onValueChanged.GetPersistentEventCount(); j++)
            {
                if (tog.onValueChanged.GetPersistentMethodName(j) == "Select")
                {
                    UnityEventTools.RemovePersistentListener(tog.onValueChanged, j);
                }
            }
            UnityEventTools.AddIntPersistentListener(tog.onValueChanged, selector.Select, i);
        }

        EditorUtility.SetDirty(selector);
    }
}
