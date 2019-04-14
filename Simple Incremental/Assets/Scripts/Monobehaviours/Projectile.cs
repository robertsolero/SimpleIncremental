﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour
{
    Rigidbody2D rb2d = null;
    SpriteRenderer spriteRenderer = null;
    int damage = 0;
    float falloffTime = 0f;
    int maxPenetrations = 1;
    float speed = 1f;
    float rotation = 20;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void FixedUpdate()
    {
        if (rb2d.velocity.x > 0.9 || rb2d.velocity.x < -0.9 )
            transform.Rotate(new Vector3(0, 0, rotation));
    }

    public void Launch(Vector2 direction, Sprite _sprite, int _damage, float _falloffTime, int _maxPenetrations, float _speed, float _rotation)
    {
        gameObject.SetActive(true);
        Vector3 diff = direction.normalized;
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        spriteRenderer.sprite = _sprite;
        damage = _damage;
        falloffTime = _falloffTime;
        maxPenetrations = _maxPenetrations;
        speed = _speed;
        rotation = _rotation;
        rb2d.velocity = direction.normalized * speed;
        StartCoroutine(Lifetime());
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(falloffTime);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            CharacterHealth ch = collision.gameObject.GetComponent<CharacterHealth>();
            if (ch != null)
            {
                if (maxPenetrations-- > 0)
                {
                    ch.TakeDamage(damage);
                }
                if (maxPenetrations <= 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }
        //Collided with Ground
        if (collision.gameObject.layer == 12)
        {
            rb2d.rotation = 0;
            rb2d.velocity = new Vector2(0, 0);
        }
    }
}
