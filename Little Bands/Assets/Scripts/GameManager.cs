﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /*  Notes for UI update:
     *  We now need to start off on an Avatar Select Page
     *  The user will select either Red, Green or Blue as their avatar
     * 
     *  We can remove the InstrumentSelectPage and go directly to the RecordPage
     *  
     *  On the record page our instrument buttons will now have two knew functions:
     *  On first click if instrument has not been selected
     *                  then select that instrument
     *  else
     *  On every subsequent click toggle the audio being played from 
     *      0. teacher audio
     *      1. if user has recording for insturment
     *          then users recording
     *      else count = 2
     *      2. mute
     *  
     *  There also be the addition of a new Audio Source to play a teacher guide to play each instrument
     */
    public static GameManager instance = null;
    public AudioRead audioReader;

    // Avatar Select Page Variables
    public GameObject AvatarSelecetPage;
    private Avatar userAvatar;
    
    // Song Select Page Variables
    public GameObject SongListPage;
    private SongItem SelectedSong;

    // Record Page Variables
    public GameObject RecordPage;

    private string SelectedInstrument;
    public GameObject avatarDisplay;
    public GameObject avatarDisplayText;
    public GameObject audioSlider;

    private bool playing_layeredAudio;
    public GameObject layeredAudio_playBtn;
    public GameObject layeredAudio_playBtnTxt;
    public Texture playTexture;
    public Texture stopTexture;

    public GameObject sheetMusicPopUp;
    public GameObject sheetMusicTitle;

    public AudioSource fullAudioSource;
    public AudioSource guitarAudioSource;
    public AudioSource bassAudioSource;
    public AudioSource pianoAudioSource;
    public AudioSource drumsAudioSource;
    public AudioSource voiceAudioSource;


    // Awake is called once after all game objects are initialized
    void Awake()
    {
        // Make Sure Only One Game Manager Exists
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        //Set Up Scene On Initial Play
        AvatarSelecetPage.SetActive(true);
        SongListPage.SetActive(false);
        RecordPage.SetActive(false);

        SelectedSong = null;
        playing_layeredAudio = false;

        audioSlider.GetComponent<UnityEngine.UI.Slider>().direction = Slider.Direction.LeftToRight;
        audioSlider.GetComponent<UnityEngine.UI.Slider>().minValue = 0;
        audioSlider.GetComponent<UnityEngine.UI.Slider>().maxValue = 1;
    }

   /*  AVATAR SELECTION PAGE
    *  The game loads and the user is prompted to either choose an avatar or load last played avatar
    */
    public void SelectAvatar(GameObject avatarButton) {
        userAvatar = avatarButton.GetComponent<Avatar>();
        AvatarSelecetPage.SetActive(false);
        SongListPage.SetActive(true);
    }

    public void Load() {
        //Here is where we will call harris's load method

        // ! UNCOMMENT ONCE LOAD FUNTIONALITY IS ADDED ! 
        //AvatarSelecetPage.SetActive(false);
        //SongListPage.SetActive(true);
    }

    /* SONG SELECTION PAGE
     * Once  the desired avatar has been selected the user will select a song
     * 
     * selecting a song will
     * 
     * Set SelectedSong
     *     Audio Source for instrument files
     *     These will be based on instrument toggels associated with the selected song
     */
    public void selectSong(GameObject SongListItem) {
        SelectedSong = SongListItem.GetComponent<SongItem>();

        //Set Audio Tracks
        fullAudioSource.clip = SelectedSong.original_full_audio;
        // Set guitar track
        switch (SelectedSong.guitarToggleCount) {
            case 0: // Teacher
                guitarAudioSource.clip = SelectedSong.original_guitar;
                guitarAudioSource.volume = 1;
                break;
            case 1: // Student
                guitarAudioSource.clip = SelectedSong.recorded_guitarClip;
                guitarAudioSource.volume = 1;
                break;
            case 2: // Muted
                guitarAudioSource.volume = 0;
                break;
        }
        // Set bass track
        switch (SelectedSong.bassToggleCount) {
            case 0:
                bassAudioSource.clip = SelectedSong.original_bass;
                bassAudioSource.volume = 1;
                break;
            case 1:
                bassAudioSource.clip = SelectedSong.recorded_bassClip;
                bassAudioSource.volume = 1;
                break;
            case 2:
                bassAudioSource.volume = 0;
                break;
        }
        // Set piano track
        switch (SelectedSong.pianoToggleCount) {
            case 0:
                pianoAudioSource.clip = SelectedSong.original_piano;
                pianoAudioSource.volume = 1;
                break;
            case 1:
                pianoAudioSource.clip = SelectedSong.recorded_pianoClip;
                pianoAudioSource.volume = 1;
                break;
            case 2:
                pianoAudioSource.volume = 0;
                break;
        }
        // Set drums track
        switch (SelectedSong.drumsToggleCount) {
            case 0:
                drumsAudioSource.clip = SelectedSong.original_drums;
                drumsAudioSource.volume = 1;
                break;
            case 1:
                drumsAudioSource.clip = SelectedSong.recorded_drumsClip;
                drumsAudioSource.volume = 1;
                break;
            case 2:
                drumsAudioSource.volume = 0;
                break;
        }
        // Set voice track
        switch (SelectedSong.voiceToggleCount) {
            case 0:
                voiceAudioSource.clip = SelectedSong.original_voice;
                voiceAudioSource.volume = 1;
                break;
            case 1:
                voiceAudioSource.clip = SelectedSong.recorded_voiceClip;
                voiceAudioSource.volume = 1;
                break;
            case 2:
                voiceAudioSource.volume = 0;
                break;
        }

        // Set Correct Avatar to display

        // Change View
        SongListPage.SetActive(false);
        RecordPage.SetActive(true);
    }

    public void backToSongs() {
        //Deselect song and audio
        SelectedSong = null;
        fullAudioSource.clip = null;
        guitarAudioSource.clip = null;
        bassAudioSource.clip = null;
        pianoAudioSource.clip = null;
        drumsAudioSource.clip = null;
        voiceAudioSource.clip = null;

        // Change view
        RecordPage.SetActive(false);
        SongListPage.SetActive(true);
    }

    /* RECORDING PAGE
     * After selecting a song the user will be need to select in instrument before they are able to record
     * 
     */

    // New Record page instrument buttons
    public void InstrumentButtonOnClick(string instrument) {
        if(SelectedInstrument != instrument) {
            SelectedInstrument = instrument;
        } else {
            if(instrument == "guitar") {
                switch (SelectedSong.guitarToggleCount) {
                    case 0:
                        if (SelectedSong.recorded_guitar != null) {
                            guitarAudioSource.clip = SelectedSong.recorded_guitarClip;
                            SelectedSong.guitarToggleCount = 1;
                            guitarAudioSource.volume = 1;
                        } else {
                            guitarAudioSource.clip = SelectedSong.original_guitar;
                            SelectedSong.guitarToggleCount = 2;
                            guitarAudioSource.volume = 0;
                        }
                        break;
                    case 1:
                        guitarAudioSource.clip = SelectedSong.original_guitar;
                        SelectedSong.guitarToggleCount = 2;
                        guitarAudioSource.volume = 0;
                        break;
                    case 2:
                        guitarAudioSource.volume = 1;
                        SelectedSong.guitarToggleCount = 0;
                        break;
                }
            }
            if (instrument == "bass") {
                switch (SelectedSong.bassToggleCount) {
                    case 0:
                        if (SelectedSong.recorded_bass != null) {
                            bassAudioSource.clip = SelectedSong.recorded_bassClip;
                            SelectedSong.bassToggleCount = 1;
                            bassAudioSource.volume = 1;
                        } else {
                            bassAudioSource.clip = SelectedSong.original_bass;
                            SelectedSong.bassToggleCount = 2;
                            bassAudioSource.volume = 0;
                        }
                        break;
                    case 1:
                        bassAudioSource.clip = SelectedSong.original_bass;
                        SelectedSong.bassToggleCount = 2;
                        bassAudioSource.volume = 0;
                        break;
                    case 2:
                        bassAudioSource.volume = 1;
                        SelectedSong.bassToggleCount = 0;
                        break;
                }
            }
            if (instrument == "piano") {
                switch (SelectedSong.pianoToggleCount) {
                    case 0:
                        if (SelectedSong.recorded_piano != null) {
                            pianoAudioSource.clip = SelectedSong.recorded_pianoClip;
                            SelectedSong.pianoToggleCount = 1;
                            pianoAudioSource.volume = 1;
                        } else {
                            pianoAudioSource.clip = SelectedSong.original_piano;
                            SelectedSong.pianoToggleCount = 2;
                            pianoAudioSource.volume = 0;
                        }
                        break;
                    case 1:
                        pianoAudioSource.clip = SelectedSong.original_piano;
                        SelectedSong.pianoToggleCount = 2;
                        pianoAudioSource.volume = 0;
                        break;
                    case 2:
                        pianoAudioSource.volume = 1;
                        SelectedSong.pianoToggleCount = 0;
                        break;
                }
            }
            if (instrument == "drums") {
                switch (SelectedSong.drumsToggleCount) {
                    case 0:
                        if (SelectedSong.recorded_drums != null) {
                            drumsAudioSource.clip = SelectedSong.recorded_drumsClip;
                            SelectedSong.drumsToggleCount = 1;
                            drumsAudioSource.volume = 1;
                        } else {
                            drumsAudioSource.clip = SelectedSong.original_drums;
                            SelectedSong.drumsToggleCount = 2;
                            drumsAudioSource.volume = 0;
                            break;
                        }
                        break;
                    case 1:
                        drumsAudioSource.clip = SelectedSong.original_drums;
                        SelectedSong.drumsToggleCount = 2;
                        drumsAudioSource.volume = 0;
                        break;
                    case 2:
                        drumsAudioSource.volume = 1;
                        SelectedSong.drumsToggleCount = 0;
                        break;
                }
            }
            if (instrument == "voice") {
                switch (SelectedSong.voiceToggleCount) {
                    case 0:
                        if (SelectedSong.recorded_voice != null) {
                            voiceAudioSource.clip = SelectedSong.recorded_voiceClip;
                            SelectedSong.voiceToggleCount = 1;
                            voiceAudioSource.volume = 1;
                        } else {
                            voiceAudioSource.clip = SelectedSong.original_voice;
                            SelectedSong.voiceToggleCount = 2;
                            voiceAudioSource.volume = 0;
                        }
                        break;
                    case 1:
                        voiceAudioSource.clip = SelectedSong.original_voice;
                        SelectedSong.voiceToggleCount = 2;
                        voiceAudioSource.volume = 0;
                        break;
                    case 2:
                        voiceAudioSource.volume = 1;
                        SelectedSong.voiceToggleCount = 0;
                        break;
                }
            }
        }
    }

    public void OpenSheetMusic() {
        sheetMusicPopUp.SetActive(true);
    }

    public void CloseSheetMusic() {
        sheetMusicPopUp.SetActive(false);
    }

    /*
    public void playFullAudio() {
        if (playing_fullAudio == false) {
            fullAudioSource.Play();
            fullAudio_playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = stopTexture;
            fullAudio_playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\n\n\nStop";
            playing_fullAudio = true;
        } else {
            fullAudioSource.Stop();
            fullAudio_playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
            fullAudio_playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\n\n\nPlay";
            playing_fullAudio = false;
        }
    }
    */

    public void playLayeredAudio() {
        if (playing_layeredAudio == false) {
            audioSlider.GetComponent<UnityEngine.UI.Slider>().maxValue = SelectedSong.original_full_audio.length;

            guitarAudioSource.Play();
            bassAudioSource.Play();
            pianoAudioSource.Play();
            drumsAudioSource.Play();
            voiceAudioSource.Play();

            layeredAudio_playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = stopTexture;
            layeredAudio_playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\nStop";
            playing_layeredAudio = true;
        } else {
            guitarAudioSource.Stop();
            bassAudioSource.Stop();
            pianoAudioSource.Stop();
            drumsAudioSource.Stop();
            voiceAudioSource.Stop();
            layeredAudio_playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
            layeredAudio_playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\nPlay";
            playing_layeredAudio = false;
        }
    }

    public void Record() {
        playLayeredAudio();
        audioReader.startRecord = true;

        if (playing_layeredAudio == false)
        {
            switch (SelectedInstrument)
            {
                case "guitar":     
                    SelectedSong.recorded_guitar = audioReader.recordedClips[0];
                    audioReader.startRecord = false;
                    break;
                case "bass":
                    SelectedSong.recorded_bass = audioReader.recordedClips[0];
                    audioReader.startRecord = false;
                    break;
                case "piano":
                    SelectedSong.recorded_piano = audioReader.recordedClips[0];
                    audioReader.startRecord = false;
                    break;
                case "drums":
                    SelectedSong.recorded_drums = audioReader.recordedClips[0];
                    audioReader.startRecord = false;
                    break;
                case "voice":
                    SelectedSong.recorded_voice = audioReader.recordedClips[0];
                    audioReader.startRecord = false;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (userAvatar != null) {
            switch (SelectedInstrument) {
                case "guitar":
                    avatarDisplay.GetComponent<RawImage>().texture = userAvatar.avatarGuitar;
                    avatarDisplayText.GetComponent<UnityEngine.UI.Text>().text = userAvatar.avatarName + ": Guitar";
                    break;
                case "bass":
                    avatarDisplay.GetComponent<RawImage>().texture = userAvatar.avatarBass;
                    avatarDisplayText.GetComponent<UnityEngine.UI.Text>().text = userAvatar.avatarName + ": Bass";
                    break;
                case "piano":
                    avatarDisplay.GetComponent<RawImage>().texture = userAvatar.avatarPiano;
                    avatarDisplayText.GetComponent<UnityEngine.UI.Text>().text = userAvatar.avatarName + ": Piano";
                    break;
                case "drums":
                    avatarDisplay.GetComponent<RawImage>().texture = userAvatar.avatarDrums;
                    avatarDisplayText.GetComponent<UnityEngine.UI.Text>().text = userAvatar.avatarName + ": Drums";
                    break;
                case "voice":
                    avatarDisplay.GetComponent<RawImage>().texture = userAvatar.avatarVoice;
                    avatarDisplayText.GetComponent<UnityEngine.UI.Text>().text = userAvatar.avatarName + ": Voice";
                    break;
                default:
                    avatarDisplay.GetComponent<RawImage>().texture = userAvatar.avatar;
                    avatarDisplayText.GetComponent<UnityEngine.UI.Text>().text = userAvatar.avatarName + ": No Instrument";
                    break;
            }
        }

        /*
        if (!fullAudioSource.isPlaying) {
            fullAudio_playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
            fullAudio_playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\n\n\nPlay";
            playing_fullAudio = false;
        }
        */
        if (!playing_layeredAudio) {
            layeredAudio_playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
            layeredAudio_playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\nPlay";
        }

        if(SelectedInstrument != null) {
            switch (SelectedInstrument) {
                case "guitar":
                    audioSlider.GetComponent<UnityEngine.UI.Slider>().value = guitarAudioSource.time;
                    if (!guitarAudioSource.isPlaying) {
                        playing_layeredAudio = false;
                        /* Save recording when song finishes
                        if(audioReader.isRecording == true) {
                            SelectedSong.recorded_guitar = audioReader.recordedClips[0];
                            audioReader.startRecord = true;
                        }
                        */
                    }
                    break;
                case "bass":
                    audioSlider.GetComponent<UnityEngine.UI.Slider>().value = bassAudioSource.time;
                    if (!bassAudioSource.isPlaying) {
                        playing_layeredAudio = false;
                        /* Save recording when song finishes
                        if (audioReader.isRecording == true) {
                            SelectedSong.recorded_bass = audioReader.recordedClips[0];
                            audioReader.startRecord = true;
                        }
                        */
                    }
                    break;
                case "piano":
                    audioSlider.GetComponent<UnityEngine.UI.Slider>().value = pianoAudioSource.time;
                    if (!pianoAudioSource.isPlaying) {
                        playing_layeredAudio = false;
                        /* Save recording when song finishes
                        if (audioReader.isRecording == true) {
                            SelectedSong.recorded_piano = audioReader.recordedClips[0];
                            audioReader.startRecord = true;
                        }
                        */
                    }
                    break;
                case "drums":
                    audioSlider.GetComponent<UnityEngine.UI.Slider>().value = drumsAudioSource.time;
                    if (!drumsAudioSource.isPlaying) {
                        playing_layeredAudio = false;
                        /* Save recording when song finishes
                        if (audioReader.isRecording == true) {
                            SelectedSong.recorded_drums = audioReader.recordedClips[0];
                            audioReader.startRecord = true;
                        }
                        */
                    }
                    break;
                case "voice":
                    audioSlider.GetComponent<UnityEngine.UI.Slider>().value = voiceAudioSource.time;
                    if (!voiceAudioSource.isPlaying) {
                        playing_layeredAudio = false;
                        /* Save recording when song finishes
                        if (audioReader.isRecording == true) {
                            SelectedSong.recorded_voice = audioReader.recordedClips[0];
                            audioReader.startRecord = true;
                        }
                        */
                    }
                    break;
            }
        }
    }
}
