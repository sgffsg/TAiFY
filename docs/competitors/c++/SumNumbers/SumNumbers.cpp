#include <iostream>
#include <string>
using namespace std;

int main() {
    string s;
    double x, sum = 0;

    cout << "Enter real numbers (empty line to finish):" << endl;
    getline(cin, s);

    while (!s.empty()) {
        x = stod(s);
        sum += x;
        getline(cin, s);
    }

    cout << "Sum = " << sum << endl;
    return 0;
}