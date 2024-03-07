using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Editor;
using TestingLibrary;
using Button = Editor.Button;

namespace Sandbox.TestingLibrary;

[Dock( "Editor", "Testing Library", "list" )]
public sealed class TestingLibraryWidget : Widget
{
	private Layout Body;
	private Layout Header;
	private TreeView TreeView;
	public bool IsDirty;

	protected List<TestClass> TestClasses = new();
	private bool _runningTests;
	
	public TestingLibraryWidget( Widget parent ) : base( parent )
	{
		Layout = Layout.Column();
		
		BuildUI();
	}

	[EditorEvent.Hotload]
	public async Task BuildUI()
	{
		TestClasses.Clear();
		Layout.Clear( true );
		
		SetStyles( "background-color: #171819;" );
		
		Header = Layout.AddColumn();
		
		var runButton = new Button("Run Tests" );
		runButton.Clicked += async () =>
		{
			if ( !_runningTests )
			{
				BuildUI();
				
				await Task.Delay( 100 );
			}
			
			_runningTests = true;
			foreach (var testClass in TestClasses)
			{
				testClass.RunTest();
			}
			_runningTests = false;
		};
		
		Header.Add( runButton );
		
		Body = Layout.AddColumn();
		
		TreeView = new TreeView(Parent);

		Body.Add( TreeView );
		
		GetTests();
 	}
	
	[EditorEvent.Frame]
	public void Frame()
	{
		if ( !IsDirty )
		{
			return;
		}

		IsDirty = false;

		foreach ( var @class in TestClasses )
		{
			@class.Node.Dirty();
		}
	}
	
	private void GetTests()
	{
		var typesWithAttribute = Assembly.GetExecutingAssembly().GetTypes()
			.Where(type => type.GetCustomAttributes(typeof(Test), true).Length > 0);

		foreach (var type in typesWithAttribute)
		{
			var classData = new TestClass( type, this );
			TestClasses.Add( classData );
			
			var node = new TestNode(classData);
			
			classData.Node = node;
			
			TreeView.Open(node);
			
			TreeView.AddItem( node );
		}
	}
}

public enum TestStatus
{
	NotRun,
	Running,
	Passed,
	Failed
}

public class TestClass
{
	public string Name;
	public Type Type;
	public TestStatus Status = TestStatus.NotRun;
	public TestNode Node;
	
	private TestingLibraryWidget _widget;
	
	public List<TestMethod> Methods = new();
	private int testsRan;
	public string FullName => $"{Name} ({testsRan}/{Methods.Count})";

	public TestClass( Type type, TestingLibraryWidget widget)
	{
		_widget = widget;
		
		Type = type;
		{
			var attr = type.GetCustomAttribute<Test>();

			var name = attr?.Name ?? type.Name;

			Name = name;
		}
		
		var methods = type.GetMethods();
		
		foreach (var method in methods)
		{
			if ( method.GetCustomAttributes( typeof(Test), true ).Length <= 0 )
			{
				continue;
			}
			
			var attr = method.GetCustomAttribute<Test>();

			var methodName = attr?.Name ?? method.Name;
			
			Methods.Add( new TestMethod { Name = methodName, DisplayName = methodName, MethodInfo = method } );
		}
	}

	public async Task RunTest()
	{
		var testFailed = false;
		Status = TestStatus.Running;
		
		foreach (var method in Methods)
		{
			method.Status = TestStatus.Running;
			var timer = new Stopwatch();
			timer.Start();
			try
			{
				var instance = Activator.CreateInstance( Type );
				var result = method.MethodInfo.Invoke(instance, null);
				if (result is Task task)
				{
					await task;
				}
				method.Status = TestStatus.Passed;
			}
			catch ( Exception e )
			{
				testFailed = true;
				method.Status = TestStatus.Failed;
			}
			finally
			{
				testsRan++;
				timer.Stop();
				method.DisplayName += $" [{timer.ElapsedMilliseconds}ms]";
			}
			
			_widget.IsDirty = true;
		}
		
		Status = testFailed ? TestStatus.Failed : TestStatus.Passed;
	}
}

public class TestMethod {
	public string Name;
	public string DisplayName;
	
	public MethodInfo MethodInfo;
	public TestStatus Status = TestStatus.NotRun;
}
