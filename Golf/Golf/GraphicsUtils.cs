using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using System.Diagnostics;
namespace Golf
{
    public static class GraphicsUtils
    {
        //Helper function for drawing startup menu
        public static void DrawMenu(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, SpriteFont menuFont)
        {
            string menuText = "Press 1 For Single Player Mode\nPress 2 For Two Player Mode";

            // These two lines must be present any time you draw to the screen
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            spriteBatch.Begin();

            // Calculate center screen position
            Vector2 textSize = menuFont.MeasureString(menuText);
            Vector2 screenCenter = new Vector2(graphicsDevice.Viewport.Width / 2f, graphicsDevice.Viewport.Height / 2f);
            Vector2 textPosition = screenCenter - (textSize / 2f);

            //Draw menu text
            spriteBatch.DrawString(menuFont, menuText, textPosition, Color.White);

            // End 2d drawing
            spriteBatch.End();
        }

        public static void DrawScore(SpriteBatch spriteBatch, SpriteFont scoreFont, GraphicsDevice graphicsDevice, int score)
        {
            string scoreText = $"Score: {score}";
            Vector2 textSize = scoreFont.MeasureString(scoreText);
            Vector2 position = new Vector2(graphicsDevice.Viewport.Width - textSize.X - 20, 20); // Top-right corner with padding

            graphicsDevice.DepthStencilState = DepthStencilState.None;
            spriteBatch.Begin();
            spriteBatch.DrawString(scoreFont, scoreText, position, Color.White);
            spriteBatch.End();
        }

        public static void DrawShotMeter(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, float chargeTime, float maxChargeTime)
        {
            int meterWidth = 200;
            int meterHeight = 20;
            int x = 20; // Position on screen (bottom-left corner)
            int y = graphicsDevice.Viewport.Height - 50;

            //Calculate the filled width based on charge time
            float percent = MathHelper.Clamp(chargeTime / maxChargeTime, 0f, 1f);
            int filledWidth = (int)(percent * meterWidth);

            // Set background and fill of bar
            Texture2D meterBackground = new Texture2D(graphicsDevice, 1, 1);
            meterBackground.SetData(new[] { Color.Gray });

            Texture2D meterFill = new Texture2D(graphicsDevice, 1, 1);
            meterFill.SetData(new[] { Color.LimeGreen });

            // Draw the background of the meter
            spriteBatch.Begin();
            spriteBatch.Draw(meterBackground, new Rectangle(x, y, meterWidth, meterHeight), Color.Gray);

            // Draw the filled portion
            spriteBatch.Draw(meterFill, new Rectangle(x, y, filledWidth, meterHeight), Color.LimeGreen);
            spriteBatch.End();
        }

        public static void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }
    }
}
