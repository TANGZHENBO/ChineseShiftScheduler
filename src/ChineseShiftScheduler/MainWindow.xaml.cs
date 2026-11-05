using System.IO;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using ChineseShiftScheduler.Models;
using ChineseShiftScheduler.Services;
namespace ChineseShiftScheduler;
public partial class MainWindow : Window {
 private readonly ScheduleStore _store=new();
 private DateTime _month=new(DateTime.Today.Year,DateTime.Today.Month,1);
 private List<Employee> _employees=[];
 private readonly Dictionary<string,Shift> _shifts=Shift.Defaults.ToDictionary(x=>x.Name);
 public MainWindow(){InitializeComponent();RefreshView();}
 private void RefreshView(){_employees=_store.Employees();MonthLabel.Text=_month.ToString("yyyy年 M月",CultureInfo.GetCultureInfo("zh-CN"));BuildCalendar();}
 private static SolidColorBrush B(string c)=>new((Color)ColorConverter.ConvertFromString(c));
 private void Cell(string text,int row,int col,string color,bool bold=false){var b=new Border{Background=B(color),BorderBrush=B("#DCE4EC"),BorderThickness=new Thickness(.5),Padding=new Thickness(3)};b.Child=new TextBlock{Text=text,TextAlignment=TextAlignment.Center,VerticalAlignment=VerticalAlignment.Center,HorizontalAlignment=HorizontalAlignment.Center,FontSize=12,FontWeight=bold?FontWeights.SemiBold:FontWeights.Normal,Foreground=B("#334155")};Grid.SetRow(b,row);Grid.SetColumn(b,col);CalendarGrid.Children.Add(b);}
 private string ShiftFor(Employee e,DateTime d)=>_store.Override(e.Id,d)??ScheduleStore.Cycle(d,e.CycleStartDate);
 private void BuildCalendar(){CalendarGrid.Children.Clear();CalendarGrid.RowDefinitions.Clear();CalendarGrid.ColumnDefinitions.Clear();int days=DateTime.DaysInMonth(_month.Year,_month.Month);CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition{Width=new GridLength(145)});for(int d=0;d<days;d++)CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition{Width=new GridLength(64)});CalendarGrid.RowDefinitions.Add(new RowDefinition{Height=new GridLength(45)});foreach(var e in _employees)CalendarGrid.RowDefinitions.Add(new RowDefinition{Height=new GridLength(54)});CalendarGrid.RowDefinitions.Add(new RowDefinition{Height=new GridLength(40)});Cell("员工 / 日期",0,0,"#EAF0F5",true);for(int d=1;d<=days;d++){var date=new DateTime(_month.Year,_month.Month,d);string w="日一二三四五六"[(int)date.DayOfWeek].ToString();Cell($"{d}\n{w}",0,d,date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday?"#FFF1F2":"#EAF0F5",true);}
 for(int r=0;r<_employees.Count;r++){var emp=_employees[r];Cell($"{emp.Name}\n{emp.Position}",r+1,0,"#F8FAFC",true);for(int d=1;d<=days;d++){var date=new DateTime(_month.Year,_month.Month,d);string s=ShiftFor(emp,date);var sh=_shifts.GetValueOrDefault(s)??new Shift(s,"#64748B");var btn=new Button{Content=s=="休息"?"休":s,Background=B(sh.Color),Foreground=Brushes.White,FontWeight=FontWeights.SemiBold,Margin=new Thickness(1),Padding=new Thickness(1),ToolTip=$"{emp.Name} {date:yyyy-MM-dd} {s}"};btn.Click+=(_,_)=>EditShift(emp,date);Grid.SetRow(btn,r+1);Grid.SetColumn(btn,d);CalendarGrid.Children.Add(btn);}}
 int sr=_employees.Count+1;Cell("每日在岗人数",sr,0,"#EAF0F5",true);for(int d=1;d<=days;d++){var date=new DateTime(_month.Year,_month.Month,d);int n=_employees.Count(e=>ShiftFor(e,date)!="休息");Cell(n.ToString(),sr,d,"#F8FAFC");}}
 private void EditShift(Employee emp,DateTime date){var menu=new ContextMenu();foreach(var sh in _shifts.Values){var item=new MenuItem{Header=sh.Name};item.Click+=(_,_)=>{_store.SetOverride(emp.Id,date,sh.Name);RefreshView();StatusText.Text=$"{emp.Name} {date:MM-dd} 已调整为 {sh.Name}";};menu.Items.Add(item);}menu.Items.Add(new Separator());var reset=new MenuItem{Header="按循环恢复"};reset.Click+=(_,_)=>{_store.ClearOverride(emp.Id,date);RefreshView();StatusText.Text="已恢复循环班次";};menu.Items.Add(reset);menu.IsOpen=true;}
 private void Previous_Click(object s,RoutedEventArgs e){_month=_month.AddMonths(-1);RefreshView();}
 private void Next_Click(object s,RoutedEventArgs e){_month=_month.AddMonths(1);RefreshView();}
 private void Today_Click(object s,RoutedEventArgs e){_month=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);RefreshView();}
 private void Generate_Click(object s,RoutedEventArgs e){RefreshView();StatusText.Text="已重新计算本月循环排班；已保存的手动调班不会被覆盖。";}
 private void AddEmployee_Click(object s,RoutedEventArgs e){var dlg=new EmployeeDialog();dlg.Owner=this;if(dlg.ShowDialog()==true){_store.Save(dlg.Result);RefreshView();}}
 private void EditEmployee_Click(object s,RoutedEventArgs e){var emp=_employees.FirstOrDefault();if(emp==null){AddEmployee_Click(s,e);return;}var dlg=new EmployeeDialog(emp){Owner=this};if(dlg.ShowDialog()==true){_store.Save(dlg.Result);RefreshView();}}
 private void DeleteEmployee_Click(object s,RoutedEventArgs e){var emp=_employees.FirstOrDefault();if(emp==null)return;if(MessageBox.Show($"确定删除员工 {emp.Name}？","确认",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes){_store.Delete(emp.Id);RefreshView();}}
 private void Export_Click(object s,RoutedEventArgs e){var dlg=new SaveFileDialog{Filter="CSV 文件 (*.csv)|*.csv",FileName=$"排班表_{_month:yyyy年MM月}.csv"};if(dlg.ShowDialog()!=true)return;var lines=new List<string>{"员工,"+string.Join(",",Enumerable.Range(1,DateTime.DaysInMonth(_month.Year,_month.Month)).Select(d=>$"{d}日"))};foreach(var emp in _employees){var vals=Enumerable.Range(1,DateTime.DaysInMonth(_month.Year,_month.Month)).Select(d=>ShiftFor(emp,new DateTime(_month.Year,_month.Month,d)));lines.Add(emp.Name+","+string.Join(",",vals));}File.WriteAllText(dlg.FileName,"\uFEFF"+string.Join(Environment.NewLine,lines));StatusText.Text="CSV 导出完成："+dlg.FileName;}
}