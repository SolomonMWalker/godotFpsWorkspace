using System;
using System.Diagnostics;
using Godot;

public partial class Player : Node3D
{
	public float mouseSensitivity = 0.002f;
	public float movementSpeed = 5.0f;
	public float jumpHeight = 1.2f;
	public float gravityForce = 9.8f;

    private CharacterBody3D _characterBody;
    private Camera3D _camera;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _characterBody = GetNode<CharacterBody3D>("CharacterBody3D");
        _camera = GetNode<Camera3D>("CharacterBody3D/Camera3D");

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotionEvent)
		{
			RotateToMouse(mouseMotionEvent);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		var deltaFloat = (float) delta;
		var input = GetButtonPressInput();
		Move(input, deltaFloat);
	}

	public InputHandler GetButtonPressInput()
	{	
		if(Input.IsKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}

		var movementInputVector = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");

		return new InputHandler
		{
			CharacterMoveInput = new Vector3(movementInputVector.X, 0, movementInputVector.Y)
		};
	}

	public void Move(InputHandler inputHandler, float delta)
	{
		var newVelocity = _characterBody.Velocity;
		newVelocity += Vector3.Down * gravityForce * delta;

		var movementDirection = (_characterBody.Transform.Basis * inputHandler.CharacterMoveInput).Normalized();
		newVelocity.X = movementDirection.X * movementSpeed;
		newVelocity.Z = movementDirection.Z * movementSpeed;
		
		if (_characterBody.IsOnFloor())
		{
			if (Input.IsActionJustPressed("jump"))
			{
				newVelocity.Y = Mathf.Sqrt(2 * jumpHeight * gravityForce);
			}
		}

		_characterBody.Velocity = newVelocity;

		_characterBody.MoveAndSlide();
	}

	public void RotateToMouse(InputEventMouseMotion mouseMotionEvent)
	{
		_characterBody.RotateY(-mouseMotionEvent.Relative.X * mouseSensitivity);

		_camera.RotateX(-mouseMotionEvent.Relative.Y * mouseSensitivity);
		_camera.Rotation = Rotation with {
			X = Mathf.Clamp(
					_camera.Rotation.X,
					-Mathf.DegToRad(80),
					Mathf.DegToRad(80)
				)
		};
	}
}
