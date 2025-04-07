using System.Text;
using System.Reflection;
using UnityEngine;
using TMPro;

public class UIStatDisplay : MonoBehaviour
{

    public PlayerStats player; // The player that this stat display is rendering stats for.
    public bool displayCurrentHealth = false;
    public bool updateInEditor = false;
    TextMeshProUGUI statNames, statValues;

    // Update this stat display whenever it is set to be active.
    void OnEnable()
    {
        UpdateStatFields();
    }

    void OnDrawGizmosSelected()
    {
        if(updateInEditor) UpdateStatFields();
    }

    public void UpdateStatFields()
    {
        if (!player) return;

        // Get a reference to both Text objects to render stat names and stat values.
        if (!statNames) statNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (!statValues) statValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        // Render all stat names and values.
        // Use StringBuilders so that the string manipulation runs faster.
        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();

        if(displayCurrentHealth)
        {
            names.AppendLine("Health");
            values.AppendLine(player.CurrentHealth.ToString());
        }
        
        FieldInfo[] fields = typeof(CharacterData.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            // Add the stat name.
            names.AppendLine(field.Name);

            // Get the stat value.
            object val = field.GetValue(player.Stats);
            float fval = val is int ? (int)val : (float)val;

            // Print it as a percentage if it has an attribute assigned and is a float.
            PropertyAttribute attribute = (PropertyAttribute)field.GetCustomAttribute<RangeAttribute>() ?? field.GetCustomAttribute<MinAttribute>();
            if (attribute != null && field.FieldType == typeof(float))
            {
                float percentage = Mathf.Round(fval * 100 - 100);

                // If the stat value is 0, just put a dash.
                if (Mathf.Approximately(percentage, 0))
                {
                    values.Append('-').Append('\n');
                }
                else
                {
                    if (percentage > 0)
                        values.Append('+');
                    values.Append(percentage).Append('%').Append('\n');
                }
            }
            else
            {
                values.Append(fval).Append('\n');
            }

            // Updates the fields with the strings we built.
            statNames.text = PrettifyNames(names);
            statValues.text = values.ToString();
        }
    }

    public static string PrettifyNames(StringBuilder input)
    {
        // Return an empty string if StringBuilder is empty.
        if (input.Length <= 0) return string.Empty;

        StringBuilder result = new StringBuilder();
        char last = '\0';
        for(int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // Check when to uppercase or add spaces to a character.
            if(last == '\0' || char.IsWhiteSpace(last))
            {
                c = char.ToUpper(c);
            } else if (char.IsUpper(c))
            {
                result.Append(' '); // Insert space before capital letter
            }
            result.Append(c);

            last = c;
        }
        
        return result.ToString();
    }

    void Reset()
    {
        player = FindObjectOfType<PlayerStats>();
    }
}