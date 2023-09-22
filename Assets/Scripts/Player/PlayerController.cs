using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private Vector2 position;
    
    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Cannon.Shoot.performed += ShootGem;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }
    private void OnDisable()
    {
        playerInput.Disable();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 shtrDir = ((Camera.main.ScreenToWorldPoint(playerInput.Cannon.Direction.ReadValue<Vector2>()) -
                           this.transform.position)*100).normalized;
        position = new Vector2(shtrDir.x, Mathf.Clamp(shtrDir.y, .2f, 1f));
    }

    private void ShootGem(InputAction.CallbackContext context)
    {
        GameManager.instance.LaunchGem(position);
    }

}
