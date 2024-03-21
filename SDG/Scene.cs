using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDG.Systems;

namespace SDG;

public abstract class Scene
{
    private enum CommandType
    {
        Start,
        End
    }
    
    /// <summary>
    /// Command to send to the Scene.Manager. All commands are activated when Scene.Manager.ApplyChanges is called.
    /// This "reifies" the functionality of scene starting / ending.
    /// </summary>
    /// <param name="Type">The type of command to execute</param>
    /// <param name="Scene">If Type=Start: the next scene to start, If Type=Stop: the scene to stop</param>
    /// <param name="ReplaceCurrent">If Type=Start: whether to end the current scene (true), or pause it (false)</param>
    private readonly record struct SceneCommand(CommandType Type, Scene Scene, bool ReplaceCurrent);
    
    public class Manager
    {
        private readonly Stack<Scene> _current = new();
        private readonly List<SceneCommand> _commands = new();
        private readonly Dictionary<Type, object> _services = new();

        /// <summary>
        /// Provide a service to the Scene.Manager that can later be retrieved by scenes with GetService
        /// </summary>
        /// <param name="service">The service to provide</param>
        /// <typeparam name="T">Type of the service, used as a key</typeparam>
        public void ProvideService<T>(T service) where T : class
        {
            _services.TryAdd(typeof(T), service);
        }

        public void Update(GameTime time)
        {
            CurrentScene?.Update(time);
            ApplyChanges();
        }

        public void Draw(GameTime time, SpriteBatch batch)
        {
            CurrentScene?.Draw(time, batch);
        }

        /// <summary>
        /// Get a service that was previously provided to the Scene.Manager via Scene.Manager.ProvideService.
        /// </summary>
        /// <typeparam name="T">Type of service to retrieved.</typeparam>
        /// <returns>Service of type `T` or null if there is no such service available.</returns>
        public T GetService<T>() where T : class
        {
            return _services[typeof(T)] as T;
        }
        
        /// <summary>
        /// Start a scene
        /// </summary>
        /// <param name="stopCurrentScene">
        /// Whether to stop the currently running scene, if any.
        /// If false, it will pause the currently running scene (if any).
        /// </param>
        /// <typeparam name="T">Type of scene to start</typeparam>
        public void StartScene<T>(bool stopCurrentScene = true) where T : Scene, new()
        {
            var scene = new T
            {
                _manager = this
            };

            _commands.Add(new SceneCommand(CommandType.Start, scene, stopCurrentScene));
        }

        /// <summary>
        /// Stops the currently running scene. Please make sure that there is an underlying scene that
        /// was previously paused, or start a new one in the scene's End callback.
        /// </summary>
        public void EndCurrentScene()
        {
            if (_current.Count == 0) return;
            
            _commands.Add(new SceneCommand(CommandType.End, _current.Peek(), true));
        }

        /// <summary>
        /// The current scene, or null if none are on the scene stack
        /// </summary>
        public Scene CurrentScene => _current.Count > 0 ? _current.Peek() : null;
        

        /// <summary>
        /// This needs to be called after a call to StartScene to apply scene change
        /// </summary>
        public void ApplyChanges()
        {
            if (_commands.Count == 0) return;

            foreach (var command in _commands)
            {
                switch (command.Type)
                {
                    case CommandType.Start:
                    {
                        if (command.Scene != null) // make sure there is a next scene to start
                        {
                            // check if there is a current scene that should be ended or paused
                            if (_current.Count > 0)
                            {
                                if (command.ReplaceCurrent)
                                {
                                    // end scene and remove it
                                    _current.Pop().End();
                                }
                                else
                                {
                                    // notify current scene it is paused
                                    _current.Peek().Paused();
                                }
                            }
                            
                            // now start / add the scene
                            command.Scene.Start();
                            _current.Push(command.Scene);
                        }

                        break;
                    }
                    
                    case CommandType.End:
                    {
                        if (_current.Count > 0 && _current.Peek() == command.Scene)
                        {
                            // end scene and remove it
                            _current.Pop().End();
                            
                            // now check if there are any underlying scenes to notify it is resumed
                            if (_current.Count > 0)
                            {
                                // there is, notify it
                                _current.Peek().Resumed();
                            }
                        }
                        break;
                    }
                }
            }

            _commands.Clear();
        }
        // end ApplyChanges
    }
    
    private Manager _manager;
    
    /// <summary>
    /// Game-wide services that persist across scenes.
    /// (This is separate from the Game's service container)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected T GetService<T>() where T : class
    {
        return _manager.GetService<T>();
    }
    
    /// <summary>
    /// This scene event occurs when the scene pauses as the result of another one openening with `stopCurrent`
    /// set to false
    /// </summary>
    protected abstract void Paused();
    /// <summary>
    /// This scene event occurs when the scene is resumed as the result of another one stopping
    /// </summary>
    protected abstract void Resumed();
    /// <summary>
    /// This scene event occurs when this scene begins as the result of calling `StartScene` on this scene
    /// </summary>
    protected abstract void Start();
    /// <summary>
    /// This scene event occurs once every update tick, which should occur at ~60fps on most modern systems.
    /// `Draw` comes after this event.
    /// </summary>
    /// <param name="time"></param>
    protected abstract void Update(GameTime time);
    /// <summary>
    /// This scene event occurs once every update tick, after the `Update` event.
    /// </summary>
    /// <param name="time">time struct containing delta time and total time info</param>
    /// <param name="batch">sprite batch to draw with</param>
    protected abstract void Draw(GameTime time, SpriteBatch batch);
    /// <summary>
    /// This scene event occurs when this scene ends as the result of calling `StopCurrentScene` when this scene is
    /// running.
    /// </summary>
    protected abstract void End();


}