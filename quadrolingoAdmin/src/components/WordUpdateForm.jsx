import { useState, useEffect } from 'react';

const WordUpdateForm = ({ word, languages, onSubmit, onCancel }) => {
  const [newWord, setNewWord] = useState(word.base);
  const [wordLang, setWordLang] = useState(word.lang);
  const [translations, setTranslations] = useState([]);

  // Initialize the translations from the word's existing data
  useEffect(() => {
    const translationList = Object.entries(word.translations).flatMap(([lang, texts]) => {
      return texts.map(text => ({ lang, text }));
    });
    setTranslations(translationList);
  }, [word]);

  const handleUpdateWord = async (e) => {
    e.preventDefault();

    if (!newWord || !wordLang) {
      alert('Please provide a word and select a language');
      return;
    }

    // Group translations by their respective language code
    const groupedTranslations = translations.reduce((acc, { lang, text }) => {
      if (!text) return acc; // Skip empty translations
      if (!acc[lang]) acc[lang] = [];
      acc[lang].push(text);
      return acc;
    }, {});

    const updatedWord = {
      ...word,
      base: newWord,
      lang: wordLang,
      translations: groupedTranslations,
    };

    onSubmit(updatedWord); // Send the updated word to the parent
  };

  // Function to handle updating a translation's text or language
  const handleTranslationChange = (index, field, value) => {
    const updatedTranslations = translations.map((translation, i) =>
      i === index ? { ...translation, [field]: value } : translation
    );
    setTranslations(updatedTranslations);
  };

  // Function to handle adding a new translation input
  const handleAddTranslation = () => {
    setTranslations(translations.concat({ lang: '', text: '' }));
  };

  // Function to handle removing a translation input
  const handleRemoveTranslation = (index) => {
    const updatedTranslations = translations.filter((_, i) => i !== index);
    setTranslations(updatedTranslations);
  };

  return (
    <form onSubmit={handleUpdateWord}>
      <input
        type="text"
        value={newWord}
        onChange={(e) => setNewWord(e.target.value)}
        placeholder="Update the word"
      />

      <select
        value={wordLang}
        onChange={(e) => setWordLang(e.target.value)}
      >
        <option value="">Select language</option>
        {languages.map(lang => (
          <option key={lang.code} value={lang.code}>
            {lang.name}
          </option>
        ))}
      </select>

      <h3>Translations</h3>
      {translations.map((translation, index) => (
        <div key={index}>
          <input
            type="text"
            value={translation.text}
            onChange={(e) => handleTranslationChange(index, 'text', e.target.value)}
            placeholder={`Translation ${index + 1}`}
          />
          <select
            value={translation.lang}
            onChange={(e) => handleTranslationChange(index, 'lang', e.target.value)}
          >
            <option value="">Select language</option>
            {languages.map(lang => (
              <option key={lang.code} value={lang.code}>
                {lang.name}
              </option>
            ))}
          </select>
          <button type="button" onClick={() => handleRemoveTranslation(index)}>Remove</button>
        </div>
      ))}

      <button type="button" onClick={handleAddTranslation}>Add Translation</button>

      <button type="submit">Update Word</button>
      <button type="button" onClick={onCancel}>Go back</button>
    </form>
  );
};

export default WordUpdateForm;