using Godot;
using System;

public partial class Hud : Control
{

	private ProgressBar progressBar;

	private Label bulletLabel;

	private Label weaponNameLabel;

	private TextureRect weaponTextureRect;

	public override void _Ready()
	{
		progressBar = GetNode<ProgressBar>("HpControl/HpBar");
		bulletLabel = GetNode<Label>("WeaponHUDControl/Bullet");
		weaponNameLabel = GetNode<Label>("WeaponHUDControl/WeaponName");
		weaponTextureRect = GetNode<TextureRect>("WeaponHUDControl/TextureRect");

		// 链接PlayerManager -> OnPlayerHpChanged信号
		PlayerManager.Instance.Connect(
			PlayerManager.SignalName.OnPlayerHpChanged,
			Callable.From<int, int>(OnPlayerHpChanged)
		);

		// 链接PlayerManager -> OnBulletCountChanged信号
		PlayerManager.Instance.Connect(
			PlayerManager.SignalName.OnBulletCountChanged,
			Callable.From<int, int>(OnBulletCountChanged)
		);

		// 链接PlayerManager -> OnBulletCountChanged信号
		PlayerManager.Instance.Connect(
			PlayerManager.SignalName.OnWeaponReload,
			Callable.From(OnWeaponReload)
		);

		// 链接PlayerManager -> OnWeaponChanged
		PlayerManager.Instance.Connect(
			PlayerManager.SignalName.OnWeaponChanged,
			Callable.From<BaseWeapon>(OnWeaponChanged)
		);


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnPlayerHpChanged(int currentHp, int maxHp)
	{
		progressBar.MaxValue = maxHp;
		progressBar.Value = currentHp;
	}

	private void OnBulletCountChanged(int current, int max)
	{
		bulletLabel.Text = $"{current} / {max}";
	}

	private void OnWeaponReload()
	{
		bulletLabel.Text = "换弹中...";
	}

	private void OnWeaponChanged(BaseWeapon weapon)
	{
		weaponNameLabel.Text = weapon.weaponName;
		weaponTextureRect.Texture = weapon.sprite.Texture;
	}
}
