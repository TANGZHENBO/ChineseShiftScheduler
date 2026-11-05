namespace ChineseShiftScheduler.Models;
public sealed record Shift(string Name, string Color) {
 public static readonly Shift[] Defaults = [ new("白班", "#2879E8"), new("夜班", "#7739D9"), new("休息", "#20B989") ];
}