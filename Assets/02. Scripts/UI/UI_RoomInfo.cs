using TMPro;
using UnityEngine;
using Photon.Realtime;

public class UI_RoomInfo : MonoBehaviour
{
    [Header("# UI")]
    [SerializeField] private TextMeshProUGUI _roomNameText;
    [SerializeField] private TextMeshProUGUI _playerCountText;

    private void Start()
    {
        RoomManager.Instance.OnRoomDataChanged += Refresh;
    }

    private void Refresh()
    {
        Room room = RoomManager.Instance.Room;
        if(room == null)
        {
            return;
        }
        _roomNameText.text = $"Room : {room.Name}";
        _playerCountText.text = $"{room.PlayerCount} / {room.MaxPlayers}";
    }

    public void OnExitButtonCliked()
    {
        Exit();
    }

    private void Exit()
    {

    }
}
