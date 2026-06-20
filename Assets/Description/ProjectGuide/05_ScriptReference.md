# スクリプトを読む順番

1. `Scripts/Player.cs` — 入力、移動、弾を出す
2. `Scripts/Bullet.cs` — 弾が進み、敵へダメージを渡す
3. `Scripts/Enemy.cs` — ダメージを受け、倒れたことを知らせる
4. `Scripts/GameRules.cs` — 敵を出し、クリアを判定する
5. `Scripts/ScreenFlow.cs` — Sceneを切り替える

最初は、各クラスの `Update` とTooltipが付いたpublic変数だけ追えば十分です。継承、Interface、DIコンテナ、イベントバスは使っていません。
