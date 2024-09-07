import { useState, useEffect } from 'react'

import Word from './components/Word'
import WordForm from './components/WordForm'
import WordUpdateForm from './components/WordUpdateForm'

import wordsService from './services/words'
import languagesService from './services/languages'

const App = () => {
  const [words, setWords] = useState([])
  const [languages, setLanguages] = useState([])
  const [wordToEdit, setWordToEdit] = useState(null);
  const [viewLanguages, setViewLanguages] = useState(false); // State to toggle between words and languages
  const [viewProfile, setViewProfile] = useState(false); // State to toggle between sections and user profile

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

  const deleteWord = async (newWord) => {
    console.log('word deleted', newWord);
  }

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

  // Function to handle updating an existing word
  const handleUpdateWord = (word) => {
    setWordToEdit(word); // Set the word to be updated, which will show the update form
  };

  const handleCancelUpdate = () => {
    setWordToEdit(null); // Cancel updating the word, returning to normal state
  };

  const handleViewLanguages = () => {
    setViewLanguages(true);
    setViewProfile(false); // Hide profile section
  };

  const handleViewWords = () => {
    setViewLanguages(false);
    setViewProfile(false); // Hide profile section
  };

  const handleViewProfile = () => {
    setViewLanguages(false);
    setViewProfile(true); // Show profile section
  };

  return (
    <div>
      {/* Navigation buttons for switching between sections */}
      <button onClick={handleViewLanguages}>View Languages</button>
      <button onClick={handleViewWords}>View Words</button>
      <button onClick={handleViewProfile}>View Profile</button>

      {/* Conditional rendering for the User Profile section */}
      {viewProfile ? (
        <div>
          <h2>User Profile</h2>
          {/* Render user profile section here */}
          <form>
            <label>Find User by Username:</label>
            <input type="text" placeholder="Enter username" />
            <button type="submit">Find User</button>
          </form>
        </div>
      ) : viewLanguages ? (
        <div>
          <h2>Languages Section</h2>
          {/* Render the languages section here */}
        </div>
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
                  deleteWord={() => deleteWord(w)} // Click handler for deleting a word
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
