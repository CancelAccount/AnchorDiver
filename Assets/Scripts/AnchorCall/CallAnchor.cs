using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallAnchor : MonoBehaviour
{
    [SerializeField] private GameObject anchor;
    [SerializeField] private Transform callAnchor;
    [SerializeField] private float anchorSpeed;

    private void Start()
    {
    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameObject obj = Instantiate(anchor, callAnchor.position, Quaternion.identity);
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb == null)
                rb = obj.AddComponent<Rigidbody2D>();

            transform.Translate(Vector2.down * anchorSpeed * Time.deltaTime);// 直接给一个向下的速度
                                                                       // 或使用 rb.AddForce(Vector3.down * 10f, ForceMode.VelocityChange);
        }
    }

}
