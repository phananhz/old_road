import math
import struct
import wave
import os

SAMPLE_RATE = 44100

def note_to_freq(note_name):
    notes = {'C': 0, 'C#': 1, 'Db': 1, 'D': 2, 'D#': 3, 'Eb': 3, 'E': 4, 'F': 5, 
             'F#': 6, 'Gb': 6, 'G': 7, 'G#': 8, 'Ab': 8, 'A': 9, 'A#': 10, 'Bb': 10, 'B': 11}
    letter = ""
    octave_str = ""
    for c in note_name:
        if c.isalpha() or c in '#b':
            letter += c
        elif c.isdigit() or c == '-':
            octave_str += c
    semitone = notes[letter]
    octave = int(octave_str)
    # MIDI note number: C4 is 60, A4 is 69 (440Hz)
    midi = (octave + 1) * 12 + semitone
    return 440.0 * (2.0 ** ((midi - 69) / 12.0))

class SoundBuffer:
    def __init__(self, duration_sec):
        self.num_samples = int(duration_sec * SAMPLE_RATE)
        self.left = [0.0] * self.num_samples
        self.right = [0.0] * self.num_samples

    def add_sample(self, index, l_val, r_val):
        # Wrap index for seamless looping
        idx = index % self.num_samples
        self.left[idx] += l_val
        self.right[idx] += r_val

    def render_guitar_note(self, start_sec, freq, duration_sec, volume=0.3, pan=0.0):
        start_idx = int(start_sec * SAMPLE_RATE)
        samples = int(duration_sec * SAMPLE_RATE)
        
        # Multi-harmonic plucked string with physical damping
        f1, f2, f3, f4, f5 = freq, freq * 2, freq * 3, freq * 4, freq * 5
        decay = 3.2 + (freq / 300.0) * 1.5
        
        for i in range(samples):
            t = i / SAMPLE_RATE
            env = math.exp(-decay * t)
            if env < 0.0001:
                break
            
            # String pluck waveform (fundamental + warm harmonics + soft transient)
            w1 = math.sin(2.0 * math.pi * f1 * t)
            w2 = math.sin(2.0 * math.pi * f2 * t) * 0.45 * math.exp(-t * 5.0)
            w3 = math.sin(2.0 * math.pi * f3 * t) * 0.25 * math.exp(-t * 8.0)
            w4 = math.sin(2.0 * math.pi * f4 * t) * 0.12 * math.exp(-t * 12.0)
            
            # Gentle wooden body resonance
            body = math.sin(2.0 * math.pi * 115.0 * t) * 0.15 * math.exp(-t * 14.0)
            
            val = (w1 + w2 + w3 + w4 + body) * env * volume
            
            l_gain = (1.0 - pan) * 0.5
            r_gain = (1.0 + pan) * 0.5
            self.add_sample(start_idx + i, val * l_gain, val * r_gain)

    def render_flute_note(self, start_sec, freq, duration_sec, volume=0.25, pan=0.0):
        start_idx = int(start_sec * SAMPLE_RATE)
        samples = int(duration_sec * SAMPLE_RATE)
        attack_sec = min(0.08, duration_sec * 0.2)
        release_sec = min(0.12, duration_sec * 0.3)
        
        for i in range(samples):
            t = i / SAMPLE_RATE
            
            # ADSR envelope
            if t < attack_sec:
                env = (t / attack_sec)
            elif t > (duration_sec - release_sec):
                env = max(0.0, (duration_sec - t) / release_sec)
            else:
                env = 1.0
            
            # Expressive musical vibrato (starts smoothly after 0.1s)
            vib_depth = min(1.0, max(0.0, (t - 0.1) * 3.0)) * 0.008
            vibrato = math.sin(2.0 * math.pi * 5.2 * t) * vib_depth
            mod_freq = freq * (1.0 + vibrato)
            
            # Warm woodwind timbre (odd and even gentle harmonics + soft warm breath)
            w1 = math.sin(2.0 * math.pi * mod_freq * t)
            w2 = math.sin(2.0 * math.pi * mod_freq * 2.0 * t) * 0.22
            w3 = math.sin(2.0 * math.pi * mod_freq * 3.0 * t) * 0.08
            
            val = (w1 + w2 + w3) * env * volume
            
            l_gain = (1.0 - pan) * 0.5
            r_gain = (1.0 + pan) * 0.5
            self.add_sample(start_idx + i, val * l_gain, val * r_gain)

    def render_piano_note(self, start_sec, freq, duration_sec, volume=0.22, pan=0.0):
        start_idx = int(start_sec * SAMPLE_RATE)
        samples = int(duration_sec * SAMPLE_RATE)
        decay = 1.8 + (freq / 400.0) * 1.2
        
        for i in range(samples):
            t = i / SAMPLE_RATE
            env = math.exp(-decay * t)
            if env < 0.0001:
                break
            
            # Felt piano / bell tone
            w1 = math.sin(2.0 * math.pi * freq * t)
            w2 = math.sin(2.0 * math.pi * freq * 2.0 * t) * 0.35 * math.exp(-t * 4.0)
            w3 = math.sin(2.0 * math.pi * freq * 3.0 * t) * 0.15 * math.exp(-t * 7.0)
            w4 = math.sin(2.0 * math.pi * freq * 4.0 * t) * 0.06 * math.exp(-t * 10.0)
            
            val = (w1 + w2 + w3 + w4) * env * volume
            
            l_gain = (1.0 - pan) * 0.5
            r_gain = (1.0 + pan) * 0.5
            self.add_sample(start_idx + i, val * l_gain, val * r_gain)

    def render_musicbox_note(self, start_sec, freq, duration_sec, volume=0.18, pan=0.0):
        start_idx = int(start_sec * SAMPLE_RATE)
        samples = int(duration_sec * SAMPLE_RATE)
        decay = 2.2
        
        for i in range(samples):
            t = i / SAMPLE_RATE
            env = math.exp(-decay * t)
            if env < 0.0001:
                break
            
            # Pure crystalline bell chime
            w1 = math.sin(2.0 * math.pi * freq * t)
            w2 = math.sin(2.0 * math.pi * freq * 2.756 * t) * 0.18 * math.exp(-t * 6.0)
            w3 = math.sin(2.0 * math.pi * freq * 5.404 * t) * 0.08 * math.exp(-t * 9.0)
            
            val = (w1 + w2 + w3) * env * volume
            
            l_gain = (1.0 - pan) * 0.5
            r_gain = (1.0 + pan) * 0.5
            self.add_sample(start_idx + i, val * l_gain, val * r_gain)

    def render_cello_note(self, start_sec, freq, duration_sec, volume=0.22, pan=0.0):
        start_idx = int(start_sec * SAMPLE_RATE)
        samples = int(duration_sec * SAMPLE_RATE)
        attack_sec = 0.12
        release_sec = 0.20
        
        for i in range(samples):
            t = i / SAMPLE_RATE
            if t < attack_sec:
                env = t / attack_sec
            elif t > (duration_sec - release_sec):
                env = max(0.0, (duration_sec - t) / release_sec)
            else:
                env = 1.0
            
            # Rich bowed string (warm saw-like harmonics)
            w1 = math.sin(2.0 * math.pi * freq * t)
            w2 = math.sin(2.0 * math.pi * freq * 2.0 * t) * 0.50
            w3 = math.sin(2.0 * math.pi * freq * 3.0 * t) * 0.30
            w4 = math.sin(2.0 * math.pi * freq * 4.0 * t) * 0.18
            w5 = math.sin(2.0 * math.pi * freq * 5.0 * t) * 0.08
            
            val = (w1 + w2 + w3 + w4 + w5) * 0.5 * env * volume
            
            l_gain = (1.0 - pan) * 0.5
            r_gain = (1.0 + pan) * 0.5
            self.add_sample(start_idx + i, val * l_gain, val * r_gain)

    def render_string_pad(self, start_sec, notes_list, duration_sec, volume=0.12):
        start_idx = int(start_sec * SAMPLE_RATE)
        samples = int(duration_sec * SAMPLE_RATE)
        attack_sec = min(0.6, duration_sec * 0.3)
        release_sec = min(0.8, duration_sec * 0.3)
        
        freqs = [note_to_freq(n) for n in notes_list]
        
        for i in range(samples):
            t = i / SAMPLE_RATE
            if t < attack_sec:
                env = t / attack_sec
            elif t > (duration_sec - release_sec):
                env = max(0.0, (duration_sec - t) / release_sec)
            else:
                env = 1.0
            
            chord_val_l = 0.0
            chord_val_r = 0.0
            for idx, f in enumerate(freqs):
                # Subtle stereo chorus detuning
                detune1 = 1.0 + 0.0015 * math.sin(2.0 * math.pi * 0.4 * t + idx)
                detune2 = 1.0 - 0.0015 * math.cos(2.0 * math.pi * 0.35 * t + idx)
                
                s1 = math.sin(2.0 * math.pi * f * detune1 * t) + math.sin(2.0 * math.pi * f * 2.0 * t) * 0.25
                s2 = math.sin(2.0 * math.pi * f * detune2 * t) + math.sin(2.0 * math.pi * f * 2.0 * t) * 0.25
                
                chord_val_l += s1
                chord_val_r += s2
            
            scale = (volume / len(freqs)) * env
            self.add_sample(start_idx + i, chord_val_l * scale, chord_val_r * scale)

    def write_wav(self, file_path):
        os.makedirs(os.path.dirname(os.path.abspath(file_path)), exist_ok=True)
        
        # Master limiter / soft clipper to prevent any harsh distortion
        max_val = 0.0001
        for i in range(self.num_samples):
            max_val = max(max_val, abs(self.left[i]), abs(self.right[i]))
        
        gain = 0.92 / max_val if max_val > 0.92 else 1.0
        
        with wave.open(file_path, 'wb') as wav_file:
            wav_file.setnchannels(2)
            wav_file.setsampwidth(2) # 16-bit PCM
            wav_file.setframerate(SAMPLE_RATE)
            
            frames = bytearray()
            for i in range(self.num_samples):
                # Soft tanh limiting
                l_sample = math.tanh(self.left[i] * gain)
                r_sample = math.tanh(self.right[i] * gain)
                
                l_int = int(max(-32767, min(32767, l_sample * 32767.0)))
                r_int = int(max(-32767, min(32767, r_sample * 32767.0)))
                
                frames.extend(struct.pack('<hh', l_int, r_int))
            
            wav_file.writeframes(frames)
        print(f"Generated WAV successfully: {file_path}")

