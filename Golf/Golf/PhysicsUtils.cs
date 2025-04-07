using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

//using MonoBlocks;

using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUutilities;

using XnaMatrix = Microsoft.Xna.Framework.Matrix;
using XnaVector3 = Microsoft.Xna.Framework.Vector3;
using XnaBoundingBox = Microsoft.Xna.Framework.BoundingBox;
using XnaMathHelper = Microsoft.Xna.Framework.MathHelper;
using BEPUMatrix = BEPUutilities.Matrix;
using BEPUVector3 = BEPUutilities.Vector3;
using BEPUMathHelper = BEPUutilities.MathHelper;
using System.Diagnostics;
namespace Golf
{
    public static class PhysicsUtils
    {
        //Function returns true if ball's speed is very low
        public static bool BallStopped(Sphere ballBody)
        {
            if (ballBody.LinearVelocity.LengthSquared() < 0.1f && ballBody.AngularVelocity.LengthSquared() < 0.1f)
            {
                ballBody.LinearVelocity = BEPUVector3.Zero;
                ballBody.AngularVelocity = BEPUVector3.Zero;
                return true;
            }
            return false;
        }

        public static void Charging(
            ref bool isCharging,
            ref float chargeTime,
            float maxChargeTime,
            GameTime gameTime,
            KeyboardState keyboardState,
            Camera camera,
            Sphere ballBody,
            ref int score)
        {
            if (keyboardState.IsKeyDown(Keys.Space) && !isCharging)
            {
                isCharging = true;
                chargeTime = 0f;
            }

            if (keyboardState.IsKeyDown(Keys.Space) && isCharging)
            {
                chargeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                chargeTime = Microsoft.Xna.Framework.MathHelper.Min(chargeTime, maxChargeTime);
            }

            if (keyboardState.IsKeyUp(Keys.Space) && isCharging)
            {
                isCharging = false;

                float strengthRatio = chargeTime / maxChargeTime;
                float puttStrength = strengthRatio * 1200f;

                XnaVector3 ballPosition = ConversionHelper.MathConverter.Convert(ballBody.Position);
                XnaVector3 forwardDirection = ballPosition - ConversionHelper.MathConverter.Convert(camera.Position);
                forwardDirection.Y = 1;
                forwardDirection.Normalize();

                BEPUVector3 forceDirection = ConversionHelper.MathConverter.Convert(forwardDirection * puttStrength);
                ballBody.ApplyLinearImpulse(ref forceDirection);

                score++;
            }
        }


    }
}
