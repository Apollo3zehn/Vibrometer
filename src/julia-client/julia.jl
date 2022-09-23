# https://stackoverflow.com/a/64708697
using GLMakie
using Sockets

if @isdefined connection
    close(connection)
end

connection = connect("172.26.48.43", 5555)

println("Connected!")

buffer = zeros(Int16, 1024)

x = 0 : 1 : (length(buffer) - 1)
y = zeros(Int16, length(buffer))
y_observable = Node(y)

lines(x, y_observable) |> display

limits!(0, length(buffer), 0, 10)

while true
    read!(connection, buffer)
    y_observable[] = buffer
end