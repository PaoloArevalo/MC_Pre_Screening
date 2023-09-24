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
        playerInput.Cannon.PauseGame.performed += PauseGame;
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
                           this.transform.position)).normalized;

        transform.right = shtrDir;
        transform.eulerAngles = new Vector3(0, 0, Mathf.Clamp(transform.eulerAngles.z, 20, 160));
    }

    private void ShootGem(InputAction.CallbackContext context)
    {
        if (GameManager.instance.GameOnGoing && !GameManager.instance.GameIsPause)
            GameManager.instance.LaunchGem(transform.right);
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        if (!GameManager.instance.GameIsPause)
        {
            GameManager.instance.GameIsPause = true;
            GameUI.Instance.ShouldPause(true);
        }
        else
        {
            GameUI.Instance.ShouldPause(false);
            GameManager.instance.GameIsPause = false;
        }
    }

}
