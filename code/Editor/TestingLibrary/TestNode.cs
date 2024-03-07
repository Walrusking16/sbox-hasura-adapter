using System;
using Editor;

namespace Sandbox.TestingLibrary;

public sealed class TestNode : TreeNode
{
	private Color _orange = Color.Parse( "#e67e22" ).GetValueOrDefault();
	public TestClass ClassData;
	
	public TestNode( TestClass @class )
	{
		ClassData = @class;
	}
	
	protected override void BuildChildren()
	{
		SetChildren( ClassData.Methods, x => new TestMethodNode( x ) );
	}

	public override void OnPaint( VirtualWidget item )
	{
		var fullSpanRect = item.Rect;
		fullSpanRect.Left = 0;
		fullSpanRect.Right = TreeView.Width;

		if ( item.Selected )
		{
			Paint.ClearPen();
			Paint.SetBrush( Theme.Blue.WithAlpha( 0.4f ) );
			Paint.DrawRect( fullSpanRect );

			Paint.SetPen( Color.White );
		}
		else
		{
			Paint.SetPen( Theme.ControlText );
		}

		var r = item.Rect;
		
		switch ( ClassData.Status )
		{
			case TestStatus.Running:
			{
				Paint.SetPen( _orange );
				Paint.DrawIcon( r, "sync", 14, TextFlag.LeftCenter );
				r.Left += 22;
				break;
			}
			case TestStatus.Passed:
			{
				Paint.SetPen( Theme.Green );
				Paint.DrawIcon( r, "check", 14, TextFlag.LeftCenter );
				r.Left += 22;
				break;
			}
			case TestStatus.Failed:
			{
				Paint.SetPen( Theme.Red );
				Paint.DrawIcon( r, "close", 14, TextFlag.LeftCenter );
				r.Left += 22;
				break;
			}
			default:
			{
				Paint.SetPen( Theme.ControlText );
				break;
			}
		}

		Paint.SetDefaultFont();
		Paint.DrawText( r, $"{ClassData.FullName}", TextFlag.LeftCenter );
	}
}

public class TestMethodNode : TreeNode
{
	private Color _orange = Color.Parse( "#e67e22" ).GetValueOrDefault();
	public TestMethod MethodData;
	

	public TestMethodNode( TestMethod method )
	{
		MethodData = method;
	}

	public override void OnPaint( VirtualWidget item )
	{
		var fullSpanRect = item.Rect;
		fullSpanRect.Left = 0;
		fullSpanRect.Right = TreeView.Width;

		if ( item.Selected )
		{
			Paint.ClearPen();
			Paint.SetBrush( Theme.Blue.WithAlpha( 0.4f ) );
			Paint.DrawRect( fullSpanRect );

			Paint.SetPen( Color.White );
		}
		else
		{
			Paint.SetPen( Theme.ControlText );
		}

		var r = item.Rect;
		
		switch ( MethodData.Status )
		{
			case TestStatus.Running:
				{
					Paint.SetPen( _orange );
					Paint.DrawIcon( r, "sync", 14, TextFlag.LeftCenter );
					r.Left += 22;
					break;
				}
			case TestStatus.Passed:
				{
					Paint.SetPen( Theme.Green );
					Paint.DrawIcon( r, "check", 14, TextFlag.LeftCenter );
					r.Left += 22;
					break;
				}
			case TestStatus.Failed:
				{
					Paint.SetPen( Theme.Red );
					Paint.DrawIcon( r, "close", 14, TextFlag.LeftCenter );
					r.Left += 22;
					break;
				}
			default:
				{
					Paint.SetPen( Theme.ControlText );
					break;
				}
		}

		Paint.SetDefaultFont();
		Paint.DrawText( r, $"{MethodData.DisplayName}", TextFlag.LeftCenter );
	}
}
