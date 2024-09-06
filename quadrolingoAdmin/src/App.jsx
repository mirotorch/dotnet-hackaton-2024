import { useState, useEffect } from 'react'

import Word from './components/Word'
import wordsService from './services/words'

const App = () => {
  const [words, setWords] = useState([])
  const [wordLang, setWordLang] = useState('')
  const [newWord, setNewWord] = useState('')
  const [translations, setTranslations] = useState([{ text: '', lang: '' }]);

  const languages = [
    { name: 'English', code: 'en' },
    { name: 'Spanish', code: 'es' },
    { name: 'French', code: 'fr' },
    { name: 'German', code: 'de' },
    { name: 'Chinese', code: 'zh' },
    { name: 'Japanese', code: 'ja' }
  ];

  useEffect(() => {
    wordsService.getAll().then(initialWords => {
      setWords(initialWords);
    });
  }, []);

  const updateWord = async (newWord) => {
    console.log('word updated', newWord);
  }

  const deleteWord = async (newWord) => {
    console.log('word deleted', newWord);
  }

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

    wordsService.create(wordToAdd).then(returnedWord => {
      setWords(words.concat(returnedWord));
      setNewWord('');
      setWordLang('');
      setTranslations([{ lang: '', text: '' }]);
    });
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
    <div>
      <h2>Words</h2>
      {words.map(w => (
        <Word key={w.id} word={w.base} wordLang={w.lang} updateWord={updateWord} deleteWord={deleteWord} />
      ))}

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
    </div>
  )
}

export default App