def generate_peaceful_valley_bgm(output_path):
    # 84 BPM -> 1 beat = 60/84 = 0.7142857 sec
    # 16 bars of 4 beats = 64 beats total = 45.7142857 sec
    bpm = 84.0
    beat_sec = 60.0 / bpm
    bar_sec = beat_sec * 4.0
    total_duration = bar_sec * 16.0
    
    buf = SoundBuffer(total_duration)
    
    # 16-bar Chord Progression in G Major (Stardew Valley / Cozy RPG Pastoral Style)
    chords = [
        # Section A: Gentle Pastoral Morning
        ("G", ["G3", "B3", "D4", "G4"], "G2", 0.0),
        ("Cmaj7", ["C3", "G3", "B3", "E4"], "C2", 1.0),
        ("Dadd9", ["D3", "A3", "D4", "F#4"], "D2", 2.0),
        ("Em7", ["E3", "B3", "D4", "G4"], "E2", 3.0),
        ("C", ["C3", "G3", "C4", "E4"], "C2", 4.0),
        ("G/B", ["B2", "G3", "D4", "G4"], "B1", 5.0),
        ("Am7", ["A2", "E3", "G3", "C4"], "A1", 6.0),
        ("D7", ["D3", "A3", "C4", "F#4"], "D2", 7.0),
        
        # Section B: Uplifting Adventure Melodic Lift
        ("G", ["G3", "D4", "G4", "B4"], "G2", 8.0),
        ("Bm7", ["B2", "F#3", "A3", "D4"], "B1", 9.0),
        ("Cmaj7", ["C3", "G3", "B3", "E4"], "C2", 10.0),
        ("D7", ["D3", "A3", "C4", "F#4"], "D2", 11.0),
        ("Em", ["E3", "B3", "E4", "G4"], "E2", 12.0),
        ("C", ["C3", "G3", "C4", "E4"], "C2", 13.0),
        ("Am7", ["A2", "E3", "G3", "C4"], "A1", 14.0),
        ("D7sus4", ["D3", "A3", "C4", "G4"], "D2", 15.0)
    ]
    
    # 1. Layer String Pads & Cello Bass
    for name, notes, bass_note, bar_idx in chords:
        t_start = bar_idx * bar_sec
        # Lush String Pad
        buf.render_string_pad(t_start, notes, bar_sec + 0.5, volume=0.14)
        
        # Cello on Beat 1 & Beat 3
        bass_freq = note_to_freq(bass_note)
        buf.render_cello_note(t_start, bass_freq, beat_sec * 1.8, volume=0.28, pan=-0.25)
        fifth_freq = bass_freq * 1.5
        buf.render_cello_note(t_start + beat_sec * 2.0, fifth_freq, beat_sec * 1.6, volume=0.22, pan=-0.2)
        
        # 2. Acoustic Guitar Fingerpicking (8th note pattern)
        guitar_notes = [note_to_freq(n) for n in notes]
        # Arpeggio pattern: 0, 1, 2, 3, 2, 1, 2, 3
        pattern = [0, 1, 2, 3, 2, 1, 2, 3]
        for step in range(8):
            note_time = t_start + step * (beat_sec * 0.5)
            f = guitar_notes[pattern[step] % len(guitar_notes)]
            # Subtle velocity accent
            vel = 0.28 if step in [0, 4] else (0.22 if step in [2, 6] else 0.17)
            pan = -0.35 if step % 2 == 0 else -0.15
            buf.render_guitar_note(note_time, f, beat_sec * 1.2, volume=vel, pan=pan)
            
        # 3. Soft Felt Piano Chords on Beat 1 & Beat 2.5
        buf.render_piano_note(t_start, guitar_notes[0], beat_sec * 2.0, volume=0.16, pan=0.25)
        buf.render_piano_note(t_start, guitar_notes[1], beat_sec * 2.0, volume=0.14, pan=0.30)
        buf.render_piano_note(t_start, guitar_notes[2], beat_sec * 2.0, volume=0.14, pan=0.35)
        buf.render_piano_note(t_start + beat_sec * 2.5, guitar_notes[-1], beat_sec * 1.5, volume=0.15, pan=0.3)

    # 4. Memorable Pastoral Flute Melody
    # Section A Melody (Bars 0-7)
    melody_events_A = [
        # Bar 0 (G): G4 -> B4 -> D5
        (0.0, "D4", 1.0), (1.0, "G4", 1.5), (2.5, "B4", 0.5), (3.0, "D5", 1.0),
        # Bar 1 (Cmaj7): E5 -> D5 -> B4 -> G4
        (4.0, "E5", 2.0), (6.0, "D5", 1.0), (7.0, "B4", 1.0),
        # Bar 2 (Dadd9): A4 -> B4 -> D5 -> F#5
        (8.0, "A4", 1.5), (9.5, "B4", 0.5), (10.0, "D5", 1.5), (11.5, "F#5", 0.5),
        # Bar 3 (Em7): G5 -> E5 -> D5
        (12.0, "G5", 2.5), (14.5, "E5", 0.5), (15.0, "D5", 1.0),
        # Bar 4 (C): C5 -> D5 -> E5 -> G5
        (16.0, "C5", 1.5), (17.5, "D5", 0.5), (18.0, "E5", 1.5), (19.5, "G5", 0.5),
        # Bar 5 (G/B): D5 -> B4 -> G4
        (20.0, "D5", 2.0), (22.0, "B4", 1.0), (23.0, "G4", 1.0),
        # Bar 6 (Am7): A4 -> C5 -> E5 -> D5
        (24.0, "A4", 1.5), (25.5, "C5", 0.5), (26.0, "E5", 1.0), (27.0, "D5", 1.0),
        # Bar 7 (D7): B4 -> A4 -> G4
        (28.0, "B4", 1.5), (29.5, "A4", 1.5), (31.0, "F#4", 1.0),
    ]
    
    # Section B Melody (Bars 8-15)
    melody_events_B = [
        # Bar 8 (G): B4 -> D5 -> G5
        (32.0, "B4", 1.0), (33.0, "D5", 1.0), (34.0, "G5", 2.0),
        # Bar 9 (Bm7): F#5 -> E5 -> D5
        (36.0, "F#5", 1.5), (37.5, "E5", 0.5), (38.0, "D5", 2.0),
        # Bar 10 (Cmaj7): E5 -> G5 -> B5 -> A5
        (40.0, "E5", 1.5), (41.5, "G5", 0.5), (42.0, "B5", 1.0), (43.0, "A5", 1.0),
        # Bar 11 (D7): F#5 -> D5 -> A4
        (44.0, "F#5", 2.0), (46.0, "D5", 1.0), (47.0, "A4", 1.0),
        # Bar 12 (Em): G5 -> F#5 -> E5 -> D5
        (48.0, "G5", 1.5), (49.5, "F#5", 0.5), (50.0, "E5", 1.0), (51.0, "D5", 1.0),
        # Bar 13 (C): E5 -> G5 -> C6
        (52.0, "E5", 1.5), (53.5, "G5", 0.5), (54.0, "C6", 2.0),
        # Bar 14 (Am7): B5 -> A5 -> G5 -> E5
        (56.0, "B5", 1.0), (57.0, "A5", 1.0), (58.0, "G5", 1.0), (59.0, "E5", 1.0),
        # Bar 15 (D7sus4): D5 -> G5 -> A5 -> G5 (smooth transition into Bar 0)
        (60.0, "D5", 1.5), (61.5, "G5", 0.5), (62.0, "A5", 1.0), (63.0, "B5", 1.0)
    ]
    
    for beat_offset, note, dur_beats in melody_events_A + melody_events_B:
        t = beat_offset * beat_sec
        f = note_to_freq(note)
        dur = dur_beats * beat_sec
        # Flute Lead in Center-Right
        buf.render_flute_note(t, f, dur, volume=0.28, pan=0.15)
        # Music Box sparkle doubling key notes
        if dur_beats >= 1.5:
            buf.render_musicbox_note(t, f * 2.0, beat_sec * 1.5, volume=0.12, pan=0.4)

    buf.write_wav(output_path)

