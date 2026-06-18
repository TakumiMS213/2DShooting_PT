# 05 スクリプト参照

## 読む優先度

初心者は、最初から全部読む必要はありません。

まずは次の順番で見るのがおすすめです。

1. `Presentation/Views/PlayerView.cs`
2. `Presentation/Views/BulletView.cs`
3. `Presentation/Views/EnemyView.cs`
4. `Presentation/Configs/PlayerConfig.cs`
3. `Infrastructure/KeyboardPlayerInputSource.cs`

## Views

```text
Assets/_Project/2DShooting/Scripts/Presentation/Views
```

ゲーム画面に近い処理が入っています。

見るポイント:

- `PlayerView`: 自機の移動と射撃。
- `BulletView`: 弾の移動と当たり判定。
- `EnemyView`: 敵のHP、移動、倒された時の処理。
- `GameHudView`: 画面上の文字。
- `TitleScenePresenterView`: タイトル画面の開始処理。
- `ResultScenePresenterView`: リザルト画面のボタン処理。

## Configs

```text
Assets/_Project/2DShooting/Scripts/Presentation/Configs
```

Inspectorに表示される設定項目が書かれています。

見るポイント:

- `PlayerConfig`
- `EnemyConfig`
- `BulletConfig`
- `StageConfig`

## KeyboardPlayerInputSource.cs

```text
Assets/_Project/2DShooting/Scripts/Infrastructure/KeyboardPlayerInputSource.cs
```

キーボード入力を読んでいます。

見るポイント:

- `WASD`
- `矢印キー`
- `Space`
- `Z`

## まとめ

ゲームを調整するだけなら、主に `Configs` を触れば十分です。
