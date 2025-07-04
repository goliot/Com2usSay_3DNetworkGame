using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private TMP_InputField _roomIdInputField;

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

        PhotonNetwork.NickName = nickname; // 닉네임 설정

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        roomOptions.IsOpen = true; // 룸 입장 가능 여부
        roomOptions.IsVisible = true; // 룸 목록에 노출시킬지 여부

        // 생성 -> 접속까지
        PhotonNetwork.JoinOrCreateRoom(roomId, roomOptions, TypedLobby.Default);
    }
}
