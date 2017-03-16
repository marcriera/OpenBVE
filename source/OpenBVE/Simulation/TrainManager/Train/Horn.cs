﻿using OpenBveApi.Math;

namespace OpenBve
{
	public static partial class TrainManager
	{
		/// <summary>Represents a horn or whistle</summary>
		internal class Horn
		{
			/// <summary>The sound source for this horn</summary>
			internal Sounds.SoundSource Source;
			/// <summary>The sound buffer to be played once when playback commences</summary>
			internal Sounds.SoundBuffer StartSound;
			/// <summary>The loop sound</summary>
			internal Sounds.SoundBuffer LoopSound;
			/// <summary>The sound buffer to be played once when playback ends</summary>
			internal Sounds.SoundBuffer EndSound;
			/// <summary>The position of the sound within the train car</summary>
			internal Vector3 SoundPosition;
			/// <summary>Whether this horn has start and end sounds, or uses the legacy loop/ stretch method</summary>
			internal bool StartEndSounds;
			/// <summary>Stores whether the sound is looped or stretched when using the legacy method</summary>
			internal bool Loop;
			/// <summary>Stores the loop state</summary>
			private bool LoopStarted;

			/// <summary>The default constructor</summary>
			internal Horn()
			{
				this.StartSound = null;
				this.LoopSound = null;
				this.EndSound = null;
				this.Loop = false;
			}

			internal Horn(Sounds.SoundBuffer startSound, Sounds.SoundBuffer loopSound, Sounds.SoundBuffer endSound, bool loop)
			{
				this.Source = null;
				this.StartSound = startSound;
				this.LoopSound = loopSound;
				this.EndSound = endSound;
				this.Loop = loop;
				this.StartEndSounds = false;
				this.LoopStarted = false;
				SoundPosition = new Vector3();
			}

			/// <summary>Called by the controls loop to start playback of this horn</summary>
			internal void Play()
			{
				if (StartEndSounds == true)
				{
					if (LoopStarted == false)
					{
						if (!Sounds.IsPlaying(Source))
						{
							if (StartSound != null)
							{
								//The start sound is not currently playing, so start it
								Source = Sounds.PlaySound(StartSound, 1.0, 1.0, SoundPosition,
									TrainManager.PlayerTrain, TrainManager.PlayerTrain.DriverCar, false);
							}
							//Set the loop control variable to started
							LoopStarted = true;
						}
					}
					else
					{
						if (!Sounds.IsPlaying(Source))
						{
							//Start our loop sound playing if the start sound is finished
							Source = Sounds.PlaySound(LoopSound, 1.0, 1.0, SoundPosition,
										TrainManager.PlayerTrain, TrainManager.PlayerTrain.DriverCar, true);
						}
					}
				}
				else
				{
					if (LoopSound != null)
					{
						if (Loop)
						{
							if (!Sounds.IsPlaying(Source))
							{
								Source = Sounds.PlaySound(LoopSound, 1.0, 1.0, SoundPosition,
										TrainManager.PlayerTrain, TrainManager.PlayerTrain.DriverCar, true);
							}
						}
						else
						{
							if (!LoopStarted)
							{
								Source = Sounds.PlaySound(LoopSound, 1.0, 1.0, SoundPosition, TrainManager.PlayerTrain,
									TrainManager.PlayerTrain.DriverCar, false);
							}
							LoopStarted = true;
						}
					}
				}

			}

			/// <summary>Called by the controls loop to stop playback of this horn</summary>
			internal void Stop()
			{
				//Reset loop control variable
				LoopStarted = false;
				if (Sounds.IsPlaying(Source))
				{
					//Stop the loop sound playing
					Sounds.StopSound(Source);
				}
				if (StartEndSounds && !Sounds.IsPlaying(Source) && EndSound != null)
				{
					//If our end sound is defined and in use, play once
					Source = Sounds.PlaySound(EndSound, 1.0, 1.0, SoundPosition,
										TrainManager.PlayerTrain,
										TrainManager.PlayerTrain.DriverCar, false);
				}
			}
		}
	}
}
