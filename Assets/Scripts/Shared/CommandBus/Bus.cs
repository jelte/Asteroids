﻿using Asteroids.Shared.Animation.Commands;
using Asteroids.Shared.Animation.Handlers;
using Asteroids.Shared.Audio.Command;
using Asteroids.Shared.Audio.Handler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Asteroids.Shared.CommandBus
{
    public class Bus : MonoBehaviour
    {
        #region Static properties
        public static Bus Instance { get; private set; }
        #endregion
        
        #region Static Methods
        /**
         * Execute a command
         */
        public static void Execute<T>(T command) where T : ICommand
        {
            Instance.DoExecute(command);
        }

        /**
         * Execute 2 commands
         **/
        public static void Execute<T0, T1>(T0 command0, T1 command1) where T0 : ICommand where T1 : ICommand
        {
            Instance.DoExecute(command0);
            Instance.DoExecute(command1);
        }
        #endregion

        #region References
        private IDictionary<Type, List<IHandler>> handlers;
        #endregion

        #region methods
        private void DoExecute<T>(T command) where T : ICommand
        {
            Type commandType = typeof(T);
            // TODO: Do something when no handler is available for command
            if (!handlers.ContainsKey(commandType)) return;
            
            // Register the number of expected callbacks
            command.Callbacks = handlers[commandType].Count;
            // Get each routine.
            List<IEnumerator> routines = handlers[commandType].ConvertAll(delegate (IHandler handler)
            {
                return ((IHandler<T>) handler).Handle(command);
            });

            // start all the routines.
            routines.ForEach(delegate (IEnumerator routine)
            {
                StartCoroutine(routine);
            });
        }
        #endregion

        #region Unity Methods
        void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(this);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);

            // TODO: Make dynamic
            handlers = new Dictionary<Type, List<IHandler>>();
            handlers.Add(typeof(Move), new List<IHandler>() { new MoveHandler() });
            handlers.Add(typeof(Rotate), new List<IHandler>() { new RotateHandler() });
            handlers.Add(typeof(Delegation), new List<IHandler>() { new DelegationHandler() });
            handlers.Add(typeof(Play), new List<IHandler>() { new PlayHandler() });

            // IEnumerable<Type> handlerTypes; = GetType().Assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IHandler)));
        }
        #endregion
    }
}
