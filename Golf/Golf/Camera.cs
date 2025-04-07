
using BEPUutilities;
using Microsoft.Xna.Framework.Input;
using Golf;
using System;
using System.Diagnostics;
using XnaMathHelper = Microsoft.Xna.Framework.MathHelper;

using XnaMatrix = Microsoft.Xna.Framework.Matrix;
using XnaVector3 = Microsoft.Xna.Framework.Vector3;

namespace Golf
{
    /// <summary>
    /// Basic camera class supporting mouse/keyboard/gamepad-based movement.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// Gets or sets the position of the camera.
        /// </summary>
        public Vector3 Position { get; set; }
        float yaw;
        float pitch;
        /// <summary>
        /// Gets or sets the yaw rotation of the camera.
        /// </summary>
        public float Yaw
        {
            get
            {
                return yaw;
            }
            set
            {
                yaw = MathHelper.WrapAngle(value);
            }
        }
        /// <summary>
        /// Gets or sets the pitch rotation of the camera.
        /// </summary>
        public float Pitch
        {
            get
            {
                return pitch;
            }
            set
            {
                pitch = MathHelper.Clamp(value, -MathHelper.PiOver2, MathHelper.PiOver2);
            }
        }

        /// <summary>
        /// Gets or sets the speed at which the camera moves.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Gets the view matrix of the camera.
        /// </summary>
        public Matrix ViewMatrix { get; private set; }
        /// <summary>
        /// Gets or sets the projection matrix of the camera.
        /// </summary>
        public Matrix ProjectionMatrix { get; set; }

        /// <summary>
        /// Gets the world transformation of the camera.
        /// </summary>
        public Matrix WorldMatrix { get; private set; }

        /// <summary>
        /// Gets the game owning the camera.
        /// </summary>
        public Game1 Game { get; private set; }

        /// <summary>
        /// This will be the position of the ball -CP
        /// </summary>
        public Vector3? Target { get; set; }

        /// <summary>
        /// Keeps track of whether the camera is locked on to a target -CP
        /// </summary>
        private bool isLockedOn = true;

        /// <summary>
        /// Prevents rapid toggle if key is held down -CP
        /// </summary>
        private bool togglePressed = false;


        /// <summary>
        /// Tracks the position of the camera -CP
        /// </summary>
        private float rotationAngle = 0f;

        /// <summary>
        /// Constructs a new camera.
        /// </summary>
        /// <param name="game">Game that this camera belongs to.</param>
        /// <param name="position">Initial position of the camera.</param>
        /// <param name="speed">Initial movement speed of the camera.</param>
        public Camera(Game1 game1, Vector3 position, float speed)
        {
            Game = game1;
            Position = position;
            Speed = speed;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfViewRH(MathHelper.PiOver4, 4f / 3f, .1f, 10000.0f);
            Mouse.SetPosition(200, 200);
        }

        /// <summary>
        /// Moves the camera forward using its speed.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        public void MoveForward(float dt)
        {
            Position += WorldMatrix.Forward * (dt * Speed);
        }
        /// <summary>
        /// Moves the camera right using its speed.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        /// 
        public void MoveRight(float dt)
        {
            Position += WorldMatrix.Right * (dt * Speed);
        }
        /// <summary>
        /// Moves the camera up using its speed.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        /// 
        public void MoveUp(float dt)
        {
            Position += new Vector3(0, (dt * Speed), 0);
        }

        /// <summary>
        /// Updates the camera's view matrix.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        public void Update(float dt)
        {
            // Handle toggling lock-on mode
            if (Game.KeyboardState.IsKeyDown(Keys.P))
            {
                if (!togglePressed)
                {
                    isLockedOn = !isLockedOn; // Toggle lock-on mode
                    togglePressed = true; // Prevent repeated toggling in one frame
                }
            }
            else
            {
                togglePressed = false; // Reset toggle press
            }

            float distance = Speed * dt;
            if (isLockedOn && Target.HasValue)
            {
                LookAtTarget(dt);
            }
            else
            {
                if (Game.KeyboardState.IsKeyDown(Keys.E)) MoveForward(distance);
                if (Game.KeyboardState.IsKeyDown(Keys.D)) MoveForward(-distance);
                if (Game.KeyboardState.IsKeyDown(Keys.S)) MoveRight(-distance);
                if (Game.KeyboardState.IsKeyDown(Keys.F)) MoveRight(distance);
                if (Game.KeyboardState.IsKeyDown(Keys.A)) MoveUp(distance);
                if (Game.KeyboardState.IsKeyDown(Keys.Z)) MoveUp(-distance);
                // Free look mode
                Yaw += (200 - Game.MouseState.X) * dt * .12f;
                Pitch += (200 - Game.MouseState.Y) * dt * .12f;
                Mouse.SetPosition(200, 200);
            }

            WorldMatrix = Matrix.CreateFromAxisAngle(Vector3.Right, Pitch) * Matrix.CreateFromAxisAngle(Vector3.Up, Yaw);

            



            WorldMatrix = WorldMatrix * Matrix.CreateTranslation(Position);
            ViewMatrix = Matrix.Invert(WorldMatrix);
        }


        /// <summary>
        /// Locks camera onto ball
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        private void LookAtTarget(float dt)
        {
            if (Target.HasValue)
            {
                // Adjust rotation angle based on user input
                if (Game.KeyboardState.IsKeyDown(Keys.S))  // Rotate left (counterclockwise)
                {
                    rotationAngle -= dt * 2.0f; // Adjust speed as needed
                }
                if (Game.KeyboardState.IsKeyDown(Keys.F))  // Rotate right (clockwise)
                {
                    rotationAngle += dt * 2.0f;
                }

                // Define camera distance from the ball
                float distance = 30f; // Distance of the camera from the ball
                float height = 15f;   // Height offset above the ball

                // Calculate new camera position using rotationAngle
                float x = Target.Value.X + (float)Math.Sin(rotationAngle) * distance;
                float z = Target.Value.Z + (float)Math.Cos(rotationAngle) * distance;
                float y = Target.Value.Y + height;

                Position = new Vector3(x, y, z); // Update camera position

                // Compute direction from camera to target
                Vector3 direction = Position - Target.Value;
                if (direction.LengthSquared() > 0) // Prevent NaN errors
                {
                    direction.Normalize();
                }

                // Compute yaw and pitch to face the ball
                Yaw = (float)Math.Atan2(direction.X, direction.Z);
                Pitch = -(float)Math.Asin(direction.Y);
            }
        }
    }
}
