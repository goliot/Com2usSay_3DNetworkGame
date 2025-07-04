using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private TMP_InputField _roomIdInputField;

    [SerializeField] private Toggle _chemicalManToggle;
    [SerializeField] private Toggle _samuraiToggle;

    public void OnClickEnterRoomButton()
    {
        // 방 입장 버튼 클릭 시
        string nickname = _nicknameInputField.text;
        string roomId = _roomIdInputField.text;
        if (string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(roomId))
        {
            Debug.LogError("닉네임과 방 ID를 입력해주세요.");
            return;
        }
        //PhotonServerManager.Instance.EnterRoom(nickname, roomId);

        if (!CheckToggle())
        {
            Debug.LogError("캐릭터를 선택해주세요.");
            return;
        }

        PhotonNetwork.NickName = nickname; // 닉네임 설정

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        roomOptions.IsOpen = true; // 룸 입장 가능 여부
        roomOptions.IsVisible = true; // 룸 목록에 노출시킬지 여부

        // 생성 -> 접속까지
        PhotonNetwork.JoinOrCreateRoom(roomId, roomOptions, TypedLobby.Default);
    }

    private bool CheckToggle()
    {
        if(_chemicalManToggle.isOn)
        {
            PhotonServerManager.Instance.SetCharacter("ChemicalMan");
            return true;
        }
        else if(_samuraiToggle.isOn)
        {
            PhotonServerManager.Instance.SetCharacter("Samurai");
            return true;
        }
        else
        {
            Debug.LogError("캐릭터를 선택해주세요.");
            return false;
        }
    }
}
