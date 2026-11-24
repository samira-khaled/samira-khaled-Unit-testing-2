# Testing Notes

Name: < samira khaled anwar shaltout >
Group / ID: < group 3>

## 1. How many tests?
- Total: 20
- Passed: 20
- Failed: 0

## 2. Line coverage
- Total line coverage: 90.3%
- Domain: 100%
- Application: 95.7%

## 3. Where I used each library
- Moq: I used it to make fake versions of the inventory, repository and notification services.
- AutoFixture: I used it to create a random order automatically, with quantity above 0 and price 0 or more.
- Bogus: I used it to create a fake customer name and email, with a fixed seed so they are the same every run.

## 4. Is it fair? (Example B vs Example C)
No. A customer who pays 999.99 pays more than a customer who buys for 1,000 and pays only 900.

## 5. Bugs
I did not find a bug that breaks a rule. I only noticed two things (I did not change the code):
- Every order of 1,000 or more gets a discount of exactly 100, because 10% of 1,000 is already 100.
- The order is saved before the confirmation is sent. If sending fails, the order stays saved.