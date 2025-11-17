using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace group_11_assignment7;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _asteroidTexture;
    
    private List<Asteroid> _asteroids;
    private List<Asteroid> _asteroidsToRemove = new List<Asteroid>();
    private Random _random;
    private float _asteroidSpawnTimer;
    private float _asteroidSpawnInterval = 2.5f;
    
    private float _levelTimer;
    private int _currentLevel = 1;
    private const float LEVEL_DURATION = 20f;

    private Spaceship spaceship;
    private Texture2D _spaceshipTexture;
    
    private Texture2D _heartTexture;
    private SpriteFont _font;
    private bool _gameOver = false;
    private KeyboardState _previousKeyboardState;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.IsFullScreen = false;
        Window.IsBorderless = true;
    }

    protected override void Initialize()
    {
        _asteroids = new List<Asteroid>();
        _random = new Random();
        _asteroidSpawnTimer = 0f;
        _levelTimer = 0f;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _asteroidTexture = Content.Load<Texture2D>("textures/asteroid");
        _spaceshipTexture = Content.Load<Texture2D>("textures/spaceshipTexture");
        _heartTexture = Content.Load<Texture2D>("textures/heart");
        _font = Content.Load<SpriteFont>("Fonts/GameFont");
        spaceship = new Spaceship(_spaceshipTexture, new Vector2(_spaceshipTexture.Width / 2f, _spaceshipTexture.Height / 2f), 5f);
    }



    protected override void Update(GameTime gameTime)
    {
        KeyboardState k = Keyboard.GetState();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            k.IsKeyDown(Keys.Escape))
            Exit();
        
        if (_gameOver)
        {
            return;
        }

        spaceship.Update(gameTime, k, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        
        if (spaceship.GetLives() <= 0)
        {
            _gameOver = true;
            _previousKeyboardState = k;
            return;
        }

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        _levelTimer += deltaTime;
        if (_levelTimer >= LEVEL_DURATION)
        {
            _levelTimer = 0f;
            _currentLevel++;
            _asteroidSpawnInterval = Math.Max(0.5f, _asteroidSpawnInterval - 0.1f);
        }
        
        _asteroidSpawnTimer += deltaTime;
        if (_asteroidSpawnTimer >= _asteroidSpawnInterval)
        {
            _asteroidSpawnTimer = 0f;
            SpawnAsteroid();
        }
        
        for (int i = _asteroids.Count - 1; i >= 0; i--)
        {
            _asteroids[i].Update(gameTime);
            
            if (_asteroids[i].IsOffScreen(_graphics.PreferredBackBufferWidth, 
                _graphics.PreferredBackBufferHeight))
            {
                _asteroids.RemoveAt(i);
            }
        }

        foreach (var a in _asteroids)
        {
            if (spaceship.GetBounds().Intersects(a.GetBoundingBox()))
            {
                spaceship.LoseLife();
                _asteroidsToRemove.Add(a);
            }
        } 
        
        _asteroids.RemoveAll(a => _asteroidsToRemove.Contains(a));
        _asteroidsToRemove.Clear();
        base.Update(gameTime);
    }

    private void SpawnAsteroid()
    {
        int screenWidth = _graphics.PreferredBackBufferWidth;

        Vector2 spawnPosition = new Vector2(_random.Next(screenWidth), -50);
        
        float baseSpeed = 1f + (_currentLevel * 0.15f);
        float speed = baseSpeed + (float)(_random.NextDouble() * 0.5f);
        Vector2 velocity = new Vector2(0, speed);
        
        float rotationSpeed = (float)(_random.NextDouble() - 0.5) * 0.1f;
        
        Asteroid asteroid = new Asteroid(spawnPosition, velocity, _asteroidTexture, rotationSpeed);
        _asteroids.Add(asteroid);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        
        if (!_gameOver)
        {
            spaceship.Draw(_spriteBatch);
            foreach (var asteroid in _asteroids)
            {
                asteroid.Draw(_spriteBatch);
            }

            DrawHearts();
        }
        else
        {
            DrawGameOverScreen();
        }
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawHearts()
    {
        int heartSize = 30;
        int heartSpacing = 40;
        int startX = 20;
        int startY = 20;
        int maxLives = 3;
        int currentLives = spaceship.GetLives();
        
        for (int i = 0; i < maxLives; i++)
        {
            Vector2 position = new Vector2(startX + (i * heartSpacing), startY);
            Color heartColor = i < currentLives ? Color.Red : Color.Gray;
            
            _spriteBatch.Draw(
                _heartTexture,
                position,
                null,
                heartColor,
                0f,
                Vector2.Zero,
                1.5f,
                SpriteEffects.None,
                0f
            );
        }
    }

    private void DrawGameOverScreen()
    {
        int screenWidth = _graphics.PreferredBackBufferWidth;
        int screenHeight = _graphics.PreferredBackBufferHeight;
        
        Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        _spriteBatch.Draw(
            pixel,
            new Rectangle(0, 0, screenWidth, screenHeight),
            Color.Black * 0.7f
        );
        
        string gameOverText = "GAME OVER";
        Vector2 gameOverSize = _font.MeasureString(gameOverText);
        Vector2 gameOverPos = new Vector2(
            (screenWidth - gameOverSize.X) / 2f,
            screenHeight / 2f - 100
        );
        _spriteBatch.DrawString(_font, gameOverText, gameOverPos, Color.Red);

        string levelText = $"Level Reached: {_currentLevel}";
        Vector2 levelSize = _font.MeasureString(levelText);
        Vector2 levelPos = new Vector2(
            (screenWidth - levelSize.X) / 2f,
            screenHeight / 2f - 20
        );
        _spriteBatch.DrawString(_font, levelText, levelPos, Color.White);
    }
}