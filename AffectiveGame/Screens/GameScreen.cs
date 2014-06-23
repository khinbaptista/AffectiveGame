using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AffectiveGame.Screens
{
    /// <summary>
    /// State of the screen
    /// "Underneath" means there is a pop up above it, so input and update shouldn't apply, but the screen is still visible
    /// "Hidden" means the screen is not visible (input, update and draw do not apply)
    /// </summary>
    public enum ScreenState
    {
        TransitionOn,
        Active,
        Underneath,
        TransitionOff,
        Hidden
    }

    class GameScreen
    {
        
    }
}
