using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

// 포톤 서버 관리자(서버 연결, 로비 입장, 방 입장, 게임 입장)
public class PhotonServerManager : MonoBehaviourPunCallbacks
{
    private readonly string _gameVersion = "1.0.0";
    private readonly string _nickname = "goliot";

    private void Awake()
    {
        // 0. 데이터 송 수신 빈도 매 초당 30회로 설정 (기본 = 10)
        PhotonNetwork.SendRate = 60; // TargetRate일 뿐 보장은 아님
        PhotonNetwork.SerializationRate = 60;

        // 1. 버전 : 버전이 다르면 다른 서버로 접속이 된다. -> 패치 한 유저와 안한 유저가 다른 서버에서 놀게 할 때
        PhotonNetwork.GameVersion = _gameVersion;
        // 2. 닉네임 : 게임에서 사용할 사용자의 별명(중복 가능 -> 판별을 위해서는 ActorID)
        PhotonNetwork.NickName = _nickname;

        // 방장이 로드한 씬으로 다른 참여자가 똑같이 이동하게끔 동기화하는 옵션
        // 방장 : 방을 만든 소유자, "마스터 클라이언트" (방 마다 한 명의 마스터 클라이언트가 존재)
        PhotonNetwork.AutomaticallySyncScene = true;

        // 설정값을 이용해 서버 접속 시도
        // 네임 서버 접속 -> 방 목록이 있는 마스터 서버까지 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnected()
    {
        Debug.Log("네임 서버 접속 완료");
        Debug.Log(PhotonNetwork.CloudRegion);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"마스터 서버 접속 {PhotonNetwork.InLobby}");

        PhotonNetwork.JoinLobby();
        // PhotonNetwork.JoinLobby(TypedLobby.Default); // 위에꺼랑 똑같음
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"로비 접속 {PhotonNetwork.InLobby}");

        // 랜덤 방에 들어간다.
        PhotonNetwork.JoinRandomRoom();

        // 랜덤 방에 들어가거나 없으면 생성해서 들어간다.
        //PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"방 입장 완료 {PhotonNetwork.InRoom} : {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"플레이어 : {PhotonNetwork.CurrentRoom.PlayerCount} 명");

        //룸에 접속한 사용자 정보
        Dictionary<int, Player> roomPlayers = PhotonNetwork.CurrentRoom.Players;

        foreach(var pair in roomPlayers)
        {
            // ActorNumber는 방에 들어온 순서대로 1, 2, 3...
            Debug.Log($"{pair.Value.NickName} : {pair.Value.ActorNumber}");

            // 진짜 고유 ID
            Debug.Log(pair.Value.UserId); // 친구 기능, 귓속말 등에 쓰임
        }

        PhotonNetwork.Instantiate("ChemicalMan", Vector3.zero, Quaternion.identity);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"{returnCode} : {message}");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        roomOptions.IsOpen = true; // 룸 입장 가능 여부
        roomOptions.IsVisible = true; // 룸 목록에 노출시킬지 여부

        // 생성 -> 접속까지
        PhotonNetwork.CreateRoom("TestRoom", roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"룸 생성 성공 {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"룸 생성 실패 : {returnCode} : {message}");
    }
}