﻿using System;
using System.Linq;
using UnityEngine;

namespace AnimFlex.Sequencer {
	[Flags]
	internal enum SequenceFlags {
		Active = 1 << 1,
		Paused = 1 << 2,
		Stopping = 1 << 3
	}

	internal static class SequenceFlagsExtensions {
		public static bool HasFlagFast(this SequenceFlags value, SequenceFlags flag) {
			return ( value & flag ) != 0;
		}
	}

	[Serializable]
	public sealed class Sequence {

		/// <summary>
		/// when true, it won't wait for the next Tick (frame) to activate the next clip.
		/// </summary>
		public bool activateNextClipsASAP = true;
		
		/// <summary>
		/// executes when the sequence is played.
		/// </summary>
		public event Action onPlay = delegate { };

		/// <summary>
		/// executes when the sequence is completed or stopped
		/// </summary>
		public event Action onComplete = delegate { };



		[SerializeField] internal ClipNode[] nodes = Array.Empty<ClipNode>();

		/// <summary>
		/// The controller of this sequence. should be assigned before <see cref="Play"/> or <see cref="PlayOrRestart"/>
		/// </summary>
		internal SequenceController sequenceController { get; set;  }

		internal SequenceFlags flags;

		int _pendingActiveCount = 0;

		const int MAX_ITER_COUNT = 64;


#region Public playback tools

		/// <summary>
		/// pauses the sequencer.
		/// </summary>
		public void Pause() => flags |= SequenceFlags.Paused;

		/// <summary>
		/// resumes the sequencer if it was paused.
		/// </summary>
		public void Resume() => flags &= ~SequenceFlags.Paused;

		/// <summary>
		/// stops the sequencer. note that the stopping process will not happen right away.
		/// </summary>
		public void Stop() {
			if (!IsActive()) {
				Debug.LogWarning( "Sequencer is not active, so there's nothing to stop." );
				return;
			}

			flags |= SequenceFlags.Stopping;
		}

		/// <summary>
		/// plays the sequence. if you're unsure if the sequencer is already active or not,
		/// call <see cref="PlayOrRestart"/> instead.
		/// </summary>
		public void Play() {
			if (IsActive()) {
				Debug.LogError(
					$"The sequence is already active. You cannot play an active sequencer. You can call {nameof(PlayOrRestart)} instead." );
			}

			if (nodes.Length <= 0) return;
			sequenceController.AddNewSequence( this );
		}

		/// <summary>
		/// If the sequence is already active, this will restart it, or if it's not activated, it'll call <see cref="Play"/>
		/// automatically. <para/>
		/// Note that this will act a little bit slower than <see cref="Play"/> would
		/// </summary>
		public void PlayOrRestart() {
			if (IsActive() == false) // play
			{
				Play();
			}
			else // restart
			{
				Stop();
				sequenceController.delayedCall += Play;
			}
		}

		/// <summary>
		/// returns true if it's active and playing
		/// </summary>
		public bool IsPlaying() => IsActive();

#endregion

#region Internals

		internal void OnActivate() {
			for (int i = 0; i < nodes.Length; i++) nodes[i].Init( this, i );
			ActivateClip( 0 );
			onPlay();
		}

		internal void OnStop() {
			flags = 0; // empty flags
			onComplete();
		}

		internal void Tick(float deltaTime) {

			int iters = 0;
			do {
				if (iters++ == MAX_ITER_COUNT) break;
				
				for (int i = 0; i < nodes.Length; i++) {
					
					// init phase
					if (nodes[i].flags.HasFlagFast( ClipNodeFlags.PendingActive )) {
						_pendingActiveCount --;
						nodes[i].Reset();
						nodes[i].flags = ClipNodeFlags.Active;
					}
					
					// tick phase
					if ( nodes[i].flags.HasFlagFast( ClipNodeFlags.Active ) && !nodes[i].flags.HasFlagFast( ClipNodeFlags.Ticked )) {
						nodes[i].Tick( deltaTime );
						nodes[i].flags |= ClipNodeFlags.Ticked;
					}
					
				}
			} while (activateNextClipsASAP && _pendingActiveCount > 0);
			
			// flush
			for (int i = 0; i < nodes.Length; i++) {
				nodes[i].flags &= ~ClipNodeFlags.Ticked; // remove Tick
			}
		}

		internal void EditorValidate() {
			foreach (var node in nodes) node.OnValidate();
		}

		internal void ActivateClip(int index) {
			nodes[index].flags = ClipNodeFlags.PendingActive;
			_pendingActiveCount++;
		}

		internal void DeactivateClipNode(ClipNode clipNode) {
			clipNode.flags = 0;
		}

		internal bool IsActive() => flags.HasFlagFast( SequenceFlags.Active );

#endregion

#region Clip manipulations

		public void RemoveClipNodeAtIndex(int index) {
			var nodesList = nodes.ToList();
			nodesList.RemoveAt( index );
			nodes = nodesList.ToArray();
		}

		public void RemoveClipNode(ClipNode node) {
			RemoveClipNodeAtIndex( Array.IndexOf( nodes, node ) );
		}

		public void MoveClipNode(int fromIndex, int toIndex) {
			( nodes[fromIndex], nodes[toIndex] ) = ( nodes[toIndex], nodes[fromIndex] );
		}

		public void AddNewClipNode(Clip clip) {
			var tmp = nodes.ToList();
			tmp.Add( new ClipNode {
				clip = clip,
				name = $"Node {nodes.Length}"
			} );
			nodes = tmp.ToArray();
		}

		public void InsertNewClipAt(Clip clip, int index) {
			var tmp = nodes.ToList();
			tmp.Insert( index, new ClipNode {
				clip = clip,
				name = $"Node {index}",
			} );
			nodes = tmp.ToArray();
		}

#endregion
	}
}