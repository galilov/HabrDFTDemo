import math as m

# DTMF harmonic resolution.
harmonic_resolution = 35 # 35 Hz per harmonic
# Sample rate.
sample_rate = 8000 # 8000 samples per second
# Harmonic detection magnitude.
harmonic_detection_magnitude = 5
# Calculate the number of samples per chunk.
N = sample_rate // harmonic_resolution # 8000 / 35 = 228 samples per chunk
# Recalculate the harmonic resolution to be the actual value.
harmonic_resolution = sample_rate / N # 8000 / 228 = 35.09 Hz
# Calculate the number of samples per shift.
shift_interval = N // 3 # 228 / 3 = 76 samples per shift
# Silence between whole phone numbers is 0.2 seconds.
silence_interval = 0.2
# Calculate the number of iterations for the silence interval.
silence_number = round(silence_interval / (shift_interval / sample_rate))

# DTMF frequencies. See https://en.wikipedia.org/wiki/Dual-tone_multi-frequency_signaling
dtmf_frequencies = [697, 770, 852, 941, 1209, 1336, 1477, 1633]
# DTMF digits to frequency mapping.
freq_to_digit = {
     (697, 1209): '1', (697, 1336): '2', (697, 1477): '3', (697, 1633): 'A',
     (770, 1209): '4', (770, 1336): '5', (770, 1477): '6', (770, 1633): 'B',
     (852, 1209): '7', (852, 1336): '8', (852, 1477): '9', (852, 1633): 'C',
     (941, 1209): '*', (941, 1336): '0', (941, 1477): '#', (941, 1633): 'D'
}

# Get the harmonic of a frequency.
# frequency is an integer
# Returns the harmonic.
def get_harmonic(frequency: int) -> int:
    return round(frequency / harmonic_resolution)

# DTMF harmonics
dtmf_harmonics = [get_harmonic(f) for f in dtmf_frequencies]

# DTMF digits to harmonics mapping.
harmonic_to_digit = {(get_harmonic(d[0]), get_harmonic(d[1])): h for d, h in freq_to_digit.items()}

# Get the digit from the harmonics.
# harmonic1 and harmonic2 are integers
# Returns the digit or None if the harmonics are not valid DTMF harmonics.
def get_digit(harmonic1: int, harmonic2: int) -> str:
    h = (min(harmonic1, harmonic2), max(harmonic1, harmonic2))
    return harmonic_to_digit.get(h) # get the digit from the mapping


# Get the frequency of the harmonic.
# harmonic is an integer
# Returns the frequency.
def get_frequency(k):
    return k * harmonic_resolution


# Get the magnitude of the harmonic.
# phase_vector is a dictionary with 're' and 'im' keys
# Returns the magnitude.
def get_magnitude(phase_vector: dict) -> float:
    # Magnitude of the harmonic is the square root of the sum of the squares of the real and imaginary parts.
    re = phase_vector['re']
    im = phase_vector['im']
    return m.sqrt(re * re + im * im)


# Calculate the DFT for each harmonic.
# data is a list of samples
# harmonics is a list of harmonics
# Returns a list of phase vectors as {re: float, im: float} for each harmonic.
def calculate_dft(data: list[int], harmonics: list[int]) -> list[dict]:
    dft = [None] * len(harmonics) # prepare the list for {re: float, im: float} for each harmonic
    for i in range(len(harmonics)): # for each harmonic, i is the index
        k = harmonics[i] # k is the harmonic number
        sum_x = 0.0 # sum of the real parts
        sum_y = 0.0 # sum of the imaginary parts
        for n in range(N): # for each sample
            r = data[n] # get the sample
            x = r * m.cos(2 * m.pi * k * n / N) # real part of the sample
            y = r * m.sin(2 * m.pi * k * n / N) # imaginary part of the sample
            sum_x += x # accumulate the real part
            sum_y += y # accumulate the imaginary part
        sum_x /= N # divide the accumulated real part by N to get the average
        sum_y /= N # divide the accumulated imaginary part by N to get the average
        dft[i] = {'re': sum_x, 'im': sum_y} # store the average real and imaginary parts
    return dft


samples = [] # List of samples loaded from the WAV file.
found_dtmf = False # Found a DTMF tone in current chunk.
silence_counter = 0 # Number of iterations with no DTMF tones.

# The WAV file is converted from https://en.wikipedia.org/wiki/File:DTMF_dialing.ogg#file
# It contains 8 phone numbers of 10 DTMF digits each:
# 0696675356, 4646415180, 2336731416, 3608338160, 4400826146, 6253689638, 8482138178, 5073643399
with open("dtmf.wav", "rb") as f:
    # Skip the WAV header, we use the data section which
    # starts at byte 44 and we know it is 8-bit PCM data.
    f.seek(44)
    # Read one chunk and then slide the window by one sample.
    while (byte := f.read(1)): # Read one byte at a time.
        samples.append(byte[0]) # Convert byte to int and append to samples.
        if len(samples) == N:
            # Compute the DFT for each harmonic.
            dft = calculate_dft(samples, dtmf_harmonics)
            sum_magnitudes = 0 # Sum of the magnitudes of the harmonics.
            magnitudes_per_harmonic = {} # Dictionary of magnitudes per harmonic.
            for i in range(len(dft)): # For each harmonic in the DFT.
                phase_vector = dft[i]
                magnitude = get_magnitude(phase_vector)
                # Store the magnitude of the harmonic.
                magnitudes_per_harmonic[dtmf_harmonics[i]] = magnitude
                # Sum the magnitudes of the harmonics.
                sum_magnitudes += magnitude
            # Sort the harmonics by magnitude in descending order and create a list
            # of dictionaries with 'k' and 'm' keys for the harmonic number and magnitude.
            # The first two harmonics are the most significant.
            sorted_harmonics = [
                    {'k': item[0], 'm': item[1]}
                    for item in sorted(magnitudes_per_harmonic.items(), key=lambda item: item[1], reverse=True)[:2]
                ]
            # If the first two harmonics are above the threshold, we probably detected a DTMF tone.
            if len(sorted_harmonics) == 2 and \
                sorted_harmonics[0]['m'] > harmonic_detection_magnitude and \
                sorted_harmonics[1]['m'] > harmonic_detection_magnitude:
                # Try to get the digit from the harmonics.
                digit = get_digit(sorted_harmonics[0]['k'], sorted_harmonics[1]['k'])
                if digit:
                    silence_counter = 0
                    if not found_dtmf:
                        print(digit, end="")
                        found_dtmf = True
                else: # No digit found.
                    print("?", end="")
            else: # The chunk has no DTMF tones.
                found_dtmf = False
                silence_counter += 1

            if silence_counter == silence_number:
                # Print a newline because we have reached the silence interval.
                print("")

            # Shift the window forward by shift_interval samples.
            samples = samples[shift_interval:]
