using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SDG.Examples;

class TestSceneA : Scene
{
    private bool _firstUpdated = false;
    private bool _firstDrawn = false;
    
    public TestSceneA()
    {
        Console.WriteLine("TestSceneA created");
    }
    
    protected override void Paused()
    {
        Console.WriteLine("TestSceneA paused");
    }

    protected override void Resumed()
    {
        Console.WriteLine("TestSceneA resumed");
    }

    protected override void Start()
    {
        Console.WriteLine("TestSceneA started!");
    }

    protected override void Update(GameTime time)
    {
        if (!_firstUpdated)
        {
            Console.WriteLine("TestSceneA first update called");
            _firstUpdated = true;
        }
    }

    protected override void Draw(GameTime time, SpriteBatch batch)
    {
        if (!_firstDrawn)
        {
            Console.WriteLine("TestSceneA first draw called");
            _firstDrawn = true;
        }
    }

    protected override void End()
    {
        Console.WriteLine("TestSceneA end");
    }
}

class TestSceneB : Scene
{
    private bool _firstUpdated = false;
    private bool _firstDrawn = false;
    
    public TestSceneB()
    {
        Console.WriteLine("TestSceneB created");
    }
    
    protected override void Paused()
    {
        Console.WriteLine("TestSceneB paused");
    }

    protected override void Resumed()
    {
        Console.WriteLine("TestSceneB resumed");
    }

    protected override void Start()
    {
        Console.WriteLine("Scene TestSceneB starting. Last scene was {0}", 
            Scenes.CurrentScene.GetType().Name);
    }

    protected override void Update(GameTime time)
    {
        if (!_firstUpdated) // flagged to prevent stream of logs
        {
            Console.WriteLine("TestSceneB first update called");
            _firstUpdated = true;
        }
    }

    protected override void Draw(GameTime time, SpriteBatch batch)
    {
        if (!_firstDrawn) // flagged to prevent stream of logs
        {
            Console.WriteLine("TestSceneB first draw called");
            _firstDrawn = true;
        }
    }

    protected override void End()
    {
        Console.WriteLine("TestSceneB end");
    }
} 

public class SceneManagerTest : Core
{
    private readonly Scene.Manager _scenes;
    private bool _showingPauseMenu;
    private KeyboardState _lastKeyState;

    private SpriteBatch _spriteBatch;

    public SceneManagerTest()
    {
        _scenes = new Scene.Manager(this);
        IsMouseVisible = true;
    }
    
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _scenes.StartScene<TestSceneA>();
    }

    protected override void Update(GameTime time)
    {
        var kbState = Keyboard.GetState();

        if (kbState.IsKeyDown(Keys.P) && !_lastKeyState.IsKeyDown(Keys.P))
        {
            if (!_showingPauseMenu)
            {
                _scenes.StartScene<TestSceneB>(false);
                _showingPauseMenu = true;
            }
            else
            {
                if (_scenes.CurrentScene != null &&
                    _scenes.CurrentScene.GetType() == typeof(TestSceneB))
                {
                    _scenes.EndCurrentScene();
                    _showingPauseMenu = false;
                }

            }
        }
        
        _scenes.Update(time);


        _lastKeyState = kbState;
    }

    protected override void Draw(GameTime time)
    {
        _scenes.Draw(time, _spriteBatch);
    }
}