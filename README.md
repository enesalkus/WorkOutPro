# WorkOutPro - Mobile Application Project

**Document prepared for WSB Merito University, Poznan**  
**Technologies of mobile platforms**  
**Computer Science, 1st Cycle Degree of Studies**
**Date:** 2026

## The number of participants in Project Teams
The project was implemented by **Group 16**:
*   Ayet Enes Alkus
*   Emre Duman
*   Gunes Ezgi Kus

## Introduction
This document serves as the project documentation for the **WorkOutPro** mobile application, developed for the "Technologies of mobile platforms" course.

Passing the class is based on several components, with this project accounting for a significant portion of the grade. This application is designed to demonstrate proficiency in mobile application development using **.NET MAUI**.


## Project technology
The project is built using the following technologies:
*   **Framework**: .NET MAUI (Multi-platform App UI) targeting .NET 9.0
*   **Language**: C#
*   **Architecture**: MVVM (Model-View-ViewModel) utilizing `CommunityToolkit.Mvvm`
*   **Database**: SQLite with **Entity Framework Core 9.0**
*   **Target Platforms**: Android, iOS, Windows, MacCatalyst

## The form of providing the solutions
The solution is provided via:
1.  **Moodle Platform**: Submission of the project package (if required).
2.  **Git Repository**: The source code is hosted on a version control system (GitHub).

## Project Topic
**Topic 2: Training planner application**

WorkOutPro is a comprehensive training planner designed to help users manage their workouts, track exercises, and monitor their progress. It allows for the creation of customized training sessions and the logging of detailed exercise data.

## Specification

### Functional Requirements
The application implements the following core functional requirements:

1.  **User Authentication**: Users can securely register a new account and log in to access their data.
2.  **Exercise Management**: Users can view a library of available exercise types.
3.  **Training Session Management**: Users can create, update, and delete training sessions, specifying dates and durations.
4.  **Workout Tracking**: Users can log specific exercises performed during a session, recording details such as sets, repetitions, and weight/duration.
5.  **Statistics & Analysis**: Users can view statistical data regarding their training progress to track improvements over time.
6.  **Admin Panel**: Administrative users have access to manage registered users.
7.  **Data Persistence**: All records are saved locally on the device effectively acting as an offline-first application.

### Project Description
WorkOutPro is a mobile application that simplifies the process of tracking gym workouts. It replaces the traditional paper notebook with a robust digital solution that stores history and provides insights. The app is designed with a clean, intuitive interface suitable for use during training sessions where speed and ease of use are paramount.

### Potential Recipients
1.  **Gym Goers**: Individuals who want to verify their lifting progress over time.
2.  **Personal Trainers**: Professionals who need to track their clients' sessions and consistency.
3.  **Fitness Enthusiasts**: Anyone looking for a structured way to log physical activity and maintain a healthy lifestyle.

### Project Goal
*   To provide a seamless, offline-capable tool for recording workout data and visualizing progress to motivate consistency and physical improvement.

### Example Accounts
To facilitate testing and demonstration, the following example accounts can be used (or created):

| Role | Username | Password | Notes |
| :--- | :--- | :--- | :--- |
| **User** | `user@workoutpro.local` | `User123!` | Standard access to workout logging. |
| **Admin** | `admin@workoutpro.local` | `Admin123!` | Access to User Management panel. |

## Functional Layer
The implementation strictly adheres to the functional requirements:
*   **Data Integrity**: Used **Entity Framework Core** to manage relationships between Users, Training Sessions, and Exercises.
*   **Separation of Concerns**: Logic is encapsulated in **Services** (handling DB operations) and **ViewModels** (handling UI state), keeping the Views clean.

## View Layer
The User Interface is built with **XAML**, ensuring a responsive native experience on all platforms.
*   **Compliance**: The UI matches the functional requirements, providing dedicated pages for Login (`LoginPage`), Statistics (`StatsPage`), and Session Management (`TrainingSessionsPage`).
*   **Responsiveness**: The application utilizes MAUI's flexible layout controls (`Grid`, `StackLayout`, `CollectionView`) to ensure correct display on various screen sizes, from mobile phones to desktop windows.

## Naming Conventions & Code Formatting
The project follows standard C# and .NET coding conventions:
*   **PascalCase** for classes, methods, and public properties.
*   **camelCase** for private fields and local variables.
*   **MVVM Pattern**: ViewModels are suffixed with `ViewModel` (e.g., `LoginViewModel`), Pages with `Page` (e.g., `LoginPage`).
*   **Asynchronous Programming**: Extensive use of `async/await` patterns for database operations to prevent UI blocking.
*   **Dependency Injection**: Services and ViewModels are registered and managing via the .NET container in `MauiProgram.cs`.

---
_Project created for the assessment of "Technologies of mobile platforms"_