def generate_night_peace_bgm(output_path):
    # 68 BPM -> 1 beat = 0.88235 sec, 16 bars = 56.47 sec
    bpm = 68.0
    beat_sec = 60.0 / bpm
    bar_sec = beat_sec * 4.0
    total_duration = bar_sec * 16.0
    
    buf = SoundBuffer(total_duration)
    
    night_chords = [
        ("Em7", ["E3", "G3", "B3", "D4"], "E2", 0.0),
        ("Cmaj7", ["C3", "E3", "G3", "B3"], "C2", 1.0),
        ("G", ["G2", "D3", "G3", "B3"], "G1", 2.0),
        ("D/F#", ["F#2", "D3", "F#3", "A3"], "F#1", 3.0),
        ("Em7", ["E3", "G3", "B3", "D4"], "E2", 4.0),
        ("Am7", ["A2", "E3", "G3", "C4"], "A1", 5.0),
        ("Bm7", ["B2", "F#3", "A3", "D4"], "B1", 6.0),
        ("Em", ["E2", "B2", "E3", "G3"], "E1", 7.0),
        
        ("Cmaj7", ["C3", "G3", "B3", "E4"], "C2", 8.0),
        ("D7", ["D3", "A3", "C4", "F#4"], "D2", 9.0),
        ("Bm7", ["B2", "F#3", "A3", "D4"], "B1", 10.0),
        ("Em", ["E3", "B3", "E4", "G4"], "E2", 11.0),
        ("Am7", ["A2", "E3", "G3", "C4"], "A1", 12.0),
        ("D7", ["D3", "A3", "C4", "F#4"], "D2", 13.0),
        ("Gmaj7", ["G2", "D3", "F#3", "B3"], "G1", 14.0),
        ("B7", ["B2", "D#3", "A3", "B3"], "B1", 15.0)
    ]
    
    for name, notes, bass, bar_idx in night_chords:
        t_start = bar_idx * bar_sec
        # Warm ambient night pad
        buf.render_string_pad(t_start, notes, bar_sec + 0.8, volume=0.12)
        
        # Soft cello bass on 1
        buf.render_cello_note(t_start, note_to_freq(bass), beat_sec * 3.5, volume=0.22, pan=-0.2)
        
        # Delicate music box & harp arpeggio
        freqs = [note_to_freq(n) for n in notes]
        for step in range(4):
            note_time = t_start + step * beat_sec
            f = freqs[step % len(freqs)]
            buf.render_musicbox_note(note_time, f * 2.0, beat_sec * 2.0, volume=0.18, pan=0.25 if step % 2 == 0 else -0.25)
            buf.render_piano_note(note_time + beat_sec * 0.5, f, beat_sec * 1.5, volume=0.12, pan=0.0)

    # Soothing Lullaby Flute for Night
    night_melody = [
        (0.0, "B4", 2.0), (2.0, "D5", 2.0),
        (4.0, "G5", 3.0), (7.0, "F#5", 1.0),
        (8.0, "E5", 2.5), (10.5, "D5", 0.5), (11.0, "B4", 1.0),
        (12.0, "A4", 3.0), (15.0, "B4", 1.0),
        (16.0, "G4", 3.0), (19.0, "A4", 1.0),
        (20.0, "B4", 2.0), (22.0, "D5", 2.0),
        (24.0, "E5", 3.0), (27.0, "D5", 1.0),
        (28.0, "B4", 4.0),
        
        (32.0, "E5", 2.0), (34.0, "G5", 2.0),
        (36.0, "F#5", 2.0), (38.0, "D5", 2.0),
        (40.0, "D5", 2.0), (42.0, "B4", 2.0),
        (44.0, "G4", 4.0),
        (48.0, "C5", 2.0), (50.0, "E5", 2.0),
        (52.0, "D5", 2.0), (54.0, "A4", 2.0),
        (56.0, "B4", 4.0),
        (60.0, "G4", 4.0)
    ]
    for beat_off, note, dur_b in night_melody:
        t = beat_off * beat_sec
        buf.render_flute_note(t, note_to_freq(note), dur_b * beat_sec, volume=0.22, pan=0.1)

    buf.write_wav(output_path)

