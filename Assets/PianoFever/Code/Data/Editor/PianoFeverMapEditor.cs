using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PianoFeverMap))]
public class PianoFeverMapEditor : Editor
{
    #region StaticAndConsts
    const float RulerHeight = 18f;
    const float DefaultWaveHeight = 64f;
    const float MinWaveHeight = 40f;
    const float MaxWaveHeight = 600f;
    const float ResizeHandleHeight = 8f;
    const float MaxWaveGain = 16f;
    const string WaveHeightPrefKey = "PianoFeverMapEditor.WaveHeight";
    const string WaveGainPrefKey = "PianoFeverMapEditor.WaveGain";
    const int MaxNodeVoicesCount = 8;
    const string MainVolumePrefKey = "PianoFeverMapEditor.MainVolume";
    const string NodeVolumePrefKey = "PianoFeverMapEditor.NodeVolume";
    const string VolumeFoldoutPrefKey = "PianoFeverMapEditor.Foldout.Volume";
    const string NodeAudioFoldoutPrefKey = "PianoFeverMapEditor.Foldout.NodeAudio";
    const string ControlsFoldoutPrefKey = "PianoFeverMapEditor.Foldout.Controls";
    const float LaneHeight = 28f;
    const float SidePadding = 8f;
    const float NodeHitRadius = 7f;
    const int WaveBuckets = 4096;
    const float MinViewDuration = 0.05f;   // maksymalny zoom: tyle sekund na całą szerokość
    const float ZoomStep = 1.1f;            // mnożnik zoomu na jednostkę scrolla
    const int MaxSamplesPerPixel = 256;     // limit odczytu próbek na kolumnę przy dużym zoomie

    static readonly Color BgColor = new Color(0.16f, 0.16f, 0.16f);
    static readonly Color RulerColor = new Color(0.22f, 0.22f, 0.22f);
    static readonly Color TickColor = new Color(1f, 1f, 1f, 0.35f);
    static readonly Color WaveColor = new Color(0.35f, 0.65f, 0.95f, 0.5f);
    static readonly Color WavePlayedColor = new Color(0.45f, 0.78f, 1f, 0.95f);
    static readonly Color LaneColor = new Color(0.12f, 0.12f, 0.12f);
    static readonly Color NodeColor = new Color(1f, 0.75f, 0.2f);
    static readonly Color NodeSelectedColor = new Color(0.3f, 1f, 0.45f);
    static readonly Color PlayheadColor = new Color(1f, 0.25f, 0.25f);

    static readonly float[] RulerSteps = { 0.01f, 0.02f, 0.05f, 0.1f, 0.25f, 0.5f, 1f, 2f, 5f, 10f, 15f, 30f, 60f, 120f, 300f, 600f };

    static readonly string[,] ControlsList =
    {
    { "Spacja", "Play / Pause" },
    { "N", "Dodaj node w pozycji playheada" },
    { "Delete", "Usuń zaznaczone nody" },
    { "Ctrl/Cmd + A", "Zaznacz wszystkie nody" },
    { "Esc", "Odznacz wszystko" },
    { "F", "Pokaż cały klip" },
    { "Klik / przeciąganie po linijce lub waveformie", "Przesuń playhead" },
    { "Klik w node", "Zaznacz tylko ten node" },
    { "Shift/Ctrl + klik w node", "Dodaj do zaznaczenia lub odejmij od niego" },
    { "Przeciąganie po pustym pasku nodów", "Zaznaczanie prostokątem (z Shift/Ctrl dodaje)" },
    { "Przeciąganie zaznaczonego noda", "Przesuń wszystkie zaznaczone nody" },
    { "Dwuklik w node", "Przeskocz playheadem do noda" },
    { "Scroll", "Zoom timeline'u wokół kursora" },
    { "Shift + Scroll / środkowy przycisk", "Przewijanie timeline'u w poziomie" },
    { "Ctrl/Cmd + Scroll", "Wzmocnienie amplitudy waveformu" },
    { "Przeciąganie uchwytu pod timelinem", "Wysokość timeline'u (dwuklik = domyślna)" }
};
    #endregion
    #region Fields
    PianoFeverMap _timeline;
    AudioSource _source;

    // dźwięk odgrywany na nodach
    AudioSource[] _nodeVoices;
    int _nextNodeVoice;
    float _scheduledUpTo; // czas na timelinie, do którego dźwięki nodów są już zaplanowane

    // odtwarzanie
    float _playhead;
    bool _isPlaying;

    // scrubowanie
    bool _isScrubbing;
    bool _wasPlayingBeforeScrub;

    // widok (zoom / przesunięcie)
    float _viewStart;
    float _viewDuration;
    bool _isPanning;

    // wysokość i wzmocnienie waveformu
    float _waveHeight = DefaultWaveHeight;
    float _waveGain = 1f;
    bool _isResizing;
    float _resizeStartMouseY;
    float _resizeStartHeight;

    // nody
    readonly HashSet<int> _selection = new HashSet<int>();
    int _panelCount;      // stan zaznaczenia zamrożony na czas jednego cyklu Layout->Repaint
    int _panelIndex = -1;

    // przesuwanie grupy nodów
    bool _isDraggingNodes;
    bool _dragMoved;
    bool _selectOnlyOnMouseUp;
    int _clickedNode = -1;
    float _dragAnchorTime;
    readonly Dictionary<int, float> _dragStartTimes = new Dictionary<int, float>();

    // zaznaczanie prostokątem
    bool _isMarquee;
    float _marqueeStartTime;
    float _marqueeEndTime;
    readonly HashSet<int> _marqueeBase = new HashSet<int>();

