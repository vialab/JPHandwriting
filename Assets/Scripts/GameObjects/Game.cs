using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameData;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class Game : EventSubscriber, ILoggable, IObjectMover, OnVocabItemMenuState.IHandler, OnVocabItemLearned.IHandler {
    public static Game Instance { get; private set; }

    /// <summary>
    /// Session files loaded from JSON.
    /// </summary>
    public Session userSession { get; protected set; }

    /// <summary>
    /// The user ID. Used to load their JSON file. 
    /// </summary>
    public string userID;

    private VocabItemSpawner vocabItemSpawner;

    // ====================
    // Where to place items
    // ====================

    [SerializeField] private List<Transform> learnedSpawnRows = new();
    [SerializeField] private List<Transform> unlearnedSpawnRows = new();
    [ReadOnly] private List<Vector3> learnedSpawns = new();
    public List<Transform> GetSpawnRows() => learnedSpawnRows;
    [SerializeField] private Transform itemPlacePosition;

    // =============================================
    // Things related to the current vocabulary item
    // =============================================

    private VocabItem currentVocabItem = null;
    public VocabItem CurrentVocabItem => currentVocabItem;
    public bool currentVocabItemTracing => currentVocabItem.TracingEnabled;
    public string currentVocabItemJPName => currentVocabItem.JPName;

    // =====================================
    // List of learned/unlearned vocab items
    // =====================================

    private List<VocabItem> _unlearnedVocabItems = new();

    private List<VocabItem> _learnedVocabItems = new();

    private void Awake() {
        EnsureSingleton();
        vocabItemSpawner = GetComponent<VocabItemSpawner>();
        learnedSpawns = ((IObjectMover)this).CreateSpawnPositions();
    }

    protected override void Start() {
        base.Start();
        LoadSessionInfo();

        if (vocabItemSpawner != null) {
            vocabItemSpawner.SetSpawns(unlearnedSpawnRows);
            _unlearnedVocabItems = vocabItemSpawner.Spawn(userSession.VocabItems);
            LogEvent($"Items spawned: {string.Join(", ", _unlearnedVocabItems)}");
            ShowNextItem();
        }
    }

    private void LoadSessionInfo() {
        LogEvent($"Loading session data from {userID}.json");

        // TODO: not hardcode this
        userSession = JsonFile.ReadFile<Session>(Path.Combine("SessionData", $"{userID}.json"));
    }

    void OnVocabItemMenuState.IHandler.OnEvent(VocabItem vocabItem) {
        if (currentVocabItem != null) {
            currentVocabItem.HideUI();
            currentVocabItem = null;
        }

        if (currentVocabItem == vocabItem) return;

        currentVocabItem = vocabItem;
        LogEvent($"Current vocabulary item is {vocabItem.ENName} ({vocabItem.JPName})");
    }

    void OnVocabItemLearned.IHandler.OnEvent(VocabItem vocabItem) {
        _learnedVocabItems.Add(vocabItem);
        _unlearnedVocabItems.Remove(vocabItem);

        LogEvent($"{vocabItem.ENName} added to list of learned vocab items");

        MoveItemToLearned(vocabItem);
    }

    private void ShowNextItem() {
        if (_unlearnedVocabItems.Count == 0) return;

        var newVocabItem = _unlearnedVocabItems[0];
        newVocabItem.transform.eulerAngles = itemPlacePosition.forward;
        ((IObjectMover)this).PlaceItem(newVocabItem, itemPlacePosition.position);
        
        LogEvent($"{newVocabItem.ENName} moved to {itemPlacePosition.position}", LogLevel.Debug);
        LogEvent($"Next vocab item is {newVocabItem.ENName}");
    }

    private void MoveItemToLearned(VocabItem vocabItem) {
        var pos = learnedSpawns[^1];
        learnedSpawns.RemoveAt(learnedSpawns.Count - 1);

        ((IObjectMover)this).PlaceItem(vocabItem, pos);
        LogEvent($"{vocabItem.ENName} moved to {pos}", LogLevel.Debug);
        
        ShowNextItem();
    }

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message, level);
    }

    private void EnsureSingleton() {
        if (Instance != this && Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
