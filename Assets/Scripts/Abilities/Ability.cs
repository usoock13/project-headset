using UnityEngine;

public abstract class Ability : MonoBehaviour {
    protected abstract (Sprite icon, string name, string description) InformationEN { get; }
    protected abstract (Sprite icon, string name, string description) InformationKO { get; }

    public Sprite Icon =>
        GameManager.instance.SelectedLocale.LocaleName switch {
            "English (en)" => InformationEN.icon,
             "Korean (ko)" => InformationKO.icon,

            _ => InformationEN.icon,
        };
    public string Name =>
        GameManager.instance.SelectedLocale.LocaleName switch {
            "English (en)" => InformationEN.name,
             "Korean (ko)" => InformationKO.name,

            _ => InformationEN.name,
        };
    public string Description =>
        GameManager.instance.SelectedLocale.LocaleName switch {
            "English (en)" => InformationEN.description,
             "Korean (ko)" => InformationKO.description,

            _ => InformationEN.description,
        };
    
    public abstract void OnTaken(Character character);
    public abstract void OnReleased(Character character);
}