    // głośności podglądu (tylko edytor, w EditorPrefs)
    float _mainVolume = 1f;
    float _nodeVolume = 1f;
    int _dragUndoGroup;

    // próbki klipu (waveform + scrub)
    float[] _samples;
    int _channels;
    int _frequency;
    float[] _peaks;
    float _peakNorm = 1f;
    AudioClip _dataClip;

    GUIStyle _labelStyle;
    #endregion

    // ───────────────────────── lifecycle ─────────────────────────

    void OnEnable()
    {
        _timeline = (PianoFeverMap)target;

        var mainAudioGo = EditorUtility.CreateGameObjectWithHideFlags("PianoFeverMapPreview", HideFlags.HideAndDontSave, typeof(AudioSource));
        _source = mainAudioGo.GetComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.loop = false;
        _source.spatialBlend = 0f;

        var voicesGo = EditorUtility.CreateGameObjectWithHideFlags("PianoFeverMapNodeVoices", HideFlags.HideAndDontSave);
        _nodeVoices = new AudioSource[MaxNodeVoicesCount];
        for (int i = 0; i < MaxNodeVoicesCount; i++)
        {
            var voice = voicesGo.AddComponent<AudioSource>();
            voice.playOnAwake = false;
            voice.loop = false;
            voice.spatialBlend = 0f;
            _nodeVoices[i] = voice;
        }

        _waveHeight = Mathf.Clamp(EditorPrefs.GetFloat(WaveHeightPrefKey, DefaultWaveHeight), MinWaveHeight, MaxWaveHeight);
        _waveGain = Mathf.Clamp(EditorPrefs.GetFloat(WaveGainPrefKey, 1f), 1f, MaxWaveGain);
        _mainVolume = Mathf.Clamp01(EditorPrefs.GetFloat(MainVolumePrefKey, 1f));
        _nodeVolume = Mathf.Clamp01(EditorPrefs.GetFloat(NodeVolumePrefKey, 1f));
        ApplyVolumes();

        EditorApplication.update += OnEditorUpdate;
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
        Undo.undoRedoPerformed -= OnUndoRedo;

        if (_source != null) DestroyImmediate(_source.gameObject);
        if (_nodeVoices != null && _nodeVoices[0] != null) DestroyImmediate(_nodeVoices[0].gameObject);
        _nodeVoices = null;

        _dataClip = null;
        _samples = null;
    }

    void OnUndoRedo()
    {
        if (_timeline == null) return;
        PruneSelection();
        Repaint();
    }

    void OnEditorUpdate()
    {
        if (_timeline == null || _source == null || _timeline.clip == null) return;

        if (_isScrubbing)
        {
            Repaint();
            return;
        }

        if (_isPlaying)
        {
            if (_source.isPlaying)
            {
                _playhead = _source.time;
                ScheduleNodeSounds(_playhead, _timeline.clip.length);
                FollowPlayhead(_timeline.clip.length);
            }
            else
            {
                _isPlaying = false;
                _playhead = _timeline.clip.length;
            }
            Repaint();
        }
    }

    // ───────────────────────── GUI ─────────────────────────

