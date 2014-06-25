using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

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

    abstract public class GameScreen
    {
        #region Attributes

        /// <summary>
        /// The game to which this screen belongs
        /// </summary>
        protected GameMain game;

        /// <summary>
        /// The screen that created this one
        /// </summary>
        protected GameScreen father;

        /// <summary>
        /// Time the screen takes to transition on
        /// </summary>
        public TimeSpan transitionOnTime { get; protected set; }

        /// <summary>
        /// Time the screen takes to transition off
        /// </summary>
        public TimeSpan transitionOffTime { get; protected set; }

        /// <summary>
        /// State of the transition, where 0 means fully active, and 1 means completely invisible
        /// </summary>
        public float transitionState { get; protected set; }

        /// <summary>
        /// The alpha value of the transition at the current time
        /// </summary>
        public byte transitionAlpha { get { return (byte)(255 * transitionState); } }

        /// <summary>
        /// The blank texture used to cover the screen, giving the impression of fading
        /// </summary>
        private Texture2D blank;

        /// <summary>
        /// Current screen state.
        /// </summary>
        public ScreenState screenState { get; protected set; }

        /// <summary>
        /// Indicates whether the screen should be deleted when hidden.
        /// </summary>
        public bool isExiting { get; protected set; }

        /// <summary>
        /// Tells the game manager to delete this screen
        /// </summary>
        public bool delete { get { return (isExiting && transitionState == 1); } }

        /// <summary>
        /// The viewport where to draw this screen
        /// </summary>
        protected Viewport viewport;

        /// <summary>
        /// Indicates whether or not this screen is a pop up
        /// </summary>
        public bool isPopup { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of a game screen
        /// </summary>
        /// <param name="game">The game to which this screen belongs</param>
        /// <param name="viewport">The viewport where to draw this screen</param>
        /// <param name="state">The state this screen must be initialized with</param>
        public GameScreen(GameMain game, GameScreen father, Viewport viewport, ScreenState state = ScreenState.TransitionOn)
        {
            this.game = game;
            this.father = father;
            this.viewport = viewport;
            this.screenState = state;

            if (screenState == ScreenState.TransitionOn || screenState == ScreenState.Hidden)
                transitionState = 1;
            else
                transitionState = 0;
        }

        public virtual void LoadContent(ContentManager content)
        {
            blank = content.Load<Texture2D>("blank");
        }

        public virtual void HandleInput(InputHandler input) { return; }

        public virtual void Update(GameTime gameTime)
        {
            // Indicates whether the transitionState should be updated
            bool isTransitioningOn = false;
            bool isTransitioningOff = false;

            // Updates the screen state, if neccessary
            if (screenState == ScreenState.TransitionOn)
                if (transitionState == 0)
                    screenState = ScreenState.Active;
                else
                    isTransitioningOn = true;
            else if (screenState == ScreenState.TransitionOff)
                if (transitionState == 1)
                    screenState = ScreenState.Hidden;
                else
                    isTransitioningOff = true;

            if (!isTransitioningOn && !isTransitioningOff)
                return;

            // If the screen is transitioning, scale the step of the transition to match the time of the transition
            float transitionDelta = 0;

            if (isTransitioningOn)
                if (transitionOnTime == TimeSpan.Zero)
                    transitionDelta = 1;
                else
                    transitionDelta = (float)(gameTime.ElapsedGameTime.Milliseconds / transitionOnTime.TotalMilliseconds);
            else if (isTransitioningOff)
                if (transitionOffTime == TimeSpan.Zero)
                    transitionDelta = 1;
                else
                    transitionDelta = (float)(gameTime.ElapsedGameTime.Milliseconds / transitionOffTime.TotalMilliseconds);

            // Apply changes
            if (screenState == ScreenState.TransitionOn)
                transitionState -= transitionDelta;
            else if (screenState == ScreenState.TransitionOff)
                transitionState += transitionDelta;

            transitionState = MathHelper.Clamp(transitionState, 0.0f, 1.0f);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (screenState == ScreenState.Hidden)
                return;

            spriteBatch.Begin();
            spriteBatch.Draw(blank, new Rectangle(0, 0, viewport.Width, viewport.Height), new Color(0, 0, 0, transitionAlpha));
            spriteBatch.End();
        }

        /// <summary>
        /// Tells the screen to transition off and delete itself
        /// </summary>
        public void ExitScreen()
        {
            screenState = ScreenState.TransitionOff;
            isExiting = true;
            transitionState = 0;
        }

        /// <summary>
        /// Hides the current screen with transition (good for pause screens)
        /// </summary>
        public void Hide()
        {
            screenState = ScreenState.TransitionOff;
            isExiting = false;
            transitionState = 0;
        }

        /// <summary>
        /// Unhides this screen with transition (good for pause screens)
        /// </summary>
        public void Unhide()
        {
            screenState = ScreenState.TransitionOn;
            transitionState = 1;
        }

        /// <summary>
        /// Toggles screen state between active and underneath (good for pop ups)
        /// </summary>
        public void ToggleUnderneath()
        {
            if (screenState == ScreenState.Underneath)
                screenState = ScreenState.Active;
            else if (screenState == ScreenState.Active)
                screenState = ScreenState.Underneath;
        }

        /// <summary>
        /// Kills the screen without transition (good for pop ups)
        /// </summary>
        public void KillScreen()
        {
            screenState = ScreenState.Hidden;
            transitionState = 1;
            isExiting = true;
        }

        /// <summary>
        /// Creates a new pause screen managing the transition
        /// </summary>
        /// <param name="pauseScreen"></param>
        protected void CreatePauseScreen(GameScreen pauseScreen)
        {
            this.Hide();
            game.AddScreen(pauseScreen);
        }

        /// <summary>
        /// If this is a pause screen, finish it managing the transition
        /// </summary>
        protected void FinishPauseScreen()
        {
            if (father == null)
                this.ExitScreen();

            father.Unhide();
            this.ExitScreen();
        }

        /// <summary>
        /// Creates a new pop up, managing the transition
        /// </summary>
        /// <param name="newPopupScreen"></param>
        protected void CreatePopup(GameScreen newPopupScreen)
        {
            game.AddScreen(newPopupScreen);
            this.ToggleUnderneath();
        }

        /// <summary>
        /// Finishes this screen (if it is a pop up) managing the transition
        /// </summary>
        protected void FinishPopup()
        {
            if (!isPopup || father == null)
                return;

            this.KillScreen();
            father.ToggleUnderneath();
        }

        #endregion
    }
}
