# 2DShooting 触る場所ガイド

## 最初に見る場所

このプロジェクトで初心者が主に触る場所は3つです。

- `Scenes`: ゲームの画面。
- `Configs`: ゲームの数値や見た目の設定。
- `Prefabs`: Player、Enemy、Bulletの部品。

スクリプトは、最初から全部読む必要はありません。

## 画面

```text
Assets/_Project/2DShooting/Scenes
```

- `TitleScene.unity`: タイトル画面。
- `GameScene.unity`: 実際に遊ぶ画面。
- `ResultScene.unity`: 結果画面。

まずは `TitleScene` から再生してください。

## 設定

```text
Assets/_Project/2DShooting/Configs
```

ここにあるファイルを選ぶと、Inspectorでゲームの値を変えられます。

- `PlayerConfig.asset`: 自機の速さ、色、大きさ、射撃間隔。
- `EnemyConfig.asset`: 敵のHP、色、大きさ、数、動き。
- `BulletConfig.asset`: 弾の速さ、ダメージ、数、弾道。
- `StageConfig.asset`: クリア条件、Scene名、背景速度。

## 操作

- 移動: `WASD` または `矢印キー`
- ショット: `Space` または `Z`
- 開始/リトライ: `Enter` または `Space`
- リザルトからタイトルへ戻る: `Esc`

## おすすめの触り方

1. `GameScene` を開いて、PlayerとEnemyの位置を見る。
2. `PlayerConfig.asset` の `Move Speed` を変えて再生する。
3. `EnemyConfig.asset` の `Max Health` を変えて再生する。
4. `BulletConfig.asset` の `Damage` や `Speed` を変えて再生する。
5. `EnemyConfig.asset` の `Enemy Count` と `StageConfig.asset` の `Required Enemy Defeats` を同じ数にする。

## 注意

`Tools > 2DShooting > Build Prototype` を実行すると、SceneやPrefabが再生成される可能性があります。

講座中に手動で編集した後は、必要がない限り実行しないでください。
