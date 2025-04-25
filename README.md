# GlobalRide API

This repository contains the source code for the API of the GlobalRide app, an application designed to meet the diverse needs of a global car rental business.

## Features & User Stories

- Global Reach and Localization

  - As a user, I want to view the app in my preferred language so that I can use it comfortably.
  - As a user, I want prices displayed in my local currency and distances in my preferred units so that I can make decisions easily.
    (In Progress)

- User-Friendly Booking Flow
  - As a user, I want to search for cars based on filters like location, date, car type, and features so that I can find the best option.
    (In Progress)
  - As a user, I want to see real-time availability of cars so that I can make immediate decisions.
    (In Progress)
  - As a user, I want to pick up a car at one location and drop it off at another for greater convenience.
    (In Progress)

## Business Rules

### One-Way Rentals

| Rule No. | Rule Description                                                                                                 | Condition                                                                        | Action                                                               |
| -------- | ---------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------- | -------------------------------------------------------------------- |
| 1        | **Branch Eligibility**: Only designated "hub" branches (e.g., major cities, airports) support one-way drop-offs. | Drop-off branch is not designated as a hub.                                      | Block one-way rentals if drop-off location is ineligible.            |
| 2        | **Cross-Border Rentals**: Only allowed for pre-approved country pairs.                                           | Drop-off country is not pre-approved.                                            | Block one-way rentals if country pair is not pre-approved.           |
| 3        | **Vehicle Availability Constraints**: Drop-off branch must have capacity for the incoming vehicle.               | Drop-off branch has no available space.                                          | Check capacity. Block booking if full.                               |
| 4        | **Restricted Vehicle Types**: Specialty vehicles cannot be rented for one-way trips.                             | Selected vehicle is a luxury car or specialty vehicle.                           | Hide restricted vehicle types in search results for one-way rentals. |
| 5        | **One-Way Fees**: Drop-off fees vary based on distance, cross-border, season, and vehicle type.                  | Distance between branches, cross-border, vehicle type, and season determine fee. | Auto-calculate and display fees during booking.                      |

## Tech Stack

- ASP.NET Core
- EF Core
- SQL Server
- MediatR
- xUnit
- Clean Architecture

**Check out the [SPA repo](https://github.com/nagiashraf/globalride-frontend-web).**
