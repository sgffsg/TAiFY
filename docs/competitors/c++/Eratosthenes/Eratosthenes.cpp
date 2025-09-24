#include <iostream>
#include <vector>

std::vector<int> generatePrimes(int maxNumber)
{
	std::vector<int> primes;

	if (maxNumber < 2)
	{
		return primes;
	}

	std::vector<bool> isPrime(maxNumber + 1, true);
	isPrime[0] = isPrime[1] = false;

	for (int currentNumber = 2; currentNumber * currentNumber <= maxNumber; ++currentNumber)
	{
		if (isPrime[currentNumber])
		{
			for (int multiple = currentNumber * currentNumber; multiple <= maxNumber; multiple += currentNumber)
			{
				isPrime[multiple] = false;
			}
		}
	}

	for (int number = 2; number <= maxNumber; ++number)
	{
		if (isPrime[number])
		{
			primes.push_back(number);
		}
	}

	return primes;
}

int main()
{
	int userInput;
	std::cout << "Enter an integer: ";
	std::cin >> userInput;

	std::vector<int> primes = generatePrimes(userInput);

	if (primes.empty())
	{
		std::cout << "No prime numbers" << std::endl;
	}
	else
	{
		std::cout << "Prime numbers: ";
		for (int prime : primes)
		{
			std::cout << prime << " ";
		}
		std::cout << std::endl;
	}

	return 0;
}