    public override void OnInspectorGUI()
    {
        _timeline = (PianoFeverMap)target;
        if (_labelStyle == null) _labelStyle = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(1f, 1f, 1f, 0.85f) } };

        HandleKeyboard();

        EditorGUI.BeginChangeCheck();
        var newClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", _timeline.clip, typeof(AudioClip), false);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_timeline, "Change Audio Clip");
            _timeline.clip = newClip;
            EditorUtility.SetDirty(_timeline);
            Stop();
        }

        var clip = _timeline.clip;
        if (clip == null)
        {
            EditorGUILayout.HelpBox("Przypisz AudioClip, żeby zobaczyć timeline.", MessageType.Info);
            return;
        }

        EnsureSourceClip(clip);

        if (clip.loadType != AudioClipLoadType.DecompressOnLoad) 
            EditorGUILayout.HelpBox("Waveform wymaga Load Type = Decompress On Load na klipie (odtwarzanie działa niezależnie od tego).", MessageType.Info);
        if (EditorUtility.audioMasterMute)
            EditorGUILayout.HelpBox("Audio edytora jest wyciszone (Mute Audio w Game View).", MessageType.Warning);

        EditorGUILayout.Space(4);
        DrawToolbar(clip);
        DrawTimeline(clip);
        DrawScrollbar(clip.length);
        DrawSelectedNodePanel(clip);

        EditorGUILayout.Space(6);
        DrawFoldout(VolumeFoldoutPrefKey, "Volume Settings", DrawVolumeSettings);
        DrawFoldout(NodeAudioFoldoutPrefKey, "Node Audio Settings", DrawNodeAudioSettings);
        DrawFoldout(ControlsFoldoutPrefKey, "Controls", DrawControls);

        DrawNodeList();
    }

    void DrawNodeList()
    {
        EditorGUILayout.Space(6);
        serializedObject.Update();
        var nodesProp = serializedObject.FindProperty(nameof(PianoFeverMap.nodes));
        EditorGUILayout.PropertyField(nodesProp, new GUIContent($"Nodes ({nodesProp.arraySize})"), true);

        if (serializedObject.ApplyModifiedProperties()) PruneSelection();
    }

    void DrawFoldout(string prefKey, string title, System.Action drawContent)
    {
        bool expanded = EditorPrefs.GetBool(prefKey, false);
        bool newExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(expanded, title);
        if (newExpanded != expanded) EditorPrefs.SetBool(prefKey, newExpanded);

        if (newExpanded)
        {
            EditorGUI.indentLevel++;
            drawContent();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    void DrawNodeAudioSettings()
    {
        EditorGUI.BeginChangeCheck();
        bool playOnNode = EditorGUILayout.Toggle("Play Audio On Node", _timeline.playAudioOnNode);
        var nodeAudio = (AudioClip)EditorGUILayout.ObjectField("Node Audio", _timeline.nodeAudio, typeof(AudioClip), false);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_timeline, "Change Node Audio");
            _timeline.playAudioOnNode = playOnNode;
            _timeline.nodeAudio = nodeAudio;
            EditorUtility.SetDirty(_timeline);
        }
    }

    void DrawControls()
    {
        for (int i = 0; i < ControlsList.GetLength(0); i++)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(ControlsList[i, 0], EditorStyles.boldLabel, GUILayout.Width(250f));
                EditorGUILayout.LabelField(ControlsList[i, 1], EditorStyles.wordWrappedLabel);
            }
        }
        EditorGUILayout.Space(2);
        EditorGUILayout.LabelField("Skróty klawiszowe działają, gdy Inspector ma focus (wystarczy kliknąć w timeline).", EditorStyles.miniLabel);
    }

    void DrawVolumeSettings()
    {
        EditorGUI.BeginChangeCheck();
        _mainVolume = EditorGUILayout.Slider("Main Volume", _mainVolume, 0f, 1f);
        _nodeVolume = EditorGUILayout.Slider("Node Audio Volume", _nodeVolume, 0f, 1f);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetFloat(MainVolumePrefKey, _mainVolume);
            EditorPrefs.SetFloat(NodeVolumePrefKey, _nodeVolume);
            ApplyVolumes();
        }
    }

    void ApplyVolumes()
    {
        if (_source != null) _source.volume = _mainVolume;
        if (_nodeVoices == null) return;
        foreach (var voice in _nodeVoices)
        {
            if (voice != null) voice.volume = _nodeVolume;
        }
    }

    void DrawToolbar(AudioClip clip)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            var playContent = _isPlaying
                ? new GUIContent(" Pause", EditorGUIUtility.IconContent("PauseButton").image)
                : new GUIContent(" Play", EditorGUIUtility.IconContent("PlayButton").image);

            if (GUILayout.Button(playContent, GUILayout.Width(80), GUILayout.Height(22))) TogglePlay();

            GUILayout.FlexibleSpace();
            GUILayout.Label($"{FormatTime(_playhead)} / {FormatTime(clip.length)}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            float zoom = _viewDuration > 0f ? clip.length / _viewDuration : 1f;
            if (_waveGain > 1.01f) GUILayout.Label($"amp x{_waveGain:0.#}", EditorStyles.miniLabel);
            GUILayout.Label($"x{zoom:0.#}", EditorStyles.miniLabel);
            if (GUILayout.Button("+ Add Node", GUILayout.Width(90), GUILayout.Height(22))) AddNode(_playhead);
        }
    }

    void DrawTimeline(AudioClip clip)
    {
        float length = clip.length;
        ClampView(length);

        float totalHeight = RulerHeight + _waveHeight + LaneHeight;
        Rect reserved = GUILayoutUtility.GetRect(0, totalHeight + ResizeHandleHeight, GUILayout.ExpandWidth(true));
        Rect full = new Rect(reserved.x, reserved.y, reserved.width, totalHeight);
        Rect handle = new Rect(reserved.x + SidePadding, full.yMax, reserved.width - SidePadding * 2f, ResizeHandleHeight);
        Rect area = new Rect(full.x + SidePadding, full.y, full.width - SidePadding * 2f, totalHeight);
        Rect ruler = new Rect(area.x, area.y, area.width, RulerHeight);
        Rect wave = new Rect(area.x, ruler.yMax, area.width, _waveHeight);
        Rect lane = new Rect(area.x, wave.yMax, area.width, LaneHeight);

        int id = GUIUtility.GetControlID(FocusType.Passive);
        int resizeId = GUIUtility.GetControlID(FocusType.Passive);
        Event evt = Event.current;

        HandleResizeInput(evt, resizeId, handle);
        HandleTimelineInput(evt, id, area, lane, length);

        if (evt.type == EventType.Repaint)
        {
            EditorGUI.DrawRect(full, BgColor);
            DrawRuler(ruler);
            DrawWaveform(wave, clip);
            DrawNodes(wave, lane);
            DrawMarquee(wave, lane);
            DrawPlayhead(area);
            DrawResizeHandle(handle);
        }
        EditorGUIUtility.AddCursorRect(handle, MouseCursor.ResizeVertical);
    }

    void HandleResizeInput(Event evt, int id, Rect handle)
    {
        switch (evt.GetTypeForControl(id))
        {
            case EventType.MouseDown:
                if (evt.button != 0 || !handle.Contains(evt.mousePosition)) break;

                if (evt.clickCount == 2)
                {
                    _waveHeight = DefaultWaveHeight; // dwuklik = domyślna wysokość
                    EditorPrefs.SetFloat(WaveHeightPrefKey, _waveHeight);
                }
                else
                {
                    GUIUtility.hotControl = id;
                    _isResizing = true;
                    _resizeStartMouseY = GUIUtility.GUIToScreenPoint(evt.mousePosition).y;
                    _resizeStartHeight = _waveHeight;
                }
                evt.Use();
                break;

            case EventType.MouseDrag:
                if (GUIUtility.hotControl != id || !_isResizing) break;
                // pozycja ekranowa, bo cały layout pod spodem przesuwa się razem z uchwytem
                float delta = GUIUtility.GUIToScreenPoint(evt.mousePosition).y - _resizeStartMouseY;
                _waveHeight = Mathf.Clamp(_resizeStartHeight + delta, MinWaveHeight, MaxWaveHeight);
                evt.Use();
                Repaint();
                break;

            case EventType.MouseUp:
                if (GUIUtility.hotControl != id) break;
                GUIUtility.hotControl = 0;
                _isResizing = false;
                EditorPrefs.SetFloat(WaveHeightPrefKey, _waveHeight);
                evt.Use();
                break;
        }
    }

    void DrawResizeHandle(Rect handle)
    {
        Color color = _isResizing ? new Color(1f, 1f, 1f, 0.6f) : new Color(1f, 1f, 1f, 0.25f);
        float centerX = handle.center.x;
        float centerY = handle.center.y;
        EditorGUI.DrawRect(new Rect(centerX - 16f, centerY - 2f, 32f, 1f), color);
        EditorGUI.DrawRect(new Rect(centerX - 16f, centerY + 1f, 32f, 1f), color);
    }

    void DrawScrollbar(float length)
    {
        Rect rect = GUILayoutUtility.GetRect(0, 14f, GUILayout.ExpandWidth(true));
        rect.x += SidePadding;
        rect.width -= SidePadding * 2f;

        EditorGUI.BeginChangeCheck();
        float start = GUI.HorizontalScrollbar(rect, _viewStart, _viewDuration, 0f, length);
        if (EditorGUI.EndChangeCheck())
        {
            _viewStart = start;
            ClampView(length);
        }
    }

    void HandleTimelineInput(Event evt, int id, Rect area, Rect lane, float length)
    {
        switch (evt.GetTypeForControl(id))
        {
            case EventType.ScrollWheel:
            {
                if (!area.Contains(evt.mousePosition)) break;

                bool horizontal = Mathf.Abs(evt.delta.x) > Mathf.Abs(evt.delta.y);
                if (evt.control || evt.command)
                {
                    // wzmocnienie amplitudy waveformu (ciche fragmenty stają się czytelne)
                    _waveGain = Mathf.Clamp(_waveGain * Mathf.Pow(ZoomStep, -evt.delta.y), 1f, MaxWaveGain);
                    EditorPrefs.SetFloat(WaveGainPrefKey, _waveGain);
                }
                else if (evt.shift || horizontal)
                {
                    float amount = horizontal ? evt.delta.x : evt.delta.y;
                    _viewStart += amount * _viewDuration * 0.03f;
                    ClampView(length);
                }
                else
                {
                    ZoomAround(XToTime(evt.mousePosition.x, area, length), Mathf.Pow(ZoomStep, evt.delta.y), length);
                }
                evt.Use();
                Repaint();
                break;
            }
            case EventType.MouseDown:
            {
                if (!area.Contains(evt.mousePosition)) break;

                if (evt.button == 2)
                {
                    GUIUtility.hotControl = id;
                    _isPanning = true;
                    evt.Use();
                    break;
                }
                if (evt.button != 0) break;

                GUIUtility.hotControl = id;
                GUIUtility.keyboardControl = 0; // zdejmij focus z pól tekstowych

                bool additive = evt.shift || evt.control || evt.command;
                float mouseTime = XToTime(evt.mousePosition.x, area, length);
                int hit = lane.Contains(evt.mousePosition) ? HitTestNode(evt.mousePosition.x, area) : -1;

                if (hit >= 0)
                {
                    if (additive)
                    {
                        if (!_selection.Remove(hit)) _selection.Add(hit);
                    }
                    else if (!_selection.Contains(hit))
                    {
                        _selection.Clear();
                        _selection.Add(hit);
                    }
                    else
                    {
                        // klik (bez przeciągania) w node z większej grupy = zaznacz tylko ten
                        _selectOnlyOnMouseUp = _selection.Count > 1;
                    }

                    if (_selection.Contains(hit))
                        BeginNodesDrag(hit, mouseTime);

                    if (evt.clickCount == 2)
                        SeekTo(_timeline.nodes[hit].timestamp);
                }
                else if (lane.Contains(evt.mousePosition))
                {
                    // przeciąganie po pustym miejscu paska nodów = zaznaczanie prostokątem
                    _marqueeBase.Clear();
                    if (additive) _marqueeBase.UnionWith(_selection);
                    else _selection.Clear();

                    _isMarquee = true;
                    _marqueeStartTime = mouseTime;
                    _marqueeEndTime = mouseTime;
                }
                else
                {
                    BeginScrub(mouseTime);
                }
                evt.Use();
                break;
            }
            case EventType.MouseDrag:
            {
                if (GUIUtility.hotControl != id) break;
                float mouseTime = XToTime(evt.mousePosition.x, area, length);

                if (_isPanning)
                {
                    _viewStart -= evt.delta.x / area.width * _viewDuration;
                    ClampView(length);
                }
                else if (_isDraggingNodes)
                {
                    DragNodes(mouseTime, length);
                }
                else if (_isMarquee)
                {
                    _marqueeEndTime = mouseTime;
                    UpdateMarqueeSelection();
                    EnsureVisible(mouseTime, length);
                }
                else if (_isScrubbing)
                {
                    DragScrub(mouseTime);
                    EnsureVisible(mouseTime, length);
                }
                evt.Use();
                break;
            }
            case EventType.MouseUp:
            {
                if (GUIUtility.hotControl != id) break;
                GUIUtility.hotControl = 0;

                _isPanning = false;
                if (_isDraggingNodes)
                {
                    _isDraggingNodes = false;
                    Undo.CollapseUndoOperations(_dragUndoGroup);

                    if (!_dragMoved && _selectOnlyOnMouseUp && IsValidIndex(_clickedNode))
                    {
                        _selection.Clear();
                        _selection.Add(_clickedNode);
                    }
                }
                _selectOnlyOnMouseUp = false;
                _isMarquee = false;
                if (_isScrubbing) EndScrub();

                evt.Use();
                break;
            }
        }
    }

    void BeginNodesDrag(int clickedNode, float mouseTime)
    {
        _isDraggingNodes = true;
        _dragMoved = false;
        _clickedNode = clickedNode;
        _dragAnchorTime = mouseTime;

        _dragStartTimes.Clear();
        foreach (int i in _selection)
            if (IsValidIndex(i)) _dragStartTimes[i] = _timeline.nodes[i].timestamp;

        Undo.IncrementCurrentGroup();
        _dragUndoGroup = Undo.GetCurrentGroup();
    }

    void DragNodes(float mouseTime, float length)
    {
        if (_dragStartTimes.Count == 0) return;

        float min = float.MaxValue, max = float.MinValue;
        foreach (var t in _dragStartTimes.Values)
        {
            min = Mathf.Min(min, t);
            max = Mathf.Max(max, t);
        }

        // cała grupa przesuwa się o ten sam offset i nie wyjeżdża poza klip
        float delta = Mathf.Clamp(mouseTime - _dragAnchorTime, -min, length - max);

        Undo.RecordObject(_timeline, _dragStartTimes.Count > 1 ? "Move Nodes" : "Move Node");
        foreach (var pair in _dragStartTimes)
        {
            if (IsValidIndex(pair.Key)) _timeline.nodes[pair.Key].timestamp = pair.Value + delta;
        }
        EditorUtility.SetDirty(_timeline);

        _dragMoved = true;
        if (_dragStartTimes.TryGetValue(_clickedNode, out float clickedStart))
        {
            EnsureVisible(clickedStart + delta, length); // autoscroll przy wyjechaniu poza krawędź
        }
    }

    void UpdateMarqueeSelection()
    {
        float lo = Mathf.Min(_marqueeStartTime, _marqueeEndTime);
        float hi = Mathf.Max(_marqueeStartTime, _marqueeEndTime);

        _selection.Clear();
        _selection.UnionWith(_marqueeBase);
        for (int i = 0; i < _timeline.nodes.Count; i++)
        {
            float timestamp = _timeline.nodes[i].timestamp;
            if (timestamp >= lo && timestamp <= hi) _selection.Add(i);
        }
    }

    void DrawSelectedNodePanel(AudioClip clip)
    {
        if (Event.current.type == EventType.Layout)
        {
            PruneSelection();
            _panelCount = _selection.Count;
            _panelIndex = _panelCount == 1 ? FirstSelected() : -1;
        }

        EditorGUILayout.Space(6);

        if (_panelCount > 1)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"{_panelCount} nodes selected", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Przeciągnij dowolny z nich, żeby przesunąć całą grupę.", EditorStyles.miniLabel);
                if (DeleteButton("Delete Selected", 120f)) DeleteSelected();
            }
            return;
        }

        if (!IsValidIndex(_panelIndex))
        {
            EditorGUILayout.LabelField("Brak zaznaczonego noda.", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        var node = _timeline.nodes[_panelIndex];
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Selected Node", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            string newName = EditorGUILayout.TextField("Name", node.name);
            float newTime = EditorGUILayout.FloatField("Timestamp (s)", node.timestamp);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_timeline, "Edit Node");
                node.name = newName;
                node.timestamp = Mathf.Clamp(newTime, 0f, clip.length);
                EditorUtility.SetDirty(_timeline);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Go To")) SeekTo(node.timestamp);

                if (GUILayout.Button("Set To Playhead"))
                {
                    Undo.RecordObject(_timeline, "Set Node Time");
                    node.timestamp = _playhead;
                    EditorUtility.SetDirty(_timeline);
                }

                if (DeleteButton("Delete", 70f)) DeleteSelected();
            }
        }
    }

    static bool DeleteButton(string label, float width)
    {
        var prevBg = GUI.backgroundColor;
        GUI.backgroundColor = new Color(1f, 0.45f, 0.45f);
        bool clicked = GUILayout.Button(label, GUILayout.Width(width));
        GUI.backgroundColor = prevBg;
        return clicked;
    }

    void HandleKeyboard()
    {
        var evt = Event.current;
        // keyboardControl != 0 -> użytkownik pisze w polu tekstowym, nie przechwytujemy klawiszy
        if (evt.type != EventType.KeyDown || GUIUtility.keyboardControl != 0 || _timeline.clip == null) return;

        switch (evt.keyCode)
        {
            case KeyCode.Space:
                TogglePlay();
                evt.Use();
                break;

            case KeyCode.A:
                if (!evt.control && !evt.command) break;
                _selection.Clear();
                for (int i = 0; i < _timeline.nodes.Count; i++) _selection.Add(i);
                evt.Use();
                Repaint();
                break;

            case KeyCode.Escape:
                if (_selection.Count == 0) break;
                _selection.Clear();
                evt.Use();
                Repaint();
                break;

            case KeyCode.N:
                AddNode(_playhead);
                evt.Use();
                Repaint();
                break;

            case KeyCode.F:
                FitView(_timeline.clip.length);
                evt.Use();
                Repaint();
                break;

            case KeyCode.Delete:
                if (_selection.Count == 0) break;
                evt.Use();
                DeleteSelected();
                break;
        }
    }

    // ───────────────────────── widok ─────────────────────────

    void ClampView(float length)
    {
        if (length <= 0f) { _viewStart = 0f; _viewDuration = 0f; return; }
        if (_viewDuration <= 0f) _viewDuration = length;

        _viewDuration = Mathf.Clamp(_viewDuration, Mathf.Min(MinViewDuration, length), length);
        _viewStart = Mathf.Clamp(_viewStart, 0f, length - _viewDuration);
    }

    void FitView(float length)
    {
        _viewStart = 0f;
        _viewDuration = length;
    }

    void ZoomAround(float pivotTime, float factor, float length)
    {
        float ratio = _viewDuration > 0f ? (pivotTime - _viewStart) / _viewDuration : 0f;
        _viewDuration *= factor;
        ClampView(length);
        _viewStart = pivotTime - ratio * _viewDuration;
        ClampView(length);
    }

    /// Minimalnie przesuwa widok, żeby czas był widoczny (autoscroll przy przeciąganiu).
    void EnsureVisible(float time, float length)
    {
        if (time < _viewStart) _viewStart = time;
        else if (time > _viewStart + _viewDuration) _viewStart = time - _viewDuration;
        ClampView(length);
    }

    /// Centruje widok na czasie, jeśli jest poza ekranem.
    void RevealTime(float time, float length)
    {
        if (time >= _viewStart && time <= _viewStart + _viewDuration) return;
        _viewStart = time - _viewDuration * 0.5f;
        ClampView(length);
    }

    /// Podczas odtwarzania przewija widok stronami.
    void FollowPlayhead(float length)
    {
        if (_playhead >= _viewStart && _playhead <= _viewStart + _viewDuration) return;
        _viewStart = _playhead - _viewDuration * 0.05f;
        ClampView(length);
    }

    float TimeToX(float time, Rect r)
    {
        return r.x + (_viewDuration > 0f ? (time - _viewStart) / _viewDuration : 0f) * r.width;
    }

    float XToTime(float x, Rect r, float length)
    {
        return Mathf.Clamp(_viewStart + (x - r.x) / r.width * _viewDuration, 0f, length);
    }

    bool IsTimeVisible(float time, float margin = 0f)
    {
        return time >= _viewStart - margin && time <= _viewStart + _viewDuration + margin;
    }

    // ───────────────────────── rysowanie ─────────────────────────

    void DrawRuler(Rect rect)
    {
        EditorGUI.DrawRect(rect, RulerColor);
        if (_viewDuration <= 0f) return;

        float pixelsPerSecond = rect.width / _viewDuration;
        float lastStep = RulerSteps[RulerSteps.Length - 1];
        foreach (var step in RulerSteps)
        {
            if (step * pixelsPerSecond >= 70f) 
            { 
                lastStep = step; 
                break; 
            }
        }

        float viewEnd = _viewStart + _viewDuration;
        int first = Mathf.FloorToInt(_viewStart / lastStep);
        int last = Mathf.CeilToInt(viewEnd / lastStep);

        for (int i = first; i <= last; i++)
        {
            float time = i * lastStep;
            float x = TimeToX(time, rect);
            if (x >= rect.x && x <= rect.xMax)
            {
                EditorGUI.DrawRect(new Rect(x, rect.yMax - 6f, 1f, 6f), TickColor);
                if (x + 40f < rect.xMax) GUI.Label(new Rect(x + 2f, rect.y, 60f, 14f), FormatShort(time, lastStep), _labelStyle);
            }

            float hx = TimeToX(time + lastStep * 0.5f, rect);
            if (hx >= rect.x && hx <= rect.xMax)
                EditorGUI.DrawRect(new Rect(hx, rect.yMax - 3f, 1f, 3f), TickColor);
        }
    }

    void DrawWaveform(Rect r, AudioClip clip)
    {
        EnsureSampleData(clip);
        float mid = r.center.y;

        if (_peaks == null || _samples == null || _viewDuration <= 0f)
        {
            EditorGUI.DrawRect(new Rect(r.x, mid, r.width, 1f), WaveColor);
            return;
        }

        float length = clip.length;
        int totalFrames = _samples.Length / _channels;
        int columns = Mathf.Max(1, Mathf.FloorToInt(r.width));
        float halfHeight = r.height * 0.5f - 2f;
        float playheadX = TimeToX(_playhead, r);

        float secondsPerColumn = _viewDuration / columns;
        // przy oddaleniu korzystamy z prekalkulowanych peaków, przy zoomie czytamy próbki bezpośrednio
        bool useBuckets = secondsPerColumn >= length / WaveBuckets;

        for (int px = 0; px < columns; px++)
        {
            float t0 = _viewStart + px * secondsPerColumn;
            float t1 = t0 + secondsPerColumn;
            float peak = 0f;

            if (useBuckets)
            {
                int b0 = Mathf.Clamp((int)(t0 / length * WaveBuckets), 0, WaveBuckets - 1);
                int b1 = Mathf.Clamp((int)(t1 / length * WaveBuckets), b0 + 1, WaveBuckets);
                for (int b = b0; b < b1; b++)
                    peak = Mathf.Max(peak, _peaks[b]);
            }
            else
            {
                int f0 = Mathf.Clamp((int)(t0 * _frequency), 0, totalFrames - 1);
                int f1 = Mathf.Clamp((int)(t1 * _frequency), f0 + 1, totalFrames);
                int stride = Mathf.Max(1, (f1 - f0) / MaxSamplesPerPixel);
                for (int f = f0; f < f1; f += stride)
                {
                    int o = f * _channels;
                    for (int c = 0; c < _channels; c++)
                        peak = Mathf.Max(peak, Mathf.Abs(_samples[o + c]));
                }
                peak *= _peakNorm;
            }

            float h = Mathf.Max(1f, Mathf.Min(peak * _waveGain, 1f) * halfHeight);
            float x = r.x + px;
            EditorGUI.DrawRect(new Rect(x, mid - h, 1f, h * 2f), x <= playheadX ? WavePlayedColor : WaveColor);
        }
    }

    void DrawNodes(Rect wave, Rect lane)
    {
        EditorGUI.DrawRect(lane, LaneColor);

        var nodes = _timeline.nodes;
        for (int pass = 0; pass < 2; pass++) // zaznaczone rysowane na wierzchu
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                bool selected = _selection.Contains(i);
                if (selected != (pass == 1)) continue;

                var node = nodes[i];
                float x = TimeToX(node.timestamp, lane);
                if (x < lane.x - 6f || x > lane.xMax + 6f) continue;

                Color c = selected ? NodeSelectedColor : NodeColor;
                EditorGUI.DrawRect(new Rect(x - 0.5f, wave.y, 1f, wave.height + 4f), new Color(c.r, c.g, c.b, 0.45f));
                EditorGUI.DrawRect(new Rect(x - 5f, lane.y + 4f, 10f, 10f), c);

                if (_isDraggingNodes && i == _clickedNode)
                    GUI.Label(new Rect(x + 4f, wave.y + 2f, 80f, 14f), FormatTime(node.timestamp), _labelStyle);

                EditorGUIUtility.AddCursorRect(
                    new Rect(x - NodeHitRadius, lane.y, NodeHitRadius * 2f, lane.height), MouseCursor.SlideArrow);
            }
        }
    }

    void DrawMarquee(Rect wave, Rect lane)
    {
        if (!_isMarquee) return;

        float x0 = Mathf.Clamp(TimeToX(Mathf.Min(_marqueeStartTime, _marqueeEndTime), lane), lane.x, lane.xMax);
        float x1 = Mathf.Clamp(TimeToX(Mathf.Max(_marqueeStartTime, _marqueeEndTime), lane), lane.x, lane.xMax);
        var r = new Rect(x0, wave.y, Mathf.Max(1f, x1 - x0), lane.yMax - wave.y);

        var border = new Color(0.3f, 1f, 0.45f, 0.8f);
        EditorGUI.DrawRect(r, new Color(0.3f, 1f, 0.45f, 0.1f));
        EditorGUI.DrawRect(new Rect(r.x, r.y, 1f, r.height), border);
        EditorGUI.DrawRect(new Rect(r.xMax - 1f, r.y, 1f, r.height), border);
    }

    void DrawPlayhead(Rect area)
    {
        if (!IsTimeVisible(_playhead)) return;

        float x = TimeToX(_playhead, area);
        EditorGUI.DrawRect(new Rect(x - 1f, area.y, 2f, area.height), PlayheadColor);
        EditorGUI.DrawRect(new Rect(x - 4f, area.y, 9f, 6f), PlayheadColor);
    }

    // ───────────────────────── audio ─────────────────────────

    void EnsureSourceClip(AudioClip clip)
    {
        if (_source.clip == clip) return;

        _source.Stop();
        StopNodeVoices();
        _isPlaying = false;
        _source.clip = clip;
        if (clip != null)
        {
            if (clip.loadState == AudioDataLoadState.Unloaded) clip.LoadAudioData();
            _playhead = Mathf.Clamp(_playhead, 0f, clip.length);
            FitView(clip.length);
        }
    }

    void TogglePlay()
    {
        if (_isPlaying) Pause();
        else Play();
    }

    void Play()
    {
        var clip = _timeline.clip;
        if (clip == null) return;
        EnsureSourceClip(clip);

        if (_playhead >= clip.length - 0.01f) _playhead = 0f;
        _source.pitch = 1f;
        _source.Play();
        SetSourceTime(_playhead);
        _isPlaying = true;
        ResetNodeScheduling(_playhead);
    }

    void Pause()
    {
        _source.Pause();
        _isPlaying = false;
        StopNodeVoices();
    }

    void Stop()
    {
        if (_source != null) _source.Stop();
        StopNodeVoices();
        _isPlaying = false;
        _playhead = 0f;
        _viewStart = 0f;
    }

    void SeekTo(float time)
    {
        _playhead = time;
        if (_isPlaying)
        {
            SetSourceTime(time);
            StopNodeVoices();
            ResetNodeScheduling(time);
        }
        if (_timeline.clip != null) RevealTime(time, _timeline.clip.length);
    }

    /// Node leżący dokładnie w miejscu startu też ma zagrać.
    void ResetNodeScheduling(float fromTime)
    {
        _scheduledUpTo = fromTime - 0.0001f;
    }

    /// Planuje (PlayScheduled) dźwięki nodów, które wypadają w najbliższym oknie czasu,
    /// dzięki czemu trafiają w swój moment niezależnie od częstotliwości odświeżania edytora.
    void ScheduleNodeSounds(float currentTime, float length)
    {
        if (!_timeline.playAudioOnNode || _timeline.nodeAudio == null || _nodeVoices == null)
        {
            _scheduledUpTo = currentTime;
            return;
        }

        float until = Mathf.Min(currentTime, length);
        if (until <= _scheduledUpTo) return;

        double dspNow = AudioSettings.dspTime;
        foreach (var node in _timeline.nodes)
        {
            if (node.timestamp <= _scheduledUpTo || node.timestamp > until) continue;

            var voice = _nodeVoices[_nextNodeVoice];
            _nextNodeVoice = (_nextNodeVoice + 1) % _nodeVoices.Length;

            voice.Stop();
            voice.clip = _timeline.nodeAudio;
            voice.PlayScheduled(dspNow + Mathf.Max(0f, node.timestamp - currentTime));
        }

        _scheduledUpTo = until;
    }

    void StopNodeVoices()
    {
        if (_nodeVoices == null) return;
        foreach (var voice in _nodeVoices)
            if (voice != null) voice.Stop();
    }

    void SetSourceTime(float time)
    {
        if (_source.clip == null) return;
        _source.time = Mathf.Clamp(time, 0f, Mathf.Max(0f, _source.clip.length - 0.01f));
    }

    void BeginScrub(float time)
    {
        _wasPlayingBeforeScrub = _isPlaying;
        _isPlaying = false;
        _isScrubbing = true;
        _source.Pause();
        StopNodeVoices();

        _playhead = time;
    }

    void DragScrub(float time)
    {
        _playhead = time;
    }

    void EndScrub()
    {
        _isScrubbing = false;

        // jeśli grało przed przesuwaniem, wznów od nowej pozycji
        if (_wasPlayingBeforeScrub) Play();
    }

    // ───────────────────────── nody ─────────────────────────

    void AddNode(float time)
    {
        Undo.RecordObject(_timeline, "Add Node");
        _timeline.nodes.Add(new PianoFeverMap.Node { timestamp = time });
        _selection.Clear();
        _selection.Add(_timeline.nodes.Count - 1);
        EditorUtility.SetDirty(_timeline);
    }

    void DeleteSelected()
    {
        PruneSelection();
        if (_selection.Count == 0) return;

        Undo.RecordObject(_timeline, _selection.Count > 1 ? "Delete Nodes" : "Delete Node");
        var indices = new List<int>(_selection);
        indices.Sort();
        for (int i = indices.Count - 1; i >= 0; i--)
            _timeline.nodes.RemoveAt(indices[i]);

        _selection.Clear();
        EditorUtility.SetDirty(_timeline);
        GUIUtility.ExitGUI(); // zmiana struktury layoutu w trakcie eventu
    }

    void PruneSelection()
    {
        if (_timeline == null) return;
        int count = _timeline.nodes.Count;
        _selection.RemoveWhere(i => i < 0 || i >= count);
    }

    int FirstSelected()
    {
        foreach (int i in _selection) return i;
        return -1;
    }

    int HitTestNode(float mouseX, Rect area)
    {
        int best = -1;
        float bestDist = NodeHitRadius;
        var nodes = _timeline.nodes;
        for (int i = 0; i < nodes.Count; i++)
        {
            float d = Mathf.Abs(TimeToX(nodes[i].timestamp, area) - mouseX);
            if (d <= bestDist)
            {
                best = i;
                bestDist = d;
            }
        }
        return best;
    }

    bool IsValidIndex(int index) => _timeline != null && index >= 0 && index < _timeline.nodes.Count;

    // ───────────────────────── dane próbek ─────────────────────────

    void EnsureSampleData(AudioClip clip)
    {
        if (_dataClip == clip) return;
        _dataClip = clip;
        _samples = null;
        _peaks = null;

        if (clip == null || clip.loadType != AudioClipLoadType.DecompressOnLoad) return;
        if (clip.loadState != AudioDataLoadState.Loaded)
        {
            clip.LoadAudioData();
            if (clip.loadState != AudioDataLoadState.Loaded)
            {
                _dataClip = null; // spróbuj ponownie przy następnym repaincie
                return;
            }
        }

        var data = new float[clip.samples * clip.channels];
        if (!clip.GetData(data, 0)) return;

        _samples = data;
        _channels = clip.channels;
        _frequency = clip.frequency;
        _peaks = ComputePeaks(data, clip.channels, out float max);
        _peakNorm = max > 0f ? 1f / max : 1f;
    }

    static float[] ComputePeaks(float[] data, int channels, out float max)
    {
        int frames = data.Length / channels;
        var peaks = new float[WaveBuckets];
        max = 0f;

        for (int b = 0; b < WaveBuckets; b++)
        {
            int start = (int)((long)b * frames / WaveBuckets) * channels;
            int end = (int)((long)(b + 1) * frames / WaveBuckets) * channels;

            float peak = 0f;
            for (int i = start; i < end; i++)
                peak = Mathf.Max(peak, Mathf.Abs(data[i]));

            peaks[b] = peak;
            max = Mathf.Max(max, peak);
        }

        if (max > 0f)
            for (int b = 0; b < WaveBuckets; b++) peaks[b] /= max;

        return peaks;
    }

    // ───────────────────────── format ─────────────────────────

    static string FormatTime(float t)
    {
        int minutes = (int)(t / 60f);
        return $"{minutes:00}:{(t - minutes * 60f):00.000}";
    }

    static string FormatShort(float t, float step)
    {
        int minutes = (int)(t / 60f);
        float seconds = t - minutes * 60f;
        if (step >= 1f) return $"{minutes}:{(int)seconds:00}";
        if (step >= 0.1f) return $"{minutes}:{seconds:00.0#}";
        return $"{minutes}:{seconds:00.00#}";
    }
}