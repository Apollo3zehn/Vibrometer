clc
clear
close all 

%%

height = 3;
width = 2;

%raw = cast([10532, 12499, 14081, 15173, 15668, 15511, 14749, 13423, 11667, 9588, 7401, 5257, 3395, 1971, 1026, 731, 1052, 1984, 3449, 5332, 7418, 9657, 11685, 13449, 14789, 15520, 15669, 15141, 14044, 12428, 10433, 8312, 6134, 4152, 2513, 1357, 774, 847, 1532, 2778, 4540, 6568, 8728, 10889, 12781, 14281, 15286, 15674, 15445, 14527, 13195, 11332, 9277, 7014, 4931, 3164, 1756, 932, 742, 1179, 2216, 3757, 5656, 7840, 9975, 12029, 13697, 14933, 15581, 15614, 15007, 13800, 12099, 10146, 7971, 5811, 3857, 2284, 1219, 745, 904, 1712, 3021, 4812, 6919, 9039, 11223, 13067, 14493, 15391], 'int16');

for i = 1 : 20
   fprintf('   cmp: %s\n', dec2bin(typecast(raw(i), 'uint16'), 16)); 
end

% uint16
data = raw;
data = typecast(data, 'uint16');
fprintf('uint16: %s\n', dec2bin(data(1), 16));
subplot(height, width, 1)
plot(data)
grid on
title('uint16')

% int16
data = raw;
fprintf(' int16: %s\n', dec2bin(typecast(data(1), 'uint16'), 16));
subplot(height, width, 2)
plot(data)
grid on
title('int16')

% int16, sign extended
data = raw;
signSet = bitget(data, 14);

for i = 1 : length(signSet)
    data(i) = bitset(data(i), 15, signSet(i));
    data(i) = bitset(data(i), 16, signSet(i));
end

fprintf('   ext: %s\n', dec2bin(typecast(data(1), 'uint16'), 16));
subplot(height, width, 3)
plot(data)
grid on
title('int16, sign extended')

% int16, sign extended and inverted
data = raw;
signSet = bitget(data, 14);

for i = 1 : length(signSet)
    data(i) = bitset(data(i), 14, ~signSet(i));
    data(i) = bitset(data(i), 15, ~signSet(i));
    data(i) = bitset(data(i), 16, ~signSet(i));
end

fprintf('extinv: %s\n', dec2bin(typecast(data(1), 'uint16'), 16));
subplot(height, width, 4)
plot(data)
grid on
title('int16, sign extended and inverted')

% int16, sign extended and inverted, and all bits inverted again
data = raw;
signSet = bitget(data, 14);

for i = 1 : length(signSet)
    data(i) = bitset(data(i), 14, ~signSet(i));
    data(i) = bitset(data(i), 15, ~signSet(i));
    data(i) = bitset(data(i), 16, ~signSet(i));
end

data = bitcmp(data);

fprintf('extin2: %s\n', dec2bin(typecast(data(1), 'uint16'), 16));
subplot(height, width, 5)
plot(data)
grid on
title('int16, sign extended and inverted, and all bits inverted again')