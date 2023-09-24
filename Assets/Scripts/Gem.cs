using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private GemTypes _gemType;
    [SerializeField] private int _gemID;
    [SerializeField] private SpriteRenderer _gemGFX;

    [SerializeField] private Vector2 _direction;
    [SerializeField] private float _speed;
    [SerializeField] private bool isProjectile = false;

    public GemTypes GetGemType => _gemType;
    void Start()
    {
        
    }

    void Update()
    {
        if(isProjectile)
            transform.Translate(_direction * (_speed * Time.deltaTime));
    }

    public void ConstructGem(GemTypes gemType, Sprite sprite, int id)
    {
        _gemType = gemType;
        _gemGFX.sprite = sprite;
        _gemID = id;
    }
    

    public void LaunchGem(Vector2 dir, float spd)
    {
        _direction = dir;
        _speed = spd;
        isProjectile = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Gem>()!=null && isProjectile )
        {
            GameManager.instance.getHexGrid.SetGemInGrid(this);
            isProjectile = false;
        }
        else if(other.gameObject.layer == 6 && isProjectile)
        {
            GameManager.instance.getHexGrid.SetGemInGrid(this);
            isProjectile = false;
        }
        else
        {
            _direction = new Vector2(_direction.x*-1,_direction.y);
        }
    }
}

