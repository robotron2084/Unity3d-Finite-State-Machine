/*
 * Copyright (c) 2016 Made With Monster Love (Pty) Ltd
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * Heavily modified by Chris Hill to remove connections to MonoBehaviour.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = System.Object;

namespace EnemyHideout.StateMachine
{
	public class StateMachineRunner
	{
		private static StateMachineRunner _instance;
		public static StateMachineRunner Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new StateMachineRunner();
				}

				return _instance;
			}
		}
		private List<IStateMachine> stateMachineList = new List<IStateMachine>();
		
		/// <summary>
		/// Creates a stateMachine token object which is used to managed to the state of a monobehaviour. 
		/// </summary>
		/// <typeparam name="T">An Enum listing different state transitions</typeparam>
		/// <param name="actor">The component whose state will be managed</param>
		/// <returns></returns>
		public StateMachine<T> Initialize<T>(object actor) where T : struct, IConvertible, IComparable
		{
			var fsm = new StateMachine<T>(this, actor);

			stateMachineList.Add(fsm);

			return fsm;
		}

		/// <summary>
		/// Creates a stateMachine token object which is used to managed to the state of a monobehaviour. Will automatically transition the startState
		/// </summary>
		/// <typeparam name="T">An Enum listing different state transitions</typeparam>
		/// <param name="actor">The component whose state will be managed</param>
		/// <param name="startState">The default start state</param>
		/// <returns></returns>
		public StateMachine<T> Initialize<T>(object actor, T startState) where T : struct, IConvertible, IComparable
		{
			var fsm = Initialize<T>(actor);

			fsm.ChangeState(startState);

			return fsm;
		}

		void FixedUpdate()
		{
			for (int i = 0; i < stateMachineList.Count; i++)
			{
				var fsm = stateMachineList[i];
				if(!fsm.IsInTransition ) fsm.CurrentStateMap.FixedUpdate();
			}
		}

		public void Update()
		{
			for (int i = 0; i < stateMachineList.Count; i++)
			{
				var fsm = stateMachineList[i];
				if (!fsm.IsInTransition )
				{
					fsm.CurrentStateMap.Update();
				}
				fsm.UpdateCoroutines();
			}
		}

		void LateUpdate()
		{
			for (int i = 0; i < stateMachineList.Count; i++)
			{
				var fsm = stateMachineList[i];
				if (!fsm.IsInTransition )
				{
					fsm.CurrentStateMap.LateUpdate();
				}
			}
		}
		
		public static void DoNothing()
		{
		}

		public static void DoNothingCollider(Collider other)
		{
		}

		public static void DoNothingCollision(Collision other)
		{
		}

		public static IEnumerator DoNothingCoroutine()
		{
			yield break;
		}

		public void RemoveInstance<T>(StateMachine<T> stateMachine) where T : struct, IConvertible, IComparable
		{
			stateMachineList.Remove(stateMachine);
		}
	}

	
	public class StateMapping
	{
		public object state;

		public bool hasEnterRoutine;
		public Action EnterCall = StateMachineRunner.DoNothing;
		public Func<IEnumerator> EnterRoutine = StateMachineRunner.DoNothingCoroutine;

		public bool hasExitRoutine;
		public Action ExitCall = StateMachineRunner.DoNothing;
		public Func<IEnumerator> ExitRoutine = StateMachineRunner.DoNothingCoroutine;

		public Action Finally = StateMachineRunner.DoNothing;
		public Action Update = StateMachineRunner.DoNothing;
		public Action LateUpdate = StateMachineRunner.DoNothing;
		public Action FixedUpdate = StateMachineRunner.DoNothing;
		public Action<Collision> OnCollisionEnter = StateMachineRunner.DoNothingCollision;

		public StateMapping(object state)
		{
			this.state = state;
		}

	}
}


