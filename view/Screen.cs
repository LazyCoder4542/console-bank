using System;
using System.Runtime.InteropServices;

namespace bank_app.view;

public interface IView
{
  public void GenerateScreenContent();

  public IView? OnInput(string input);
}

internal class Screen
{
  internal IView? currentView;
  internal IView _initialView;

  internal Screen(IView initialView)
  {
    _initialView = initialView;
  }

  public void SwitchTo(IView v)
  {
    currentView = v;
    v.GenerateScreenContent();
  }

  public void Start()
  {
    currentView = _initialView;
    while (currentView is not null)
    {
      currentView.GenerateScreenContent();

      string? input;
      IView? nextView;
      while (true)
      {
        input = Console.ReadLine();
        if (input is null)
        {
          Console.WriteLine("Invalid input");
        }
        else
        {
          nextView = currentView.OnInput(input);

          if (nextView is not null)
          {
            break;
          }
        }
      }
      currentView = nextView;
    }
  }
}
