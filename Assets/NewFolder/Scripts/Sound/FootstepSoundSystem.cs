using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Pool;

public class FootstepSoundSystem : MonoBehaviour {

    internal class CellSound {
        internal AudioSource audioSource;
        internal float lastStartTime;
        internal float lastDuration;

        public CellSound(AudioSource audioSource) {
            this.audioSource = audioSource;
            lastStartTime = float.NegativeInfinity;

        }
    }
    
    [SerializeField] private AudioSource footstepSourcePrefab;
    [SerializeField] private AudioClip[] footstepSounds;

    private ObjectPool<AudioSource> audioSourcePool;
    private SpatialFootstepGrid footstepGrid;
    private Vector3 referencePoint;
    private FootstepGridCell[] resultBuffer;
    private Dictionary<Vector2Int, CellSound> cellSounds;

    private void Awake() {
        footstepGrid = new (5, 10, 10, new Vector3(-25, 0, -25));
        audioSourcePool = new ObjectPool<AudioSource>(createFunc: CreateAudioSource);
        resultBuffer = new FootstepGridCell[10];
        cellSounds = new ();
    }

    private AudioSource CreateAudioSource() {
        var source = GameObject.Instantiate(footstepSourcePrefab);
        source.transform.parent = transform;
        return source;
    }

    public void SetReferencePoint(Vector3 point) {
        referencePoint = point;
    }

    public void RegisterFootstep(Vector3 position, float speedNormalized) {
        footstepGrid.AddRecord(position, speedNormalized);
    }

    private void Update() {
        referencePoint = Camera.main.transform.position;
    }

    private void LateUpdate() {
        ReleaseNonPlayingAudioSources();
        footstepGrid.GetSortedCells(referencePoint, resultBuffer);
        footstepGrid.ClearActiveRecords();
        foreach (var cell in resultBuffer) {
            var sound = PrepareCellSound(cell.index);
            if (sound.lastStartTime + sound.lastDuration < Time.time) {
                var nextFootstepClip = GetNextFootstepClip();

                sound.lastStartTime = Time.time;
                sound.lastDuration = nextFootstepClip.length;
                sound.audioSource.PlayOneShot(nextFootstepClip);
                sound.audioSource.transform.position = cell.averagePosition;
            }
        }
    }

    private AudioClip GetNextFootstepClip() {
        return SelectRandom(footstepSounds);
    }

    public static AudioClip SelectRandom(AudioClip[] clips) {
        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }

    private void ReleaseNonPlayingAudioSources() {
        var soundsToRemove = new List<Vector2Int>();
        foreach (var cell in cellSounds.Keys) {
            var sounds = cellSounds[cell];
            if (!sounds.audioSource.isPlaying) {
                audioSourcePool.Release(sounds.audioSource);
                soundsToRemove.Add(cell);
            }
        }
        foreach (var cell in soundsToRemove) {
            cellSounds.Remove(cell);
        }
    }

    private CellSound PrepareCellSound(Vector2Int cell) {
        if (!cellSounds.TryGetValue(cell, out var sound)) {
            sound = new CellSound(audioSourcePool.Get());
            cellSounds[cell] = sound;
        }
        return sound;
    }
}