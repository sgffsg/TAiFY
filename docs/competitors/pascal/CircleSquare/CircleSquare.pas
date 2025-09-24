program CircleSquare;
var
    radius, area: real;
begin
    write('Enter the radius of the circle: ');
    readln(radius);
    area := Pi * sqr(radius);
    writeln('The area of a circle with a radius ', radius:0:2, ' is ', area:0:2);
end.
