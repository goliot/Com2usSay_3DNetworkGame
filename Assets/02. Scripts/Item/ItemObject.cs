using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerStatHolder playerStatHolder = other.GetComponent<PlayerStatHolder>();
            if (playerStatHolder != null)
            {
                playerStatHolder.Score += 10;
                Debug.Log("아이템 획득: " + gameObject.name);

                Destroy(gameObject);
            }
        }
    }
}
