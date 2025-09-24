program SumNumbers;
var
  s: string;
  x, sum: real;
begin
  sum := 0;
  writeln('Enter real numbers (empty line to finish):');
  readln(s);
  while s <> '' do
  begin
    x := real.Parse(s);
    sum := sum + x;
    readln(s);
  end;
  writeln('Sum = ', sum);
end.