def generate_ambient_day(output_path):
    duration = 12.0
    buf = SoundBuffer(duration)
    # Gentle soft breeze (pink-noise filtered), no high beeps!
    import random
    random.seed(42)
    
    # Generate smoothed pink noise
    b0 = b1 = b2 = b3 = b4 = b5 = b6 = 0.0
    for i in range(buf.num_samples):
        t = i / SAMPLE_RATE
        white = (random.random() * 2.0 - 1.0)
        b0 = 0.99886 * b0 + white * 0.0555179
        b1 = 0.99332 * b1 + white * 0.0750759
        b2 = 0.96900 * b2 + white * 0.1538520
        b3 = 0.86650 * b3 + white * 0.3104856
        b4 = 0.55000 * b4 + white * 0.5329522
        b5 = -0.7616 * b5 - white * 0.0168980
        pink = (b0 + b1 + b2 + b3 + b4 + b5 + b6 + white * 0.5362) * 0.02
        b6 = white * 0.115926
        
        # Soft gentle breathing swell
        wind_mod = 0.4 + 0.6 * math.sin(2.0 * math.pi * (1.0 / duration) * t)
        buf.left[i] = pink * wind_mod * 0.45
        buf.right[i] = pink * wind_mod * 0.45
        
    buf.write_wav(output_path)

