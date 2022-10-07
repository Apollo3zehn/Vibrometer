## Initialize

# https://stackoverflow.com/a/64708697
using GLMakie
using Sockets
using Statistics

## Continue

if @isdefined connection
    close(connection)
end

connection = connect("172.26.48.51", 5555)

println("Connected!")

kernelSize = 1000
myLength = 1024
buffer = zeros(Int16, myLength * 2, kernelSize)
spectrum_buffer = zeros(Float32, myLength, kernelSize)

x = 0 : 1 : (myLength - 1)
y = zeros(Float32, myLength)
y_observable = Observable(y)

lines(x, y_observable) |> display

limits!(0, myLength, 0, 30000)

while true

    for i in 1 : kernelSize
        column = @view buffer[:, i]
        read!(connection, column)
        real_imag = reshape(column, 2, :)
        real = convert(Vector{Float32}, real_imag[1, :])
        imag = convert(Vector{Float32}, real_imag[2, :])
        spectrum_buffer[:, i] = sqrt.(real.^2 + imag.^2)
    end

    println("Update")

    # raw, kernelSize = 1
    #y_observable[] = spectrum_buffer[:, 1]

    # mean
    y_observable[] = mean(spectrum_buffer, dims=2)[:,1]

    # peak hold
    # y_observable[] = maximum(spectrum_buffer, dims=2)[:,1]
end