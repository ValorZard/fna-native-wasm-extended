using FontStashSharp;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Input;

namespace GameCore;
using Microsoft.Xna.Framework;
public class GameMain : Game
{
    public GameMain()
    {
        GraphicsDeviceManager gdm = new GraphicsDeviceManager(this);

        // Typically you would load a config here...
        gdm.PreferredBackBufferWidth = 512;
        gdm.PreferredBackBufferHeight = 512;
        gdm.IsFullScreen = false;
        // Turn off VSync
        gdm.SynchronizeWithVerticalRetrace = false;

        // All content loaded will be in a "Content" folder
        Content.RootDirectory = "Content";
    }

    byte r = 0;
    byte g = 0;
    byte b = 0;
    DateTime lastUpdate = DateTime.UnixEpoch;
    int updateCount = 0;
    private SpriteBatch batch;
    
    private Texture2D texture;
    private SoundEffect sound;
    private Song song;
    private FontSystem _fontSystem;
    private FrameCounter _frameCounter = new FrameCounter();
    private Vector2 playerPosition = new Vector2(100, 100);
    private const float PLAYER_SPEED = 100.0f;
    
    protected override void Initialize()
    {
        /* This is a nice place to start up the engine, after
         * loading configuration stuff in the constructor
         */
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Load textures, sounds, and so on in here...
        // Create the batch...
        batch = new SpriteBatch(GraphicsDevice);
        texture = Content.Load<Texture2D>("images/popsicle");
        sound = Content.Load<SoundEffect>("sounds/sfx_jump");
        song = Content.Load<Song>("songs/The_Entertainer_-_Scott_Joplin");
        _fontSystem = new FontSystem();
        _fontSystem.AddFont(File.ReadAllBytes(@"Content/fonts/DroidSans.ttf"));
        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        // Clean up after yourself!
        batch.Dispose();
        texture.Dispose();
        sound.Dispose();
        song.Dispose();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        // Run game logic in here. Do NOT render anything here!
        base.Update(gameTime);
        updateCount++;
        DateTime now = DateTime.UtcNow;
        if ((now - lastUpdate).TotalSeconds > 1.0)
        {
            Console.WriteLine($"Main loop still running at: {now}; {Math.Round(updateCount / (now - lastUpdate).TotalSeconds, MidpointRounding.AwayFromZero)} UPS");
            lastUpdate = now;
            updateCount = 0;
        }
        KeyboardExtended.Update();
        KeyboardStateExtended keyboardState = KeyboardExtended.GetState();
        
        if(keyboardState.WasKeyPressed(Keys.Space))
        {
            sound.Play();
        }

        Vector2 inputDirection = Vector2.Zero;
        if (keyboardState.IsKeyDown(Keys.W))
        {
            inputDirection.Y += -1;
        }
        if (keyboardState.IsKeyDown(Keys.S))
        {
            inputDirection.Y += 1;
        }
        if (keyboardState.IsKeyDown(Keys.A))
        {
            inputDirection.X += -1;
        }
        if (keyboardState.IsKeyDown(Keys.D))
        {
            inputDirection.X += 1;
        }
        if (inputDirection != Vector2.Zero)
            inputDirection.Normalize();
        
        playerPosition += inputDirection * (float)gameTime.ElapsedGameTime.TotalSeconds * PLAYER_SPEED;
        
        // music 
        // Just keep playing the song over and over
        if (MediaPlayer.State == MediaState.Stopped)
        {
            MediaPlayer.Play(song);
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        // Render stuff in here. Do NOT run game logic in here!
        GraphicsDevice.Clear(new Color(r, g, b));
         // Draw the texture to the corner of the screen
        batch.Begin();
        batch.Draw(texture, new Rectangle((int)playerPosition.X, (int)playerPosition.Y, texture.Width / 5, texture.Height / 5), Color.White);
        SpriteFontBase font18 = _fontSystem.GetFont(18);
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _frameCounter.Update(deltaTime);

        var fpsString = $"FPS: {_frameCounter.AverageFramesPerSecond}";

        batch.DrawString(font18, fpsString, new Vector2(1, 1), Color.White);
        SpriteFontBase font30 = _fontSystem.GetFont(30);
        batch.DrawString(font30, "The quick brown fox\njumps over\nthe lazy dog", new Vector2(0, 80), Color.Yellow);

        batch.End();
        base.Draw(gameTime);
    }
}