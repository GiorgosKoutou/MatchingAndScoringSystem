# Matching and Scoring System

## Prerequisites

- .NET SDK 8 or later (developed and tested with .NET 9)
- 
- Visual Studio 2022 or Visual Studio Code

## How to Run

### 1. Build the Solution

```bash
dotnet build
```

### 2. Run the Tests

```bash
dotnet test
```

All tests should pass successfully.

## Solution Structure

```
MatchingAndScoringSystem/
├── DomainModels/
│   ├── Models/           # Domain entities (Provider, Requestor, Service, etc.)
│   └── Enums/            # Enumerations (CostProfile)
├── DomainServices/
│   └── Services/         # Business logic (ProviderScoringService, MatchingService)
└── UnitTests/
    └── ServiceTests/     # NUnit tests
```

## Implementation Overview

### Part 1: Domain Models
All required entities implemented with proper data types, nullable types, and validation attributes:
- Provider, Requestor, Service, Certification
- ProviderSkill, ProviderCertification
- MatchingRequest, MatchingResult

### Part 2: Provider Scoring Service
Calculates provider scores based on 5 factors:
- Certifications (1 or 9 points)
- Assessment Score (1, 3, or 9 points)
- Recency (1, 3, or 6 points)
- Frequency (1, 6, or 12 points)
- Monetary Value (1, 3, or 6 points)

Returns the average of all available factors, or 0 if no factors can be calculated.

### Part 3: Matching Service
Finds and ranks the top 3 providers matching a request:
- Hard filters: Service Type, User Capacity
- Soft criteria for ranking: Cost Profile, Digital Maturity, Location Proximity
- Includes relaxed matching fallback if no strict matches found
- Filters expired certifications automatically

### Part 4: Unit Tests
Testing framework: **NUnit**

Coverage includes:
- ProviderScoringService: 4 tests (all factors, missing factors, edge cases, correct calculations)
- MatchingService: 6 tests (exact matches, no matches, fewer than 3, ranking, filtering)

All tests follow AAA pattern (Arrange-Act-Assert) and use proper naming conventions.

## Notes

- Navigation properties added to Provider model to facilitate matching logic
- Expired certifications are automatically filtered based on ExpiryDate
- Relaxed matching implemented as fallback when strict criteria yield no results
