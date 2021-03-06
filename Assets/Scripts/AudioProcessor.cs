﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioProcessor : MonoBehaviour {

	float spawnRate = 0f;
	private int sampleRate;
	private ResourseManager resourceManager;
	private CSVParsor pitchCSVParsor;
	private CSVParsor beatCSVParsor;
	float nextBeatInterval = 0f;
	float currentBeatInterval = 0f;

	// Use this for initialization
	void Start () {
		DataManager dataManager = GameObject.Find ("DataManager").GetComponentInChildren<DataManager> ();
		pitchCSVParsor = new CSVParsor ();
		pitchCSVParsor.path = dataManager.pitch_csv_path;
		pitchCSVParsor.ReadAllLines ();

		beatCSVParsor = new CSVParsor ();
		beatCSVParsor.path = dataManager.beat_csv_path;
		beatCSVParsor.ReadAllLines ();
		beatCSVParsor.ReadRecord ();

		GUIManager guiManager = GameObject.Find ("GUIManager").GetComponentInChildren<GUIManager> ();
		resourceManager = guiManager.GetComponentInChildren<ResourseManager> ();
	}

	// Update is called once per frame
	void Update () {
		LoadAttributeData ();
		if (sampleRate == 0) {
			AudioSource audioSource = GetComponent<AudioSource> ();
			sampleRate = audioSource.clip.frequency;
		}
		List<string> fields = new List<string> ();
		if (spawnRate <= 0) {
			currentBeatInterval = nextBeatInterval;
			UpdateNextSpawnRate ();
			spawnRate = nextBeatInterval;

			string note = "";
			fields = pitchCSVParsor.ReadRecord();

			while (fields != null && float.Parse(fields [0]) < Time.timeSinceLevelLoad) {
				fields = pitchCSVParsor.ReadRecord ();
			}
			if (fields != null && currentBeatInterval != 0) {
				float pitch = 0f;
				if (float.TryParse (fields [1], out pitch)) {
					string result = CalcNoteAndRegister (pitch);
					if (result.Length == 2) {
						note = result [0].ToString ();
						string register = result [1].ToString ();
						resourceManager.InstantiateMusicSymbol (note, register, currentBeatInterval, nextBeatInterval);
					}
				} 
			}
		

		} else {
			spawnRate -= Time.deltaTime;
		}
	}

	/* This method will compare the stored time of the attributeData in the dictionary with
	 the current unity system scene loaded time. If the stored time is smaller or equal to
	 the unity time, then we will set this data to the current one and remove this from the 
	 dictionary. */
	private void LoadAttributeData() {
		DataManager dataManager = GameObject.Find ("DataManager").GetComponentInChildren<DataManager> ();
		foreach (KeyValuePair<int, AttributeData> pair in dataManager.attributeDataDic) {
			int time = pair.Key;
			if (time <= Time.timeSinceLevelLoad) {
				AttributeData data = dataManager.attributeDataDic [time];
				dataManager.currentAttributeData = data;
				dataManager.attributeDataDic.Remove (time);
				break;
			}

		}
	}

	/* This method will fetch two records from the beats CSV file and set nextSpawnedRate
	 * to be the sum of two beats intervals.
	 the sum of two beat intervals.
	 */
	private void UpdateNextSpawnRate() {
		nextBeatInterval = 0f;
		List<string> fields = new List<string> ();
		for (int i = 0; i < 2; i++) {
			fields = beatCSVParsor.ReadRecord ();
			if (fields != null) {
				nextBeatInterval += float.Parse (fields [1]);
			}
		}
	}

	/* This method uses the pitch results table to calculate the corresponding
	note and register of the given pitch frequency. Please notice this method
	ignores all the sharp notes. 
	*/
	private string CalcNoteAndRegister(float fundFreq) {
		int register = 1;
		List<string> notes = new List<string> {"C", "D", "E", "F", "G", "A", "B"};
		string note = "";
		float noteFreq = 32.7f;
		const float multiple = 1.05946f;
		const float MAX_NOTE_FREQ = 3951.1f;
		float prevNoteFreq = 0;
		while (fundFreq >= noteFreq && fundFreq <= MAX_NOTE_FREQ) {
			if (fundFreq >= noteFreq * 2) {
				noteFreq *= 2;
				register++;
			} else {
				int pow = 0;
				while (fundFreq > noteFreq && pow <= 6) {
					prevNoteFreq = noteFreq;
					if (pow != 2) {
						noteFreq *= Mathf.Pow (multiple, 2);				
					} else {
						noteFreq *= multiple;
					}
					noteFreq = Mathf.Round (noteFreq * 10) / 10;
				
					if (fundFreq >= noteFreq || noteFreq - fundFreq < fundFreq - prevNoteFreq) {
						pow++;
					}

				}
				if (pow == 7) {
					pow = 0;
					register++;
				}
				return notes [pow] + register;
			}
		}
		return note + register;
	}
		
}
