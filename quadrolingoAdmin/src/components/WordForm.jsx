import { useState } from 'react';

const WordForm = ({ languages, onSubmit }) => {
  const [wordLang, setWordLang] = useState('')
  const [newWord, setNewWord] = useState('')
  const [translations, setTranslations] = useState([{ text: '', lang: '' }]);


  const handleAddWord = async (e) => {
    e.preventDefault()

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

    const wordToAdd = {
      base: newWord,
      lang: wordLang,
      translations: groupedTranslations
    };

    console.log('created word', wordToAdd);

    onSubmit(wordToAdd);

    // Reset form after submission
    setNewWord('');
    setWordLang('');
    setTranslations([{ lang: '', text: '' }]);
  }

  // Function to handle updating a translation's text or language
  const handleTranslationChange = (index, field, value) => {
    const updatedTranslations = translations.map((translation, i) => (
      i === index ? { ...translation, [field]: value } : translation
    ));
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
    <form onSubmit={handleAddWord}>
    <input
      type="text"
      value={newWord}
      onChange={(e) => setNewWord(e.target.value)}
      placeholder="Add a new word"
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

    <button type="submit">Add Word</button>
  </form>
  )
}

export default WordForm