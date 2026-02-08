using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum StateType
{
    None,
    Main,
    Login,
    EnterWorld,
    InventoryOpen,
}

public class StateManager
{
    public static StateManager Instance { get; } = new StateManager();

    public StateType CurrentState { get; set; } = StateType.None;
    public StateType PreviousState { get; set; } = StateType.None;

    public bool IsLogin { get; set; } = false;
    public bool EnterWorld { get; set; } = false;
}