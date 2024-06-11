using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : Singleton<InputHandler>
{
    public PlayerControls playerControls;
    public Vector2 InputVector { get; protected set; }

    public Vector2 MousePosition { get; protected set; }
    protected void OnEnable(){
        if (playerControls == null)
        {
            playerControls = new();
            playerControls.Player.Move.performed += i => InputVector = i.ReadValue<Vector2>();
            playerControls.Player.Move.canceled += i => InputVector = Vector2.zero;
            playerControls.Player.Look.performed += i => MousePosition = i.ReadValue<Vector2>();
            playerControls.Player.Look.canceled += i => MousePosition = Vector2.zero;
        }
        playerControls.Enable();

    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
