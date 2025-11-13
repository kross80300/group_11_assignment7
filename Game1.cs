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
    private Random _random;
    private float _asteroidSpawnTimer;
    private float _asteroidSpawnInterval = 2.5f;
    
    private float _levelTimer;
    private int _currentLevel = 1;
    private const float LEVEL_DURATION = 40f;

    private Spaceship spaceship;

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
        spaceship = new Spaceship(_spaceshipTexture, new Vector2(width / 2, height / 2), 5f);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState k = Keyboard.GetState();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            k.IsKeyDown(Keys.Escape))
            Exit();

        spaceship.Update(gameTime, k, height, width);
        
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
        spaceship.Draw(_spriteBatch);
        foreach (var asteroid in _asteroids)
        {
            asteroid.Draw(_spriteBatch);
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
