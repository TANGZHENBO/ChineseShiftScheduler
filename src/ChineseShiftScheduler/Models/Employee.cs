namespace ChineseShiftScheduler.Models;
public sealed class Employee {
 public int Id { get; set; }
 public string Name { get; set; } = "";
 public string Position { get; set; } = "";
 public DateTime CycleStartDate { get; set; } = DateTime.Today;
}