using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDieState : IState
{
    private PlayerPresenter _presenter;

    public PlayerDieState(PlayerPresenter presenter)
    {
        _presenter = presenter;
    }

    public void Enter()
    {
        Debug.Log("Entering Die State");
        _presenter.View.PlayDeathAnim();
    }

    public void Tick()
    {
        // Chờ cho đến khi animation chết kết thúc (có thể dùng sự kiện animation để làm việc này)
        
    }

    public void Exit()
    {
        Debug.Log("Exiting Die State");
    }
}
