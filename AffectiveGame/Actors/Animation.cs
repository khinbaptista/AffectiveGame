using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AffectiveGame.Actors
{
    class Animation
    {
        # region Attributes

        private List<Rectangle> frames;

        /// <summary>
        /// The current frame of the animation
        /// </summary>
        public int currentFrameIndex { get; private set; }
        
        /// <summary>
        /// Determines if the animation should loop
        /// </summary>
        public bool isLoop { get; private set; }
        
        /// <summary>
        /// Determines whether or not the animation has ended
        /// </summary>
        public bool isFinished { get; private set; }

        # endregion

        #region Methods

        public Animation(bool isLoop = true)
        {
            frames = new List<Rectangle>();

            this.isLoop = isLoop;
            isFinished = false;
        }

        /// <summary>
        /// Inserts a frame to the animarion
        /// </summary>
        /// <param name="frame">The clipping rectangle over the sprite sheet</param>
        public void InsertFrame(Rectangle frame) { frames.Add(frame); }

        /// <summary>
        /// Inserts a sequency of frames to the current animation
        /// </summary>
        /// <param name="frames">Sequency of clipping rectangles over the sprite sheet</param>
        public void InsertFrameList(List<Rectangle> frames) { this.frames.AddRange(frames); }

        /// <summary>
        /// Sets the animation
        /// </summary>
        /// <param name="frames">List of frames in the animation</param>
        public void SetAnimation(List<Rectangle> frames) { this.frames = frames; }

        /// <summary>
        /// Gets the current frame of the animation
        /// </summary>
        /// <returns>The clipping rectangle of the current frame</returns>
        public Rectangle GetFrame() { return frames[currentFrameIndex]; }

        /// <summary>
        /// Gets a frame of the animation
        /// </summary>
        /// <param name="frameIndex">The desired frame index</param>
        /// <returns>The clipping rectangle of the frame</returns>
        public Rectangle GetFrame(int frameIndex) { return frames[frameIndex]; }

        /// <summary>
        /// Starts playing the animation
        /// </summary>
        public void Play() { currentFrameIndex = 0; isFinished = false; }

        /// <summary>
        /// Updates the animation (gets the next frame)
        /// </summary>
        public void UpdateFrame()
        {
            currentFrameIndex++;

            if (!isLoop && currentFrameIndex == frames.Count - 1)
                isFinished = true;
            else if (isLoop && currentFrameIndex == frames.Count)
                currentFrameIndex = 0;
        }

        # endregion
    }
}
