using Godot;
using System;

public partial class HitEffect : Node2D
{
	private GpuParticles2D _partGPUParticles;

	public override void _Ready()
	{
		_partGPUParticles = GetNode<GpuParticles2D>("PartGPUParticles");
		_partGPUParticles.Emitting = true;

		_partGPUParticles.Connect(GpuParticles2D.SignalName.Finished, Callable.From(OnPartGPUParticlesFinished));
	}

	public override void _Process(double delta)
	{
	}

	private void OnPartGPUParticlesFinished()
	{
		QueueFree();
	}
}
