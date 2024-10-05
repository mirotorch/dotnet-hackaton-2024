# 🤖 Quadrolingo: Learn Foreign Languages with Fun
## 📚 Introduction

Quadrolingo is an interactive Telegram bot designed to help users learn foreign languages in a fun and engaging way. Built entirely in C#, it guides learners through a two-step process of acquiring and reinforcing new vocabulary. The project includes an admin interface developed in React, allowing administrators to manage vocabulary and track users' progress. Additionally, Quadrolingo integrates with MyMemory translator to automatically provide translations for new words, making learning more accessible.

The project was created during a 24-hour Online .NET Hackathon organized by HYS Enterprise.

<img src="https://github.com/user-attachments/assets/771060be-c07b-4a5f-8d1e-49debb61f216" alt="Quadrolingo bot demo"/>

## 🚀 Key Features

1. **Interactive Learning**:
    - Users learn new words through engaging activities.
    - The learning process is broken down into two main steps:
        - Learning new vocabulary.
        - Repeating the learned words through multiple-choice quizzes.

2. **Progress Tracking**:
    - After each quiz, users receive detailed feedback on their performance.
    - Progress is tracked over time, allowing users to compare their results and see improvements.

3. **Admin Capabilities**:
    - Admins can manage content effectively using the admin panel.
    - They can monitor users’ activities, add new vocabulary, and ensure the system stays up-to-date.


## 📝 Project Overview
### 🛠 Structure
1. **Bot Client**:
   * Built using the Telegram.Bot library.
   * Implements an event-based architecture for user interactions.

2. **Backend**:
    * Developed with ASP.NET using the MVC (Model-View-Controller) pattern.
    * Communicates with the front end via RESTful API.
    * Example endpoint: GET `api/users/{id}/known_words` retrieves a list of words the user knows.

3. **Admin Panel**:
    * Built with React for managing system content and users.
    * Allows admins to:
      * Add new languages to the study list.
      * Add and delete words.
      * View and manage the words along with their translations.
      * Search users and track their learning progress.

4. **Database**:
    - Stores user data, vocabulary, and progress information.
    
### 🔗 Third-Party Integration

- **MyMemory Translator**:
    - Automatically translates newly added words into the target languages, ensuring users have access to accurate translations.


## 🛠 Steps Involved
1. **🌐 Bot Implementation**:
    - The Telegram bot is built using an event-driven model, ensuring users receive timely responses.
    - Users interact with the bot to learn, take quizzes, and receive progress updates.

2. **💻 Backend and API Development**:
    - The backend uses ASP.NET and follows MVC architecture.
    - The RESTful API is used to communicate between the bot, admin panel, and database.

3. **📊 Admin Panel Functionality**:
    - The admin panel is developed in React.
    - Admins can add new content, track users, and analyze performance data.

4. **🤝 Integration with MyMemory**:
    - Automatic translations ensure a consistent and efficient user experience.
    - All newly added words are translated into selected target languages using MyMemory's API.
