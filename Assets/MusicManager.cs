using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource out1;
    private AudioSource out2;
    private AudioSource out3;
    private AudioClip C;
    private AudioClip Cs;
    private AudioClip D;
    private AudioClip Ds;
    private AudioClip E;
    private AudioClip F;
    private AudioClip Fs;
    private AudioClip G;
    private AudioClip Gs;
    private AudioClip A;
    private AudioClip As;
    private AudioClip B;
    private AudioClip C2;
    private AudioClip D2;

    private AudioClip C4;
    private AudioClip C4s;
    private AudioClip D4;
    private AudioClip D4s;
    private AudioClip E4;
    private AudioClip F4;
    private AudioClip F4s;
    private AudioClip G4;
    private AudioClip G4s;
    private AudioClip A4;
    private AudioClip A4s;
    private AudioClip B4;
    private AudioClip C5;
    private AudioClip C5s;
    private AudioClip D5;

    private AudioClip mainTheme;
    private AudioClip simpleMinorSong;
    private AudioClip simpleMajor0;
    private AudioClip jittery;
    
    private float volume;

    void Awake()
    {
        volume = 0.005f;
        out1 = gameObject.AddComponent<AudioSource>();
        out2 = gameObject.AddComponent<AudioSource>();
        out3 = gameObject.AddComponent<AudioSource>();
        C = Resources.Load("Sounds/Sawtooth/C") as AudioClip;
        Cs = Resources.Load("Sounds/Sawtooth/C#") as AudioClip;
        D = Resources.Load("Sounds/Sawtooth/D") as AudioClip;
        Ds = Resources.Load("Sounds/Sawtooth/D#") as AudioClip;
        E = Resources.Load("Sounds/Sawtooth/E") as AudioClip;
        F = Resources.Load("Sounds/Sawtooth/F") as AudioClip;
        Fs = Resources.Load("Sounds/Sawtooth/F#") as AudioClip;
        G = Resources.Load("Sounds/Sawtooth/G") as AudioClip;
        Gs = Resources.Load("Sounds/Sawtooth/G#") as AudioClip;
        A = Resources.Load("Sounds/Sawtooth/A") as AudioClip;
        As = Resources.Load("Sounds/Sawtooth/A#") as AudioClip;
        B = Resources.Load("Sounds/Sawtooth/B") as AudioClip;
        C2 = Resources.Load("Sounds/Sawtooth/C2") as AudioClip;
        D2 = Resources.Load("Sounds/Sawtooth/D2") as AudioClip;

        C4 = Resources.Load("Sounds/Sine/C4") as AudioClip;
        C4s = Resources.Load("Sounds/Sine/C4#") as AudioClip;
        D4 = Resources.Load("Sounds/Sine/D4") as AudioClip;
        D4s = Resources.Load("Sounds/Sine/D4#") as AudioClip;
        E4 = Resources.Load("Sounds/Sine/E4") as AudioClip;
        F4 = Resources.Load("Sounds/Sine/F4") as AudioClip;
        F4s = Resources.Load("Sounds/Sine/F4#") as AudioClip;
        G4 = Resources.Load("Sounds/Sine/G4") as AudioClip;
        G4s = Resources.Load("Sounds/Sine/G4#") as AudioClip;
        A4 = Resources.Load("Sounds/Sine/A4") as AudioClip;
        A4s = Resources.Load("Sounds/Sine/A4#") as AudioClip;
        B4 = Resources.Load("Sounds/Sine/B4") as AudioClip;
        C5 = Resources.Load("Sounds/Sine/C5") as AudioClip;
        C5s = Resources.Load("Sounds/Sine/C5#") as AudioClip;
        D5 = Resources.Load("Sounds/Sine/D5") as AudioClip;

        mainTheme = Resources.Load("Songs/mainTheme") as AudioClip;
        simpleMinorSong = Resources.Load("Songs/simpleMinorSong") as AudioClip;
        simpleMajor0 = Resources.Load("Songs/simpleMajorSong") as AudioClip;
        jittery = Resources.Load("Songs/Jittery") as AudioClip;
    }
    // Start is called before the first frame update
    void Start()
    {
        out1.volume = volume;
        out1.loop = true;
        out2.volume = volume;
        out2.loop = true;
        out3.volume = volume;
        out3.loop = true;
        switch(GameManager.gameMode)
        {
            case GameManager.GameMode.Menu:
                playSimpleMajor0();
                break;
            case GameManager.GameMode.Search:
                if(GameManager.isTightSpaces)
                {
                    if (GameManager.isDarkMode)
                    {
                        //playTightDark();
                    }
                    else
                    {
                        //playTightLight();
                    }
                }
                else
                {
                    if (GameManager.isDarkMode)
                    {
                        //playNormalDark();
                    }
                    else
                    {
                        //playNormalLight();
                    }
                }
                break;
            case GameManager.GameMode.TimeTrial:
                out1.clip = jittery;
                out1.volume = 0.50f;
                out1.loop = true;
                out1.Play();
                break;
            case GameManager.GameMode.BossFight:
                break;
            case GameManager.GameMode.Tutorial:
                break;
            case GameManager.GameMode.FlightTraining:
                break;
        }
    }

    private void playMenuSong()
    {
        StartCoroutine(playMenuSong1(out1));
        StartCoroutine(playMenuSong2(out2));
    }
    private IEnumerator playMenuSong1(AudioSource output)
    {
        while (!GameManager.isMusicMuted)
        {
            StartCoroutine(whole(C, output));
            yield return whole(C, output);
            StartCoroutine(whole(D, output));
            yield return whole(D, output);
            StartCoroutine(whole(C, output));
            yield return whole(C, output);
            StartCoroutine(whole(D, output));
            yield return whole(D, output);
        }
        output.Stop();
    }
    private IEnumerator playMenuSong2(AudioSource output)
    {
        while (!GameManager.isMusicMuted)
        {
            // on top of C
            StartCoroutine(quarter(D, output));
            yield return quarter(D, output);
            StartCoroutine(quarter(E, output));
            yield return quarter(E, output);
            StartCoroutine(quarter(F, output));
            yield return quarter(F, output);
            StartCoroutine(quarter(G, output));
            yield return quarter(G, output);

            // on top of D
            StartCoroutine(quarter(E, output));
            yield return quarter(E, output);
            StartCoroutine(quarter(A, output));
            yield return quarter(A, output);
            StartCoroutine(quarter(Fs, output));
            yield return quarter(Fs, output);
            StartCoroutine(quarter(G, output));
            yield return quarter(G, output);

            // on top of C
            StartCoroutine(quarter(D, output));
            yield return quarter(D, output);
            StartCoroutine(quarter(E, output));
            yield return quarter(E, output);
            StartCoroutine(quarter(F, output));
            yield return quarter(F, output);
            StartCoroutine(quarter(G, output));
            yield return quarter(G, output);

            // on top of D
            StartCoroutine(quarter(A, output));
            yield return quarter(A, output);
            StartCoroutine(quarter(G, output));
            yield return quarter(G, output);
            StartCoroutine(quarter(A, output));
            yield return quarter(A, output);
            StartCoroutine(quarter(G, output));
            yield return quarter(G, output);
        }
        output.Stop();
    }

    private void playSimpleMajor0()
    {
        out1.clip = simpleMajor0;
        out1.volume = 0.20f;
        out1.loop = true;
        out1.Play();
    }
    private void playTightDark()
    {
        out1.clip = mainTheme;
        out1.volume = 0.25f;
        out1.loop = true;
        out1.Play();
    }
    private void playTightLight()
    {
        /*
        out1.clip = simpleMinorSong;
        out1.volume = 0.25f;
        out1.loop = true;
        out1.Play();
        */
    }
    private void playNormalDark()
    {
        out1.clip = simpleMinorSong;
        out1.volume = 0.50f;
        out1.loop = true;
        out1.Play();
    }
    private void playNormalLight()
    {
        /*
        out1.clip = simpleMinorSong;
        out1.volume = 0.25f;
        out1.loop = true;
        out1.Play();
        */
    }

    private IEnumerator quarter(AudioClip note, AudioSource output)
    {
        output.clip = note;
        output.Play();
        yield return new WaitForSeconds(1.0f);
    }
    private IEnumerator whole(AudioClip note, AudioSource output)
    {
        output.clip = note;
        output.Play();
        yield return new WaitForSeconds(4.0f);
    }
}
