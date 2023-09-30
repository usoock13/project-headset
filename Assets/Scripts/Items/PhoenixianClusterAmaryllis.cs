using UnityEngine;

public class PhoenixianClusterAmaryllis : Item {
    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "페니키시안 클러스터 아마릴리스";
    public override string Description => "꽃잎 마구 흩날리기!";

    public override void OnGotten() {
        base.OnGotten();
    }
}