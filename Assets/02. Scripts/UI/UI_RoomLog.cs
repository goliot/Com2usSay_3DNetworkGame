using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UI_RoomLog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _logText;
    [SerializeField] private ScrollRect _scrollRect;

    private string _logMessage = "방에 입장했습니다.";

    private void Start()
    {
        Refresh();

        RoomManager.Instance.OnPlayerEnter += PlayerEnterLog;
        RoomManager.Instance.OnPlayerLeft += PlayerExitLog;
        RoomManager.Instance.OnPlayerDead += PlayerDeathLog;
    }

    private void Refresh()
    {
        _logText.text = _logMessage;
        StartCoroutine(ScrollToBottomNextFrame());
    }

    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return null; // 한 프레임 기다리기
        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 0f;
    }

    public void PlayerEnterLog(string playerName)
    {
        _logMessage += $"\n<color=#00FF00>{playerName}</color>님이 <color=#00FFFF>입장</color>하였습니다.";
        Refresh();
    }

    public void PlayerExitLog(string playerName)
    {
        _logMessage += $"\n<color=#00FF00>{playerName}</color>님이 <color=red>퇴장</color>하였습니다.";
        Refresh();
    }

    public void PlayerDeathLog(string playerName, string attackerName)
    {
        _logMessage += $"\n<color=#00FF00>{attackerName}</color>님이 <color=red>{playerName}</color>님을 <color=red>처치</color>하였습니다.";
        Refresh();
    }
}
