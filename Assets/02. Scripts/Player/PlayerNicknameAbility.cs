using TMPro;
using UnityEngine;

public class PlayerNicknameAbility : PlayerAbility
{
    [SerializeField] private TextMeshProUGUI NicknameText;

    private void Start()
    {
        NicknameText.text = $"{_photonView.Owner.NickName}_{_photonView.Owner.ActorNumber}";

        NicknameText.color = _photonView.IsMine ? Color.green : Color.red;
    }
}
