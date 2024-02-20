using UnityEngine;
using System.Collections;

public class ArtifactApron : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] healPerMeat = new float[] {  0.02f,  0.03f,  0.04f,  0.05f,  0.06f };
    private readonly int maxCount = 50;
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Apron",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"When each time character gets the <color=#f40>Meat</color>, gets bonus <color=#f40>HP Recovery</color>."
                   + $"\n"
                   + $"\nHeal per Meat : <color=#f40>{healPerMeat[0]}</color>"
                   + $"\nMax Count : {maxCount}"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"When each time character gets the <color=#f40>Meat</color>, gets bonus <color=#f40>HP Recovery</color>."
                   + $"\n"
                   + $"\nHeal per Meat : <color=#f40>{healPerMeat[level-1]}</color> > <color=#f40>{healPerMeat[NextLevelIndex]}</color>"
                   + $"\nMax Count : {maxCount}"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "앞치마",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터가 고기를 획득할 때 마다 추가 <color=#f40>체력 회복 속도</color>를 얻습니다."
                   + $"\n"
                   + $"\n고기당 증가량 : <color=#f40>{healPerMeat[0]}</color>"
                   + $"\n최대 증가량 : {maxCount}"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터가 고기를 획득할 때 마다 추가 <color=#f40>체력 회복 속도</color>를 얻습니다."
                   + $"\n"
                   + $"\n고기당 증가량 : <color=#f40>{healPerMeat[level-1]}</color> > <color=#f40>{healPerMeat[NextLevelIndex]}</color>"
                   + $"\n최대 증가량 : {maxCount}"
                   + $"</nobr>",
            }
    );
    #endregion Artifact Information

    private int meatCount = 0;
    private Coroutine healCoroutine;

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.onGetItem += CountMeatGotten;
        extraInformation = meatCount.ToString();
        
        healCoroutine = StartCoroutine(HealCoroutine());
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.onGetItem -= CountMeatGotten;
        
        if(healCoroutine != null)
            StopCoroutine(healCoroutine);
    }

    private void CountMeatGotten(Item item) {
        if(item is Meat) {
            meatCount = meatCount >= maxCount  ?  maxCount  :  meatCount+1;
            extraInformation = meatCount.ToString();
            GameManager.instance.StageManager.StageUIManager.UpdateArtifactList();
        }
    }

    private IEnumerator HealCoroutine() {
        while(true) {
            _Character.TakeHeal(meatCount * healPerMeat[level-1]);
            yield return new WaitForSeconds(1f);
        }
    }
}
