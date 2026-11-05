# 智能循环排班日历

Windows 10/11 中文桌面排班工具，C# WPF / .NET 8 / SQLite。

## 当前功能
- 月度员工排班日历
- 四天循环：白班 → 夜班 → 休息 → 休息
- 员工独立循环起始日期，跨月连续计算
- 单日手动调班，支持恢复循环
- 员工新增、编辑、删除
- 每日出勤人数统计
- CSV 导出
- SQLite 本地持久化
- GitHub Actions 自动编译；推送 v* 标签自动发布 Windows x64 ZIP

## 开发与运行
需要 Windows 10/11、.NET 8 SDK、Visual Studio 2022（.NET 桌面开发）。

    dotnet restore
    dotnet build src/ChineseShiftScheduler/ChineseShiftScheduler.csproj -c Release
    dotnet run --project src/ChineseShiftScheduler/ChineseShiftScheduler.csproj

## 发布
    git tag v1.0.0
    git push origin v1.0.0

数据库位于 %LOCALAPPDATA%/ChineseShiftScheduler/scheduler.db。