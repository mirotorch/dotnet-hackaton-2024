import { useState } from 'react'

// LanguagesList component that takes initialLanguages as a prop
const LanguagesList = ({ languages, onSubmit }) => {
  const [isCreating, setIsCreating] = useState(false); // Track whether the form is being shown
  const [newLanguageCode, setNewLanguageCode] = useState(''); // State for new language code
  const [newLanguageName, setNewLanguageName] = useState(''); // State for new language name
  const [errorMessage, setErrorMessage] = useState(''); // State for storing validation error message

  // Function to handle form submission
  const handleCreateLanguage = async (e) => {
    e.preventDefault();

    // Validation: Ensure the language code is exactly two characters
    if (newLanguageCode.length !== 2) {
      setErrorMessage('Language code must be exactly 2 characters.');
      return; // Prevent form submission
    }
    
    // If validation passes, reset the error message
    setErrorMessage('');

    const newLanguage = {
      code: newLanguageCode,
      name: newLanguageName,
    };

    try {
      await onSubmit(newLanguage);

      console.log('newLanguage:', newLanguage);
      
      // Reset form fields and close the form
      setNewLanguageCode('');
      setNewLanguageName('');
      setIsCreating(false);
    } catch (error) {
       // Catch any errors from the create service and set an appropriate error message
       if (error.response && error.response.status === 409) {
        setErrorMessage('Language already exists.');
      } else {
        setErrorMessage('An unexpected error occurred. Please try again.');
      }
    }
    
  };

  return (
    <div>
      <h2>Languages</h2>
      {/* Button to toggle the create language form */}
      <button onClick={() => setIsCreating(true)}>Create New Language</button>

      {/* Show form if the user is creating a new language */}
      {isCreating && (
        <form onSubmit={handleCreateLanguage}>
          <div>
            <label>Language Code: </label>
            <input
              type="text"
              value={newLanguageCode}
              onChange={(e) => setNewLanguageCode(e.target.value)}
              placeholder="Enter language code"
            />
          </div>
          <div>
            <label>Language Name: </label>
            <input
              type="text"
              value={newLanguageName}
              onChange={(e) => setNewLanguageName(e.target.value)}
              placeholder="Enter language name"
            />
          </div>

           {/* Display validation error message */}
           {errorMessage && <p style={{ color: 'red' }}>{errorMessage}</p>}

          <button type="submit">Add Language</button>
          <button type="button" onClick={() => setIsCreating(false)}>Cancel</button>
        </form>
      )}

      {/* Display the list of existing languages */}
      <ul>
        {languages.map((language, index) => (
          <li key={index}>
            <strong>{language.code}</strong>: {language.name}
          </li>
        ))}
      </ul>
    </div>
  );
};
export default LanguagesList;
