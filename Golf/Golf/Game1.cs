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
using XnaVector2 = Microsoft.Xna.Framework.Vector2;
using XnaBoundingBox = Microsoft.Xna.Framework.BoundingBox;
using XnaMathHelper = Microsoft.Xna.Framework.MathHelper;
using BEPUMatrix = BEPUutilities.Matrix;
using BEPUVector3 = BEPUutilities.Vector3;
using BEPUMathHelper = BEPUutilities.MathHelper;
using System.Diagnostics;



namespace Golf;

//TODO: 

//1: Game Play
// a-hole detection
// b-2 players
// c-High Scores
// d-friction
//2:Sound
//3:Another hole model


public class Game1 : Game
{
    // Define game state enum for tracking menu and one and two player modes
    enum GameState
    {
        Menu,
        Single,
        Double
    }

    // Define objects for graphics and sprite batch
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _scoreFont;
    private SpriteFont _menuFont;

    // Visual models
    private Model _holeModel;
    private Model _ballModel;

    //Bepu model for ball
    Sphere ballBody = new Sphere(new BEPUutilities.Vector3(0, 50, 0), 3.5f, 10.0f);
    XnaMatrix ballPos;

    //Bepu model for hole
    private StaticMesh holeBody;

    //Physics space
    private Space space;

    //Vector at 0,0,0
    private XnaMatrix world = XnaMatrix.CreateTranslation(new XnaVector3(0, 0, 0));
    
    //Camera object taken from bepuphysics1 tutorial on github
    Camera camera;

    //Keyboard and mouse state
    public KeyboardState KeyboardState;
    public MouseState MouseState;

    //Variables for adjusting power based on spacebar press time
    bool isCharging = false;
    float chargeTime = 0f;
    float maxChargeTime = 3f;

    GameState currentGameState = GameState.Menu;

    private int p1_score = 0;
    private int p2_score = 0;

    //Initialize graphics, etc.
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        //Load assets for game
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _scoreFont = Content.Load<SpriteFont>("ScoreFont");
        _menuFont = Content.Load<SpriteFont>("MenuFont");

        _holeModel = Content.Load<Model>("test_hole");
        _ballModel = Content.Load<Model>("ball");

        //initialize camera and physics space
        camera = new Camera(this, new BEPUVector3(0, 10, -10), 10.0f);
        space = new Space();

        //Load hole model based on vertices
        DataExtractor.GetVerticesAndIndicesFromModel(_holeModel, out BEPUVector3[] vertices, out int[] indices);
        holeBody = new StaticMesh(vertices, indices);
        XnaVector3 holeCenter = new XnaVector3(0, 0, 0);
        XnaMatrix holeWorld = XnaMatrix.CreateTranslation(holeCenter);

        // Set the holes friction and bounciness
        holeBody.Material.KineticFriction = 0.2f;
        holeBody.Material.StaticFriction = 2.0f;
        holeBody.Material.Bounciness = 0.6f;


        ballBody.Position = new BEPUutilities.Vector3(0, 50, 0); // ball spawn loc

        // Set the ball's friction and bounciness
        ballBody.Material.KineticFriction = 30.0f;   
        ballBody.Material.StaticFriction = 0.3f;    
        ballBody.Material.Bounciness = 0.1f;

        //Add both the hole and ball model to physics space
        space.Add(holeBody);
        space.Add(ballBody);

        // Set the gravity for the physics space
        space.ForceUpdater.Gravity = new BEPUVector3(0, -60.0f, 0);
    }

    // Think of this as the game loop running once per frame
    protected override void Update(GameTime gameTime)
    {
        //Get the keyboard and mouse state
        KeyboardState = Keyboard.GetState();
        MouseState = Mouse.GetState();

        //Exit if user ever presses escape
        if (KeyboardState.IsKeyDown(Keys.Escape))
            Exit();

        // Game menu
        if (currentGameState == GameState.Menu)
        {
            // Press Enter to start the game
            if (KeyboardState.IsKeyDown(Keys.D1))
            {
                currentGameState = GameState.Single;
            }
            if (KeyboardState.IsKeyDown(Keys.D2))
            {
                currentGameState = GameState.Double;
            }

            base.Update(gameTime);
            return;
        }
        else
        {
            //Update physics space
            space.Update();

            //Set camera target to the ball position
            camera.Target = (ballBody.Position);
            camera.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            //Set graphics object position to physics object position
            ballPos = XnaMatrix.CreateTranslation(ConversionHelper.MathConverter.Convert(ballBody.Position));

            //If the ball is stopped, let the player hit the ball with space
            if (PhysicsUtils.BallStopped(ballBody))
            {
                // Function checks if user is pressing space bar and handles all power logic
                PhysicsUtils.Charging(ref isCharging, ref chargeTime, maxChargeTime, gameTime, KeyboardState, camera, ballBody, ref p1_score);
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        if (currentGameState == GameState.Menu)
        {
            GraphicsUtils.DrawMenu(GraphicsDevice, _spriteBatch, _menuFont);
            return; // skip 3D drawing if still in menu
        }

        // Re-enable depth buffering for 3D rendering
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        // Draw 3D objects
        GraphicsUtils.DrawModel(_ballModel, ballPos, ConversionHelper.MathConverter.Convert(camera.ViewMatrix), ConversionHelper.MathConverter.Convert(camera.ProjectionMatrix));
        GraphicsUtils.DrawModel(_holeModel, world, ConversionHelper.MathConverter.Convert(camera.ViewMatrix), ConversionHelper.MathConverter.Convert(camera.ProjectionMatrix));

        //Draw the score
        GraphicsUtils.DrawScore(_spriteBatch, _scoreFont, GraphicsDevice, p1_score);

        //Show shot meter if user is charging
        if (isCharging)
        {
            GraphicsUtils.DrawShotMeter(_spriteBatch, GraphicsDevice, chargeTime, maxChargeTime);
        }

        base.Draw(gameTime);
    }
}
