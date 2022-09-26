## Initialize

# https://stackoverflow.com/a/64708697
using GLMakie
using Sockets
using Statistics

## Continue

if @isdefined connection
    close(connection)
end

connection = connect("localhost", 5555)

println("Connected!")

kernelSize = 10
myLength = 1024
buffer = zeros(Int32, myLength, kernelSize)

x = 0 : 1 : (myLength - 1)
y = zeros(Float32, myLength)
y_observable = Observable(y)

lines(x, y_observable) |> display

limits!(0, myLength, -5, 20)

while true
    for i in 1 : kernelSize
        @views read!(connection, buffer[:, i])
    end

    # mean
    # y_observable[] = mean(convert(Matrix{Int32}, buffer), dims=2)[:,1]

    # peak hold
    # y_observable[] = maximum(convert(Matrix{Int32}, buffer), dims=2)[:,1]
end