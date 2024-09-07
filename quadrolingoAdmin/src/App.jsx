import { useState, useEffect } from 'react'

import Word from './components/Word'
import WordForm from './components/WordForm'
import WordUpdateForm from './components/WordUpdateForm'
import LanguagesList from './components/LanguageList'

import wordsService from './services/words'
import languagesService from './services/languages'
import usersService from './services/users'


const App = () => {
  const [words, setWords] = useState([])
  const [languages, setLanguages] = useState([])
  const [wordToEdit, setWordToEdit] = useState(null);
  const [viewLanguages, setViewLanguages] = useState(false); // State to toggle between words and languages
  const [viewUserSearch, setViewUserSearch] = useState(false); // State to toggle between sections and user profile
  const [username, setUsername] = useState(''); // State to store the username
  const [userResults, setUserResults] = useState([]); // State to store the search results
  const [selectedUserProfile, setSelectedUserProfile] = useState(null); // Store the selected user profile
  const [userKnownWords, setUserKnownWords] = useState([]); // Store the known words of the user
  const [userStudyStatistics, setUserStudyStatistics] = useState([]); // Store the study statistics
  const [showKnownWords, setShowKnownWords] = useState(false); // State to toggle the display of known words
  const [showStudyStatistics, setShowStudyStatistics] = useState(false); // State to toggle the display of study statistics

  // Fetch words from the API
  useEffect(() => {
    wordsService.getAll().then(initialWords => {
      setWords(initialWords);
    });
  }, []);

  // Fetch languages from the API
  useEffect(() => {
    languagesService.getAll().then(initialLanguages => {
      console.log('initialLanguages:', initialLanguages);
      setLanguages(initialLanguages);
    });
  }, []);

  useEffect(() => {
    if (username) {
      // Call the API every time the username is updated
      usersService.getUsersByName(username)
        .then((results) => {
          setUserResults(results);
        })
        .catch((error) => {
          console.error('Error fetching users:', error);
        });
    } else {
      setUserResults([]); // Clear results if no username is provided
    }
  }, [username]);

  const updateWord = async (updatedWord) => {
    try {
      // Call wordsService to update the word
      const returnedWord = await wordsService.update(updatedWord);
      
      // Update the state with the updated word
      setWords(words.map(w => (w.id === returnedWord.id ? returnedWord : w)));

      // Reset the wordToEdit state to hide the update form
      setWordToEdit(null);
      console.log('Word successfully updated:', returnedWord);
    } catch (error) {
      console.error('Error updating word:', error);
    }
  };

  const deleteWord = async (wordId) => {
    try {
      // Call the remove method from wordsService
      await wordsService.remove(wordId);
      console.log(wordId)
  
      // Update the words
      const updatedWords = await wordsService.getAll();
      setWords(updatedWords);
  
      console.log(`Word with ID ${wordId} successfully deleted`);
    } catch (error) {
      console.error(`Error deleting word with ID ${wordId}:`, error);
      alert('Failed to delete the word. Please try again.');
    }
  };

  const addWord = async (newWord) => {
    try {
      // Attempt to create a new word
      const returnedWord = await wordsService.create(newWord);
      
      // Add the newly created word to the words list
      setWords(words.concat(returnedWord));
  
      console.log('Word successfully added:', returnedWord);
    } catch (error) {
      // Log the error and handle it
      console.error('Error adding word:', error);
      
      // Optionally, you could display an error message to the user
      alert('Failed to add the word. Please try again.');
    }
  };

  const addLanguage = async (newLanguage) => {
    try {
      setLanguages((prevLanguages) => [...prevLanguages, newLanguage]); // Update languages state with the new language
      const returnedLanguage = await languagesService.create(newLanguage);
      console.log('Language successfully added:', returnedLanguage);
    } catch (error) {
      console.error('Error adding language:', error);
      alert('Failed to add the language. Please try again.');
    }
  };

  // Function to handle updating an existing word
  const handleUpdateWord = (word) => {
    setWordToEdit(word); // Set the word to be updated, which will show the update form
  };

  const handleCancelUpdate = () => {
    setWordToEdit(null); // Cancel updating the word, returning to normal state
  };

  const handleViewLanguages = () => {
    setViewLanguages(true);
    setViewUserSearch(false); // Hide profile section
  };

  const handleViewWords = () => {
    setViewLanguages(false);
    setViewUserSearch(false); // Hide profile section
  };

  const handleFindUsers = () => {
    setViewLanguages(false);
    setViewUserSearch(true); // Show profile section
  };

  const handleBackToUsersSearch = () => {
    setSelectedUserProfile(null); // Clear the selected profile and show the search again
  }

  const handleViewUserProfile = async (userId) => {
    try {
      const userProfile = await usersService.getUserProfileById(userId);
      console.log('User Profile:', userProfile);
      setSelectedUserProfile(userProfile); // Store the fetched user profile in state
      // You can set the user profile in state and render it in the UI
    } catch (error) {
      console.error('Error fetching user profile:', error);
    }
  };

   // Handler for toggling the display of known words
   const handleViewUserKnownWords = () => {
    setShowKnownWords((prev) => !prev);
    setShowStudyStatistics(false); // Hide study statistics when viewing known words

    // call db
    usersService.getUserKnownWords(selectedUserProfile.id)
      .then((knownWords) => {
        setUserKnownWords(knownWords);
      })
      .catch((error) => {
        console.error('Error fetching known words:', error);
      });
  };

  // Handler for toggling the display of study statistics
  const handleViewUserStudyStatistics = () => {
    setShowStudyStatistics((prev) => !prev);
    setShowKnownWords(false); // Hide known words when viewing study statistics

    // call db
    usersService.getProgress(selectedUserProfile.id)
      .then((progress) => {
        setUserStudyStatistics(progress);
      })
      .catch((error) => {
        console.error('Error fetching study statistics:', error);
      });
  };

  return (
    <div>
      {/* Navigation buttons for switching between sections */}
      <button onClick={handleViewLanguages}>View Languages</button>
      <button onClick={handleViewWords}>View Words</button>
      <button onClick={handleFindUsers}>Find users</button>

      {/* Conditional rendering for the User Profile section */}
      {viewUserSearch ? (
        <div>
          {/* If no profile is selected, show the search form */}
          {!selectedUserProfile ? (
            <div>
              <h2>Find Users</h2>
              <form>
                <label>Find User by Username:</label>
                <input
                  type="text"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  placeholder="Enter username"
                />
              </form>

              <h3>User Results</h3>
              <ul>
                {userResults.length > 0 ? (
                  userResults.map((user) => (
                    <li key={user.id}>
                      {user.username}
                      <button onClick={() => handleViewUserProfile(user.id)}>
                        View Profile
                      </button>
                    </li>
                  ))
                ) : (
                  <li>No users found</li>
                )}
              </ul>
            </div>
          ) : (
            <div>
              <h2>Profile details</h2>
              <p>Name: {selectedUserProfile.first_name} {selectedUserProfile.last_name}</p>
              <p>Username: {selectedUserProfile.username}</p>
              <p>Telegram ID: {selectedUserProfile.telegram_id}</p>
              <p>Chat ID: {selectedUserProfile.chat_id}</p>

              <h3>Study information</h3>
              <p>Base language: {selectedUserProfile.base_lang}</p>
              <p>Study language: {selectedUserProfile.study_lang}</p>
              <button onClick={handleBackToUsersSearch}>Back to Search</button>
              <button onClick={handleViewUserKnownWords}>View Known Words</button>
              <button onClick={handleViewUserStudyStatistics}>View Study Statistics</button>

              {/* Conditionally render the known words section */}
              {showKnownWords && (
                <div>
                  <h3>Known Words</h3>
                  <ul>
                    {userKnownWords.length > 0 ? (
                      userKnownWords.map((word, index) => (
                        <li key={index}>{word}</li>
                      ))
                    ) : (
                      <li>No known words available</li>
                    )}
                  </ul>
                </div>
              )}

              {/* Conditionally render the study statistics section */}
              {showStudyStatistics && (
                <div>
                  <h3>Study Statistics</h3>
                  <ul>
                    {userStudyStatistics.length > 0 ? (
                      userStudyStatistics.map((stat, index) => (
                        <li key={index}>{stat}</li>
                      ))
                    ) : (
                      <li>No study statistics available</li>
                    )}
                  </ul>
                </div>
              )}
            </div>
          )}
        </div>
      ) : viewLanguages ? (
        <LanguagesList languages={languages} onSubmit={addLanguage} />
      ) : (
        <div>
          <h2>Words</h2>
          {/* If a word is being updated, show the update form, otherwise show the list */}
          {wordToEdit ? (
            <WordUpdateForm
              word={wordToEdit}
              languages={languages}
              onSubmit={updateWord}
              onCancel={handleCancelUpdate} // Cancel button handler
            />
          ) : (
            <>
              {words.map((w) => (
                <Word
                  key={w.id}
                  word={w.base}
                  wordLang={w.lang}
                  updateWord={() => handleUpdateWord(w)} // Click handler for updating a word
                  deleteWord={() => deleteWord(w.id)} // Click handler for deleting a word
                />
              ))}

              <h2>Add Word</h2>
              <WordForm languages={languages} onSubmit={addWord} />
            </>
          )}
        </div>
      )}
    </div>
  )
}

export default App
