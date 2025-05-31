using System;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

public class UISceneDataDisplay : UIPropertyDisplay
{
    public UILevelSelector levelSelector;
    TextMeshProUGUI extraStageInfo;

    public override object GetReadObjects()
    {
        if (levelSelector == null)
        {
            Debug.LogError("LeevlSelector is null!");
            return new UILevelSelector.SceneData();
        }
            //Debug.Log($"Selected Level: {UILevelSelector.selectedLevel}");


        if (levelSelector && UILevelSelector.selectedLevel >= 0)
        {
            return levelSelector.levels[UILevelSelector.selectedLevel];
        }
        return new UILevelSelector.SceneData();
    }

    public override void UpdateFields()
    {
        if (!propertyNames) propertyNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (!propertyValues) propertyValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        if (!extraStageInfo) extraStageInfo = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        StringBuilder[] allStats = GetProperties(BindingFlags.Public | BindingFlags.Instance, "UILevelSelector+SceneData");

        UILevelSelector.SceneData dat = (UILevelSelector.SceneData)GetReadObjects();

        if (dat == null)
        {
            Debug.LogError("SceneData is null!");
            return;
        }

        allStats[0].AppendLine("Velocidade de movimento").AppendLine("Bônus de Ouro").AppendLine("Bônus de sorte").AppendLine("Bônus de Experiencia").AppendLine("Vida dos Inimigos");


        Type characterDataStats = typeof(CharacterData.Stats);
        ProcessValue(dat.playerModifier.moveSpeed, allStats[1], characterDataStats.GetField("moveSpeed"));
        ProcessValue(dat.playerModifier.greed, allStats[1], characterDataStats.GetField("greed"));
        ProcessValue(dat.playerModifier.luck, allStats[1], characterDataStats.GetField("luck"));
        ProcessValue(dat.playerModifier.growth, allStats[1], characterDataStats.GetField("growth"));

        Type enemyStats = typeof(EnemyStats.Stats);
        ProcessValue(dat.enemyModifier.moveSpeed, allStats[1], characterDataStats.GetField("maxHealth"));

        if (propertyNames) propertyNames.text = allStats[0].ToString();
        if (propertyValues) propertyValues.text = allStats[1].ToString();
    }

    protected override bool IsFieldShown(FieldInfo field)
    {
        switch (field.Name)
        {
            default:
                return false;
            case "timeLimit":
            case "clockSpeed":
            case "moveSpeed":
            case "greed":
            case "luck":
            case "growth":
            case "maxHealth":
                return true;
        }
    }

    protected override StringBuilder ProcessName(string name, StringBuilder output, FieldInfo field)
    {
        if (field.Name == "exxtraNotes") return output;
        return base.ProcessName(name, output, field);
    }

    protected override StringBuilder ProcessValue(object value, StringBuilder output, FieldInfo field)
    {
        float fval;
        switch (field.Name)
        {
            case "timeLimit":
                fval = value is int ? (int)value : (float)value;
                if (fval == 0)
                {
                    output.Append(DASH).Append('\n');
                }
                else
                {
                    string minutes = Mathf.FloorToInt(fval / 60).ToString();
                    string seconds = (fval % 60).ToString();
                    if (fval % 60 < 10)
                    {
                        seconds += "0";
                    }
                    output.Append(minutes).Append(":").Append(seconds).Append("\n");
                }
                return output;

            case "clockSpeed":
                fval = value is int ? (int)value : (float)value;
                output.Append(fval).Append("x").Append('\n');
                return output;

            case "maxHealth":
            case "moveSpeed":
            case "greed":
            case "luck":
            case "growth":
                fval = value is int ? (int)value : (float)value;
                float percentage = Mathf.Round(fval * 100);

                if (Mathf.Approximately(percentage, 0))
                {
                    output.Append(DASH).Append('\n');
                }
                else
                {
                    if (percentage > 0)
                    {
                        output.Append('+');
                    }
                    output.Append(percentage).Append('%').Append('\n');
                }
                return output;

            case "extraNotes":
                if (value == null) return output;
                string msg = value.ToString();
                extraStageInfo.text = string.IsNullOrWhiteSpace(msg) ? DASH : msg;
                return output;
        }
        return base.ProcessValue(value, output, field);
    }
    void Reset()
    {
        levelSelector = FindObjectOfType<UILevelSelector>();
    }

}
