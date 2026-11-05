using System.Windows;
using System.Windows.Controls;
using ChineseShiftScheduler.Models;
namespace ChineseShiftScheduler;
public sealed class EmployeeDialog : Window {
 private readonly TextBox _name=new(){MinWidth=230};
 private readonly TextBox _position=new(){MinWidth=230};
 private readonly DatePicker _start=new(){SelectedDate=DateTime.Today};
 public Employee Result {get;}
 public EmployeeDialog(Employee? source=null){Result=source==null?new Employee():new Employee{Id=source.Id,Name=source.Name,Position=source.Position,CycleStartDate=source.CycleStartDate};Title=source==null?"新增员工":"编辑员工";Width=370;Height=280;WindowStartupLocation=WindowStartupLocation.CenterOwner;_name.Text=Result.Name;_position.Text=Result.Position;_start.SelectedDate=Result.CycleStartDate;var p=new StackPanel{Margin=new Thickness(18)};p.Children.Add(new TextBlock{Text="员工姓名"});p.Children.Add(_name);p.Children.Add(new TextBlock{Text="岗位 / 职务",Margin=new Thickness(0,8,0,0)});p.Children.Add(_position);p.Children.Add(new TextBlock{Text="循环起始日期（第1天：白班）",Margin=new Thickness(0,8,0,0)});p.Children.Add(_start);var row=new StackPanel{Orientation=Orientation.Horizontal,HorizontalAlignment=HorizontalAlignment.Right,Margin=new Thickness(0,14,0,0)};var ok=new Button{Content="保存",Width=80,IsDefault=true};ok.Click+=(_,_)=>{if(string.IsNullOrWhiteSpace(_name.Text)){MessageBox.Show("请输入员工姓名");return;}Result.Name=_name.Text.Trim();Result.Position=_position.Text.Trim();Result.CycleStartDate=_start.SelectedDate??DateTime.Today;DialogResult=true;};row.Children.Add(ok);row.Children.Add(new Button{Content="取消",Width=80,Background=System.Windows.Media.Brushes.Gray,IsCancel=true});p.Children.Add(row);Content=p;}
}