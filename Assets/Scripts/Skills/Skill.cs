using System;
using UnityEngine;

public abstract class Skill : MonoBehaviour {
    [SerializeField] protected Character character;
    protected float cost = 100f;

    public record SkillInformation(Sprite Icon, string Name, string Description);
    
    public abstract SkillInformation InformationEN { get; }
    public abstract SkillInformation InformationKO { get; }

    public Sprite Icon { get => 
        GameManager.instance.SelectedLocale.LocaleName switch {
            "English (en)" => InformationEN.Icon,
             "Korean (ko)" => InformationKO.Icon,

                         _ => InformationEN.Icon,
        };
    }

    public string Name { get => 
        GameManager.instance.SelectedLocale.LocaleName switch {
            "English (en)" => InformationEN.Name,
             "Korean (ko)" => InformationKO.Name,

                         _ => InformationEN.Name,
        };
    }

    public string Description { get => 
        GameManager.instance.SelectedLocale.LocaleName switch {
            "English (en)" => InformationEN.Description,
             "Korean (ko)" => InformationKO.Description,

                         _ => InformationEN.Description,
        };
    }

    public abstract void Active();
}