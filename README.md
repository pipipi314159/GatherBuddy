# GatherBuddy (TW)

[GatherBuddy](https://github.com/Ottermandias/GatherBuddy) 的 FFXIV 台服（繁體中文）移植版，適用於台服 Dalamud（API 13）。

GatherBuddy 可以用物品名稱查詢採集點與魚的位置、顯示限時採集點與魚的出現時間、天氣預報、鬧鐘，以及釣魚計時器。

> 這是非官方移植版。使用上的問題請回報到本 repo，**不要**回報給原作者。

## 版本資訊

| 項目 | 內容 |
|---|---|
| 原版 | [Ottermandias/GatherBuddy](https://github.com/Ottermandias/GatherBuddy) |
| 基於版本 | 3.8.3.1（commit `93ac72057ee306bddb7996c41701fab683edc45e`，2025-10-12） |
| Dalamud SDK | Dalamud.NET.Sdk 13.1.0（API 13） |
| 分支 | `tw-api13` |

3.8.3.1 是原版改用 API 14（7.4 更新）前的最後一個版本。

## 程式修改

原版只支援英、德、法、日文用戶端，在台服會因語言不符而無法正常運作，因此做了以下修改：

1. **繁中名稱**（`GatherBuddy.GameData/Utility/MultiString.cs`）：物品與地名加入用戶端本身語言（繁中）的名稱；遇到不支援的語言時改用繁中，不再丟出例外。缺少的語言表格只會嘗試讀取一次。
2. **不依賴英文表格**（`GatherBuddy/FishTimer/FishRecorder.Record.cs`、`GatherBuddy.GameData/GameData.cs`）：讀不到英文表格時改用預設值或用戶端語言，避免載入失敗。
3. **釣魚聊天訊息**（`GatherBuddy/FishTimer/Parser/FishingParser.Regexes.cs`）：新增繁中的拋竿、未知釣場、發現新釣場、以小釣大訊息比對規則。
4. **特徵碼容錯**（`GatherBuddy/SeFunctions/SeAddressBase.cs`、`SeFunctionBase.cs`、`CurrentBait.cs`、`CurrentWeather.cs`）：找不到特徵碼時只停用該功能並記錄錯誤，不會讓整個插件載入失敗。
5. 插件顯示名稱改為 `GatherBuddy (TW)`，`InternalName` 維持 `GatherBuddy`，原有設定不會遺失。

## 台服已確認可用

- 採集物、魚、地點以繁中顯示（1117 個採集物、1729 種魚）
- `/gather 物品名` 中文查詢、標示地圖位置、自動傳送
- 艾奧傑亞時間、目前與下一個天氣
- 釣魚計時器：辨識釣場、魚餌、未知釣場、發現新釣場、以小釣大

## 注意事項

- 介面（分頁、按鈕、設定）仍是原版的英文。
- 載入時 `/xllog` 會出現大量 `Could not find fish ...`：這些是國際服新版本的魚，台服尚未更新，可以忽略。
- `/gather` 預設會自動傳送並花費 Gil，不需要的話請到 Config 關閉傳送選項。
- 釣魚計時器預設為編輯模式，請到 Config → Interface → Fish Timer Window 取消勾選「Edit Fish Timer」。

## 編譯

需要 .NET SDK 與台服 Dalamud。

```bat
git clone --recurse-submodules -b tw-api13 https://github.com/pipipi314159/GatherBuddy.git
cd GatherBuddy
set DALAMUD_HOME=<台服 Dalamud 的 Injector 資料夾>
dotnet build GatherBuddy\GatherBuddy.csproj -c Release
```

輸出檔案：`GatherBuddy\bin\Release\GatherBuddy.dll`。在遊戲中用 `/xlsettings` → 實驗性功能 → 開發插件位置加入此路徑。

## 原作者與授權

原作者：[Ottermandias](https://github.com/Ottermandias)。本專案沿用原版的 [Apache License 2.0](LICENSE)。
