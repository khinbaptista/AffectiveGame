using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AffectiveGame
{
    /// <summary>
    /// Possible controller inputs
    /// </summary>
    public enum Input
    {
        Up,
        Down,
        Left,
        Right,
        A,
        B,
        X,
        Y,
        LeftBumper,
        LeftTrigger,
        RightBumper,
        RightTrigger,
        Start,
        Back
    }

    /// <summary>
    /// Handles input
    /// </summary>
    public class InputHandler
    {
        /// <summary>
        /// Gives the input values for non-binary buttons (triggers and left thumbstick)
        /// </summary>
        public struct TriggerValues
        {
            public float LeftTrigger;
            public float RightTrigger;

            public float XaxisLeft;
            public float YaxisLeft;

            public float XaxisRight;
            public float YaxisRight;
        }

        private List<Input> previousStatus;
        private TriggerValues previousValues;
        private List<Input> currentStatus;
        private TriggerValues currentValues;

        private const float buttonThreshold = 0.5f;

        public InputHandler()
        {
            previousStatus = new List<Input>();
            previousValues = new TriggerValues();
            currentStatus = new List<Input>();
            currentValues = new TriggerValues();

            currentStatus = new List<Input>();
        }

        public void Update()
        {
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            // Clear previous status
            previousStatus = new List<Input>();

            // Copy old status into new previous status
            foreach (Input input in currentStatus)
                previousStatus.Add(input);

            // Clear old current status
            currentStatus = new List<Input>();

            // Update new current status
            if (padState.ThumbSticks.Left.Y >= buttonThreshold)
                currentStatus.Add(Input.Up);

            if (padState.ThumbSticks.Left.Y <= -buttonThreshold)
                currentStatus.Add(Input.Down);

            if (padState.ThumbSticks.Left.X >= buttonThreshold)
                currentStatus.Add(Input.Right);

            if (padState.ThumbSticks.Left.X <= -buttonThreshold)
                currentStatus.Add(Input.Left);

            if (padState.Buttons.A == ButtonState.Pressed)
                currentStatus.Add(Input.A);

            if (padState.Buttons.B == ButtonState.Pressed)
                currentStatus.Add(Input.B);

            if (padState.Buttons.X == ButtonState.Pressed)
                currentStatus.Add(Input.X);

            if (padState.Buttons.Y == ButtonState.Pressed)
                currentStatus.Add(Input.Y);

            if (padState.Buttons.LeftShoulder == ButtonState.Pressed)
                currentStatus.Add(Input.LeftBumper);

            if (padState.Triggers.Left >= buttonThreshold)
                currentStatus.Add(Input.LeftTrigger);

            if (padState.Buttons.RightShoulder == ButtonState.Pressed)
                currentStatus.Add(Input.RightBumper);

            if (padState.Triggers.Right >= buttonThreshold)
                currentStatus.Add(Input.RightTrigger);

            if (padState.Buttons.Start == ButtonState.Pressed)
                currentStatus.Add(Input.Start);

            if (padState.Buttons.Back == ButtonState.Pressed)
                currentStatus.Add(Input.Back);

            // Copy old current trigger values to new previous trigger values
            previousValues = currentValues;

            // Update new current trigger values
            currentValues = new TriggerValues();

            currentValues.LeftTrigger = padState.Triggers.Left;
            currentValues.RightTrigger = padState.Triggers.Right;

            currentValues.XaxisLeft = padState.ThumbSticks.Left.X;
            currentValues.YaxisLeft = padState.ThumbSticks.Left.Y;

            currentValues.XaxisRight = padState.ThumbSticks.Right.X;
            currentValues.YaxisRight = padState.ThumbSticks.Right.Y;
        }

        public List<Input> getPreviousStatus()
        {
            return previousStatus;
        }

        public TriggerValues getPreviousValues()
        {
            return previousValues;
        }

        public List<Input> getStatus()
        {
            return currentStatus;
        }

        public TriggerValues getValues()
        {
            return currentValues;
        }

        /// <summary>
        /// Checks input
        /// </summary>
        /// <param name="input">The input item you are looking for</param>
        /// <returns>Wheter or not the player input was found</returns>
        public bool Contains(Input input)
        {
            return currentStatus.Contains(input);
        }

        public bool IsNewStatus(Input input)
        {
            return currentStatus.Contains(input) && !previousStatus.Contains(input);
        }
    }
}
