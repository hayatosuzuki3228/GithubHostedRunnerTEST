# PostgreSQLの接続方法
1. 以下のように`appsettings.json`を作成してください．
データベース名，ユーザ名，パスワードはPostgreSQLの設定に合わせてください
```
{
  "ConnectionStrings": {
    "AppConfigDb": "Host=localhost;Database=hutzper;Username=hutzper;password=password"
  }
}
```
2. `Hutzper.Project.Makiki/bin/Debug/`および`Hutzper.Project.Mekiki/bin/x64/Debug/`以下に`appsettings.json`を配置してください
3. プログラムの実行中はバックグラウンドでPostgreSQLが必ず動作するようにしてください

# マイグレーションの作成
PowerShell上で以下を実行してください
```
dotnet ef migrations add <マイグレーション名>
```
# マイグレーションのDBへの適用
PowerShell上で以下を実行してください
```
dotnet ef database update
```

## 動作確認済環境
- Windows 11
- PostgreSQL(Windows build) 17.4-1

