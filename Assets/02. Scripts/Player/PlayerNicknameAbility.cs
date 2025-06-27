using TMPro;
using UnityEngine;

public class PlayerNicknameAbility : PlayerAbility
{
    [SerializeField] private TextMeshPro NicknameText;

    private void Start()
    {
        NicknameText.text = $"{_photonView.Owner.NickName}_{_photonView.Owner.ActorNumber}";

        NicknameText.color = _photonView.IsMine ? Color.green : Color.red;
    }
}
