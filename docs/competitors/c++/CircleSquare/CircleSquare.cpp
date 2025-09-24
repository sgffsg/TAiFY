#include <cmath>
#include <iomanip>
#include <iostream>

int main()
{
	double radius, area;

	std::cout << "Enter the radius of the circle: ";
	std::cin >> radius;

	area = M_PI * pow(radius, 2);

	std::cout << std::fixed << std::setprecision(2);
	std::cout << "The area of a circle with a radius " << radius << " is " << area << std::endl;

	return 0;
}
