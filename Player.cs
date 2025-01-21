using Godot;
using System;

public partial class Player : Node3D
{
    [Export]
    public float MouseSensitivity = 0.1f;
    [Export]
    public float MoveSpeed = 5.0f;

    private Vector2 _mouseDelta = Vector2.Zero;
    private Camera3D _camera;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _camera = GetNode<Camera3D>("Camera3D");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            _mouseDelta = mouseMotionEvent.Relative;
        }
    }

    public override void _Process(double delta)
    {
        HandleMouseLook(delta);
        HandleMovement(delta);
    }

    private void HandleMouseLook(double delta)
    {
        Vector3 rotation = RotationDegrees;
        rotation.Y -= _mouseDelta.X * MouseSensitivity;
        RotationDegrees = rotation;

        Vector3 cameraRotation = _camera.RotationDegrees;
        cameraRotation.X -= _mouseDelta.Y * MouseSensitivity;
        cameraRotation.X = Mathf.Clamp(cameraRotation.X, -90, 90);
        _camera.RotationDegrees = cameraRotation;

        _mouseDelta = Vector2.Zero;
    }

    private void HandleMovement(double delta)
    {
        Vector3 direction = Vector3.Zero;

        if (Input.IsActionPressed("move_forward"))
        {
            direction -= Transform.Basis.Z;
        }
        if (Input.IsActionPressed("move_backward"))
        {
            direction += Transform.Basis.Z;
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction -= Transform.Basis.X;
        }
        if (Input.IsActionPressed("move_right"))
        {
            direction += Transform.Basis.X;
        }

        direction = direction.Normalized();
        GlobalTransform = new Transform3D(GlobalTransform.Basis, GlobalTransform.Origin + direction * MoveSpeed * (float)delta);
    }
}
