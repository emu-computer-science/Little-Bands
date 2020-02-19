﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject SongListPage;
    public GameObject InstrumentSelect;
    public GameObject RecordPage;

    public AudioRead audioReader;

    public GameObject fullAudio_playBtn;
    public Texture playTexture;
    public Texture stopTexture;
    public GameObject fullAudio_playBtnTxt;
    public GameObject audioSlider;

    public GameObject sheetMusicPopUp;
    public GameObject sheetMusicTitle;

    public GameObject layeredAudio_playBtn;
    public GameObject layeredAudio_playBtnTxt;

    private SongItem SelectedSong;
    private string SelectedInstrument;

    private bool playing_fullAudio;
    private bool playing_layeredAudio;

    public AudioSource fullAudioSource;
    public AudioSource guitarAudioSource;
    public AudioSource bassAudioSource;
    public AudioSource pianoAudioSource;
    public AudioSource drumsAudioSource;
    public AudioSource voiceAudioSource;

    private bool guitarMuted;
    private bool bassMuted;
    private bool pianoMuted;
    private bool drumsMuted;
    private bool voiceMuted;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        SongListPage.SetActive(true);
        InstrumentSelect.SetActive(false);
        RecordPage.SetActive(false);
        SelectedSong = null;
        playing_fullAudio = false;
        playing_layeredAudio = false;
        audioSlider.GetComponent<UnityEngine.UI.Slider>().direction = Slider.Direction.LeftToRight;
        audioSlider.GetComponent<UnityEngine.UI.Slider>().minValue = 0;
        audioSlider.GetComponent<UnityEngine.UI.Slider>().maxValue = 1;
        guitarMuted = false;
        bassMuted = false;
        pianoMuted = false;
        drumsMuted = false;
        voiceMuted = false;
    }

    public void selectSong(GameObject SongListItem) {
        SelectedSong = SongListItem.GetComponent<SongItem>();
        fullAudioSource.clip = SelectedSong.original_full_audio;
        SongListPage.SetActive(false);
        InstrumentSelect.SetActive(true);
    }

   public void backToSongs() {
        SelectedSong = null;
        fullAudioSource.clip = null;
        InstrumentSelect.SetActive(false);
        SongListPage.SetActive(true);
        fullAudioSource.Stop();
        fullAudio_playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
        fullAudio_playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\n\n\nPlay";
        playing_fullAudio = false;
    }

    public void selectInstrument(string instrument) {
        SelectedInstrument = instrument;
        InstrumentSelect.SetActive(false);
        fullAudioSource.Stop();
        fullAudio_playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
        fullAudio_playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\n\n\nPlay";
        playing_fullAudio = false;
        guitarAudioSource.clip = SelectedSong.original_guitar;
        bassAudioSource.clip = SelectedSong.original_bass;
        pianoAudioSource.clip = SelectedSong.original_piano;
        drumsAudioSource.clip = SelectedSong.original_drums;
        voiceAudioSource.clip = SelectedSong.original_voice;
        RecordPage.SetActive(true);
        sheetMusicTitle.GetComponent<UnityEngine.UI.Text>().text = SelectedInstrument + " - " + SelectedSong.title;
        sheetMusicPopUp.SetActive(false);
    }

    public void backToInstrument() {
        SelectedInstrument = null;
        RecordPage.SetActive(false);
        InstrumentSelect.SetActive(true);

        guitarAudioSource.Stop();
        bassAudioSource.Stop();
        pianoAudioSource.Stop();
        drumsAudioSource.Stop();
        voiceAudioSource.Stop();

        guitarAudioSource.clip = null;
        bassAudioSource.clip = null;
        pianoAudioSource.clip = null;
        drumsAudioSource.clip = null;
        voiceAudioSource.clip = null;

        layeredAudio_playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
        layeredAudio_playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\nPlay";
        playing_layeredAudio = false;
    }

    public void OpenSheetMusic() {
        sheetMusicPopUp.SetActive(true);
    }

    public void CloseSheetMusic() {
        sheetMusicPopUp.SetActive(false);
    }

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
                    audioReader.startRecord = false;
                    SelectedSong.recorded_guitar = audioReader.recordedClips[0];
                    break;
                case "bass":
                    audioReader.startRecord = false;
                    SelectedSong.recorded_bass = audioReader.recordedClips[0];
                    break;
                case "piano":
                    audioReader.startRecord = false;
                    SelectedSong.recorded_piano = audioReader.recordedClips[0];
                    break;
                case "drums":
                    audioReader.startRecord = false;
                    SelectedSong.recorded_drums = audioReader.recordedClips[0];
                    break;
                case "voice":
                    audioReader.startRecord = false;
                    SelectedSong.recorded_voice = audioReader.recordedClips[0];
                    break;
            }
        }
    }

    public void guitarMuteToggle() {
        if (guitarMuted == false) {
            guitarMuted = true;
            guitarAudioSource.volume = 0;
        } else {
            guitarMuted = false;
            guitarAudioSource.volume = 1;
        }
    }

    public void bassMuteToggle() {
        if (bassMuted == false) {
            bassMuted = true;
            bassAudioSource.volume = 0;
        } else {
            bassMuted = false;
            bassAudioSource.volume = 1;
        }
    }

    public void pianoMuteToggle() {
        if (pianoMuted == false) {
            pianoMuted = true;
            pianoAudioSource.volume = 0;
        } else {
            pianoMuted = false;
            pianoAudioSource.volume = 1;
        }
    }

    public void drumsMuteToggle() {
        if (drumsMuted == false) {
            drumsMuted = true;
            drumsAudioSource.volume = 0;
        } else {
            drumsMuted = false;
            drumsAudioSource.volume = 1;
        }
    }

    public void voiceMuteToggle() {
        if (voiceMuted == false) {
            voiceMuted = true;
            voiceAudioSource.volume = 0;
        } else {
            voiceMuted = false;
            voiceAudioSource.volume = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        audioSlider.GetComponent<UnityEngine.UI.Slider>().value = guitarAudioSource.time;
    }
}
