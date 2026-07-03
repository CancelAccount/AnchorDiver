using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMap : MonoBehaviour
{
    [Header("传送设置")]
    public Transform destination;          // 传送到哪里
    public float teleportCooldown = 1f;    // 冷却时间，防止反复触发

    private float lastTeleportTime = -99f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("1");
        if (!collision.gameObject.CompareTag("Player")) return;

        Teleport(collision.gameObject);

        lastTeleportTime = Time.time;
    }

    void Teleport(GameObject player)
    {
        Vector3 targetPos = destination.position;
        player.transform.position = targetPos;

    }
}
