program Eratosthenes;

function GeneratePrimes(maxNumber: integer): array of integer;
var
  isPrime: array of boolean;
  count: integer;
  i, j: integer;
begin
  if maxNumber < 2 then
  begin
    SetLength(Result, 0);
    Exit;
  end;

  SetLength(isPrime, maxNumber + 1);
  for var k := 0 to maxNumber do
    isPrime[k] := true;

  isPrime[0] := false;
  isPrime[1] := false;

  i := 2;
  while i * i <= maxNumber do
  begin
    if isPrime[i] then
    begin
      j := i * i;
      while j <= maxNumber do
      begin
        isPrime[j] := false;
        j := j + i;
      end;
    end;
    i := i + 1;
  end;

  count := 0;
  for var k := 2 to maxNumber do
    if isPrime[k] then
      count := count + 1;

  SetLength(Result, count);
  count := 0;
  for var k := 2 to maxNumber do
    if isPrime[k] then
    begin
      Result[count] := k;
      count := count + 1;
    end;
end;

var
  n: integer;
  primes: array of integer;
begin
  write('Enter an integer: ');
  readln(n);
  
  primes := GeneratePrimes(n);
  
  if Length(primes) = 0 then
    writeln('No prime numbers')
  else
  begin
    write('Prime numbers: ');
    for var i := 0 to Length(primes) - 1 do
      write(primes[i], ' ');
    writeln;
  end;
end.