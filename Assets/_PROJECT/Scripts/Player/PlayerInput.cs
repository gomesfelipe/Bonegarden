using UnityEngine;
using System.Collections;

public struct KeyState
{
    public bool Held;
    public bool Down;
    public bool Up;
}

public struct Command
{
    public Vector2 Movement;
    public KeyState Jump;
    public KeyState Duck;
    public KeyState Stunt;
}

public class PlayerInput : MonoBehaviour
{

    [Header("Keys")]
    public string ForwardKey;
    public string BackKey;
    public string LeftKey;
    public string RightKey;
    public string JumpKey;
    public string DuckKey;
    public string DuckKey2;
    public string StuntKey;

    [Header("External Commands")]
    public bool UseExternalCommand;

    private Command _localCommand;
    private Command _externalCommand;

    public Command GetCommand()
    {
        return _localCommand;
    }

    public void SetExternalCommand(Command other)
    {
        _externalCommand = other;
    }

    void DoInput()
    {
        _localCommand = new Command();

        if (Input.GetKey(ForwardKey))
        {
            _localCommand.Movement.y = 1;
        }
        else if (Input.GetKey(BackKey))
        {
            _localCommand.Movement.y = -1;
        }

        if (Input.GetKey(LeftKey))
        {
            _localCommand.Movement.x = -1;
        }
        else if (Input.GetKey(RightKey))
        {
            _localCommand.Movement.x = 1;
        }

        _localCommand.Jump = new KeyState
        {
            Down = Input.GetKeyDown(JumpKey),
            Held = Input.GetKey(JumpKey),
            Up = Input.GetKeyUp(JumpKey)
        };

        _localCommand.Duck = new KeyState
        {
            Down = Input.GetKeyDown(DuckKey) || Input.GetKeyDown(DuckKey2),
            Held = Input.GetKey(DuckKey) || Input.GetKey(DuckKey2),
            Up = Input.GetKeyUp(DuckKey) || Input.GetKeyUp(DuckKey2)
        };

        _localCommand.Stunt = new KeyState
        {
            Down = Input.GetKeyDown(StuntKey),
            Held = Input.GetKey(StuntKey),
            Up = Input.GetKeyUp(StuntKey)
        };
    }

    void Update()
    {
        DoInput();
    }
}
