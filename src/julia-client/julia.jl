## Initialize

# https://stackoverflow.com/a/64708697
using GLMakie
using Sockets
using Statistics

## Continue

if @isdefined connection
    close(connection)
end

println("Connecting ...")
connection = connect("172.26.48.172", 5555)

kernelSize = 500
N = 1024
buffer = zeros(Int16, N * 2, kernelSize)
spectrum_buffer = zeros(Float32, N, kernelSize)

x = 0 : 1 : (N - 1)
y = zeros(Float32, N)
y_observable = Observable(y)

println("Plotting ...")

lines(x, y_observable, axis=(; xlabel="Frequency / Hz", ylabel="Amplitude / dBm", limits=(0, N, -120, 0))) |> display

println("Looping ...")

while true

    for i in 1 : kernelSize
        column = @view buffer[:, i]
        read!(connection, column)

        # real and imaginary parts combined
        real_imag = reshape(column, 2, :)

        # real part
        real = convert(Vector{Float32}, real_imag[1, :])

        # imaginary part
        imag = convert(Vector{Float32}, real_imag[2, :])

        # amplitude
        ampl = sqrt.(real.^2 + imag.^2) ./ N * 2
        
        # scale to Vpp
        vpp = ampl .* 0.5 ./ 2^16

        # rms
        rms = vpp ./ sqrt(2)

        # dBm
        dBm = 20 * log10.(rms ./ sqrt(50 * 0.0001))

        # result
        spectrum_buffer[:, i] = dBm
    end

    # raw, kernelSize = 1
    #y_observable[] = spectrum_buffer[:, 1]

    # mean
    y_observable[] = mean(spectrum_buffer, dims=2)[:,1]

    # peak hold
    # y_observable[] = maximum(spectrum_buffer, dims=2)[:,1]
end