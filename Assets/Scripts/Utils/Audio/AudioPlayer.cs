﻿// The MIT License(MIT)
//
// Copyright(c) 2016 Kevin Krol
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnityEngine;
using System.Collections.Generic;
using System;

namespace Snakybo.Audio
{
	/// <summary>
	/// The audio player, handles the playing, stopping and pausing of <see cref="AudioChannel"/>s.
	/// </summary>
	public static class AudioPlayer
	{
		private static class AudioManager
		{
			public const int NUM_CHANNELS = 64;

			internal static IEnumerable<AudioChannel> Channels
			{
				get
				{
					return channels;
				}
			}

			private static HashSet<AudioChannel> channels;

			static AudioManager()
			{
				channels = new HashSet<AudioChannel>();

				GameObject obj = new GameObject("Audio Manager");
				UnityEngine.Object.DontDestroyOnLoad(obj);

				for(int i = 0; i < NUM_CHANNELS; i++)
				{
					GameObject channel = new GameObject("AudioChannel " + i);
					channel.transform.SetParent(obj.transform);

					channel.AddComponent<AudioSource>();
					channels.Add(channel.AddComponent<AudioChannel>());
				}
			}

			internal static AudioChannel GetNext()
			{
				foreach(AudioChannel channel in channels)
				{
					if(!channel.IsPlaying)
					{
						return channel;
					}
				}

				return null;
			}

			internal static IEnumerable<AudioChannel> GetOfType(AudioType _type)
			{
				HashSet<AudioChannel> result = new HashSet<AudioChannel>();

				foreach(AudioChannel channel in channels)
				{
					if(channel.IsPlaying && channel.AudioObject.Type == _type)
					{
						result.Add(channel);
					}
				}

				return result;
			}
		}

		/// <summary>
		/// Attempt to play an <see cref="AudioObject"/>, if no suitable <see cref="AudioChannel"/> is found it will return null.
		/// </summary>
		/// <param name="_audioObject">The <see cref="AudioObject"/> to play.</param>
		/// <returns>The <see cref="AudioChannel"/> the <see cref="AudioObject"/> is playing on, or null.</returns>
		public static AudioChannel Play(this AudioObject _audioObject)
		{
			if(_audioObject == null)
			{
				throw new ArgumentException("AudioObject is null");
			}

			if(_audioObject is AudioObjectSingle)
			{
				AudioChannel channel = AudioManager.GetNext();

				if(channel != null)
				{
					channel.Play(_audioObject as AudioObjectSingle);
					return channel;
				}
				else
				{
					Debug.LogWarning("No free AudioChannels");
				}
			}
			else if(_audioObject is AudioObjectMultiple)
			{
				AudioObjectMultiple group = _audioObject as AudioObjectMultiple;
				AudioObjectSingle next = group.Next();

				if(next != null)
				{
					return next.Play();
				}
			}
			else
			{
				Debug.LogError("Unknown AudioObject type: " + _audioObject.GetType());
			}

			return null;
		}

		/// <summary>
		/// Stop all playing <see cref="AudioChannel"/>s.
		/// </summary>
		public static void StopAll()
		{
			foreach(AudioChannel audioChannel in AudioManager.Channels)
			{
				audioChannel.Stop();
			}
		}

		/// <summary>
		/// Stop all playing <see cref="AudioChannel"/>s of type <paramref name="_type"/>.
		/// </summary>
		/// <param name="_type">The type.</param>
		public static void StopAll(AudioType _type)
		{
			foreach(AudioChannel audioChannel in AudioManager.GetOfType(_type))
			{
				audioChannel.Stop();
			}
		}

		/// <summary>
		/// Pause all playing <see cref="AudioChannel"/>s.
		/// </summary>
		public static void PauseAll()
		{
			foreach(AudioChannel audioChannel in AudioManager.Channels)
			{
				audioChannel.Pause();
			}
		}

		/// <summary>
		/// Pause all playing <see cref="AudioChannel"/>s of type <paramref name="_type"/>.
		/// </summary>
		/// <param name="_type">The type.</param>
		public static void PauseAll(AudioType _type)
		{
			foreach(AudioChannel audioChannel in AudioManager.GetOfType(_type))
			{
				audioChannel.Pause();
			}
		}

		/// <summary>
		/// UnPause all playing <see cref="AudioChannel"/>s.
		/// </summary>
		public static void UnPauseAll()
		{
			foreach(AudioChannel audioChannel in AudioManager.Channels)
			{
				audioChannel.UnPause();
			}
		}

		/// <summary>
		/// UnPause all playing <see cref="AudioChannel"/>s of type <paramref name="_type"/>.
		/// </summary>
		/// <param name="_type">The type.</param>
		public static void UnPauseAll(AudioType _type)
		{
			foreach(AudioChannel audioChannel in AudioManager.GetOfType(_type))
			{
				audioChannel.UnPause();
			}
		}
	}
}
