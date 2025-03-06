using Godot;
using System;

public partial class Hud : Control
{

	private ProgressBar _progressBar;

	private Label _bulletLabel;

	private Label _weaponNameLabel;

	private TextureRect _weaponTextureRect;

	private Label _levelLabel;

	public override void _Ready()
	{
		_progressBar = GetNode<ProgressBar>("HpControl/HpBar");
		_bulletLabel = GetNode<Label>("WeaponHUDControl/Bullet");
		_weaponNameLabel = GetNode<Label>("WeaponHUDControl/WeaponName");
		_weaponTextureRect = GetNode<TextureRect>("WeaponHUDControl/TextureRect");
		_levelLabel = GetNode<Label>("LevelLabel");

		// 链接PlayerManager -> OnPlayerHpChanged信号
		PlayerManager.Instance.Connect(PlayerManager.SignalName.OnPlayerHpChanged, Callable.From<int, int>(OnPlayerHpChanged));

		// 链接PlayerManager -> OnBulletCountChanged信号
		PlayerManager.Instance.Connect(PlayerManager.SignalName.OnBulletCountChanged, Callable.From<int, int>(OnBulletCountChanged));

		// 链接PlayerManager -> OnBulletCountChanged信号
		PlayerManager.Instance.Connect(PlayerManager.SignalName.OnWeaponReload, Callable.From(OnWeaponReload));

		// 链接PlayerManager -> OnWeaponChanged
		PlayerManager.Instance.Connect(PlayerManager.SignalName.OnWeaponChanged, Callable.From<BaseWeapon>(OnWeaponChanged));

		// 链接LevelManager -> OnLevelChange
		LevelManager.Instance.Connect(LevelManager.SignalName.OnLevelChange, Callable.From<LevelData>(OnLevelChange));

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnPlayerHpChanged(int currentHp, int maxHp)
	{
		_progressBar.MaxValue = maxHp;
		_progressBar.Value = currentHp;
	}

	private void OnBulletCountChanged(int current, int max)
	{
		_bulletLabel.Text = $"{current} / {max}";
	}

	private void OnWeaponReload()
	{
		_bulletLabel.Text = "换弹中...";
	}

	private void OnWeaponChanged(BaseWeapon weapon)
	{
		_weaponNameLabel.Text = weapon.WeaponName;
		_weaponTextureRect.Texture = weapon.sprite.Texture;
	}

	private void OnLevelChange(LevelData _data)
	{
		_levelLabel.Text = $"关卡 {LevelManager.Instance.currentLevel}";
	}
}
