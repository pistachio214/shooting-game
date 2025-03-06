using Godot;
using System;
using System.Threading.Tasks;

public partial class HitEffect : Node2D
{
	private GpuParticles2D partGPUParticles;

	public override void _Ready()
	{
		partGPUParticles = GetNode<GpuParticles2D>("PartGPUParticles");

		partGPUParticles.Connect(GpuParticles2D.SignalName.Finished, Callable.From(OnPartGPUParticlesFinished));

		partGPUParticles.Emitting = true;
	}

	public override void _Process(double delta)
	{
	}

	private void OnPartGPUParticlesFinished()
	{
		QueueFree();
	}
}
