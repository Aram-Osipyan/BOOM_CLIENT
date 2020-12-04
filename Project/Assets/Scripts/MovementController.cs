using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] float force = 1f;
    private Rigidbody2D _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _rigidbody.MovePosition(_rigidbody.position + Vector2.up * Time.fixedDeltaTime*force);
        }
        if (Input.GetKey(KeyCode.D))
        {
            _rigidbody.MovePosition(_rigidbody.position + Vector2.right * Time.fixedDeltaTime * force);
        }
        if (Input.GetKey(KeyCode.A))
        {
            _rigidbody.MovePosition(_rigidbody.position + Vector2.left * Time.fixedDeltaTime * force);
        }
        if (Input.GetKey(KeyCode.S))
        {
            _rigidbody.MovePosition(_rigidbody.position + Vector2.down * Time.fixedDeltaTime * force);
        }

    }
   
}