def generate_ambient_night(output_path):
    duration = 12.0
    buf = SoundBuffer(duration)
    import random
    random.seed(99)
    
    b0 = b1 = b2 = b3 = b4 = 0.0
    for i in range(buf.num_samples):
        t = i / SAMPLE_RATE
        white = (random.random() * 2.0 - 1.0)
        b0 = 0.99886 * b0 + white * 0.04
        b1 = 0.99332 * b1 + white * 0.06
        b2 = 0.96900 * b2 + white * 0.12
        pink = (b0 + b1 + b2) * 0.018
        
        # Gentle calm night breeze
        wind_mod = 0.5 + 0.5 * math.cos(2.0 * math.pi * (1.0 / duration) * t)
        buf.left[i] = pink * wind_mod * 0.35
        buf.right[i] = pink * wind_mod * 0.35
        
    buf.write_wav(output_path)

if __name__ == "__main__":
    generate_peaceful_valley_bgm("e:/Unity Project/Old Road/Assets/Game/Audio/Music/bgm_peaceful_valley.wav")
    generate_night_peace_bgm("e:/Unity Project/Old Road/Assets/Game/Audio/Music/bgm_night_peace.wav")
    generate_ambient_day("e:/Unity Project/Old Road/Assets/Game/Audio/Ambient/ambient_day_nature.wav")
    generate_ambient_night("e:/Unity Project/Old Road/Assets/Game/Audio/Ambient/ambient_night_peace.wav")
    print("All soundtracks and ambient tracks generated successfully!")
